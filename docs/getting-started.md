# Getting started

## Prerequsites

- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

## Installation

The `Reddit.NET.Client` assembly is available as a [Nuget package](https://www.nuget.org/packages/Reddit.NET.Client).

You can use the Nuget package manager to install the package as follows:

```
PM> Install-Package Reddit.NET.Client -Version {Version}
```

Or directly add a `PackageReference` element to your project, if preferred:

```xml
<PackageReference Include="Reddit.NET.Client" Version="{Version}" />
```

## Usage

The `RedditClient` class provides the main entry-point for accessing reddit's API.

To create a `RedditClient` instance, the `RedditClientBuilder` class can be used. The builder requires an `IHttpClientFactory` and `ILoggerFactory` to be configured, as well an action for configuring the credentials used by the client:

```cs
using Reddit.NET.Client.Builder;

RedditClientBuilder builder = RedditClientBuilder.New;

// Configure builder
builder
    .WithHttpClientFactory(httpClientFactory)
    .WithLoggerFactory(loggerFactory)                
    .WithCredentialsConfiguration(credentialsBuilder => 
    {                    
        // Configure credentials
    });

RedditClient client = await builder.BuildAsync();
```

Continue to the [Client](./client.md) section for more details on the functionality provided by the `RedditClient` class