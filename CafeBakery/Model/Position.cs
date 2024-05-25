using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeBakery.Model
{
    public class Position
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public decimal Salary { get; set; }
        public string Responsibilities { get; set; }
        public string Requirements { get; set; }
    }
}
