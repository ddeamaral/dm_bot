using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Extensions;
using dm_bot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dm_bot.Commands {
    public class JobCommand : ModuleBase<SocketCommandContext> {
        private readonly DMContext _db;

        public JobCommand (DMContext context) {
            this._db = context;
        }

        [Command ("jobs")]
        public async Task AddJobAsync ([Remainder] string message = null) {
            try {
                var context = Context.User;

                var job = new Job ();

                await ReplyAsync ($"Added new job, '{job.Title}', to update the description, use `!dm jobs update {job.Id} \"Your description here\"`");
            } catch (ParseException exception) {
                await ReplyAsync ($"Sorry {message}, we cannot process that request.");
            }
        }
    }
}