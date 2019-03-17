namespace dm_bot.Models
{
    public class InventoryItem
    {
        public int ID { get; set; }

        public int Quantity { get; set; }

        public Item Item { get; set; }
    }
}