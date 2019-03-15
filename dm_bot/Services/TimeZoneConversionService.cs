using System;
using System.Collections.Generic;

namespace dm_bot.Services
{
    public class TimeZoneConversionService
    {
        private Dictionary<string, string> timezones = new Dictionary<string, string>() { { "", "" } };

        public static DateTime? ConvertDateTimeTo(DateTime input, string timezone = "EST")
        {
            DateTime? output = null;

            switch (timezone.ToUpper())
            {
                case "EST":
                    var utc = TimeZoneInfo.ConvertTimeToUtc(input);
                    var est = TimeZoneInfo.FindSystemTimeZoneById("EST");
                    output = TimeZoneInfo.ConvertTime(input, est);
                    break;
                default:
                    break;
            }

            return output;
        }
    }
}