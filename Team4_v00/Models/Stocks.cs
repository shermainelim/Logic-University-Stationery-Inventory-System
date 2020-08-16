using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class Stocks
    {
        public List<Stock> stocks { get; set; }

        public Stocks()
        {
            stocks = new List<Stock>();
        }
    }
}
