using System;
using System.Reactive.Subjects;
using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale.Notifications
{
    public class PointOfSellNotificator : IPointOfSellNotificator, IDisposable
    {
        private readonly Subject<CheckClosed> _checkClosedStream = new Subject<CheckClosed>();
        
        public void NotifyCheckClosed(string discountCardCode, decimal amount)
        {
            _checkClosedStream.OnNext(new CheckClosed(discountCardCode, amount));
        }

        public IDisposable SubscribeCheckClosed(Action<CheckClosed> action)
        {
            return _checkClosedStream.Subscribe(action);
        }

        public void Dispose()
        {
            _checkClosedStream?.Dispose();
        }
    }
}