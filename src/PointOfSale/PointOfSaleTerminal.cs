using System;
using System.Collections.Generic;
using System.Linq;
using PointOfSale.DAL;
using PointOfSale.Interfaces;
using PointOfSale.Models;
using PointOfSale.Notifications;
using PointOfSale.Services;

namespace PointOfSale
{
    public class PointOfSaleTerminal : IDisposable
    {
        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IPointOfSellNotificator _notificator;

        private readonly Dictionary<string, CheckItem> _scannedProducts = new Dictionary<string, CheckItem>();
        private readonly List<Discount> _discounts = new List<Discount>();
        private readonly IDisposable _subscription;

        public PointOfSaleTerminal() 
            : this(new ProductRepository(), new DiscountRepository(), new PointOfSellNotificator(), new DiscountCardService(new DiscountRepository()))
        {
        }
        
        public PointOfSaleTerminal(
            IProductRepository productRepository, 
            IDiscountRepository discountRepository, 
            IPointOfSellNotificator notificator,
            IDiscountCardService discountCardService)
        {
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _notificator = notificator;
            _subscription = _notificator.SubscribeCheckClosed(discountCardService.HandleCheckClosed);
        }
        
        public void SetPrice(string code, decimal price)
        {
            var product = new Product(code, price);
            _productRepository.AddProduct(product);
        }

        public void AddDiscount(Discount discount)
        {
            _discountRepository.AddDiscount(discount); //add or update
        }

        /// <summary>
        /// Add product to check
        /// </summary>
        /// <param name="code"></param>
        /// <param name="quantity"></param>
        /// <exception cref="ArgumentNullException">When `code` is null or empty</exception>
        /// <exception cref="ArgumentException">When `quantity` is not positive number</exception>
        public bool Scan(string code, int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            
            if (quantity <= 0)
            {
                throw new ArgumentException($"{nameof(quantity)} should be positive number.");
            }

            if (_scannedProducts.TryGetValue(code, out var item))
            {
                item.AddItems(quantity);
                return true;
            }

            var product = _productRepository.GetProduct(code);
            if (product != null)
            {
                _scannedProducts[code] = new CheckItem(product, quantity);
                return true;
            }
            var discount = _discountRepository.GetDiscount(code);
            if (discount != null)
            {
                _discounts.Add(discount);
                return true;
            }
            return false;
        }

        public decimal GetTotal()
        {
            var total = 0m;
            var totalWithoutDiscount = 0m;
            var cumulativeDiscount = _discounts.OfType<CumulativeDiscount>().FirstOrDefault();

            foreach (var item in _scannedProducts.Values)
            {
                var volumeDiscount = GetDiscount<VolumeDiscount>(item.Code);
                if (volumeDiscount != null)
                {
                    item.TryApplyDiscount(volumeDiscount);
                }

                if (cumulativeDiscount != null)
                {
                    item.TryApplyDiscount(cumulativeDiscount);
                }

                total += item.TotalPrice;
                totalWithoutDiscount += item.TotalPriceWithoutDiscount;
            }

            if (cumulativeDiscount != null)
            {
                _notificator.NotifyCheckClosed(cumulativeDiscount.Code, totalWithoutDiscount);
            }
            
            _scannedProducts.Clear();
            _discounts.Clear();
            return total;
        }

        private T GetDiscount<T>(string code) where T : Discount
        {
            return _discountRepository.GetDiscount(code) as T;
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}