using System.Linq;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Services;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace dm_bot.Commands
{
    public class BuyCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;
        private readonly IConfiguration _configuration;
        private readonly TradeService _tradeService;

        public BuyCommand (DMContext context, IConfiguration configuration, TradeService tradeService)
        {
            this._db = context;
            this._configuration = configuration;
            this._tradeService = tradeService;
        }

        [Command ("buy")]
        public async Task BuyAsync ([Remainder] string message = null)
        {
            // find the player
            var user = _db.Players
                .FirstOrDefault (p => p.DiscordMention == Context.User.Mention);

            // If we didn't find one, redirect them to staff to add them
            if (user == null)
            {
                var helpRole = _configuration.GetValue ("helpRoleName", "Staff");
                await ReplyAsync ($"Sorry, you don't appear to be in our system. Please talk to somone in {Context.Guild.Roles.First(role => role.Name == helpRole).Mention}");
                return;
            }

            var tokens = message.Split (" ");

            if (tokens == null || tokens.Length < 1)
            {
                await ReplyAsync ("Sorry, you seem to have not supplied an item id. Please use $item find <item name> to find the item id. Then use $buy <item id>");
                return;
            }

            var itemId = -1;

            if (!int.TryParse (tokens[0], out itemId) || itemId == -1)
            {
                await ReplyAsync ($"Sorry, that item does not appear to exist {Context.User.Mention}. Use '$item find <item name>' to find the item id. Then use '$buy <item id>'.");
                return;

            }

            var item = _db.Items.FirstOrDefault (i => i.Id == itemId);

            if (item == null)
            {
                await ReplyAsync ($"Sorry, that item does not appear to exist {Context.User.Mention}. Use $item find <item name> to find the item id. Then use $buy <item id>.");
                return;
            }

            if (item.IsTradeOnly)
            {
                await ReplyAsync ($"Sorry, {item.Name} can't be bought {Context.User.Mention}. You could see if someone has it and wants to trade.");
                return;
            }

            if (!_tradeService.Buy (user, item))
            {
                await ReplyAsync ($"Sorry, item could not be bought {Context.User.Mention}, please double check you have the funds. You have {user.Gold}gp, {user.Silver}sp, {user.Electrum}ep, {user.Copper}");
                return;
            }

            _db.Players.Update (user);

            await _db.SaveChangesAsync ();
        }
    }
}