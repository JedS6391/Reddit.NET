# Reddit .NET client

[![nuget][nuget-image]][nuget-url]
[![status][status-image]][status-url]
[![coverage][coverage-image]][coverage-url]


## About

Provides a .NET client for interacting with [reddit](https://www.reddit.com).

The client was designed with the following goals in mind:

- Simple, modern, asynchronous API
- Support for various authentication modes
- Modular structure with simple re-usable components

## Usage

```cs
var builder = RedditClientBuilder.New;

// Configure builder
...

var client = await builder.BuildAsync();

// Get the details of a subreddit
var askReddit = client.Subreddit("askreddit");

var askRedditDetails = await askReddit.GetDetailsAsync();

// Get the top 50 hot submissions
var topFiftyHotSubmissions = askReddit.GetSubmissionsAsync(builder => 
    builder                    
        .WithSort(SubredditSubmissionSort.Hot)                  
        .WithMaximumItems(50));

await foreach (var submission in topFiftyHotSubmissions)
{
    // Upvote the submission
    await submission
        .Interact(client)
        .UpvoteAsync();
}
```

Further examples of usage can be found in the [demos](./demos/) folder.

## Development

Following the instructions below to get started with the project in a local development environment.

### Prerequisites

- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

### Building

After cloning the source code to a destination of your choice, run the following command to build the solution:

```
dotnet build
```

### Tests

The test suite can be run using the following command:

```
dotnet test
```

## Documentation

See the [docs](./docs/) folder for documentation on how to use the client and an overview of its internals.

The documentation is also hosted on [GitHub pages](https://jeds6391.github.io/Reddit.NET/).

[nuget-image]: https://img.shields.io/nuget/v/Reddit.NET.Client?style=flat-square
[nuget-url]: https://www.nuget.org/packages/Reddit.NET.Client
[status-image]: https://img.shields.io/github/workflow/status/JedS6391/Reddit.NET/Master%20branch%20workflow/master?style=flat-square
[status-url]: https://github.com/JedS6391/Reddit.NET/actions/workflows/master.yml
[coverage-image]: https://img.shields.io/codecov/c/github/JedS6391/Reddit.NET/master?style=flat-square
[coverage-url]: https://codecov.io/gh/JedS6391/Reddit.NET