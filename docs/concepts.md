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
