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
        public async Task AddJobAsync ([Remainder] string message = null)
        {
            try
            {
                var token = message.Split (" ", 2);
                if (token.Length < 2 && !(token.Length == 1 && token[0].ToLower () == "list"))
                {
                    await ReplyAsync ($"you must specify a command, use '$jobs help' for help.");
                    return;
                }
                var job = new Job ();

                var operation = token[0].ToLower ();
                string m = "";

                var flags = new List<string> () { "link", "description", "title", "unapproved", "job", "ranks" };
                var flagParser = new FlagParser (flags);
                var results = flagParser.ParseMessage (message);

                if (operation != "list")
                {
                    m = message.Substring (message.IndexOf (" ")).Trim ();
                }

                switch (operation)
                {
                    case "add":
                        await AddNewJob (results);
                        break;
                    case "find":
                        await SearchExistingJobs (m);
                        break;
                    case "list":
                        await ListJobs ();
                        break;
                    case "approve":
                        await ApproveJob (results);
                        break;
                    case "help":
                        await ShowHelpMessage ();
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

        private async Task ApproveJob (Dictionary<string, string> request)
        {
            var user = this.Context.User as SocketGuildUser;

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
                    _db.Entry (job).State = EntityState.Modified;
                    await _db.SaveChangesAsync ();
                }
            }
        }

        private async Task AddNewJob (Dictionary<string, string> request)
        {
            var title = request.ContainsKey ("title") ? request["title"] : null;
            var description = request.ContainsKey ("description") ? request["description"] : null;
            var link = request.ContainsKey ("link") ? request["link"] : null;
            var jobs = request.ContainsKey ("jobs") ? ParseJobs (request["jobs"]) : null;

            var job = new Job () { Title = title, Description = description, JobLink = link, Author = Context.User.Mention };

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

            sb.AppendLine ("$jobs add -title <Title> -description <description> -link <link to your job> -ranks <A,B,C,D,E,F>");
            sb.AppendLine ("$jobs find <Title> - Use this command to search for a job by title");
            sb.AppendLine ("$jobs list - Use this command to see a list of all available jobs");
            sb.AppendLine ("$jobs approve <job id>");

            var embedBuilder = new EmbedBuilder ();
            embedBuilder.WithDescription (sb.ToString ());
            await Context.Channel.SendMessageAsync ("", false, embedBuilder.Build ());
        }

        private async Task SearchExistingJobs (string message)
        {
            var q = message.ToLower ();

            var matches = _db.Jobs.Where (job => job.Title.ToLower ().Contains (q)).ToList ();

            if (matches.Count > 0)
            {
                await ReplyWithJobs (matches);
                return;
            }

            await ReplyAsync ($"I could not find a job with that title, please double check it exists");
        }

        private async Task ListJobs ()
        {
            var jobs = _db.Jobs.ToList ();

            await ReplyWithJobs (jobs);
        }

        private async Task ReplyWithJobs (List<Job> jobs)
        {
            if (jobs.Count == 0)
            {
                await ReplyAsync ("no jobs have been added yet");
                return;
            }

            var sb = new StringBuilder ();
            for (int i = 0; i < jobs.Count; i++)
            {
                sb.AppendLine ($"{jobs[i].Id}) {jobs[i].Title} Difficulty: {jobs[i].Difficulty}");
            }

            var embedBuilder = new EmbedBuilder ();
            embedBuilder.WithDescription (sb.ToString ());
            await Context.Channel.SendMessageAsync ("", false, embedBuilder.Build ());
        }

        private async Task ParseJobUpdateRequest (string message)
        {
            var tokens = message.Split (" ");

            if (tokens.Length < 2)
            {
                await ReplyAsync ($"You must give a job id, and a description {this.Context.User.Mention}");
                return;
            }

            var jobId = tokens[0].ParseInt ();

            var job = _db.Jobs.FirstOrDefault (j => j.Id == jobId);

            if (job == null)
            {
                await ReplyAsync ($"Could not find a job, {this.Context.User.Mention} try using `$jobs list` to find your job.");
                return;
            }

            job.Description = string.Join (" ", tokens.Skip (1));

            _db.Jobs.Update (job);

            await _db.SaveChangesAsync ();
            await ReplyAsync ($"Job ({job.Id}) {job.Title} was added succesfully by {this.Context.User.Mention}");
        }
    }
}