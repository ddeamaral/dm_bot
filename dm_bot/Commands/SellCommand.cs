using System.Linq;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Services;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace dm_bot.Commands
{
    public class SellCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;
        private readonly IConfiguration configuration;
        private readonly TradeService tradeService;

        public SellCommand(DMContext context, IConfiguration configuration, TradeService tradeService)
        {
            this._db = context;
            this.configuration = configuration;
            this.tradeService = tradeService;
        }

        [Command("sell")]
        public async Task SellAsync(int itemId = -1, [Remainder] string message = null)
        {
            // find the player
            var user = _db.Players.FirstOrDefault(p => p.DiscordMention == Context.User.Mention);

            // If we didn't find one, redirect them to staff to add them
            if (user == null)
            {
                var helpRole = configuration.GetValue("helpRoleName", "Staff");
                await ReplyAsync($"Sorry, you don't appear to be in our system. Please talk to somone in {Context.Guild.Roles.First(role => role.Name == helpRole).Mention}");
                return;
            }

            var item = _db.Items.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                await ReplyAsync($"Sorry, that item does not appear to exist. Use $item find <item name> to find the item id. Then use $buy <item id>.");
                return;
            }

            if (item.IsTradeOnly)
            {
                await ReplyAsync($"Sorry, {item.Name} can't be bought {Context.User.Mention}. You could see if someone has it and wants to trade.");
                return;
            }

            if (!tradeService.Sell(user, item))
            {
                await ReplyAsync($"Sorry, item could not be sold {Context.User.Mention}, please double check you have done it correctly. You have {user.Gold}gp, {user.Silver}sp, {user.Electrum}ep, {user.Copper}");
                return;
            }

            _db.Players.Update(user);

            await _db.SaveChangesAsync();
        }
    }
}