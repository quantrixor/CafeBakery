using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeBakery.Model
{
    public class Menu
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public decimal Cost { get; set; }
        public TimeSpan PreparationTime { get; set; }
    }
}
