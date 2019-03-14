using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Extensions;
using dm_bot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dm_bot.Commands
{
    public class JobCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;

        public JobCommand(DMContext context)
        {
            this._db = context;
        }

        [Command("jobs")]
        public async Task AddJobAsync([Remainder] string message = null)
        {
            try
            {
                var user = Context.User;

                var token = message.Split(" ", 2);
                if (token.Length < 2 && !(token.Length == 1 && token[0].ToLower() == "list"))
                {
                    await ReplyAsync($"you must specify a command, use '!dm jobs help' for help.");
                    return;
                }
                var job = new Job();

                var operation = token[0].ToLower();
                string m = "";

                if (operation != "list")
                {
                    m = message.Substring(message.IndexOf(" ")).Trim();
                }

                switch (operation)
                {
                    case "add":
                        await AddNewJob(m);
                        break;
                    case "description":
                        await ParseJobUpdateRequest(m);
                        break;
                    case "find":
                        await SearchExistingJobs(m);
                        break;
                    case "list":
                        await ListJobs();
                        break;
                    case "help":
                        ShowHelpMessage();
                        break;

                    default:
                        break;
                }

            }
            catch (ParseException exception)
            {
                await ReplyAsync($"Sorry {message}, we cannot process that request.");
            }
        }

        private async Task AddNewJob(string message)
        {
            var job = new Job() { Title = message };

            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();
            await ReplyAsync($"Added new job, '{job.Title}', to update the description, use `!dm jobs update {job.Id} \"Your description here\"`");
        }

        private void ShowHelpMessage()
        {
            throw new NotImplementedException();
        }

        private async Task SearchExistingJobs(string message)
        {
            var matches = _db.Jobs.Where(job => job.Title.ToLower().Contains(message.ToLower())).ToList();

            if (matches.Count > 0)
            {
                await SendJobs(matches);
                return;
            }

            await ReplyAsync($"Could not find a job with that title, please double check it exists");
        }

        private async Task ListJobs()
        {
            var jobs = _db.Jobs.ToList();

            await SendJobs(jobs);
        }

        private async Task SendJobs(List<Job> jobs)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < jobs.Count; i++)
            {
                sb.AppendLine($"{jobs[i].Id}) {jobs[i].Title} Difficulty: {jobs[i].Difficulty}");
            }

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithDescription(sb.ToString());
            await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        }

        private async Task ParseJobUpdateRequest(string message)
        {
            var tokens = message.Split(" ");

            if (tokens.Length < 2)
            {
                await ReplyAsync($"You must give a job id, and a description {this.Context.User.Mention}");
                return;
            }

            var jobId = tokens[0].ParseInt();

            var job = _db.Jobs.FirstOrDefault(j => j.Id == jobId);

            if (job == null)
            {
                await ReplyAsync($"Could not find a job, {this.Context.User.Mention} try using $jobs list to find your job.");
                return;
            }

            job.Description = string.Join(" ", tokens.Skip(1));

            _db.Jobs.Update(job);

            await _db.SaveChangesAsync();
        }
    }
}