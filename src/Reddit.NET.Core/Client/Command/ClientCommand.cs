using System.Net.Http;

namespace Reddit.NET.Core.Client.Command
{
    public abstract class ClientCommand
    {
        protected ClientCommand()
        {
        }

        public abstract string Id { get; }
        public abstract HttpRequestMessage BuildRequest();
    }
}