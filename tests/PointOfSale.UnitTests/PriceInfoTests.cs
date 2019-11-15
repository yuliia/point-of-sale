﻿using System;
using PointOfSale.Models;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class PriceInfoTests
    {
        [Theory]
        [InlineData("A", 1, 1)]
        [InlineData("A", 1, 0.01)]
        public void Can_Create_Valid_Model(string code, int quantity, decimal price)
        {
            var priceInfo = new PriceInfo(code, quantity, price);
            Assert.Equal(code, priceInfo.Code);
            Assert.Equal(quantity, priceInfo.Quantity);
            Assert.Equal(price, priceInfo.Price);
        }
        
        [Theory]        
        [InlineData("", 1, 1, typeof(ArgumentNullException))]
        [InlineData(" ", 1, 1, typeof(ArgumentNullException))]
        [InlineData(null, 1, 1, typeof(ArgumentNullException))]
        [InlineData("", 0, 0, typeof(ArgumentNullException))]
        [InlineData("A", 0, 1, typeof(ArgumentException))]
        [InlineData("A", -1, 1, typeof(ArgumentException))]
        [InlineData("A", 1, 0, typeof(ArgumentException))]
        [InlineData("A", 2, -1, typeof(ArgumentException))]
        public void Validates_Model(string code, int quantity, decimal price, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => new PriceInfo(code, quantity, price));
        }
    }
}