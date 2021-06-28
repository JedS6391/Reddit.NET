# Reddit .NET client

## About

Provides a .NET client for interacting with [reddit](https://www.reddit.com).

## Build

```
dotnet build src/
```

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

## Documentation

See the [docs](./docs/) folder for documentation on how to use the client and an overview of its internals.