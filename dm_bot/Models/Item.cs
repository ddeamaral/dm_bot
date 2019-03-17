using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dm_bot.Models
{
    public class Item
    {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal GoldCost { get; set; }

        public decimal CopperCost { get; set; }

        public decimal SilverCost { get; set; }

        public decimal ElectrumCost { get; set; }

        public bool IsTradeOnly { get; set; }

        public decimal SellValue { get; set; }

        [NotMapped]
        public string CoinType
        {
            get
            {
                if (this.GoldCost > 0)
                {
                    return "gp";
                }

                if (this.SilverCost > 0)
                {
                    return "sp";
                }

                if (this.CopperCost > 0)
                {
                    return "cp";
                }

                if (this.ElectrumCost > 0)
                {
                    return "ep";
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public decimal Cost
        {
            get
            {

                if (this.GoldCost > 0)
                {
                    return this.GoldCost;
                }

                if (this.SilverCost > 0)
                {
                    return this.SilverCost / 10;
                }

                if (this.CopperCost > 0)
                {
                    return this.CopperCost / 100;
                }

                if (this.ElectrumCost > 0)
                {
                    return this.ElectrumCost / 2;
                }

                return -1;
            }
        }

        [NotMapped]
        public string DisplayValue
        {
            get
            {
                if (this.GoldCost > 0)
                {
                    return $"{(int)this.GoldCost}gp";
                }

                if (this.SilverCost > 0)
                {
                    return $"{(int)this.SilverCost}sp";
                }

                if (this.CopperCost > 0)
                {
                    return $"{(int)this.CopperCost}cp";
                }

                if (this.ElectrumCost > 0)
                {
                    return $"{(int)this.ElectrumCost}ep";
                }

                return "No value";
            }
        }
    }
}