using System.Collections.Generic;
using System.Linq;
using dm_bot.Models;
using Microsoft.EntityFrameworkCore;

namespace dm_bot.Contexts
{
    public class DMContext : DbContext
    {
        public DbSet<DungeonMasterAvailability> DungeonMasterAvailabilities { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<Rank> Ranks { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=dm.db");
        }

        public void SeedDatabase()
        {
            this.Database.EnsureCreated();

            if (Ranks.Count() == 0)
            {
                var ranks = new string[] { "F", "D", "C", "B", "A", "S" };

                foreach (var rank in ranks)
                {
                    Ranks.Add(new Rank()
                    {
                        RankLetter = rank,
                            RankName = $"{rank} Rank"
                    });
                }
            }

            SaveChanges();
        }
    }
}