using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Logging;
using Reddit.NET.Console.Examples;

namespace Reddit.NET.Console
{
    /// <summary>
    /// Contains the logic to parse command line args and run examples.
    /// </summary>
    internal sealed class EntryPoint
    {
        private readonly ILogger<EntryPoint> _logger;
        private readonly IEnumerable<IExample> _examples;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryPoint" /> class.
        /// </summary>
        /// <param name="logger">A <see cref="ILogger{TCategoryName}" /> used to write messages.</param>
        /// <param name="examples">A collection containing the available examples.</param>
        public EntryPoint(ILogger<EntryPoint> logger, IEnumerable<IExample> examples)
        {
            _logger = logger;
            _examples = examples;
        }

        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            _logger.LogTrace("RunAsync");

            var args = System.Environment.GetCommandLineArgs();

            var result = Parser
                .Default
                .ParseArguments<Options>(args)
                .WithNotParsed(errors => HandleErrors(errors));

            await result.WithParsedAsync(async options => 
                await RunExampleAsync(options.ExampleName).ConfigureAwait(false));            
        }

        private async Task RunExampleAsync(string exampleName)
        {
            var example = _examples.FirstOrDefault(e => e.Name == exampleName);

            if (example == null)
            {
                var availableExamples = string.Join(", ", _examples.Select(e => e.Name));

                _logger.LogError($"Invalid example name '{exampleName}'. Available example names are: [{availableExamples}]");

                return;
            }

            _logger.LogInformation($"Running example '{example.Name}'");

            await example.RunAsync().ConfigureAwait(false);
        }

        private void HandleErrors(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                _logger.LogError(error.ToString());
            }
        }
    }
}