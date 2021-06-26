namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents a sort option by time range.
    /// </summary>
    public sealed class TimeRangeSort : NamedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeRangeSort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort option.</param>
        private TimeRangeSort(string name)
            : base(name)
        {            
        }

        /// <summary>
        /// Gets the 'hour' sort option.
        /// </summary>
        public static TimeRangeSort Hour => new TimeRangeSort("hour");

        /// <summary>
        /// Gets the 'day' sort option.
        /// </summary>
        public static TimeRangeSort Day => new TimeRangeSort("day");

        /// <summary>
        /// Gets the 'week' sort option.
        /// </summary>
        public static TimeRangeSort Week => new TimeRangeSort("week");

        /// <summary>
        /// Gets the 'month' sort option.
        /// </summary>
        public static TimeRangeSort Month => new TimeRangeSort("month");
        
        /// <summary>
        /// Gets the 'year' sort option.
        /// </summary>
        public static TimeRangeSort Year => new TimeRangeSort("year");

        /// <summary>
        /// Gets the 'all' sort option.
        /// </summary>        
        public static TimeRangeSort AllTime => new TimeRangeSort("all");
    }
}