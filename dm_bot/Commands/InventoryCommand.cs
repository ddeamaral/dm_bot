using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Models;
using dm_bot.Services;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace dm_bot.Commands
{
    public class InventoryCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;
        private readonly IConfiguration _configuration;
        private readonly TradeService _tradeService;

        public InventoryCommand(DMContext context, IConfiguration configuration, TradeService tradeService)
        {
            this._db = context;
            this._configuration = configuration;
            this._tradeService = tradeService;
        }

        [Command("inventory")]
        public async Task InventoryAsync(string command = "list", int itemId = -1, int quantity = 1, [Remainder] string message = null)
        {
            // find the player
            var user = _db.Players.FirstOrDefault(p => p.DiscordMention == Context.User.Mention);

            // If we didn't find one, redirect them to staff to add them
            if (user == null)
            {
                var helpRole = _configuration.GetValue("helpRoleName", "Staff");
                await ReplyAsync($"Sorry, you don't appear to be in our system. Please talk to somone in {Context.Guild.Roles.First(role => role.Name == helpRole).Mention}");
                return;
            }

            switch (command.ToLower())
            {
                case "gold":
                    await ShowPlayerGold(user);
                    break;
                default:
                    await ShowHelpMessage();
                    return;
            }
        }

        private async Task ShowPlayerGold(Player user)
        {
            await ReplyAsync($"You have {user.DisplayWealth}, {Context.User.Mention}");
        }

        private Task ShowHelpMessage()
        {
            throw new NotImplementedException();
        }
    }
}