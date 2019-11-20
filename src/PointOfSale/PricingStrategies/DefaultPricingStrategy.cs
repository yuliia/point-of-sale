using PointOfSale.Models;

namespace PointOfSale.PricingStrategies
{
    public class DefaultPricingStrategy : PricingStrategyBase
    {
        public DefaultPricingStrategy() : base(PriceType.DefaultPrice)
        {
            
        }

        protected override decimal CalculatePriceImpl(CheckSubItem item, PriceInfo defaultPrice)
        {
            return defaultPrice.Price * item.Quantity;
        }

        protected override CheckItem[] ApplyPriceImpl(PriceInfo info, CheckItem[] items)
        {
            foreach (var item in items)
            {
                if (item.Code != info.Code) continue;
                item.DefaultPrice = info;
                item.SubItems.ForEach(x =>
                {
                    if (x.PriceApplied != null)
                    {
                        return;
                    }
                    x.PriceApplied = info;
                });
            }

            return items;
        }
    }
}