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

        public UsernamePasswordAuthenticator(
            ILogger<UsernamePasswordAuthenticator> logger,
            CommandFactory commandFactory, 
            Credentials credentials)
            : base(logger, credentials)
        {
            _commandFactory = commandFactory;
        }

        protected override async Task<AuthenticationContext> DoAuthenticateAsync()
        {
            var authenticateCommand = _commandFactory.Create<AuthenticateWithUsernamePasswordCommand>();

            var result = await authenticateCommand.ExecuteAsync(new AuthenticateWithUsernamePasswordCommand.Parameters
            {
                Username = Credentials.Username,
                Password = Credentials.Password,
                ClientId = Credentials.ClientId,
                ClientSecret = Credentials.ClientSecret
            });

            return new UsernamePasswordAuthenticationContext(result.Token); 
        }

        protected override async Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext)
        {
            // Username + password authentication does not support refresh tokens, so we need to completely re-authenticate.        
            // TODO: Find a better way to support 2-FA, as the 2-FA code will have expired by the point we need to refresh.
            return await DoAuthenticateAsync().ConfigureAwait(false);
        }
    }
}