using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace dm_bot.Models
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Deadly
    }

    [Owned]
    public class Job
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string JobLink { get; set; }

        public string Author { get; set; }

        public Difficulty Difficulty { get; set; }

        public string FirstApproval { get; set; }

        public string SecondApproval { get; set; }

        public ICollection<Rank> TaggedRanks { get; set; }
    }
}