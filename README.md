# Reddit .NET client

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