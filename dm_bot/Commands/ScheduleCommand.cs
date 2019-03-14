using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Extensions;
using dm_bot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dm_bot.Commands
{
    public class ScheduleCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext context;

        public ScheduleCommand(DMContext context)
        {
            this.context = context;
        }

        [Command("schedule")]
        public async Task ScheduleAsync([Remainder] string message = null)
        {
            try
            {
                var context = Context.User;

                var availability = ParseScheduleRequest(message);

                availability.DungeonMasterUserName = context.Username;

                var embedBuilder = new EmbedBuilder();
                embedBuilder.WithDescription(Templates.AvailabilityResponse(availability));
                await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());

                await ReplyAsync($"Your game has been scheduled @{context.Mention}");
            }
            catch (ParseException exception)
            {
                await ReplyAsync($"Sorry {message}, we cannot process that request.");
            }
        }

        public DungeonMasterAvailability ParseScheduleRequest(string message)
        {
            var dungeonMasterAvailability = new DungeonMasterAvailability();

            var tokens = message.Split(" ");
            var requestAttributes = new Dictionary<string, string>();

            foreach (var token in tokens)
            {
                if (token.Contains("="))
                {
                    var pair = token.Split('=');
                    requestAttributes.Add(pair[0], pair[1]);
                }
            }

            dungeonMasterAvailability = ObjectFromDictionary(requestAttributes);

            return dungeonMasterAvailability;
        }

        private DungeonMasterAvailability ObjectFromDictionary(Dictionary<string, string> dictionary)
        {
            var dm = new DungeonMasterAvailability();

            dm.ChronusTimeLink = dictionary.ContainsKey("TIMELINK") ? dictionary["TIMELINK"] : null;
            dm.RoleplayingPercent = dictionary["TIMELINK"].ParseInt(-1);
            dm.MaxHours = dictionary.ContainsKey("MAX") ? dictionary["MAX"].ParseInt() : -1;
            dm.MinHours = dictionary.ContainsKey("MIN") ? dictionary["MIN"].ParseInt() : -1;
            dm.CombatPercent = dictionary.ContainsKey("COMBAT") ? dictionary["COMBAT"].ParseInt() : -1;
            dm.ChatCommChannel = dictionary.ContainsKey("SESSION") ? dictionary["SESSION"] : null;
            dm.VoiceCommChannel = dictionary.ContainsKey("VOICE") ? dictionary["VOICE"] : null;
            dm.Jobs = ParseJobs(dictionary["JOBS"]);

            if (dictionary.ContainsKey("RANKS"))
            {
                dm.TaggedRanks = ParseRanks(dictionary["RANKS"]);
            }

            return dm;
        }

        private ICollection<Job> ParseJobs(string jobString)
        {
            var jobs = jobString.Split(",").Select(jobId => int.Parse(jobId));

            return context.Jobs.Where(job => jobs.Contains(job.Id)).ToList();
        }

        private ICollection<Rank> ParseRanks(string rankString)
        {
            var ranks = context.Ranks.ToDictionary(r => r.RankLetter, r => r);

            var returnRanks = new List<Rank>();

            foreach (var rankLetter in rankString.Split(","))
            {
                returnRanks.Add(ranks[rankLetter]);
            }

            return returnRanks;
        }
    }
}