using System;

namespace CafeBakery.Model
{
    public class Inventory
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public DateTime ProductionDate { get; set; }
        public decimal Volume { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Cost { get; set; }
        public string Supplier { get; set; }
    }
}
