﻿using System;
using System.Collections.Generic;
using System.Linq;
using PointOfSale.Exceptions;
using PointOfSale.Interfaces;
using PointOfSale.Models;
using PointOfSale.PricingStrategies;

namespace PointOfSale
{
    public class PointOfSaleTerminal : IPointOfSaleTerminal
    {
        private readonly IPricesStorage _pricesStorage;
        private readonly Dictionary<string, IReadOnlyList<PriceInfo>> _pricesCache;
        private readonly Dictionary<string, int> _check;
        private readonly Dictionary<PriceType, IPricingStrategy> _strategies = new Dictionary<PriceType, IPricingStrategy>();
            
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
                
                if (prices.First().Type == PriceType.CumulativeDiscount)
                {
                    return;
                }
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
            _pricesStorage.SetPrice(code, quantity, price, quantity == 1 ? PriceType.DefaultPrice : PriceType.VolumeDiscount);
        }

        public void AddDiscount(string code, decimal initialDiscount = 0m)
        {
            _pricesStorage.SetPrice(code, 1, initialDiscount, PriceType.CumulativeDiscount);
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
            var check = _check.Select(x => new CheckItem(x.Key, x.Value)).ToArray();

            var pricesOrdered = _pricesCache.Values.SelectMany(x => x)
                .OrderBy(x => x.Type)
                .ThenByDescending(x => x.Quantity);

            foreach (var price in pricesOrdered)
            {
                var strategy = GetStrategy(price.Type);
                check = strategy.ApplyPrice(price, check);
            }

            return check.Sum(x => 
                x.SubItems
                    .Sum(si => 
                        GetStrategy(si.PriceApplied.Type)
                            .CalculatePrice(si, x.DefaultPrice)));
        }

        private IPricingStrategy GetStrategy(PriceType type)
        {
            if (_strategies.TryGetValue(type, out var strategy))
            {
                return strategy;
            }
            switch (type)
            {
                case PriceType.DefaultPrice:
                {
                    _strategies[type] = new DefaultPricingStrategy();
                    return  _strategies[type];
                }
                
                case PriceType.VolumeDiscount:
                    _strategies[type] = new VolumeDiscountStrategy();
                    return _strategies[type];
                
                case PriceType.CumulativeDiscount:
                    _strategies[type] = new CumulativeDiscountStrategy(_pricesStorage);
                    return _strategies[type];
                default:
                    throw new NotSupportedException($"Unknown price type {type}");
            }
        }
    }
}