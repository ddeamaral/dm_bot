using System.ComponentModel.DataAnnotations;

namespace dm_bot.Models
{
    public class Rank
    {
        [Key]
        public int Id { get; set; }

        public string RankName { get; set; }

        public string RankLetter { get; set; }

        public string RankMention { get; set; }
    }
}