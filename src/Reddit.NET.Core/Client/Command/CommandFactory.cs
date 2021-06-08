using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Authentication;
using Reddit.NET.Core.Client.Command.Exceptions;
using Reddit.NET.Core.Client.Command.Submissions;
using Reddit.NET.Core.Client.Command.Subreddits;
using Reddit.NET.Core.Client.Command.Users;

namespace Reddit.NET.Core.Client.Command
{
    /// <summary>
    /// Responsible for creating <see cref="ICommand" /> instances.
    /// </summary>
    public class CommandFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;   
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<CommandFactory> _logger;     
        private readonly IDictionary<Type, Func<ICommand>> _typeToCommandMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFactory" /> class.
        /// </summary>
        /// <param name="httpClientFactory">An <see cref="IHttpClientFactory" /> instance used by created commands to make HTTP calls.</param>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory" /> instance used to create logger instances.</param>
        public CommandFactory(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            _httpClientFactory = httpClientFactory;  
            _loggerFactory = loggerFactory;          
            _logger = _loggerFactory.CreateLogger<CommandFactory>();
            _typeToCommandMap = BuildTypeToCommandMap();
        }

        /// <summary>
        /// Creates a command of the specified type.
        /// </summary>
        /// <typeparam name="TCommand">The type of command to create.</typeparam>
        /// <returns>A command of type <typeparamref name="TCommand"/>.</returns>
        /// <exception cref="CommandCreationFailureException">Thrown when the requested command type cannot be created.</exception>
        public TCommand Create<TCommand>() where TCommand : ICommand
        {        
            var type = typeof(TCommand);

            _logger.LogDebug("Creating '{Type}' command", type);

            if (!_typeToCommandMap.TryGetValue(type, out var commandCreator))
            {            
                _logger.LogError("No command creator found for '{Type}'.", type);
                
                throw new CommandCreationFailureException($"No command creator found for '{type}'");
            }

            _logger.LogDebug("Invoking '{Type}' command creator ", type);

            return (TCommand) commandCreator.Invoke();
        }

        private IDictionary<Type, Func<ICommand>> BuildTypeToCommandMap()
        {
            return new Dictionary<Type, Func<ICommand>>
            {
                { typeof(AuthenticateWithUsernamePasswordCommand), () => new AuthenticateWithUsernamePasswordCommand(_httpClientFactory, _loggerFactory) },
                { typeof(AuthenticateWithClientCredentialsCommand), () => new AuthenticateWithClientCredentialsCommand(_httpClientFactory, _loggerFactory) },
                { typeof(AuthenticateWithRefreshTokenCommand), () => new AuthenticateWithRefreshTokenCommand(_httpClientFactory, _loggerFactory) },
                { typeof(GetSubredditDetailsCommand), () => new GetSubredditDetailsCommand(_httpClientFactory, _loggerFactory) },
                { typeof(GetHotSubredditSubmissionsCommand), () => new GetHotSubredditSubmissionsCommand(_httpClientFactory, _loggerFactory) },
                { typeof(GetUserDetailsCommand), () => new GetUserDetailsCommand(_httpClientFactory, _loggerFactory) },
                { typeof(GetUserSubredditsCommand), () => new GetUserSubredditsCommand(_httpClientFactory, _loggerFactory) },
                { typeof(ApplyVoteToSubmissionCommand), () => new ApplyVoteToSubmissionCommand(_httpClientFactory, _loggerFactory) }
            };
        }
    }
}