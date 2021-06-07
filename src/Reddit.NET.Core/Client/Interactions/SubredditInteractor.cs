using System;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Models.Public;
using Reddit.NET.Core.Client.Command.Subreddits;
using Reddit.NET.Core.Client.Interactions.Abstract;

namespace Reddit.NET.Core.Client.Interactions
{
    public class SubredditInteractor : IInteractor
    {                 
        private readonly CommandFactory _commandFactory; 
        private readonly IAuthenticator _authenticator;
        private readonly GetSubredditDetailsCommand.Parameters _parameters;

        public SubredditInteractor(
            CommandFactory commandFactory,
            IAuthenticator authenticator,
            string displayName)
        {
            _commandFactory = commandFactory;
            _authenticator = authenticator;
            _parameters = new GetSubredditDetailsCommand.Parameters()
            {
                DisplayName = displayName
            };
        }

        public string DisplayName { get; private set; }

        public async Task<SubredditDetails> GetDetailsAsync()
        {
            var authenticationContext = await _authenticator.AuthenticateAsync().ConfigureAwait(false);

            var getSubredditCommand = _commandFactory.Create<GetSubredditDetailsCommand>();

            var result = await getSubredditCommand.ExecuteAsync(authenticationContext, _parameters);

            return result.Details;
        }

        // public async Task<IEnumerable<SubmissionInteractor> GetHotPostsAsync()
        // {

        // }
    }
}