using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dm_bot.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        public ulong UserId;

        public string CharacterName { get; set; }

        public string DiscordMention { get; set; }

        public decimal Gold { get; set; }

        public decimal Silver { get; set; }

        public decimal Copper { get; set; }

        public decimal Electrum { get; set; }

        public int Pips { get; set; }

        public decimal TotalWealth
        {
            get
            {
                var total = 0m;

                total += Copper / 100;
                total += Silver / 100;
                total += Electrum / 2;
                total += Gold;

                return total;
            }
        }

        public string DisplayWealth => $"{this.Gold}gp {this.Electrum}ep {this.Silver}sp {this.Copper}cp";
    }
}