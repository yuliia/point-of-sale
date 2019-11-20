using System.Collections.Generic;
using System.Linq;
using PointOfSale.Models;

namespace PointOfSale
{
    public class CheckItem
    {        public CheckItem(string code, int quantity)
        {
            Code = code;
            Quantity = quantity;
            SubItems = new List<CheckSubItem>();
            SubItems.Add(new CheckSubItem
            {
                Quantity = quantity
            });
        }
        public string Code { get; }

        public int Quantity { get; }
        
        public List<CheckSubItem> SubItems { get; }

        public PriceInfo DefaultPrice { get; set; }
    }
}