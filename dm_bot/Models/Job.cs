using System.ComponentModel.DataAnnotations;

namespace dm_bot.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string JobLink { get; set; }

        public string Author { get; set; }

        public int Difficulty { get; set; }
    }
}