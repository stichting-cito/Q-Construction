using System;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Date count class
    /// </summary>
    public class DateCount
    {
        private DateTime _date;
        /// <summary>
        ///     Date
        /// </summary>
        // [BsonDateTimeOptions(DateOnly = true)]
        public DateTime Date
        {
            get => _date.Date;
            set => _date = value.Date;
        }

        /// <summary>
        ///     Count
        /// </summary>
        public int Count { get; set; }
    }
}