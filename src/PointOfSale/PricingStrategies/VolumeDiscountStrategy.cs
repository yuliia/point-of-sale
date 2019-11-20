using System;
using System.Collections.Generic;
using PointOfSale.Models;

namespace PointOfSale.PricingStrategies
{
    public class VolumeDiscountStrategy : PricingStrategyBase
    {
        public VolumeDiscountStrategy() : base(PriceType.VolumeDiscount)
        {
        }

        protected override decimal CalculatePriceImpl(CheckSubItem item, PriceInfo defaultPrice)
        {
            return (decimal)item.Quantity / item.PriceApplied.Quantity * item.PriceApplied.Price;
        }

        protected override CheckItem[] ApplyPriceImpl(PriceInfo info, CheckItem[] items)
        {
            foreach (var item in items)
            {
                if (item.Code != info.Code || item.Quantity < info.Quantity)
                    continue;

                var newSubItems = new List<CheckSubItem>();
                foreach (var subItem in item.SubItems)
                {
                    if (subItem.PriceApplied != null && subItem.PriceApplied?.Type != PriceType.DefaultPrice)
                    {
                        continue;
                    }
                    
                    if (subItem.Quantity % info.Quantity == 0)
                    {
                        subItem.PriceApplied = info;
                        continue;
                    }
                    
                    var volumesCount = (int)Math.Floor(subItem.Quantity / (decimal)info.Quantity);
                    var quantity = volumesCount * info.Quantity;
                    
                    subItem.Quantity -= quantity;
                    
                    var newSubItem = new CheckSubItem
                    {
                        PriceApplied = info,
                        Quantity = quantity
                    };
                    
                    newSubItems.Add(newSubItem);
                }
                
                item.SubItems.AddRange(newSubItems);
            }

            return items;
        }
    }
}