using System.Threading.Tasks;

namespace Reddit.NET.Console.Examples
{
    /// <summary>
    /// Represents an example.
    /// </summary>
    internal interface IExample
    {
        /// <summary>
        /// Gets the name of the example.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Runs the example.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RunAsync();
    }
}