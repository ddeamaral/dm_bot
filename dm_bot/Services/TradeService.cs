using System;
using System.Linq;
using dm_bot.Models;

namespace dm_bot.Services
{
    public class TradeService
    {
        public bool Buy(Player player, Item item)
        {
            if (!CanAfford(player, item))
            {
                return false;
            }

            var change = player.TotalWealth - item.Cost;

            UpdatePlayerWealth(player, change);

            return player.TotalWealth > 0;
        }

        public bool Sell(Player player, Item item)
        {
            if (player == null || item == null)
            {
                return false;
            }

            var change = player.TotalWealth + (item.Cost * 0.5m);

            UpdatePlayerWealth(player, change);

            return true;
        }

        private static void UpdatePlayerWealth(Player player, decimal change)
        {
            var coins = new []
            {
                new { name = "cp", nominal = 0.01m },
                new { name = "sp", nominal = 0.10m },
                new { name = "ep", nominal = 0.50m },
                new { name = "gp", nominal = 1m }
            };

            foreach (var coin in coins)
            {
                int count = (int) (change / coin.nominal);
                change -= count * coin.nominal;

                switch (coin.name)
                {
                    case "cp":
                        player.Copper = count;
                        continue;
                    case "sp":
                        player.Silver = count;
                        continue;
                    case "ep":
                        player.Electrum = count;
                        continue;
                    case "gp":
                        player.Gold = count;
                        continue;
                    default:
                        continue;
                }
            }
        }

        private bool CanAfford(Player player, Item item) => player.TotalWealth > item.Cost;
    }
}