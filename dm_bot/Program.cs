using System;
using System.Reflection;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dm_bot {
    class Program {
        static void Main (string[] args) => new Program ().RunBotAsync ().GetAwaiter ().GetResult ();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync () {
            using (var db = new DMContext ()) {
                db.SeedDatabase ();

                _client = new DiscordSocketClient ();
                _commands = new CommandService ();

                _services = new ServiceCollection ()
                    .AddSingleton (_client)
                    .AddSingleton (_commands)
                    .AddSingleton (db)
                    .BuildServiceProvider ();

                var connectionService = new ConnectionService ();

                await RegisterCommandsAsync ();

                await _client.LoginAsync (TokenType.Bot, connectionService.GetDiscordToken ());

                await _client.StartAsync ();

                await Task.Delay (-1);
            }

        }

        public async Task RegisterCommandsAsync () {
            _client.MessageReceived += OnMessageReceived;

            await _commands.AddModulesAsync (Assembly.GetEntryAssembly (), _services);
        }

        private async Task OnMessageReceived (SocketMessage arg) {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argumentPosition = 0;

            if (message.HasStringPrefix ("!dm ", ref argumentPosition) || message.HasMentionPrefix (_client.CurrentUser, ref argumentPosition)) {
                var context = new SocketCommandContext (_client, message);

                var result = await _commands.ExecuteAsync (context, argumentPosition, _services);

                if (!result.IsSuccess) {
                    Console.WriteLine ($"Command could not be executed: {result.ErrorReason}");
                }
            }
        }
    }
}