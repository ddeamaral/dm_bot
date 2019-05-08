using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace dm_bot.Services
{
    public class FlagParser
    {
        private readonly IEnumerable<string> flags;

        public FlagParser (IEnumerable<string> flags)
        {
            this.flags = flags;
        }

        /// <summary>
        /// Parses a string for a given list of flags
        /// </summary>
        /// <param name="message">Message to be parsed for flags</param>
        /// <returns>A Dictionary<string, string></returns>
        public Dictionary<string, string> ParseMessage (string message)
        {
            var results = new Dictionary<string, string> ();
            var ignoreCase = StringComparison.CurrentCultureIgnoreCase;

            if (string.IsNullOrWhiteSpace (message))
            {
                return results;
            }

            var flagIndexes = new List < (int flagIndex, string flag) > ();

            if (flags != null && flags.Count () != 0)
            {
                // Find all indexes of the flags
                foreach (var flag in flags)
                {
                    if (message.IndexOf ($"-{flag}", ignoreCase) >= 0)
                    {
                        var indexOfFlag = message.IndexOf ($"-{flag}", ignoreCase);
                        flagIndexes.Add ((indexOfFlag, flag));
                    }
                }

                // Sort so we can process linearly
                flagIndexes.Sort ();

                for (int i = 0; i < flagIndexes.Count; i++)
                {
                    var flagIndex = flagIndexes[i].flagIndex;

                    // Get the last flag, without going out of range i.e. -add txt (-list more txt)
                    if (i == flagIndexes.Count - 1)
                    {
                        var lastIndex = flagIndex + flagIndexes[i].flag.Length + 1;
                        var lastFlagIndex = message.Length - lastIndex;
                        var lastFlagMessage = message
                            .Substring (lastIndex, lastFlagIndex)
                            .Trim ();

                        results.Add (flagIndexes[i].flag, lastFlagMessage);

                        continue;
                    }

                    // Get all other flags but the last
                    int nextFlagIndex = flagIndexes[i + 1].flagIndex;
                    int startIndex = flagIndex + flagIndexes[i].flag.Length + 1;

                    var text = message.Substring (startIndex, nextFlagIndex - startIndex).Trim ();
                    results.Add (flagIndexes[i].flag, text);
                }
            }

            return results;
        }
    }
}