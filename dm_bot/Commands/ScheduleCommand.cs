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
                var tokens = message.Split(" ");

                // $schedule list
                if (tokens.Length == 1 && !string.IsNullOrWhiteSpace(tokens[0]))
                {
                    var allAvailabilities = context.DungeonMasterAvailabilities
                        .Include(dm => dm.TaggedRanks)
                        .Include(dm => dm.Jobs)
                        .Where(dma => dma.PlayDate > DateTime.Today)
                        .ToArray();

                    await RespondWithAvailabilities(allAvailabilities);
                    return;
                }

                var availability = await ParseScheduleRequest(message);

                availability.DungeonMasterUserName = Context.User.Username;

                var embedBuilder = new EmbedBuilder();
                embedBuilder.WithDescription(Templates.AvailabilityResponse(availability));
                await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());

                await ReplyAsync($"Your game has been scheduled @{Context.User.Mention}");
            }
            catch (ParseException exception)
            {
                await ReplyAsync($"Sorry {message}, we cannot process that request.");
            }
        }

        private async Task RespondWithAvailabilities(DungeonMasterAvailability[] allAvailabilities)
        {
            var sb = new StringBuilder();

            foreach (var dmAvail in allAvailabilities)
            {
                sb.Append(Templates.AvailabilityResponse(dmAvail));
            }

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithDescription(sb.ToString());
            await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        }

        public async Task<DungeonMasterAvailability> ParseScheduleRequest(string message)
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

            dungeonMasterAvailability = await ObjectFromDictionary(requestAttributes);

            return dungeonMasterAvailability;
        }

        private async Task<DungeonMasterAvailability> ObjectFromDictionary(Dictionary<string, string> dictionary)
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

            var hasDateParts = dictionary.ContainsKey("DATE") && dictionary.ContainsKey("TIME") && dictionary.ContainsKey("TZ");

            if (hasDateParts)
            {
                var playDate = $"{dictionary["DATE"]} {dictionary["TIME"]}";
                DateTime dt;

                if (DateTime.TryParse(playDate, out dt))
                {
                    dm.PlayDate = (DateTime) TimeZoneConversionService.ToUtc(dt);
                }
                else
                {
                    await ReplyAsync($"Sorry, {Context.User.Mention}, please enter a DATE, TIME and TZ");
                }
            }

            if (dictionary.ContainsKey("RANKS"))
            {
                dm.TaggedRanks = ParseRanks(dictionary["RANKS"]);
            }

            await context.DungeonMasterAvailabilities.AddAsync(dm);

            await context.SaveChangesAsync();

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