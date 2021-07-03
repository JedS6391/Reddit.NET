namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Represents an object that can be saved. 
    /// </summary>    
    public interface ISaveable
    {
        /// <summary>
        /// Gets a value indicating whether the user has saved this thing.
        /// </summary>
        bool IsSaved { get;  }
    }
}