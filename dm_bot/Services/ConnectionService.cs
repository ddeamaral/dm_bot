using System;

namespace dm_bot.Services
{
    public class ConnectionService
    {
        /// <summary>
        /// Method to handle retrieving a discord token from the environment variables on a given machine
        /// </summary>
        /// <returns>A string value representing an api token</returns>
        public string GetDiscordToken ()
        {
            var token = Environment.GetEnvironmentVariable ("DISCORD_API_TOKEN_DM", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace (token))
            {
                token = Environment.GetEnvironmentVariable ("DISCORD_API_TOKEN_DM", EnvironmentVariableTarget.User);
            }

            if (string.IsNullOrWhiteSpace (token))
            {
                token = Environment.GetEnvironmentVariable ("DISCORD_API_TOKEN_DM", EnvironmentVariableTarget.Process);
            }

            return token;
        }

        /// <summary>
        /// method to handle extracting the database connection string from the environment variables
        /// </summary>
        /// <returns>A string representing a connection to a database</returns>
        public string GetDbConnection ()
        {
            var token = Environment.GetEnvironmentVariable ("DM_BOT_CONNECTION", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace (token))
            {
                token = Environment.GetEnvironmentVariable ("DM_BOT_CONNECTION", EnvironmentVariableTarget.User);
            }

            if (string.IsNullOrWhiteSpace (token))
            {
                token = Environment.GetEnvironmentVariable ("DM_BOT_CONNECTION", EnvironmentVariableTarget.Process);
            }

            return token;
        }
    }
}