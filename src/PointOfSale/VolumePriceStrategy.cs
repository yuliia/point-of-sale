using System;
using System.Collections.Generic;
using System.Linq;
using PointOfSale.Models;

namespace PointOfSale
{
    public class VolumePriceStrategy : IPricingStrategy
    {
        public PriceType PriceType { get; } = PriceType.VolumeDiscount;

        public decimal CalculatePrice(CheckSubItem item, PriceInfo defaultPrice)
        {
            return (decimal)item.Quantity / item.PriceApplied.Quantity * item.PriceApplied.Price;
        }

        public CheckItem[] ApplyPrice(PriceInfo info, CheckItem[] items)
        {
            //todo null checks
            //todo check for price info type
            
            foreach (var item in items)
            {
                if (item.Code != info.Code || item.Quantity < info.Quantity)
                    continue;

                var newSubItems = new List<CheckSubItem>();
                foreach (var subItem in item.SubItems)
                {
                    if (subItem.PriceApplied?.Type != PriceType.DefaultPrice)
                    {
                        continue;
                    }
                    
                    if (subItem.Quantity % info.Quantity == 0)
                    {
                        subItem.PriceApplied = info;
                        continue;
                    }
                    
                    var volumesCount = (int)Math.Floor(subItem.Quantity / (decimal)info.Quantity);

                    var newSubItem = subItem.Fission(volumesCount * info.Quantity);
                    newSubItem.PriceApplied = info;
                    
                    newSubItems.Add(newSubItem);
                }
                
                item.SubItems.AddRange(newSubItems);
            }

            return items;
        }
    }
}