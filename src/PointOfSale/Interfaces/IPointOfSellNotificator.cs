using System;
using PointOfSale.Models;

namespace PointOfSale.Interfaces
{
    public interface IPointOfSellNotificator
    {
        void NotifyCheckClosed(string discountCardCode, decimal amount);
        IDisposable SubscribeCheckClosed(Action<CheckClosed> action);
    }
}