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

        /// <summary>
        /// Resets state to default
        /// </summary>
        public void CloseCheck()
        {
            _check.Clear();
            _pricesCache.Clear();
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

        public decimal GetTotal1()
        {
            return GetTotal();
        }

        private decimal CalculatePriceWithPromotions(int quantity, IReadOnlyList<PriceInfo> priceInfos)
        {
            var sum = 0m;
            var quantityLeft = quantity;

            foreach (var priceInfo in priceInfos.OrderByDescending(x => x.Quantity))
            {
                if (priceInfo.Quantity > quantity)
                {
                    continue;
                }
                
                var volumesCount = (int)Math.Floor(quantityLeft / (decimal)priceInfo.Quantity);

                sum += volumesCount * priceInfo.Price;

                quantityLeft -= volumesCount * priceInfo.Quantity;
            }

            return sum;
        }
    }

    public class CheckItem
    {
        public string Code { get; set; }

        public int Quantity { get; set; }

        public decimal CurrentPrice { get; set; }
        public PriceInfo[] PricesToApply { get; set; }
        public PriceInfo[] PricesApplied { get; set; }
    }
    
    public interface IPricingStrategy
    {
        PriceType PriceType { get; }
        decimal CalculatePrice(PriceInfo info, CheckItem[] items);
    }

    public class DefaultPriceStrategy : IPricingStrategy
    {
        public PriceType PriceType { get; } = PriceType.Price;
        public decimal CalculatePrice(PriceInfo info, CheckItem[] items)
        {
            //todo null checks
            //todo check for price info type
            foreach (var item in items)
            {
                if (item.Code != info.Code) continue;
                item.PricesApplied = new[] {info};
                item.CurrentPrice = info.Price * item.Quantity;
            }
        }
    }

    public class VolumePriceStrategy : IPricingStrategy
    {
        public PriceType PriceType { get; } = PriceType.VolumeDiscount;
        public decimal CalculatePrice(PriceInfo info, CheckItem[] items)
        {
            //todo null checks
            //todo check for price info type
            throw new NotImplementedException();
            //will split items by quantity
        }
    }
    
    public class CumulativeDiscountStrategy : IPricingStrategy
    {
        public PriceType PriceType { get; }
        public decimal CalculatePrice(PriceInfo info, CheckItem[] items)
        {
            //todo null checks
            //todo check for price info type
            throw new NotImplementedException();
        }
    }
}