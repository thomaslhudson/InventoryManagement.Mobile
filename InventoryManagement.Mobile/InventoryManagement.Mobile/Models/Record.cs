using InventoryManagement.Mobile.Helpers.Converters;
using System;

namespace InventoryManagement.Mobile.Models
{
    public class Record : IEquatable<Record>
    {
        public string Id { get; set; }

        public byte Month { get; set; }

        public short Year { get; set; }

        public DateTime Created { get; private set; }

        public DateTime? Started { get; private set; }

        public DateTime? Ended { get; private set; }

        public string MonthYear { get => $"{Dates.GetMonthName(Month)} / {Year}"; }

        #region Equality
        public bool Equals(Record otherRecord)
        {
            return (otherRecord is Record) &&
                (Id, Month, Year, Created, Started, Ended, MonthYear)
                .Equals((otherRecord.Id, otherRecord.Month, otherRecord.Year, otherRecord.Created,
                    otherRecord.Started, otherRecord.Ended, otherRecord.MonthYear));
        }

        public override bool Equals(object obj)
        {
            return (obj is Record otherRecord) && Equals(otherRecord);
        }

        public override int GetHashCode()
        {
            return new { Id, Month, Year, Created, Started, Ended, MonthYear }.GetHashCode();
        }
        #endregion
    }
}
