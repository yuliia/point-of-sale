using System.Linq;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class CheckItemTests
    {
        [Fact]
        public void CanCreateItem()
        {
            var item = new CheckItem("code", 5);
            
            Assert.Equal("code", item.Code);
            Assert.Equal(5, item.Quantity);
            Assert.True(item.SubItems.Any());
            Assert.Equal(5, item.SubItems.First().Quantity);
        }
    }
}