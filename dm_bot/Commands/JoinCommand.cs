using System.Collections.Generic;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Extensions;
using dm_bot.Models;
using dm_bot.Services;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace dm_bot.Commands
{
    public class JoinCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext db;

        public JoinCommand (DMContext db)
        {
            this.db = db;
        }

        [Command ("join")]
        public async Task Join ([Remainder] string message)
        {
            // parse flags
            var flags = new List<string> () { "game" };
            var flagParser = new FlagParser (flags);
            var dictionary = flagParser.ParseMessage (message);

            var player = await db.Players.FirstOrDefaultAsync (p => p.UserId == this.Context.User.Id);

            if (player is null)
            {
                await ReplyAsync ($"Sorry {this.Context.User.Mention}, you are not registered. Please register yourself. See`$register help` for more information.");
                return;
            }

            if (!dictionary.ContainsKey ("game"))
            {
                await ReplyAsync ($"Sorry {player.DiscordMention}, you must specify a game. Use `$schedule list` to find your games id.");
                return;
            }

            var gameId = dictionary["game"].ParseInt (-1);

            var lobby = await db.DungeonMasterAvailabilities.FirstOrDefaultAsync (dmAvail => dmAvail.Id == gameId);

            if (lobby is null)
            {
                await ReplyAsync ($"Sorry {player.DiscordMention}, that game could not be found.  Use `$schedule list` to find your games id.");
                return;
            }

            var newLobbyEntry = new Lobby ();
            newLobbyEntry.AvailabilityId = lobby.Id;
            newLobbyEntry.PlayerId = player.Id;

            await db.Lobbies.AddAsync (newLobbyEntry);
            await db.SaveChangesAsync ();

            await ReplyAsync ($"You have signed up successfully, {player.DiscordMention}.");
        }
    }
}