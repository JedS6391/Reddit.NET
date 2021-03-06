---
layout: default
title: Internals
nav_order: 4
---

# Internals

This section details the internals of the client, including some of the components it uses. Most of these won't be directly consumed when using the client but it is worthwhile to describe their responsibilities and understand how they work/are used.

## HTTP

The client relies on the `IHttpClientFactory` abstraction to obtain `HttpClient` instance for all HTTP communication. This ensures the pooling and lifetime of the underlying message handler is managed appropriately.

All HTTP communication is performed asynchronously, making it clear which methods will perform a network request to obtain the data required.

The only component that actually issues HTTP requests is the `CommandExecutor` class. The client operates in terms of *Commands* which describe a particular operation (e.g. get the details of a subreddit, get my saved history, upvote a comment) and delegates to the executor to handle the intricacies of HTTP communication.

## Rate limiting

The reddit API imposes rate limits for its endpoints. The client aims to respect the 60 requests per minute limit through the `RateLimiter` abstraction. Before each HTTP request, the client will attempt to obtain a permit for making that request. If no permits are available, the client will wait until permits are replenished so it can make the request.

By default, permits are replenished at a rate of 1 permit per second. The client will allow up to a maximum of 5 permits to be leased at one time (i.e. 5 HTTP requests could be made at once).

This strategy aims to stay well under the limit, but in future it may be possible to have a more dynamic limiter that reacts in response to the `X-Ratelimit-*` headers returned by the reddit API. See the [reddit API rules](https://github.com/reddit-archive/reddit/wiki/API#rules) for more details.

## Retries

The client will attempt to retry a request up to three times when a transient HTTP response status code is returned by the reddit API. An exponential back-off sleep strategy is used between request retries.

**Transient HTTP response status codes:**

- [500 Internal Server Error](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500)
- [502 Bad Gateway](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/502)
- [503 Service Unavailable](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/503)
- [504 Gateway Timeout](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/504)

## Logging

Components used by the client are provided loggers via an `ILoggerFactory`.

## Authentication

The client uses the reddit OAuth2 support to authenticate. This authentication is managed by an `IAuthenticator` implementation which provides access to a given `AuthenticationContext`. The `AuthenticationContext` contains an access token which will be used to authorize requests.

The context also controls which commands can be executed, as certain commands will not be permitted in certain contexts (e.g. if no user is authenticated, then only read-only commands can be executed).

## JSON

The reddit APIs return data in JSON format and the client depends on the `System.Text.Json` assembly for any deserialization of JSON data into model classes.

The client has an internal set of models that map directly to the JSON structure returned by reddit (see the `Reddit.NET.Client.Models.Internal` namespace). These internal models are mapped to public models which are exposed by the client (see the `Reddit.NET.Client.Models.Public` namespace). This is done for two reasons:

1. The internal models have information that may not be directly relevant to consumers, which the public models can omit
2. The internal models can be simple data classes, allowing the public models to have additional behaviour depending on the type of model

### Things

A *Thing* defines the base attributes shared by all reddit API objects and has the following JSON structure:

```json
{
    "kind": "...",
    "data": {
        ...
    }
}
```

A specific entity will have a specific `kind`, as well as its own structure for the `data` node, e.g. the data node of a *Subreddit* entity will contain attributes such as the subreddit name, description and number of subscribers:

```json
{
    "kind": "t5",
    "data": {
        "display_name": "AskReddit",
        "description": "...",
        "subscribers": 32752721,
        ...
    }
}
```

The `Thing<TData>` abstract class represents this structure and serves as a base class for the internal model classes used by the client.

### Listings

The reddit API returns *Listing* objects for endpoints that support pagination. These objects have the following base structure:

```json
{
    "kind": "Listing",
    "data": {
        "children": [
            ...
        ],
        "before": "...",
        "after": "..."
    }
}
```

The `before` and `after` properties allow the client to navigate through the listing. Currently, the client only exposes enumerating forwards through a listing, meaning only the `after` properties is relevant.

The type of objects in the `children` node will vary based on the kind of listing (e.g. it may be a listing of subreddits, listing of comments, etc).

The `Listing<TData>` class represents this structure and serves as a base class for specific listing models used by the client.

### Converters

The client employs a number of `JsonConverter` implementations for custom deserialization behaviour where appropriate.

The `ThingJsonConverterFactory` is the most complex of these and provides the following benefits:

- Deserialization of JSON to a generic `IThing<TData>` instance
- Deserialization of polymorphic JSON (e.g. a list of comments and submissions)

## References

- [`IHttpClientFactory` interface](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-5.0)
- [`ILoggerFactory` interface](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.iloggerfactory?view=dotnet-plat-ext-5.0)
- [`System.Text.Json` namespace](https://docs.microsoft.com/en-us/dotnet/api/system.text.json?view=net-5.0)
- [Reddit JSON API](https://github.com/reddit-archive/reddit/wiki/JSON)
