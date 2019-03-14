using System.Threading.Tasks;
using dm_bot.Contexts;
using Discord.Commands;

namespace dm_bot.Commands {
    public class TradeCommand : ModuleBase<SocketCommandContext> {
        private readonly DMContext context;

        public TradeCommand (DMContext context) {
            this.context = context;
        }

        [Command ("buy")]
        public async Task BuyAsync ([Remainder] string message = null) {

        }

        [Command ("sell")]
        public async Task SellAsync ([Remainder] string message = null) {

        }

        [Command ("item")]
        public async Task ItemDetailsAsync ([Remainder] string message = null) {

        }
    }
}