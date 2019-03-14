using System.ComponentModel.DataAnnotations;

namespace dm_bot.Models {
    public class Item {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public float GoldCost { get; set; }

        public float SellValue { get; set; }
    }
}