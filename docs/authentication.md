---
layout: default
title: Authentication
nav_order: 4
---

# Authentication

Reddit supports a number of different authentication modes via its OAuth2 implementation. The client supports all of these and aims to provide abstractions that make them straight-forward to use.

For a full description of reddit authentication, see the [reddit OAuth2 wiki page](https://github.com/reddit-archive/reddit/wiki/OAuth2).

## Credentials

When configuring a client instance with the `RedditClientBuilder` class, a `CredentialsBuilder` is exposed to allow configuration of the credentials used by the client:

```cs
var clientId = "...";
var clientSecret = "...";

// Build a client with read-only credentials
var client = await RedditClientBuilder
    .New             
    .WithCredentialsConfiguration(credentialsBuilder => 
    {
        credentialsBuilder.ReadOnly(
            clientId,
            clientSecret,
            deviceId: Guid.NewGuid());                  
    })     
    .BuildAsync();
```

The table below describes the available authentication modes and their intended usage:

| Mode                    | Usage                                                                                                                                                                                                  |
|-------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Script                  | Intended to be used for scripts running on your own machine. This mode requires a client secret and will only have access to the account the client ID and secret was generated for.                   |
| Read-only               | Intended to be used in a similar manner to 'Script' or 'Web app' (i.e. a client secret is available), but will have read-only access.                                                                  |
| Read-only installed app | Similar to 'Read-only', but in a context where a client secret is not available (e.g. a mobile device)                                                                                                 |
| Web app                 | Intended for use on a web server. This mode requires a client secret and will use the OAuth2 authorization code grant type to obtain an access token after a user has approved access on their behalf. |
| Installed app           | Similar to 'Web app', but in a context where a client secret is not available (e.g. a mobile device)                                                                                                   |

## Interactive authentication flows

The *Web app* and *Installed app* authentication modes require that a user be sent to reddit for authorization (i.e. to approve access on their behalf) and then redirected back to a redirect URI where an access token can be obtained.

To support these interactive flows, these authentication modes expose the `InteractiveCredentials.Builder` class that can be used to:

- Obtain the authorization URI the user should be sent to in order to approve access to reddit on their behalf
- Store the authorization code returned once access has been approved
- Redeem the authorization code for an access token that can be used by the client

The example below illustrates how to perform this interactive flow:

```cs
var clientId = "...";
var clientSecret = "...";
var redirectUri = "...";
// State should be a randomly generated value
var state = "...";

var builder = await RedditClientBuilder
    .New             
    .WithCredentialsConfiguration(credentialsBuilder => 
    {                   
        // Obtain an interactive credentials builder 
        var interactiveCredentialsBuilder = credentialsBuilder.WebApp(
            clientId,
            clientSecret,
            new Uri(redirectUri),
            state);

        var authorizationUri = interactiveCredentialsBuilder.GetAuthorizationUri();

        // Send the user to the authorization URI.
        // This could be as simple as printing the authorization URI for the user to copy-paste into their browser,
        // or a more involved process of opening a browser and then redirecting back to a mobile application.
        ...

        // On redirect back, validate the state and code parameters before authorizing the builder
        var stateParameter = ...;
        var codeParameter = ...;

        interactiveCredentialsBuilder.Authorize(codeParameter);
    });

// The builder will use the authorization code to complete the interactive authentication 
// flow and obtain the appropriate credentials.
var client = await builder.BuildAsync();
```

## Sessions

In some cases, it may desirable to authenticate a client and then re-use that authenticated client at a later point. For example, a web service will need to authenticate the user and then create a new authenticated client instance for that user on each request.

Rather than require the user to completely re-authenticate each time, a session-based credential set can be used. 

Upon successful authentication, the credentials generated will be allocated a unique session ID. This session ID will be associated with the access token obtained during the authentication flow. The session ID can then be used to configure an authenticated client, without needing to complete the interactive authentication flow.

```cs
var clientId = "...";
var clientSecret = "...";
var redirectUri = "...";
// State should be a randomly generated value
var state = "...";

// Complete the interactive authentication flow to obtain a credentials instance
var interactiveCredentialsBuilder = CredentialsBuilder
    .New
    .WebApp(
        clientId,
        clientSecret,
        redirectUri,
        state);

var code = ...;

interactiveCredentialsBuilder.Authorize(code);

await interactiveCredentialsBuilder.AuthenticateAsync(
    commandExecutor,
    tokenStorage);

// At this point a token has been obtained and associated with a unique session ID.
// We can store the session ID for later usage.
var credentials = interactiveCredentialsBuilder.Build(); 

sessionService.Store(credentials.SessionId);

...

// Get the session ID and use it to get an authenticated client instance.
var sessionId = sessionService.Get();

var client = RedditClientBuilder
    .New                
    .WithTokenStorage(tokenStorage)
    .WithCredentialsConfiguration(credentialsBuilder =>
    {
        credentialsBuilder.Session(
            AuthenticationMode.WebApp,
            clientId,
            clientSecret,
            redirectUri,
            sessionId);                                    
    })
    .BuildAsync();

// Use the authenticated client
...
```

For this flow to work, an `ITokenStorage` implementation needs to be provided that can manage the token obtained when creating credentials. For example, the token could be stored encrypted on disk or temporarily in memory.