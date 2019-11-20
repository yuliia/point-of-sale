using PointOfSale.Models;

namespace PointOfSale
{
    public class DefaultPriceStrategy : IPricingStrategy
    {
        public PriceType PriceType { get; } = PriceType.DefaultPrice;

        public decimal CalculatePrice(CheckSubItem item, PriceInfo defaultPrice)
        {
            return defaultPrice.Price * item.Quantity;
        }

        public CheckItem[] ApplyPrice(PriceInfo info, CheckItem[] items)
        {
            //todo null checks
            //todo check for price info type
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