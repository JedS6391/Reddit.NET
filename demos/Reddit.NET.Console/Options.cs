using CommandLine;

namespace Reddit.NET.Console
{
    /// <summary>
    /// Defines the options for the console program.
    /// </summary>
    internal class Options
    {     
        /// <summary>
        /// Gets or sets the name of the example to run.
        /// </summary>
        [Option('e', "example", Required = true, HelpText = "Name of the example to run.")]
        public string ExampleName { get; set; }
    }
}