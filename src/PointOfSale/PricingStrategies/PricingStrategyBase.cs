using System;
using PointOfSale.Models;

namespace PointOfSale.PricingStrategies
{
    public abstract class PricingStrategyBase : IPricingStrategy
    { 
        public PriceType PriceType { get; }

        protected PricingStrategyBase(PriceType priceType)
        {
            PriceType = priceType;
        }

        public decimal CalculatePrice(CheckSubItem item, PriceInfo defaultPrice)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (defaultPrice == null)
            {
                throw new ArgumentNullException(nameof(defaultPrice));
            }
            
            return CalculatePriceImpl(item, defaultPrice);
        }

        public CheckItem[] ApplyPrice(PriceInfo info, CheckItem[] items)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (info.Type != PriceType)
            {
                throw new InvalidOperationException($"Can't apply price of type {info.Type}");
            }
            
            if (items == null)
            {
                throw new ArgumentNullException();
            }

            return ApplyPriceImpl(info, items);
        }

        protected abstract decimal CalculatePriceImpl(CheckSubItem item, PriceInfo defaultPrice);
        protected abstract CheckItem[] ApplyPriceImpl(PriceInfo info, CheckItem[] items);
    }
}