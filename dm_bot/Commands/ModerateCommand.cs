using System;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace dm_bot.Commands
{
    public class ModerateCommand : ModuleBase<SocketCommandContext>
    {
        [Command("moderate")]
        public async Task ModerateAsync(SocketUser targetUser, string moderateType, string data, [Remainder] string message = null)
        {
            switch (moderateType.ToLower().Trim())
            {
                case "pips":
                    await CalculatePlayerPips(targetUser);
                    break;
                case "gold":
                    await CalculatePlayerGold(targetUser);
                    await EchoGoldCalculation();
                    break;

                case "help":
                    await ShowHelpMessage();
                    break;
                default:
                    break;
            }
        }

        private async Task ShowHelpMessage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Use `$moderate <@ Mention User> gold <gold amount (prefix with + or - i.e. -100gp or +100gp)>` to modify a users gold.");
            sb.AppendLine("Use `$moderate <@ Mention User> pips <pip amount (prefix with + or - i.e. -1 or +1)>` to modify a users pips.");

            await ReplyAsync(sb.ToString());
        }

        private Task EchoGoldCalculation()
        {

        }

        private Task CalculatePlayerGold(SocketUser user)
        {
            throw new NotImplementedException();
        }

        private Task CalculatePlayerPips(SocketUser user)
        {
            throw new NotImplementedException();
        }
    }
}