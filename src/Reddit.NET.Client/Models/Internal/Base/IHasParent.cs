namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Represents an object that has a parent (e.g. a comment or more children object).
    /// </summary>
    public interface IHasParent
    {
        /// <summary>
        /// Gets the full name of the parent object
        /// </summary>
        string ParentFullName { get; }
    }
}