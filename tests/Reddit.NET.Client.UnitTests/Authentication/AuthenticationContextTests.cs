using System;
using System.Net.Http;
using NUnit.Framework;
using Reddit.NET.Client.Authentication.Context;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.UnitTests.Authentication
{
    public class AuthenticationContextTests
    {
        [Test]
        public void CanExecute_NoAttributeCommand_ThrowsArgumentException()
        {
            var authenticationContext = new ClientCredentialsAuthenticationContext(new Token());

            Assert.Throws<ArgumentException>(() => authenticationContext.CanExecute(new NoAttributeCommand()));
        }

        [Test]
        public void CanExecute_SingleAttributeCommandReadOnlyAuthenticationContext_ReturnsTrue()
        {
            var authenticationContext = new ClientCredentialsAuthenticationContext(new Token());

            Assert.AreEqual(true, authenticationContext.CanExecute(new SingleAttributeCommand()));
        }

        [Test]
        public void CanExecute_SingleAttributeCommandUserBasedAuthenticationContext_ReturnsFalse()
        {
            var authenticationContext = new UsernamePasswordAuthenticationContext(new Token());

            Assert.AreEqual(false, authenticationContext.CanExecute(new SingleAttributeCommand()));
        }

        [Test]
        public void CanExecute_MultipleAttributeCommandReadOnlyAuthenticationContext_ReturnsTrue()
        {
            var authenticationContext = new ClientCredentialsAuthenticationContext(new Token());

            Assert.AreEqual(true, authenticationContext.CanExecute(new MultipleAttributeCommand()));
        }

        [Test]
        public void CanExecute_MultipleAttributeCommandUserBasedAuthenticationContext_ReturnsTrue()
        {
            var authenticationContext = new UsernamePasswordAuthenticationContext(new Token());

            Assert.AreEqual(true, authenticationContext.CanExecute(new MultipleAttributeCommand()));
        }

        private class NoAttributeCommand : ClientCommand
        {
            public override string Id => nameof(NoAttributeCommand);

            public override HttpRequestMessage BuildRequest() =>
                throw new System.NotImplementedException();
        }

        [ReadOnlyAuthenticationContext]
        private class SingleAttributeCommand : ClientCommand
        {
            public override string Id => nameof(SingleAttributeCommand);

            public override HttpRequestMessage BuildRequest() =>
                throw new System.NotImplementedException();
        }

        [ReadOnlyAuthenticationContext]
        [UserAuthenticationContext]
        private class MultipleAttributeCommand : ClientCommand
        {
            public override string Id => nameof(MultipleAttributeCommand);

            public override HttpRequestMessage BuildRequest() =>
                throw new System.NotImplementedException();
        }
    }
}
