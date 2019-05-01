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
        private readonly DMContext _db;

        public ScheduleCommand (DMContext context)
        {
            this._db = context;
        }

        [Command ("schedule")]
        public async Task ScheduleAsync ([Remainder] string message = null)
        {
            try
            {
                var tokens = message.Split (" ");

                // $schedule list OR $schedule help
                if (tokens.Length == 1 && !string.IsNullOrWhiteSpace (tokens[0]))
                {
                    if (tokens[0].ToLower () == "help")
                    {
                        await PrintHelp ();
                        return;
                    }

                    var allAvailabilities = _db.DungeonMasterAvailabilities
                        .Include (dm => dm.ScheduledJobs)
                        .Where (dma => dma.PlayDate > DateTime.Today)
                        .ToArray ();

                    await RespondWithAvailabilities (allAvailabilities);
                    return;
                }

                var availability = await ParseScheduleRequest (message);

                availability.DungeonMasterUserName = Context.User.Username;

                var embedBuilder = new EmbedBuilder ();
                embedBuilder.WithDescription (Templates.AvailabilityResponse (availability));
                await Context.Channel.SendMessageAsync ("", false, embedBuilder.Build ());

                await ReplyAsync ($"Your game has been scheduled @{Context.User.Mention}");
            }
            catch (ParseException exception)
            {
                await ReplyAsync ($"Sorry {message}, we cannot process that request.");
            }
        }

        private async Task RespondWithAvailabilities (DungeonMasterAvailability[] allAvailabilities)
        {
            if (allAvailabilities.Length == 0)
            {
                await ReplyAsync ("There are no games scheduled for the future right now");
                return;
            }

            var sb = new StringBuilder ();

            foreach (var dmAvail in allAvailabilities)
            {
                sb.Append (Templates.AvailabilityResponse (dmAvail));
            }

            var embedBuilder = new EmbedBuilder ();
            embedBuilder.WithDescription (sb.ToString ());
            await Context.Channel.SendMessageAsync ("", false, embedBuilder.Build ());
        }

        public async Task<DungeonMasterAvailability> ParseScheduleRequest (string message)
        {
            var flagParser = new FlagParser (flags: new List<string> ()
            {
                "time",
                "rp",
                "max",
                "min",
                "combat",
                "voice",
                "roll20",
                "misc",
                "date",
                "tz",
                "session",
                "jobs"
            });

            var dictionary = flagParser.ParseMessage (message);

            var dungeonMasterAvailability = await ObjectFromDictionary (dictionary);

            return dungeonMasterAvailability;
        }

        /// <summary>
        /// Prints the help message
        /// </summary>
        /// <returns>An awaitable task</returns>
        private async Task PrintHelp ()
        {

            var sb = new StringBuilder ();

            sb.AppendLine ("To schedule a game, use the following command `$schedule` with the following options:");
            sb.AppendLine ("**Required data**:");
            sb.AppendLine ("-time=<Chronus link>");
            sb.AppendLine ("-min=<Min Hours for session>");
            sb.AppendLine ("-max=<Max Hours for session>");
            sb.AppendLine ("-date=<Date in MM/DD/YYYY>");
            sb.AppendLine ("-time=<Time in 24 hour HH:MM [AM/PM]>");
            sb.AppendLine ("-timezone=<Your time zone>");
            sb.AppendLine ("-jobs=<Job Ids separated by commas (no spaces, use $jobs list for id)>");
            sb.AppendLine ("-ranks=<Rank letters separated by commas (no spaces)>");
            sb.AppendLine ("-voice=<Voice channel>");
            sb.AppendLine ("-session=<Text chat channel>");
            sb.AppendLine ("-roll20=<roll20 game link>");
            sb.AppendLine ("**Optional data**:");
            sb.AppendLine ("-rp=<RP %>");
            sb.AppendLine ("-combat=<Combat %>");

            sb.AppendLine ("**Example**: $schedule -time http://a.chronus.eu/18AC248 -session Session10 -voice VC10 -min 4 -max 4 -rp=40 -combat 60 -jobs 1,2,3 -ranks A,F -misc Then here is some additional content you can have that will just show up here.");

            await ReplyAsync (sb.ToString ());
        }

        private async Task<DungeonMasterAvailability> ObjectFromDictionary (Dictionary<string, string> dictionary)
        {
            var dm = new DungeonMasterAvailability ();

            dm.ChronusTimeLink = dictionary.ContainsKey ("TIMELINK") ? dictionary["TIMELINK"] : null;
            dm.RoleplayingPercent = dictionary.ContainsKey ("RP") ? dictionary["RP"].ParseInt (-1) : -1;
            dm.MaxHours = dictionary.ContainsKey ("MAX") ? dictionary["MAX"].ParseInt () : -1;
            dm.MinHours = dictionary.ContainsKey ("MIN") ? dictionary["MIN"].ParseInt () : -1;
            dm.CombatPercent = dictionary.ContainsKey ("COMBAT") ? dictionary["COMBAT"].ParseInt () : -1;
            dm.ChatCommChannel = dictionary.ContainsKey ("SESSION") ? dictionary["SESSION"] : null;
            dm.VoiceCommChannel = dictionary.ContainsKey ("VOICE") ? dictionary["VOICE"] : null;
            dm.ScheduledJobs = dictionary.ContainsKey ("JOBS") ? ParseJobs (dictionary["JOBS"]) : null;
            dm.Roll20Link = dictionary.ContainsKey ("ROLL20LINK") ? dictionary["ROLL20LINK"] : null;
            dm.MiscellaneousText = dictionary.ContainsKey ("MISC") ? dictionary["MISC"] : null;

            var hasDateParts = dictionary.ContainsKey ("DATE") && dictionary.ContainsKey ("TIME") && dictionary.ContainsKey ("TZ");

            // Parse out the date for the game to expire
            if (hasDateParts)
            {
                var playDate = $"{dictionary["DATE"]} {dictionary["TIME"]}";
                DateTime dt;

                if (DateTime.TryParse (playDate, out dt))
                {
                    dm.PlayDateOffset = new DateTimeOffset (dt);
                    dm.PlayDate = (DateTime) TimeZoneConversionService.ToUtc (dt);
                }
                else
                {
                    await ReplyAsync ($"Sorry, {Context.User.Mention}, please enter a DATE, TIME and TZ");
                }
            }

            await _db.DungeonMasterAvailabilities.AddAsync (dm);

            await _db.SaveChangesAsync ();

            return dm;
        }

        private ICollection<Job> ParseJobs (string jobString)
        {
            var jobs = jobString.Split (",").Select (jobId => int.Parse (jobId));

            return _db.Jobs.Where (job => jobs.Contains (job.Id)).ToList ();
        }
    }
}