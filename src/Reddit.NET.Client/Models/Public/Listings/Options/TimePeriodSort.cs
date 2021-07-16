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
        /// Gets the <i>hour</i> sort option.
        /// </summary>
        public static TimeRangeSort Hour => new TimeRangeSort("hour");

        /// <summary>
        /// Gets the <i>day</i> sort option.
        /// </summary>
        public static TimeRangeSort Day => new TimeRangeSort("day");

        /// <summary>
        /// Gets the <i>week</i> sort option.
        /// </summary>
        public static TimeRangeSort Week => new TimeRangeSort("week");

        /// <summary>
        /// Gets the <i>month</i> sort option.
        /// </summary>
        public static TimeRangeSort Month => new TimeRangeSort("month");

        /// <summary>
        /// Gets the <i>year</i> sort option.
        /// </summary>
        public static TimeRangeSort Year => new TimeRangeSort("year");

        /// <summary>
        /// Gets the <i>all</i> sort option.
        /// </summary>
        public static TimeRangeSort AllTime => new TimeRangeSort("all");
    }
}
