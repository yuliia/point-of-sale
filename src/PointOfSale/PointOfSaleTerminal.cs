using System;
using System.Collections.Generic;
using System.Linq;
using PointOfSale.Exceptions;
using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale
{
    public class PointOfSaleTerminal : IPointOfSaleTerminal
    {
        private readonly IPricesStorage _pricesStorage;
        private readonly Dictionary<string, IReadOnlyList<PriceInfo>> _pricesCache;
        private readonly Dictionary<string, int> _check;
            
        public PointOfSaleTerminal() : this(new PricesStorage())
        {
        }
        
        public PointOfSaleTerminal(IPricesStorage pricesStorage)
        {
            _pricesStorage = pricesStorage;
            _pricesCache = new Dictionary<string, IReadOnlyList<PriceInfo>>();
            _check = new Dictionary<string, int>();
        }

        /// <summary>
        /// Add product to check
        /// </summary>
        /// <param name="code"></param>
        /// <param name="quantity"></param>
        /// <exception cref="ArgumentNullException">When `code` is null or empty</exception>
        /// <exception cref="ArgumentException">When `quantity` is not positive number</exception>
        public void Scan(string code, int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            
            if (quantity <= 0)
            {
                throw new ArgumentException($"{nameof(quantity)} should be positive number.");
            }

            if (!_pricesCache.ContainsKey(code))
            {
                var prices = _pricesStorage.GetPrices(code);
                if (!prices.Any())
                {
                    throw new PriceNotFoundException(code);
                }

                _pricesCache[code] = prices;
            }
            
            if (_check.ContainsKey(code))
            {
                _check[code] += quantity;
                return;
            }

            _check[code] = quantity;
        }
        
        public void SetPrice(string code, int quantity, decimal price)
        {
            _pricesStorage.SetPrice(code, quantity, price);
        }

        public decimal GetTotal()
        {
            var total = 0m;
            
            foreach (var product in _check)
            {
                total += CalculatePriceWithPromotions(product.Value,
                    _pricesCache[product.Key]);
            }

            return total;
        }

        private decimal CalculatePriceWithPromotions(int quantity, IReadOnlyList<PriceInfo> priceInfos)
        {
            var sum = 0m;
            var quantityLeft = quantity;

            foreach (var promotion in priceInfos.OrderByDescending(x => x.Quantity))
            {
                if (promotion.Quantity > quantity)
                {
                    continue;
                }
                
                var volumesCount = (int)Math.Floor(quantityLeft / (decimal)promotion.Quantity);

                sum += volumesCount * promotion.Price;

                quantityLeft -= volumesCount * promotion.Quantity;
            }

            return sum;
        }
    }
}