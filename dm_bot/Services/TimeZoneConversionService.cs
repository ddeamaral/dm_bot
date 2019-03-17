using System;
using System.Collections.Generic;

namespace dm_bot.Services
{
    public class TimeZoneConversionService
    {
        private Dictionary<string, string> timezones = new Dictionary<string, string>() { { "", "" } };

        public static DateTime? ToUtc(DateTime input)
        {
            return input.ToUniversalTime();
        }
    }
}