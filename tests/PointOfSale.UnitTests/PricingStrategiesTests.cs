using System;
using System.Linq;
using Moq;
using PointOfSale.Interfaces;
using PointOfSale.Models;
using PointOfSale.PricingStrategies;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class PricingStrategiesTests
    {
        [Theory]
        [InlineData(PriceType.CumulativeDiscount)]
        [InlineData(PriceType.VolumeDiscount)]
        public void Can_Validate_Price_Type_Default(PriceType type)
        {
            var strategy = new DefaultPricingStrategy();
            var price = new PriceInfo("A", 1, 1, type);
            Assert.Throws<InvalidOperationException>(() => strategy.ApplyPrice(price, new CheckItem[0]));
        }
        
        [Theory]
        [InlineData(PriceType.CumulativeDiscount)]
        [InlineData(PriceType.DefaultPrice)]
        public void Can_Validate_Price_Type_Volume(PriceType type)
        {
            var strategy = new VolumeDiscountStrategy();
            var price = new PriceInfo("A", 1, 1, type);
            Assert.Throws<InvalidOperationException>(() => strategy.ApplyPrice(price, new CheckItem[0]));
        }
        
        [Theory]
        [InlineData(PriceType.VolumeDiscount)]
        [InlineData(PriceType.DefaultPrice)]
        public void Can_Validate_Price_Type_Cumulative(PriceType type)
        {
            var strategy = new CumulativeDiscountStrategy(new Mock<IPricesStorage>().Object);
            var price = new PriceInfo("A", 1, 1, type);
            Assert.Throws<InvalidOperationException>(() => strategy.ApplyPrice(price, new CheckItem[0]));
        }
        
        public static CheckItem[] CheckItems => new[]
        {
            new CheckItem("A", 7),
            new CheckItem("B", 1),
            new CheckItem("C", 2),
            new CheckItem("D", 5)
        };
        
        [Fact]
        public void Can_Apply_And_Calculate_Price()
        {
            var strategy = new DefaultPricingStrategy();
            var priceInfo = new PriceInfo("A", 1, 1.25m, PriceType.DefaultPrice);
            
            var items = strategy.ApplyPrice(priceInfo, CheckItems);
            var item = items.First(x => x.Code == priceInfo.Code);
            
            Assert.Single(item.SubItems);

            var subItem = item.SubItems.Single();
            var calculatedPrice = strategy.CalculatePrice(subItem, priceInfo);
            
            Assert.Equal(priceInfo, subItem.PriceApplied);
            Assert.Equal(8.75m, calculatedPrice);
        }
        
        [Fact]
        public void Can_Apply_Volume_Discount()
        {
            var priceInfo = new PriceInfo("A", 3, 3m, PriceType.VolumeDiscount);
            var strategy = new VolumeDiscountStrategy();

            var items = strategy.ApplyPrice(priceInfo, CheckItems);
            var item = items.First(x => x.Code == "A");
            
            Assert.Equal(2, item.SubItems.Count);

            var subItem = item.SubItems.FirstOrDefault(x => Equals(x.PriceApplied, priceInfo));
            var subItemWhereVolumePriceCantBeApplied = item.SubItems.FirstOrDefault(x => !Equals(x.PriceApplied, priceInfo));
            
            Assert.NotNull(subItem);
            Assert.NotNull(subItemWhereVolumePriceCantBeApplied);
            
            Assert.Equal(6, subItem.Quantity);
            Assert.Equal(1, subItemWhereVolumePriceCantBeApplied.Quantity);

            var price = strategy.CalculatePrice(subItem, new PriceInfo("A", 1, 1.25m, PriceType.DefaultPrice));
            Assert.Equal(6, price);
        }
        
        [Fact]
        public void Can_Apply_Cumulative_Discount()
        {
            // arrange
            var defaultPrice = new PriceInfo("A", 1, 1.25m, PriceType.DefaultPrice);
            var volumePrice = new PriceInfo("A", 3, 3m, PriceType.VolumeDiscount);
            var discount = new PriceInfo("discount", 1, 0.01m, PriceType.CumulativeDiscount);
            
            var mockStorage = new Mock<IPricesStorage>();
            mockStorage.Setup(x => x.UpdateAccumulatedAmount("discount", 8.75m))
                .Verifiable("Expected UpdateAccumulatedAmount to be called");
            
            var items = new DefaultPricingStrategy().ApplyPrice(defaultPrice, CheckItems);
            items = new VolumeDiscountStrategy().ApplyPrice(volumePrice, items);
            
            var strategy = new CumulativeDiscountStrategy(mockStorage.Object);
           
            // act
            items = strategy.ApplyPrice(discount, items);
            
            // assert
            mockStorage.Verify();
            
            var item = items.First(x => x.Code == "A");
            Assert.Equal(2, item.SubItems.Count);

            var subItem = item.SubItems.FirstOrDefault(x => Equals(x.PriceApplied, discount));
            var subItemWhereVolumePriceCantBeApplied = item.SubItems.FirstOrDefault(x => !Equals(x.PriceApplied, discount));
            
            Assert.NotNull(subItem);
            Assert.NotNull(subItemWhereVolumePriceCantBeApplied);
            
            Assert.Equal(1, subItem.Quantity);
            Assert.Equal(6, subItemWhereVolumePriceCantBeApplied.Quantity);

            var price = strategy.CalculatePrice(subItem, defaultPrice);
            Assert.Equal(1.2375m, price);
        }
    }
}