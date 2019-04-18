using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dm_bot.Models
{
    public class DungeonMasterAvailability
    {
        [Key]
        public int Id { get; set; }

        public string DungeonMasterUserName { get; set; }

        public string ChronusTimeLink { get; set; }

        public string ChatCommChannel { get; set; }

        public string VoiceCommChannel { get; set; }

        public int MinHours { get; set; }

        public int MaxHours { get; set; }

        public int RoleplayingPercent { get; set; }

        public int CombatPercent { get; set; }

        public string Roll20Link { get; set; }

        public ICollection<Job> ScheduledJobs { get; set; }

        public ICollection<Rank> TaggedRanks { get; set; }

        public DateTime PlayDate { get; set; }

        public DateTimeOffset PlayDateOffset { get; set; }

        public string MiscellaneousText { get; set; }

    }
}