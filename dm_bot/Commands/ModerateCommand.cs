using System;
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
    public class ModerateCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;

        public ModerateCommand(DMContext _db)
        {
            this._db = _db;
        }

        [Command("moderate")]
        public async Task ModerateAsync(SocketUser targetUser, string moderateType, string data, [Remainder] string message = null)
        {
            var player = _db.Players.FirstOrDefault(p => p.UserId == targetUser.Id);

            if (player == null)
            {
                await ReplyAsync($"Could not find {targetUser.Mention} in the system. Please use the `$register help` command for more information.");
                return;
            }

            var amount = ParseAmount(data);

            if (amount == -decimal.MinValue)
            {
                await ReplyAsync($"Could not parse amount for {targetUser.Mention}, please double check your command is correct {Context.User.Mention}");
                return;
            }

            var operation = moderateType.ToLower().Trim();

            switch (operation)
            {
                case "pips":
                    await AddPipsToPlayer(player, amount);
                    break;
                case "gold":
                    await AddGoldToPlayer(player, amount);
                    break;

                case "help":
                    await ShowHelpMessage();
                    break;
                default:
                    break;
            }
            if (operation == "pips" || operation == "gold")
            {
                await EchoOperation(player, Context.User, amount, operation);
            }
        }

        private decimal ParseAmount(string data)
        {
            decimal amount = 0;

            if (!decimal.TryParse(data, out amount))
            {
                return -decimal.MinValue;
            }

            return amount;
        }

        private async Task ShowHelpMessage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Use `$moderate <@ Mention User> gold <gold amount (prefix with + or - i.e. -100gp or +100gp)>` to modify a users gold.");
            sb.AppendLine("Use `$moderate <@ Mention User> pips <pip amount (prefix with + or - i.e. -1 or +1)>` to modify a users pips.");

            await ReplyAsync(sb.ToString());
        }

        private async Task EchoOperation(Player player, SocketUser user, decimal amount, string amountType)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{user.Mention} has {(amount > 0 ? "given" : "taken")} {amount} {amountType} to {player.DiscordMention}");

            await ReplyAsync(sb.ToString());
        }

        private async Task AddGoldToPlayer(Player user, decimal amount)
        {
            var ps = new PlayerService();

            ps.AddGold(user, amount);

            _db.Players.Update(user);

            await _db.SaveChangesAsync();
        }

        private async Task AddPipsToPlayer(Player user, decimal amount)
        {
            var ps = new PlayerService();

            ps.AddPips(user, amount);

            _db.Players.Update(user);

            await _db.SaveChangesAsync();
        }
    }
}