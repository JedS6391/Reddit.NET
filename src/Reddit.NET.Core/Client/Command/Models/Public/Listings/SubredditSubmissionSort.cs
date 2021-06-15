namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    public class SubredditSubmissionSort
    {
        public SubredditSubmissionSort(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static SubredditSubmissionSort Hot => new SubredditSubmissionSort("hot");
        public static SubredditSubmissionSort Best => new SubredditSubmissionSort("best");
        public static SubredditSubmissionSort New => new SubredditSubmissionSort("new");
        public static SubredditSubmissionSort Rising => new SubredditSubmissionSort("rising");
        public static SubredditSubmissionSort Controversial => new SubredditSubmissionSort("controversial");
        public static SubredditSubmissionSort Top => new SubredditSubmissionSort("top");
    }
}