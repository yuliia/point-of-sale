using System;
using System.Collections.Generic;
using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale
{
    public class PricesStorage : IPricesStorage
    {
        private readonly Dictionary<string, List<PriceInfo>> _prices = new Dictionary<string, List<PriceInfo>>();

        public IReadOnlyList<PriceInfo> GetPrices(string code)
        {
            return _prices.TryGetValue(code, out var prices)
                ? prices.ToArray()
                : new PriceInfo[0];
        }

        public void SetPrice(string code, int quantity, decimal price)
        {
            if (!_prices.TryGetValue(code, out var prices))
            {
                if (quantity > 1)
                {
                    throw new InvalidOperationException("Can't add volume price without price for single item.");
                }
            
                _prices[code] = new List<PriceInfo>
                {
                    new PriceInfo(code, quantity, price)
                };
                return;
            }
            
            var volumePrice = prices.Find(x => x.Quantity == quantity);
            if (volumePrice != null)
            {
                prices.Remove(volumePrice);
            }
            
            prices.Add(new PriceInfo(code, quantity, price));
        }
    }
}