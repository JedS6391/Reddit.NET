using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Authentication;
using Reddit.NET.Core.Client.Command.Subreddits;
using Reddit.NET.Core.Client.Command.Users;

namespace Reddit.NET.Core.Client.Command
{
    public class CommandFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;   
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<CommandFactory> _logger;     
        private readonly IDictionary<Type, Func<ICommand>> _typeToCommandMap;

        public CommandFactory(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            _httpClientFactory = httpClientFactory;  
            _loggerFactory = loggerFactory;          
            _logger = _loggerFactory.CreateLogger<CommandFactory>();
            _typeToCommandMap = BuildTypeToCommandMap();
        }

        public TCommand Create<TCommand>() where TCommand : ICommand
        {        
            var type = typeof(TCommand);

            _logger.LogDebug("Creating '{Type}' command", type);

            if (!_typeToCommandMap.TryGetValue(type, out var commandCreator))
            {            
                _logger.LogError("No command creator found for '{Type}'.", type);

                // TODO: Exception type
                throw new Exception();
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
                { typeof(GetUserDetailsCommand), () => new GetUserDetailsCommand(_httpClientFactory, _loggerFactory) }            
            };
        }
    }
}