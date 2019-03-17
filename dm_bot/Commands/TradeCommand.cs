using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dm_bot.Contexts;
using Discord;
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

        [Command("item")]
        public async Task ItemAsync(string command = "list", [Remainder] string message = null)
        {
            await ShowAllItems();
        }

        private async Task ShowAllItems()
        {
            var sb = new StringBuilder();

            foreach (var item in context.Items.OrderBy(i => i.Name).ToList())
            {
                sb.AppendLine($"{item.Id}) {item.Name} {item.DisplayValue}");
                if (sb.Length > 1900)
                {
                    await this.Context.User.SendMessageAsync(sb.ToString());
                    sb.Clear();
                }
            }

        }

        [Command("trade")]
        public async Task TradeAsync(string command = "list", [Remainder] string message = null)
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