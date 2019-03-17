using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Services;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace dm_bot.Commands
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;
        private readonly IConfiguration configuration;
        private readonly TradeService tradeService;

        public HelpCommand(DMContext context, IConfiguration configuration, TradeService tradeService)
        {
            this._db = context;
            this.configuration = configuration;
            this.tradeService = tradeService;
        }

        [Command("help")]
        public async Task BuyAsync([Remainder] string message = null)
        {
            var helpStringBuilder = new StringBuilder();

            helpStringBuilder.AppendLine("Available commands with individual help options:");
            helpStringBuilder.AppendLine("$jobs help");
            helpStringBuilder.AppendLine("$buy help");
        }
    }
}