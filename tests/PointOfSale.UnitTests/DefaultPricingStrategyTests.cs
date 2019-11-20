using System.Linq;
using PointOfSale.Models;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class DefaultPricingStrategyTests
    {
        [Theory]
        [MemberData(nameof(CheckItemsData.CheckItems), MemberType = typeof(CheckItemsData))]
        public void Can_Apply_And_Calculate_Price(CheckItem[] items, PriceInfo price, decimal expectedPrice)
        {
            var strategy = new DefaultPricingStrategy();

            items = strategy.ApplyPrice(price, items);
            var item = items.First(x => x.Code == price.Code);
            
            Assert.Single(item.SubItems);

            var subItem = item.SubItems.Single();
            var calculatedPrice = strategy.CalculatePrice(subItem, price);
            
            Assert.Equal(price, subItem.PriceApplied);
            Assert.Equal(expectedPrice, calculatedPrice);
        }
    }
}