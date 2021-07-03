---
layout: default
title: Getting started
nav_order: 2
---

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

To start interacting with reddit, the `RedditClientBuilder` class can be used to obtain a `RedditClient` instance.

The builder requires an [`IHttpClientFactory` instance](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-5.0) and [`ILoggerFactory` instance](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.iloggerfactory?view=dotnet-plat-ext-5.0) to be provided, as well as an action for configuring the credentials used by the client:

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
        credentialsBuilder.Script(
            clientId,
            clientSecret,
            username,
            password);        
    });

RedditClient client = await builder.BuildAsync();
```

An extension method is provided to configure a named `HttpClient` that the client will use for HTTP communication. This ensures that a unique and descriptive User-Agent is configured, as per the [reddit API rules](https://github.com/reddit-archive/reddit/wiki/API#rules).

```cs 
IServiceCollection services = ...;

services.AddRedditHttpClient(userAgent: "<platform>:<app ID>:<version string> (by /u/<reddit username>)");
```

Continue to the [Client](./client.md) section for more details on the functionality provided by the `RedditClient` class.