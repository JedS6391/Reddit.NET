using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;

namespace Reddit.NET.Core.Client.Authentication
{
    public class UsernamePasswordAuthenticator : AutoRefreshAuthenticator
    {
        private readonly CommandFactory _commandFactory;
        private readonly UsernamePasswordAuthenticator.AuthenticationDetails _authenticationDetails;

        public UsernamePasswordAuthenticator(
            ILogger<UsernamePasswordAuthenticator> logger,
            CommandFactory commandFactory, 
            UsernamePasswordAuthenticator.AuthenticationDetails authenticationDetails)
            : base(logger)
        {
            _commandFactory = commandFactory;
            _authenticationDetails = authenticationDetails;
        }

        protected override async Task<AuthenticationContext> DoAuthenticateAsync()
        {
            var authenticateCommand = _commandFactory.Create<AuthenticateWithUsernamePasswordCommand>();

            var result = await authenticateCommand.ExecuteAsync(new AuthenticateWithUsernamePasswordCommand.Parameters
            {
                Username = _authenticationDetails.Username,
                Password = _authenticationDetails.Password,
                ClientId = _authenticationDetails.ClientId,
                ClientSecret = _authenticationDetails.ClientSecret
            });

            return new UsernamePasswordAuthenticationContext(result.Token); 
        }

        protected override async Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext)
        {
            // Username + password authentication does not support refresh tokens, so we need to completely re-authenticate.        
            // TODO: Find a better way to support 2-FA, as the 2-FA code will have expired by the point we need to refresh.
            return await DoAuthenticateAsync().ConfigureAwait(false);
        }

        public class AuthenticationDetails 
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}