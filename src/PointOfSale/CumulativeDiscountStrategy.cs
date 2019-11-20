using System;
using System.Linq;
using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale
{
    public class CumulativeDiscountStrategy : IPricingStrategy
    {
        private readonly IPricesStorage _storage;
        public PriceType PriceType { get; }

        public CumulativeDiscountStrategy(IPricesStorage storage)
        {
            _storage = storage;
        } 
        
        public decimal CalculatePrice(CheckSubItem item, PriceInfo defaultPrice)
        {
            //todo null checks
            //todo check for price info type
            return item.Quantity * defaultPrice.Price * item.PriceApplied.Price;
        }

        public CheckItem[] ApplyPrice(PriceInfo info, CheckItem[] items)
        {
            //todo null checks
            //todo check for price info type
            foreach (var item in items)
            {
                foreach (var sub in item.SubItems.Where(x => x.PriceApplied.Type == PriceType.DefaultPrice))
                {
                    sub.PriceApplied = info;
                }
            }

            var defaultPrice = items.Sum(x => x.Quantity * x.DefaultPrice.Price);
            
            //todo move it to close check method?
            _storage.UpdateAccumulatedAmount(info.Code, defaultPrice);

            return items;
        }
    }
}