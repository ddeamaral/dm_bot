using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Extensions;
using dm_bot.Models;
using dm_bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace dm_bot.Commands
{
    public class JobCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;

        private readonly IConfiguration _configuration;

        public JobCommand (DMContext context, IConfiguration configuration)
        {
            this._configuration = configuration;
            this._db = context;
        }

        // $jobs add <details>
        // $jobs list (returns list of jobs)
        // $jobs approve -job <job id>
        // $jobs find <title>

        [Command ("jobs")]
        public async Task AddJobAsync (string operation, [Remainder] string message = null)
        {
            try
            {

                var flags = new List<string> () { "link", "description", "title", "unapproved", "job", "ranks", "misc", "difficulty", "personal", "repeatable" };
                var flagParser = new FlagParser (flags);
                var results = flagParser.ParseMessage (message);

                switch (operation)
                {
                    case "add":
                        await AddNewJob (results);
                        break;
                    case "find":
                        await SearchExistingJobs (results);
                        break;

                    case "details":
                        await ShowJobDetails (results);
                        break;

                    case "list":
                        await ListJobs (results);
                        break;
                    case "approve":
                        await ApproveJob (results);
                        break;
                    case "help":
                        await ShowHelpMessage ();
                        break;
                    case "delete":
                        await DeleteJob (results);
                        break;

                    default:
                        break;
                }
            }
            catch
            {
                await ReplyAsync ($"Sorry {message}, I cannot process that request.");
            }
        }

        private async Task ShowJobDetails (Dictionary<string, string> results)
        {
            if (!results.ContainsKey ("job"))
            {
                await ReplyAsync ($"Please supply a job id {Context.User.Mention}");
                return;
            }

            var jobId = results["job"].ParseInt (-1);

            var list = _db.Jobs.ToList ();

            var job = await _db.Jobs.FirstOrDefaultAsync (j => j.Id == jobId);

            if (job is null)
            {
                await ReplyAsync ($"Could not find a job with that id {Context.User.Mention}");
                return;
            }

            await ReplyWithJobDetails (job);
        }

        private async Task DeleteJob (Dictionary<string, string> results)
        {
            if (!results.ContainsKey ("job"))
            {
                await ReplyAsync ($"Sorry {this.Context.User.Mention}, you must specify a job");
                return;
            }

            var jobId = results["job"].ParseInt (-1);

            var job = await _db.Jobs.FirstOrDefaultAsync (j => j.Id == jobId);

            if (job is null)
            {
                await ReplyAsync ($"Sorry {this.Context.User.Mention}, that job doesn't seem to exist");
                return;
            }

            if (job.AuthorId != Context.User.Id)
            {
                await ReplyAsync ($"Sorry {this.Context.User.Mention}, you are not the author of this job, so you cannot delete it");
                return;
            }

            _db.Jobs.Remove (job);
            await _db.SaveChangesAsync ();

            await ReplyAsync ($"{this.Context.User.Mention} has successfully deleted job: {job.Title} ({job.Id})!");
        }

        private async Task ApproveJob (Dictionary<string, string> request)
        {

            var user = this.Context.User as SocketGuildUser;

            if (!request.ContainsKey ("job"))
            {
                await ReplyAsync ($"Sorry {user.Mention}, you need to specify id of the job you want to approve");
                return;
            }

            var roles = new List<string> ();

            _configuration.GetSection ("approvalRoles").Bind (roles);

            if (user.Roles.Any (role => roles.Contains (role.Name)))
            {
                var jobid = request["job"].ParseInt ();
                var job = _db.Jobs.FirstOrDefault (j => j.Id == jobid);

                if (job is null)
                {
                    await ReplyAsync ($"Sorry {user.Mention}, I could not find that job, please ensure your job id is correct");
                    return;
                }

                if (string.IsNullOrWhiteSpace (job.FirstApproval))
                {
                    job.FirstApproval = user.Mention;
                    job.FirstApprovalId = user.Id;
                }
                else if (string.IsNullOrWhiteSpace (job.SecondApproval))
                {
                    job.SecondApproval = user.Mention;
                    job.SecondApprovalId = user.Id;
                }
                else
                {
                    await ReplyAsync ($"Looks like the job is already approved {Context.User.Mention}");
                    return;
                }

                _db.Entry (job).State = EntityState.Modified;
                await _db.SaveChangesAsync ();
            }
        }

        private async Task<ICollection<Rank>> GetRanks (string message)
        {
            if (string.IsNullOrWhiteSpace (message))
            {
                await ReplyAsync ($"{Context.User.Mention} Please supply the ranks that can go on this job");
                return new List<Rank> ();
            }

            var rankLetters = message.Split (",", options : StringSplitOptions.RemoveEmptyEntries);

            var ranks = _db.Ranks.Where (rank => rankLetters.Contains (rank.RankLetter));

            return ranks.ToList ();
        }

        private async Task AddNewJob (Dictionary<string, string> request)
        {
            var title = request.ContainsKey ("title") ? request["title"] : null;
            var description = request.ContainsKey ("description") ? request["description"] : null;
            var link = request.ContainsKey ("link") ? request["link"] : null;
            var jobs = request.ContainsKey ("jobs") ? ParseJobs (request["jobs"]) : null;
            var difficulty = request.ContainsKey ("difficulty") ? request["difficulty"] : null;
            var personal = request.ContainsKey ("personal") && request["personal"] == "y";
            var repeatable = request.ContainsKey ("repeatable") && request["repeatable"] == "y";
            var ranks = request.ContainsKey ("ranks") ? request["ranks"] : null;

            var job = new Job ()
            {
                Title = title,
                Description = description,
                JobLink = link,
                AuthorMention = Context.User.Mention,
                AuthorId = Context.User.Id,
                Difficulty = difficulty,
                Repeatable = repeatable,
                Personal = personal,
                TaggedRanks = await GetRanks (ranks)
            };

            _db.Jobs.Add (job);
            await _db.SaveChangesAsync ();
            await ReplyAsync ($"Successfully added job '{job.Title}', your job has the id: {job.Id}`");
        }

        private List<Rank> ParseJobs (string value)
        {
            var rankLetters = value.Split (",");

            var ranks = (this.Context.Guild as SocketGuild).Roles;

            var rankList = new List<Rank> ();

            foreach (var rankLetter in rankLetters)
            {
                var rank = new Rank ();
                rank.RankLetter = rankLetter;
                rank.RankName = ranks.FirstOrDefault (r => r.Name.Contains ($"{rank.RankLetter}-"))?.Name;
                rank.RankMention = ranks.FirstOrDefault (r => r.Name.Contains ($"{rank.RankLetter}-"))?.Mention;
            }

            return rankList;
        }

        private async Task ShowHelpMessage ()
        {
            var sb = new StringBuilder ();

            sb.AppendLine ("$jobs add -title <Title> -description <description> -link <link to your job> -ranks <A,B,C,D,E,F> -misc <some miscellaneous text you want to display here> -personal <y or n> -repeatable <y or n>");
            sb.AppendLine ("$jobs find -title <Title> - Use this command to search for a job by title");
            sb.AppendLine ("$jobs list - Use this command to see a list of all available jobs");
            sb.AppendLine ("$jobs list -unapproved - Use this command to see a list of all unapproved jobs");
            sb.AppendLine ("$jobs approve -job <job id> - to approve a job");
            sb.AppendLine ("$jobs delete -job <job id> - to delete your job");

            var embedBuilder = new EmbedBuilder ();
            embedBuilder.WithDescription (sb.ToString ());
            await Context.Channel.SendMessageAsync ("", false, embedBuilder.Build ());
        }

        private async Task SearchExistingJobs (Dictionary<string, string> request)
        {
            if (!request.ContainsKey ("title"))
            {
                await ReplyAsync ($"Please supply a title, {this.Context.User.Mention}");
                return;
            }

            var q = request["title"].ToLower ();

            var matches = _db.Jobs.Where (job => job.Title.ToLower ().Contains (q)).ToList ();

            if (matches.Count > 0)
            {
                await ReplyWithJobs (matches);
                return;
            }

            await ReplyAsync ($"{this.Context.User.Mention}, I could not find a job with that title, please double check it exists");
        }

        private async Task ReplyWithJobDetails (Job job)
        {
            var sb = new StringBuilder ();
            sb.AppendLine ($"{job.Title} - Written by {job.AuthorMention} (Difficulty: {job.Difficulty})");
            sb.AppendLine ($"{Format.Url("job link",job.JobLink)}");
            sb.AppendLine ($"{job.Description}");
            sb.AppendLine ($"Personal: {(job.Personal ? "Yes" : "No")}");
            sb.AppendLine ($"Repeatable: {(job.Repeatable ? "Yes" : "No")}");
            sb.AppendLine ($"Ranks: {job.TaggedRanks}");
            if (string.IsNullOrWhiteSpace (job.FirstApproval) || string.IsNullOrWhiteSpace (job.SecondApproval))
            {
                sb.AppendLine ("Job not yet approved");
            }
            else
            {
                sb.AppendLine ($"Approved by: {job.FirstApproval} and {job.SecondApproval}");
            }

            await ReplyAsync ($"Sent you information about {job.Title} ({job.Id}), {Context.User.Mention}");
            var embedBuilder = new EmbedBuilder ();
            embedBuilder.WithDescription (sb.ToString ());
            await this.Context.User.SendMessageAsync (embed: embedBuilder.Build ());
        }

        private async Task ListJobs (Dictionary<string, string> request)
        {
            List<Job> jobs = new List<Job> ();

            if (request.ContainsKey ("unapproved"))
            {
                jobs = await _db.Jobs.Where (job => string.IsNullOrWhiteSpace (job.FirstApproval) || string.IsNullOrWhiteSpace (job.SecondApproval)).ToListAsync ();
            }
            else
            {
                jobs = await _db.Jobs.ToListAsync ();
            }

            await ReplyWithJobs (jobs);
        }

        private async Task ReplyWithJobs (List<Job> jobs)
        {
            if (jobs.Count == 0)
            {
                await ReplyAsync ($"No jobs have been added yet {Context.User.Mention}");
                return;
            }

            var sb = new StringBuilder ();
            for (int i = 0; i < jobs.Count; i++)
            {
                sb.AppendLine ($@"{ jobs[i].Id }) { jobs[i].Title } - Difficulty: { jobs[i].Difficulty } - Author: {MentionUtils.MentionUser(jobs[i].AuthorId)}");
            }

            var embedBuilder = new EmbedBuilder ();
            embedBuilder.WithDescription (sb.ToString ());
            await Context.Channel.SendMessageAsync ("", false, embedBuilder.Build ());
        }

        private async Task ParseJobUpdateRequest (string message)
        {
            var tokens = message.Split ("");

            if (tokens.Length < 2)
            {
                await ReplyAsync ($"You must give a job id, and a description { this.Context.User.Mention }");
                return;
            }

            var jobId = tokens[0].ParseInt ();

            var job = _db.Jobs.FirstOrDefault (j => j.Id == jobId);

            if (job == null)
            {
                await ReplyAsync ($"Could not find a job, { this.Context.User.Mention } try using` $jobs list` to find your job.");
                return;
            }

            job.Description = string.Join (" ", tokens.Skip (1));

            _db.Jobs.Update (job);

            await _db.SaveChangesAsync ();
            await ReplyAsync ($"Job ({job.Id}) {job.Title} was added succesfully by {this.Context.User.Mention}");
        }
    }
}