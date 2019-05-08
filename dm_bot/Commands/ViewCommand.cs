using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Extensions;
using dm_bot.Models;
using dm_bot.Services;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace dm_bot.Commands
{
    public class ViewCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext db;

        public ViewCommand (DMContext db)
        {
            this.db = db;
        }

        [Command ("view")]
        public async Task View ([Remainder] string message)
        {
            var flags = new List<string> ()
            {
                "game"
            };

            var flagParser = new FlagParser (flags);
            var results = flagParser.ParseMessage (message);

            if (!results.ContainsKey ("game"))
            {
                await ReplyAsync ($"Sorry {this.Context.User.Mention}, could not find a game with that id");
                return;
            }

            var gameId = results["game"].ParseInt (-1);

            var game = db.Lobbies
                .Include (lobby => lobby.Player)
                .Include (lobby => lobby.Availability)
                .ThenInclude (availability => availability.ScheduledJobs)
                .Where (lobby => lobby.Id == gameId)
                .ToArray ();

            await ReplyWithPlayersAsync (game);
        }

        private async Task ReplyWithPlayersAsync (Lobby[] game)
        {
            if (game is null || game.Length == 0)
            {
                await ReplyAsync ($"Sorry {this.Context.User.Mention}, it looks like no players have joined that game, or doesn't exist.");
                return;
            }

            var gameStringBuilder = new StringBuilder ();

            var availability = game.First ().Availability;
            gameStringBuilder.AppendLine ($"DM: {availability.DungeonMasterUserName}");
        }
    }
}