using System.Collections.Generic;
using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale.DAL
{
    public class DiscountRepository : IDiscountRepository
    {
        private static readonly Dictionary<string, Discount> Discounts = new Dictionary<string, Discount>();
        
        public Discount GetDiscount(string code)
        {
            return Discounts.TryGetValue(code, out var discount) ? discount : null;
        }

        public void AddDiscount(Discount discount)
        {
            Discounts[discount.Code] = discount;
        }

        public void UpdateDiscount(Discount discount)
        {
            Discounts[discount.Code] = discount;
        }
    }
}