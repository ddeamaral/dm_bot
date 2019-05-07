using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dm_bot.Models
{
    public class Lobby
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey ("Availability")]
        public int AvailabilityId { get; set; }

        public virtual DungeonMasterAvailability Availability { get; set; }

        public int PlayerId { get; set; }

        [ForeignKey ("PlayerId")]
        public virtual Player Player { get; set; }
    }
}