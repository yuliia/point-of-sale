using System.Collections.Generic;
using System.Linq;
using PointOfSale.Models;

namespace PointOfSale
{
    public class CheckItem
    {
        public string Code { get; set; }

        public int Quantity { get; set; }
        
        public List<CheckSubItem> SubItems { get; set; } = new List<CheckSubItem>();

        public PriceInfo DefaultPrice { get; set; }
    }
}