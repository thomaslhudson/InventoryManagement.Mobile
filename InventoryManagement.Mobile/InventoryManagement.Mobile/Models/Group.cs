using System;

namespace InventoryManagement.Mobile.Models
{
    public class Group : IEquatable<Group>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        #region Equality
        public bool Equals(Group otherGroup)
        {
            return (!ReferenceEquals(otherGroup, null)) &&
                (Id, Name).Equals((otherGroup.Id, otherGroup.Name));
        }

        public override bool Equals(object obj)
        {
            //return (obj is Group) && Equals((Group)obj);
            return (obj is Group group) && Equals(group);
        }

        public override int GetHashCode()
        {
            return new
            {
                Id,
                Name
            }.GetHashCode();
        }
        #endregion
    }
}
