using System.Threading.Tasks;
using dm_bot.Contexts;
using Discord.Commands;

namespace dm_bot.Commands
{
    public class TradeCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext context;

        public TradeCommand(DMContext context)
        {
            this.context = context;
        }

        [Command("trade")]
        public async Task TradeAsync([Remainder] string message = null)
        {
            var tokens = message?.Split(" ");

            if (!(tokens.Length > 1) && string.IsNullOrWhiteSpace(tokens[0]))
            {
                var operation = tokens[0];
                switch (operation.ToLower())
                {
                    case "buy":
                        break;
                    case "sell":
                        break;
                    case "info":
                        break;
                    case "list":
                        break;
                    default:
                        break;
                }

            }

        }
    }
}