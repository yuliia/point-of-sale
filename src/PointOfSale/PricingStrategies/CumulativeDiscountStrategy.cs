using System.Linq;
using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale.PricingStrategies
{
    public class CumulativeDiscountStrategy : PricingStrategyBase
    {
        private readonly IPricesStorage _storage;

        public CumulativeDiscountStrategy(IPricesStorage storage) : base(PriceType.CumulativeDiscount)
        {
            _storage = storage;
        } 
        
        protected override decimal CalculatePriceImpl(CheckSubItem item, PriceInfo defaultPrice)
        {
            return item.Quantity * defaultPrice.Price * (1 -item.PriceApplied.Price);
        }

        protected override CheckItem[] ApplyPriceImpl(PriceInfo info, CheckItem[] items)
        {
            foreach (var item in items)
            {
                foreach (var sub in item.SubItems.Where(x => x.PriceApplied?.Type == PriceType.DefaultPrice))
                {
                    sub.PriceApplied = info;
                }
            }

            var defaultPrice = items.Sum(x => x.Quantity * (x.DefaultPrice?.Price ?? 0));
            
            //todo move it to close check method?
            _storage.UpdateAccumulatedAmount(info.Code, defaultPrice);

            return items;
        }
    }
}