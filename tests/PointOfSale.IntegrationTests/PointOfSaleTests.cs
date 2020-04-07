using System.Collections.Generic;
using System.Linq;
using PointOfSale.Models;
using Xunit;

namespace PointOfSale.IntegrationTests
{
    public class PointOfSaleTests
    {
        private readonly (string code, decimal price)[] _originalPrices = 
        {
            ("A", 1.25m),
            ("B", 4.25m),
            ("C", 1m),
            ("D", 0.75m)
        };

        private readonly (string code, int quantity, decimal price)[] _volumeDiscounts =
        {
            ("A", 3, 3m),
            ("C", 6, 5m)
        };
        
        public static List<object[]> CheckWithDifferentQuantitiesData => new List<object[]>
        {
            new object[] { new [] {("A", 1),("B", 3),("B", 1),("B", 3)}, 31m},
            new object[] { new [] {("A", 1),("C", 4),("A", 1),("C", 2)}, 7.5m},
            new object[] { new [] {("A", 3),("B", 1),("C", 2),("D", 1),("C", 1)}, 11m}
        };

        public static List<object[]> CheckWithDifferentQuantitiesDataWithDiscount => new List<object[]>
        {
            new object[] { new [] {("A", 7),("B", 1),("C", 2),("D", 4)}, 0, 16.5m},
            new object[] { new [] {("A", 7),("B", 1),("C", 2),("D", 4)}, 1000, 16.4075m},
            new object[] { new [] {("A", 7),("B", 1),("C", 2),("D", 4)}, 2000, 16.2225m},
            new object[] { new [] {("A", 7),("B", 1),("C", 2),("D", 4)}, 5000, 16.0375m},
            new object[] { new [] {("A", 7),("B", 1),("C", 2),("D", 4)}, 10000, 15.8525m},
        };
        
        [Theory]
        [InlineData("ABCDABA", 13.25)]
        [InlineData("CCCCCCC", 6)]
        [InlineData("ABCD", 7.25)]
        public void Can_Calculate_Total(string productsToScan, decimal expectedTotal)
        {
            var pos = GetSetUpInstance();

            foreach (var code in productsToScan)
            {
                pos.Scan(code.ToString());
            }
            
            Assert.Equal(expectedTotal, pos.GetTotal());
        }

        [Theory]
        [MemberData(nameof(CheckWithDifferentQuantitiesData), MemberType = typeof(PointOfSaleTests))]
        public void Can_Calculate_With_Different_Quantities_Specified(
            (string code, int quantity)[] items, decimal expectedTotal)
        {
            var pos = GetSetUpInstance();

            foreach (var item in items)
            {
                pos.Scan(item.code, item.quantity);
            }

            var total = pos.GetTotal();
            
            Assert.Equal(expectedTotal, total);
        }
        
        [Theory]
        [InlineData("A", true)]
        [InlineData("B", true)]
        [InlineData("C", true)]
        [InlineData("D", true)]
        [InlineData("F", false)]
        public void Correctly_Returns_Result_Of_Scan(string code, bool expectedResult)
        {
            var pos = GetSetUpInstance();
            
            var result = pos.Scan(code);
            
            Assert.Equal(expectedResult, result);
        }
        
        [Fact]
        public void Can_Calculate_Price_When_NotFound()
        {
            var pos = GetSetUpInstance();

            pos.Scan("B");
            pos.Scan("B");
            pos.Scan("NotExistingCode");
            
            var total = pos.GetTotal();
            
            Assert.Equal(8.5m, total);
        }

        [Fact]
        public void Can_Reset_When_Close_Check()
        {
            var pos = GetSetUpInstance();
            var newPrice = 2;
            
            pos.Scan("A");
            pos.SetPrice("A", newPrice);
            var total = pos.GetTotal();
            Assert.Equal(1.25m, total);
            
            total = pos.GetTotal();
            Assert.Equal(0, total);
            
            pos.Scan("A");
            total = pos.GetTotal();
            Assert.Equal(newPrice, total);
        }

        [Theory]
        [MemberData(nameof(CheckWithDifferentQuantitiesDataWithDiscount), MemberType = typeof(PointOfSaleTests))]
        public void Can_Calculate_With_Different_Quantities_And_Cumulative_Discount(
            (string code, int quantity)[] items, int accumulated, decimal expectedTotal)
        {
            var pos = GetSetUpInstance();
            pos.Scan("Discount");
            Enumerable.Range(0, accumulated).Select(x => pos.Scan("C")).ToArray();
            pos.GetTotal();
            
            pos.Scan("Discount");
            foreach (var item in items)
            {
                pos.Scan(item.code, item.quantity);
            }

            var total = pos.GetTotal();

            Assert.Equal(expectedTotal, total);
        }
        
        [Fact]
        public void Can_Calculate_Total_When_Prices_Changed()
        {
            var pos = GetSetUpInstance();
            
            pos.Scan("B");
            pos.SetPrice("B", 5);
            pos.Scan("B");
            var total = pos.GetTotal();
            
            Assert.Equal(8.5m, total);
        }

        
        private PointOfSaleTerminal GetSetUpInstance()
        {
            var pos = new PointOfSaleTerminal();
            foreach (var tuple in _originalPrices)
            {
                pos.SetPrice(tuple.code, tuple.price);
            }

            foreach (var tuple in _volumeDiscounts)
            {
                pos.AddDiscount(new VolumeDiscount(tuple.code, tuple.quantity, tuple.price));
            }
            pos.AddDiscount(new CumulativeDiscount("Discount"));
            return pos;
        }
    }
}