using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PointOfSale.Models;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class PricesStorageTests
    {
        private readonly (string code, int quantity, decimal price)[] _originalPrices = 
        {
            ("A", 1, 1.25m),
            ("A", 3, 3m),
            ("B", 1, 4.25m),
            ("C", 1, 1m),
            ("C", 6, 5m),
            ("D", 1, 0.75m)
        };

        public static List<object[]> PricesData => new List<object[]>
        {
            new object[] {"A", new[] {new PriceInfo("A", 1, 1.25m), new PriceInfo("A", 3, 3m)}},
            new object[] {"B", new[] {new PriceInfo("B", 1, 4.25m)}},
            new object[] {"C", new[] {new PriceInfo("C", 1, 1m), new PriceInfo("C", 6, 5m)}},
            new object[] {"D", new[] {new PriceInfo("D", 1, 0.75m)}},
        };

        public static List<object[]> UpdatePricesData => new List<object[]>
        {
            new object[] {"A", (3, 2.5m), new[] {new PriceInfo("A", 1, 1.25m), new PriceInfo("A", 3, 2.5m)}},
            new object[] {"B", (2, 6m), new[] {new PriceInfo("B", 1, 4.25m), new PriceInfo("B", 2, 6m)}}
        };


        [Theory]
        [MemberData(nameof(PricesData), MemberType = typeof(PricesStorageTests))]
        public void Can_Store_Prices(string code, PriceInfo[] expetedPrices)
        {
            var pricesStorage = new PricesStorage();
            foreach (var price in _originalPrices)
            {
                pricesStorage.SetPrice(price.code, price.quantity, price.price);
            }

            var prices = pricesStorage.GetPrices(code);
            
            CollectionAssert.AreEquivalent(expetedPrices, prices.ToArray());
        }
        
        [Theory]
        [MemberData(nameof(UpdatePricesData), MemberType = typeof(PricesStorageTests))]
        public void Can_Update_Prices(string code, (int quantity, decimal price) update, PriceInfo[] expetedPrices)
        {
            var pricesStorage = new PricesStorage();
            foreach (var price in _originalPrices)
            {
                pricesStorage.SetPrice(price.code, price.quantity, price.price);
            }

            pricesStorage.SetPrice(code, update.quantity, update.price);
            var prices = pricesStorage.GetPrices(code);
            
            CollectionAssert.AreEquivalent(expetedPrices, prices.ToArray());
        }
    }
}