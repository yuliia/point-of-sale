using System.Collections.Generic;
using PointOfSale.Exceptions;
using Xunit;

namespace PointOfSale.IntegrationTests
{
    // Probably with real project I'd split it to Unit and Integrations tests.
    // But since mock storage would look like PricesStorage, I left it as it is
    public class PointOfSaleTests
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
        
        public static List<object[]> CheckWithDifferentQuantitiesData => new List<object[]>
        {
            new object[] { new [] {("A", 1),("B", 3),("B", 1),("B", 3)}, 31m},
            new object[] { new [] {("A", 1),("C", 4),("A", 1),("C", 2)}, 7.5m},
            new object[] { new [] {("A", 3),("B", 1),("C", 2),("D", 1),("C", 1)}, 11m}
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
        public void Can_Calculate_With_Different_Quantities_Specified((string code, int quantity)[] items, decimal expectedTotal)
        {
            var pos = GetSetUpInstance();

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
            pos.SetPrice("B", 1, 5);
            pos.Scan("B");
            var total = pos.GetTotal();
            
            Assert.Equal(8.5m, total);
        }

        [Fact]
        public void Can_Throw_Exception_When_NotFound()
        {
            var pos = GetSetUpInstance();

            Assert.Throws<PriceNotFoundException>(() => pos.Scan("NotExistingCode"));
        }
        
        [Fact]
        public void Can_Calculate_Price_When_NotFound()
        {
            var pos = GetSetUpInstance();

            try
            {
                pos.Scan("B");
                pos.Scan("B");
                pos.Scan("NotExistingCode");
            }
            catch (PriceNotFoundException)
            {
            }
            
            var total = pos.GetTotal();
            
            Assert.Equal(8.5m, total);
        }

        [Fact]
        public void Can_Reset_When_Close_Check()
        {
            var pos = GetSetUpInstance();
            var newPrice = 2;
            
            pos.Scan("A");
            pos.SetPrice("A", 1, newPrice);
            var total = pos.GetTotal();
            Assert.Equal(1.25m, total);
            
            pos.CloseCheck();
            total = pos.GetTotal();
            Assert.Equal(0, total);
            
            pos.Scan("A");
            total = pos.GetTotal();
            Assert.Equal(newPrice, total);
        }

        private PointOfSaleTerminal GetSetUpInstance()
        {
            var pos = new PointOfSaleTerminal();
            foreach (var tuple in _originalPrices)
            {
                pos.SetPrice(tuple.code, tuple.quantity, tuple.price);
            }

            return pos;
        }
    }
}