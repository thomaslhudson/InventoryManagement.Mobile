using InventoryManagement.Mobile.Helpers.Converters;
using System;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Models
{
    public class RecordItem : IEquatable<RecordItem>
    {
        public string Id { get; set; }

        public decimal Quantity { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public string RecordId { get; set; }

        public byte RecordMonth { get; set; }

        public short RecordYear { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }
        
        public decimal ProductUnitPrice { get; set; }

        public string ProductGroupName { get; set; }

        public string RecordMonthYear { get => $"{Dates.GetMonthName(RecordMonth)} / {RecordYear}"; }

        #region Equality
        public bool Equals(RecordItem otherRecordItem)
        {
            return (!ReferenceEquals(otherRecordItem, null)) &&
                (Id, Quantity, DateCreated, DateModified, RecordId, RecordMonth, RecordYear, ProductId, ProductName, ProductUnitPrice)
                .Equals((otherRecordItem.Id, otherRecordItem.Quantity, otherRecordItem.DateCreated, otherRecordItem.DateModified, 
                otherRecordItem.RecordId, otherRecordItem.RecordMonth, otherRecordItem.RecordYear, otherRecordItem.ProductId, 
                otherRecordItem.ProductName, otherRecordItem.ProductUnitPrice));
        }

        public override bool Equals(object obj)
        {
            return (obj is Record otherRecord) && Equals(otherRecord);
        }

        public override int GetHashCode()
        {
            return new { Id, Quantity, DateCreated, DateModified, RecordId, RecordMonth, RecordYear, ProductId, ProductName, ProductUnitPrice }.GetHashCode();
        }
        #endregion
    }
}