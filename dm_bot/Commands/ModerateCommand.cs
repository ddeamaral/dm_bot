using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Models;
using dm_bot.Services;
using Discord.Commands;
using Discord.WebSocket;

namespace dm_bot.Commands
{
    /// <summary>
    /// Command for moderating how much gold/pips each player has
    /// </summary>
    /// <typeparam name="SocketCommandContext"></typeparam>
    public class ModerateCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;

        public ModerateCommand (DMContext _db)
        {
            this._db = _db;
        }

        [Command ("moderate")]
        public async Task ModerateManyAsync (string moderateType, string data, [Remainder] string message = null)
        {
            List<SocketUser> users = new List<SocketUser> (this.Context.Message.MentionedUsers);

            var userIds = users.Select (user => user.Id).ToList ();

            var dbUsers = _db.Players.Where (user => userIds.Contains (user.UserId)).ToList ();

            List<Player> newPlayers = new List<Player> ();

            // add users if don't exist
            foreach (var user in users)
            {
                if (!dbUsers.Any (dbUser => dbUser.UserId == user.Id))
                {
                    // create a new one and add it to the list to be added
                    var player = new Player ()
                    {
                    DiscordMention = user.Mention,
                    UserId = user.Id,
                    Pips = 0,
                    Copper = 0,
                    Electrum = 0,
                    Gold = 0,
                    Silver = 0

                    };

                    newPlayers.Add (player);
                }
            }

            if (newPlayers.Count > 0)
            {
                // add the list of new players to the database asynchronously
                await _db.Players.AddRangeAsync (newPlayers);
                await _db.SaveChangesAsync ();
            }

            // Parse the request for each tagged user
            foreach (var user in dbUsers)
            {
                await ParseRequest (user, users.FirstOrDefault (u => u.Id == user.UserId), moderateType, data);
            }
        }

        [Command ("moderate")]
        public async Task ModerateAsync (SocketUser targetUser, string moderateType, string data, [Remainder] string message = null)
        {
            var player = _db.Players.FirstOrDefault (p => p.UserId == targetUser.Id);

            await ParseRequest (player, targetUser, moderateType, data);
        }

        private async Task ParseRequest (Player player, SocketUser targetUser, string moderateType, string data)
        {
            if (player == null)
            {
                await ReplyAsync ($"Could not find {targetUser.Mention} in the system. Please use the `$register help` command for more information.");
                return;
            }

            var amount = ParseAmount (data);

            if (amount == -decimal.MinValue)
            {
                await ReplyAsync ($"Could not parse amount for {targetUser.Mention}, please double check your command is correct {Context.User.Mention}");
                return;
            }

            var operation = moderateType.ToLower ().Trim ();

            switch (operation)
            {
                case "pips":
                    await AddPipsToPlayer (player, amount);
                    break;

                case "gold":
                    await AddGoldToPlayer (player, amount);
                    break;

                case "help":
                    await ShowHelpMessage ();
                    break;
                default:
                    break;
            }

            if (operation == "pips" || operation == "gold")
            {
                await EchoOperation (player, Context.User, amount, operation);
            }
        }

        private decimal ParseAmount (string data)
        {
            decimal amount = 0;

            if (!decimal.TryParse (data, out amount))
            {
                return -decimal.MinValue;
            }

            return amount;
        }

        private async Task ShowHelpMessage ()
        {
            var sb = new StringBuilder ();

            sb.AppendLine ("Use `$moderate <@ Mention User> gold <gold amount (prefix with + or - i.e. -100gp or +100gp)>` to modify a users gold.");
            sb.AppendLine ("Use `$moderate <@ Mention User> pips <pip amount (prefix with + or - i.e. -1 or +1)>` to modify a users pips.");

            await ReplyAsync (sb.ToString ());
        }

        private async Task EchoOperation (Player player, SocketUser user, decimal amount, string amountType)
        {
            var sb = new StringBuilder ();

            sb.AppendLine ($"{user.Mention} has {(amount > 0 ? "given" : "taken")} {amount} {amountType} to {player.DiscordMention}");

            await ReplyAsync (sb.ToString ());
        }

        private async Task AddGoldToPlayer (Player user, decimal amount)
        {
            var ps = new PlayerService ();

            ps.AddGold (user, amount);

            _db.Players.Update (user);

            await _db.SaveChangesAsync ();
        }

        private async Task AddPipsToPlayer (Player user, decimal amount)
        {
            var ps = new PlayerService ();

            ps.AddPips (user, amount);

            _db.Players.Update (user);

            await _db.SaveChangesAsync ();
        }
    }
}