# Internals

This section details the internal components the client uses. Most of these won't be directly consumed when using the client.

## HTTP

The client relies on the `IHttpClientFactory` abstraction to obtain `HttpClient` instance for all HTTP communication. This ensures the pooling and lifetime of the underlying message handler is managed appropriately. 

All HTTP communication is performed asynchronously, making it clear which methods will perform a network request to obtain the data required.

The only component that actually issues HTTP requests is the `CommandExecutor` class. The client operates in terms of *Commands* which describe a particular operation (e.g. get the details of a subreddit, get my saved history) and delegates to the executor to handle the intricacies of HTTP communication.

## Logging


## Authentication


## JSON

The reddit APIs return data in JSON format and the client depends on the `System.Text.Json` assembly for any deserialization of JSON data into model classes.

The client has an internal set of models that map directly to the JSON structure returned by reddit (see the `Reddit.NET.Client.Models.Internal` namespace). These internal models are mapped to public models which are exposed by the client (see the `Reddit.NET.Client.Models.Public` namespace). This is done for two reasons:

1. The internal models have information that may not be directly relevant to consumers which the public models can omit
2. The internal models can be simple data classes, allowing the public models to have additional behaviour depending on the type of model

### Things

A *Thing* is the reddit base class and has the following JSON structure:

```json
{
    "kind": "t1",
    "data": {
        ...
    }
}
```

A specific entity will have its own structure for the `data` node, e.g. the data node of a *Comment* entity will contain attributes such as the comment author and body text.

The `Thing<TData>` abstract class represents this structure and serves as a base class for the internal model classes used by the client.

### Listings

### Converters

## References

- [`IHttpClientFactory` class](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-5.0)
- [`System.Text.Json` namespace](https://docs.microsoft.com/en-us/dotnet/api/system.text.json?view=net-5.0)
- [Reddit JSON API](https://github.com/reddit-archive/reddit/wiki/JSON)