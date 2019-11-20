using System;
using System.Collections.Generic;
using System.Linq;
using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale
{
    public class PricesStorage : IPricesStorage //todo split to service and storage separately
    {
        private readonly Dictionary<string, List<PriceInfo>> _prices = new Dictionary<string, List<PriceInfo>>();
        private readonly Dictionary<string, CumulativeDiscount> _cumulativeDiscounts = new Dictionary<string, CumulativeDiscount>();
        
        // todo separate model for rules, separate method/class to get them
        private readonly (decimal lowerLimit, decimal? upperLimit, decimal percents)[] _cumulativeRules = new[]
        {
            (1000m, (decimal?)2000, 0.01m),
            (2000m, (decimal?)5000m, 0.03m),
            (5000m, (decimal?)10000m, 0.05m),
            (10000, (decimal?)null, 0.07m)
        };
        public IReadOnlyList<PriceInfo> GetPrices(string code)
        {
            return _prices.TryGetValue(code, out var prices)
                ? prices.ToArray()
                : new PriceInfo[0];
        }
        
        public void SetPrice(string code, int quantity, decimal price, PriceType type)
        {
            if (!_prices.TryGetValue(code, out var prices))
            {
                if (quantity > 1)
                {
                    throw new InvalidOperationException("Can't add volume price without price for single item.");
                }

                var newPrice = new PriceInfo(code, quantity, price, type);
                _prices[code] = new List<PriceInfo>
                {
                    newPrice
                };
                return;
            }
            
            var volumePrice = prices.Find(x => x.Quantity == quantity);
            if (volumePrice != null)
            {
                prices.Remove(volumePrice);
            }
            var priceInfo = new PriceInfo(code, quantity, price, type);

            prices.Add(priceInfo);
        }

        public void UpdateAccumulatedAmount(string code, decimal amount)
        {
            if (!_cumulativeDiscounts.TryGetValue(code, out var discount))
            {
                _cumulativeDiscounts[code] = new CumulativeDiscount
                {
                    Code = code
                };
                discount = _cumulativeDiscounts[code];
            }

            var ruleApplied =
                _cumulativeRules.FirstOrDefault(x =>
                    discount.AmountAccumulated >= x.lowerLimit
                    && (!x.upperLimit.HasValue || discount.AmountAccumulated < x.upperLimit));
            
            discount.AmountAccumulated += amount;

            if (discount.AmountAccumulated < ruleApplied.upperLimit)
            {
                return;
            }
            
            var ruleToApply = _cumulativeRules.FirstOrDefault(x =>
                discount.AmountAccumulated >= x.lowerLimit
                && (!x.upperLimit.HasValue || discount.AmountAccumulated < x.upperLimit));
            
            SetPrice(code, 1, ruleToApply.percents, PriceType.CumulativeDiscount);
        }
    }
}