---
layout: default
title: Concepts
nav_order: 3
---

# Concepts

## Client

The `RedditClient` class is the main entry-point for accessing reddit's API.

## Interactions

The client exposes its functionality through *interactors*, which are responsible for specific high-level concepts e.g. subreddits, users, etc.

For example, to interact with a specific subreddit, a `SubredditInteractor` can be used:

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

Obtaining an interactor instance does not perform any network operations, as they simply provide access to a set of related functionality.

Interactors return read-only views over reddit data. These views can be transformed to interactors, to allow actions to be performed on them as required.

The example below illustrates retrieving the details of the subreddits the currently authenticated user is subscribed to, and obtaining an interactor for each subreddit:

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

The client abstracts this away by exposing an `IAsyncEnumerable<T>` over the listing data. Most of these methods provide the ability to configure the underlying enumerator, to allow full control of properties such as the amount of data returned, the sort of the data, etc.

For example, when retrieving the history of a user, a listing options builder is exposed:

```cs
MeInteractor me = client.Me();

// We want 50 items from the saved history of the user.
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

## Streams

The client provides the ability to stream particular collections of data, including:

- New submissions and comments made in a subreddit
- New submissions and comments made by a user
- New inbox messages

Streams are exposed as an `IAsyncEnumerable<T>` that will regularly poll for new data. The stream will use an exponential back-off to avoid querying for new data too frequently.

For example, to stream new submissions in a subreddit as they become available:

```cs
var askReddit = client.Subreddit("askreddit");

// No queries will be made until enumeration starts
var newSubmissions = askReddit.Stream.SubmissionsAsync();

await foreach (var submission in newSubmissions)
{
    // Do something with the new submission
    ...
}
```

If cancellation of the streaming operation is required, a `CancellationToken` can be provided when enumerating:

```cs
var cts = new CancellationTokenSource();

// Stream new submissions, stopping after 10 minutes.
cts.CancelAfter(TimeSpan.FromMinutes(10));

await foreach (var submission in newSubmissions.WithCancellation(cts.Token))
{
    // Do something with the new submission
    ...
}
```

## Comment threads

Navigating the comments of a submission is managed by the `CommentThreadNavigator` class.

Comment threads are highly-nested structures which the navigator abstraction aims to simplify by exposing access to a single level of comments. In practice this means that a `CommentThreadNavigator` instance will either represent:

- The top-level replies on a submission; In this case, the navigator will have no reference to a *parent* comment thread.
- The replies to another comment; In this case, the navigator will have a reference to a *parent* comment thread.

The example below illustrates navigating the replies of the top-level comments on a submission:

```cs
CommentThreadNavigator navigator = submission.GetCommentsAsync();

foreach (CommentThread topLevelThread in navigator)
{
    // Navigate replies of each thread top-level thread
    foreach (CommentThread replyThread in topLevelThread.Replies)
    {
        // Do something with reply thread.
        ...
    }
}
```

## Errors

The client will throw specific exceptions for certain error scenarios that it encounters, all of which are derived from the base `RedditClientException` type.

### `CommandNotSupportedException`

Occurs when trying to execute a command that is not supported using the particular authentication mode. For example, trying to use any of the interactions provided by `MeInteractor` when authenticated with read-only credentials.

### `RedditClientRateLimitException`

Will occur when either:

- The reddit API returns a [429 HTTP response status code](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/429)
- The rate limiter used by the client cannot permit the execution of a request

### `RedditClientApiException`

Used when the reddit API returns a [400 HTTP response status code](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400) and the response body contains an error object with details of the issue.

The `Details` property of the exception can be used to inspect the actual error.

### `RedditClientResponseException`

Thrown when the reddit API returns a non-successful status code that the client does not have any specific exception for.
