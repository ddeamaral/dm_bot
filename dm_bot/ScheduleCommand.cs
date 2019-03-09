using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace dm_bot
{
    public class ScheduleCommand : ModuleBase<SocketCommandContext>
    {
        [Command("schedule")]
        public async Task ScheduleAsync([Remainder]string message = null)
        {
            try
            {
                var context = Context.User;

                var availability = ParseScheduleRequest(message);

                await ReplyAsync("Hello world!");
            }
            catch(ParseException exception)
            {
                await ReplyAsync($"Sorry {message}, we cannot process that request.");
            }
        }

        public DungeonMasterAvailability ParseScheduleRequest(string message)
        {
            var dungeonMasterAvailability = new DungeonMasterAvailability();

            return dungeonMasterAvailability;
        }
    }
}