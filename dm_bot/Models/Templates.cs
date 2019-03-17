using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace dm_bot.Models
{
    public class Templates
    {

        public static string AvailabilityResponse(DungeonMasterAvailability dmRequest)
        {
            return $@"
            DM:{dmRequest.DungeonMasterUserName}
            Time: {dmRequest.ChronusTimeLink} ({dmRequest.MinHours} - {dmRequest.MaxHours} hours)
            {(dmRequest.RoleplayingPercent > 0 || dmRequest.CombatPercent > 0 ? $"Combat: {dmRequest.CombatPercent}%, RP: {dmRequest.RoleplayingPercent}" : "")} 
            Comms: #{dmRequest.ChatCommChannel} #{dmRequest.VoiceCommChannel}
            Jobs willing to run: {JobFormatString (dmRequest.Jobs)}
            Applicable Ranks: {RankFormatString (dmRequest.TaggedRanks.ToList())}";
        }

        private static string RankFormatString(List<Rank> taggedRanks)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < taggedRanks.Count; i++)
            {
                sb.Append($"{i+1}) @{taggedRanks[i].RankLetter}-Rank ");
            }

            return sb.ToString();
        }

        private static string JobFormatString(ICollection<Job> jobs)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < jobs.Count; i++)
            {
                sb.Append($"{i+1}) {jobs.ElementAt(i).Title} (Difficult: {jobs.ElementAt(i).Difficulty})");
            }

            return sb.ToString();
        }
    }
}