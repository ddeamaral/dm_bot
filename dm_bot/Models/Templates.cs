using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace dm_bot.Models {
    public class Templates {

        public static string AvailabilityResponse (DungeonMasterAvailability dmRequest) {
            return $@"{dmRequest.DungeonMasterUserName}
            Time: {dmRequest.ChronusTimeLink} ({dmRequest.MinHours} - {dmRequest.MaxHours} hours)
            (optional)Type: 40% RP, 60% Combat
            Comms: #{dmRequest.ChatCommChannel} #{dmRequest.VoiceCommChannel}
            Jobs willing to run: JobFormatString (dmRequest.Jobs)
            Applicable Ranks: @{RankFormatString (dmRequest.TaggedRanks.ToList())}";
        }

        private static string RankFormatString (List<Rank> taggedRanks) {
            var sb = new StringBuilder ();

            for (int i = 0; i < taggedRanks.Count; i++) {
                sb.Append ($"{i+1}) @{taggedRanks[i].RankName}-Rank");
            }

            return sb.ToString ();
        }

        private static string JobFormatString (List<Job> jobs) {
            var sb = new StringBuilder ();

            for (int i = 0; i < jobs.Count; i++) {
                sb.Append ($"{i+1}) {jobs[i].Title} (Difficult: {jobs[i].Difficulty})");
            }

            return sb.ToString ();
        }
    }
}