using System;

namespace InventoryManagement.Mobile.Models
{
    public class Product : IEquatable<Product>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Upc { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsActive { get; set; }

        public string GroupId { get; set; }

        public Group Group { get; set; }

        #region Equality
        public bool Equals(Product otherProduct)
        {
            return (!ReferenceEquals(otherProduct, null)) &&
                (Id, Name, UnitPrice, IsActive, Upc, GroupId, Group)
                .Equals((otherProduct.Id, otherProduct.Name, otherProduct.UnitPrice,
                otherProduct.IsActive, otherProduct.Upc, otherProduct.GroupId, otherProduct.Group));
        }

        public override bool Equals(object obj)
        {
            return (obj is Product otherItem) && Equals(otherItem);
        }

        public override int GetHashCode()
        {
            return new { Id, Name, UnitPrice, Upc, IsActive, Group }.GetHashCode();
        }
        #endregion

    }
}
