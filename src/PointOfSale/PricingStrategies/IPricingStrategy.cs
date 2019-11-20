using PointOfSale.Models;

namespace PointOfSale.PricingStrategies
{
    public interface IPricingStrategy
    {
        PriceType PriceType { get; }
        
        decimal CalculatePrice(CheckSubItem item, PriceInfo defaultPrice);
        CheckItem[] ApplyPrice(PriceInfo info, CheckItem[] items);
    }
}