using System.Collections.Generic;
using PointOfSale.Models;

namespace PointOfSale.Interfaces
{
    public interface IPricesStorage
    {
        // I made the calls synchronous. But in real system
        // I'd probably make them async because it's highly likely to use IO operations here.
        IReadOnlyList<PriceInfo> GetPrices(string code);
        
        void SetPrice(string code, int quantity, decimal price);
        void UpdateAccumulatedAmount(string code, decimal amount);
    }
}