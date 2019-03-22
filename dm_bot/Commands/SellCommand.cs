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
        private readonly IConfiguration _configuration;
        private readonly TradeService _tradeService;

        public SellCommand(DMContext context, IConfiguration configuration, TradeService tradeService)
        {
            this._db = context;
            this._configuration = configuration;
            this._tradeService = tradeService;
        }

        [Command("sell")]
        public async Task SellAsync(int itemId = -1, [Remainder] string message = null)
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

            var item = _db.Items.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                await ReplyAsync($"Sorry, that item does not appear to exist. Use `$item find <item name>` to find the item id. Then use `$buy <item id>`.");
                return;
            }

            if (item.IsTradeOnly)
            {
                await ReplyAsync($"Sorry, {item.Name} can't be sold {Context.User.Mention}. You could see if someone has posted it for trade! use `$trade help` for more info.");
                return;
            }

            if (!_tradeService.Sell(user, item))
            {
                await ReplyAsync($"Sorry, item could not be sold {Context.User.Mention}, please double check you have done it correctly. You have {user.DisplayWealth}");
                return;
            }

            _db.Players.Update(user);

            await _db.SaveChangesAsync();
        }
    }
}