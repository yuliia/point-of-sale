using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PointOfSale.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace PointOfSale.UnitTests
{
    public class PricesStorageTests
    {
        private readonly (string code, int quantity, decimal price, PriceType type)[] _originalPrices = 
        {
            ("A", 1, 1.25m, PriceType.DefaultPrice),
            ("A", 3, 3m, PriceType.VolumeDiscount),
            ("B", 1, 4.25m, PriceType.DefaultPrice),
            ("C", 1, 1m, PriceType.DefaultPrice),
            ("C", 6, 5m, PriceType.VolumeDiscount),
            ("D", 1, 0.75m, PriceType.DefaultPrice),
            ("Discount", 1, 0.01m, PriceType.CumulativeDiscount)
        };

        public static List<object[]> PricesData => new List<object[]>
        {
            new object[] {"A", new[] {new PriceInfo("A", 1, 1.25m, PriceType.DefaultPrice), 
                new PriceInfo("A", 3, 3m, PriceType.VolumeDiscount)}},
            new object[] {"B", new[] {new PriceInfo("B", 1, 4.25m, PriceType.DefaultPrice)}},
            new object[] {"C", new[] {new PriceInfo("C", 1, 1m, PriceType.DefaultPrice), 
                new PriceInfo("C", 6, 5m, PriceType.VolumeDiscount)}},
            new object[] {"D", new[] {new PriceInfo("D", 1, 0.75m, PriceType.DefaultPrice)}},
        };

        public static List<object[]> UpdatePricesData => new List<object[]>
        {
            new object[] {"A", (3, 2.5m, PriceType.VolumeDiscount), new[] {new PriceInfo("A", 1, 1.25m, PriceType.DefaultPrice), 
                new PriceInfo("A", 3, 2.5m, PriceType.VolumeDiscount)}},
            new object[] {"B", (2, 6m, PriceType.VolumeDiscount), new[] {new PriceInfo("B", 1, 4.25m, PriceType.DefaultPrice), 
                new PriceInfo("B", 2, 6m, PriceType.VolumeDiscount)}}
        };


        [Theory]
        [MemberData(nameof(PricesData), MemberType = typeof(PricesStorageTests))]
        public void Can_Store_Prices(string code, PriceInfo[] expetedPrices)
        {
            var pricesStorage = new PricesStorage();
            foreach (var price in _originalPrices)
            {
                pricesStorage.SetPrice(price.code, price.quantity, price.price, price.type);
            }

            var prices = pricesStorage.GetPrices(code);
            
            CollectionAssert.AreEquivalent(expetedPrices, prices.ToArray());
        }
        
        [Theory]
        [MemberData(nameof(UpdatePricesData), MemberType = typeof(PricesStorageTests))]
        public void Can_Update_Prices(string code, (int quantity, decimal price, PriceType type) update, PriceInfo[] expetedPrices)
        {
            var pricesStorage = new PricesStorage();
            foreach (var price in _originalPrices)
            {
                pricesStorage.SetPrice(price.code, price.quantity, price.price, price.type);
            }

            pricesStorage.SetPrice(code, update.quantity, update.price, update.type);
            var prices = pricesStorage.GetPrices(code);
            
            CollectionAssert.AreEquivalent(expetedPrices, prices.ToArray());
        }

        [Theory]
        [InlineData(5, 0)]
        [InlineData(999, 0)]
        [InlineData(999.99999, 0)]
        [InlineData(1000, 0.01)]
        [InlineData(1989, 0.01)]
        [InlineData(1999.9999, 0.01)]
        [InlineData(2000, 0.03)]
        [InlineData(2999.9999, 0.03)]
        [InlineData(4999.9999, 0.03)]
        [InlineData(5000, 0.05)]
        [InlineData(9999.9999, 0.05)]
        [InlineData(10000, 0.07)]
        public void Can_Update_Discount(decimal accumulated, decimal expectedDiscount)
        {
            var code = "discount";
            var storage = new PricesStorage();
            storage.SetPrice(code, 1, 0, PriceType.CumulativeDiscount);
            
            storage.UpdateAccumulatedAmount(code, accumulated);
            var discount = storage.GetPrices(code).Single();
            
            Assert.Equal(expectedDiscount, discount.Price);
        }
    }
}