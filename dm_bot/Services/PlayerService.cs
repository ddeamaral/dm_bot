using System;
using dm_bot.Models;

namespace dm_bot.Services
{
    public class PlayerService
    {
        internal decimal AddGold(Player player, decimal amount)
        {
            var ts = new TradeService();
            ts.Buy(player, new Item() { GoldCost = amount });
            return player.TotalWealth;
        }

        internal int AddPips(Player user, decimal amount)
        {
            var originalPips = user.Pips;
            user.Pips += (int) amount;

            return user.Pips;
        }
    }
}