using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Models.Public;
using Reddit.NET.Core.Client.Command.Users;
using Reddit.NET.Core.Client.Interactions.Abstract;

namespace Reddit.NET.Core.Client.Interactions
{
    public class MeInteractor : IInteractor
    {
        private readonly CommandFactory _commandFactory; 
        private readonly IAuthenticator _authenticator;

        public MeInteractor(CommandFactory commandFactory, IAuthenticator authenticator)
        {
            _commandFactory = commandFactory;
            _authenticator = authenticator;
        }

        public async Task<UserDetails> GetDetailsAsync()
        {            
            var authenticationContext = await _authenticator.AuthenticateAsync().ConfigureAwait(false);

            var getUserCommand = _commandFactory.Create<GetUserDetailsCommand>();

            var result = await getUserCommand.ExecuteAsync(authenticationContext, new GetUserDetailsCommand.Parameters());

            return result.Details;
        }

        public IAsyncEnumerable<SubredditDetails> GetSubredditsAsync()
        {
            return new UserSubredditsListingGenerator(_commandFactory, _authenticator);            
        }
    }
}