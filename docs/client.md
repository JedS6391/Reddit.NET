# Client

The `RedditClient` class provides the ability to access reddit's API.

To create a `RedditClient` instance, the `RedditClientBuilder` class can be used:

```cs
RedditClientBuilder builder = RedditClientBuilder.New;

// Configure builder
...

RedditClient client = await builder.BuildAsync();
```

## Interactions

The client provides its functionality through *interactors*, which are responsible for specific high-level concepts e.g. subreddits, users, etc. For example, to interact with a specific subreddit, a `SubredditInteractor` should be used:

```cs
// Interact with a subreddit
SubredditInteractor askReddit = client.Subreddit("askreddit");

SubredditDetails askRedditDetails = await askReddit.GetDetailsAsync(); 
```

The following interactors are provided by the client:

- `MeInteractor`: responsible for interactions with the currently authenticated user
- `InboxInteractor`: responsible for interactions with the currently authenticated user's inbox
- `UserInteractor`: responsible for interactions with the a user
- `SubredditInteractor`: responsible for interactions with a subreddit
- `SubmissionInteractor`: responsible for interactions with a submission
- `CommentInteractor`: responsible for interactions with a comment

Interactors generally return read-only views over reddit data. These views can be transformed to interactors, to allow actions to be performed on them. The example below illustrates retrieving the details of and interacting with each subreddit the currently authenticated user is subscribed to:

```cs
MeInteractor me = client.Me(); 

// Retrieve subreddit details
await foreach (SubredditDetails subreddit in me.GetSubredditsAsync())
{            
    SubredditInteractor interactor = subreddit.Interact(client);

    // Interact with the subreddit
    ...
}
```

## Pagination

Certain reddit API endpoints return data in a `Listing` object, which encapsulate a single page of data and a pointer to the next page.

The client abstracts this away by exposing an `IAsyncEnumerable<T>` over the listing data. Most of these methods provide the ability to configure the enumerator to control properties such as the amount of data returned, the sort of the data, etc.

For example, when retrieving the history of a user, a listing options builder is exposed:

```cs
MeInteractor me = client.Me();

IAsyncEnumerable<UserContentDetails> savedHistory = me.GetHistoryAsync(builder =>
    builder
        .WithType(UserHistoryType.Saved)                    
        .WithMaximumItems(50));

await foreach (var item in savedHistory)
{
    // Do something with item
    ...
}
```

Such an enumerator operates in a *lazy-fashion* and will only fetch data when:

1. The enumeration starts, i.e. the initial page needs to be loaded
2. Enumeration of the current page is finished, i.e. the next page needs to be loaded