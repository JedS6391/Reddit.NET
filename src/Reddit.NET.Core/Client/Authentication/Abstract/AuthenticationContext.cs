using System.Linq;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication.Abstract
{
    public abstract class AuthenticationContext
    {
        private readonly string[] _supportedCommandIds;

        public abstract string Id { get; }

        public abstract Token Token { get; }

        public AuthenticationContext(string[] supportedCommandIds)
        {
            _supportedCommandIds = supportedCommandIds;
        }

        public bool CanExecute(ICommand command) => _supportedCommandIds.Any(id => id == command.Id);
    }
}