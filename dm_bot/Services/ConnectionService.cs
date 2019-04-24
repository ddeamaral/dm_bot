using System;

namespace dm_bot.Services
{
    public class ConnectionService
    {
        public string GetDiscordToken()
        {
            var token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN_DM", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(token))
            {
                token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN_DM", EnvironmentVariableTarget.User);
            }

            return token;
        }
    }
}