using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace dm_bot.Models
{
    [Owned]
    public class Job
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string JobLink { get; set; }

        public string AuthorMention { get; set; }

        public ulong AuthorId { get; set; }

        public string Difficulty { get; set; }

        public string FirstApproval { get; set; }

        public ulong FirstApprovalId { get; set; }

        public string SecondApproval { get; set; }

        public ulong SecondApprovalId { get; set; }

        public ICollection<Rank> TaggedRanks { get; set; }

        public bool Personal { get; internal set; }

        public bool Repeatable { get; internal set; }
    }
}