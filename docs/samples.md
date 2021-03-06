---
layout: default
title: Samples
nav_order: 5
---

# Samples

Below are a few samples of interactions that the client exposes.

## Retrieving subreddit submissions

```cs
SubredditInteractor askReddit = client.Subreddit("askreddit");

IAsyncEnumerable<SubmissionDetails> fiftyNewSubmissions = askReddit.GetSubmissionsAsync(builder => 
    builder
        .WithSort(SubredditSubmissionSort.New)
        .WithMaximumItems(50));
        
await foreach (SubmissionDetails submission in fiftyNewSubmissions)
{
    // Do something with submission
    ...
}
```

## Retrieving subscribed subreddits

```cs
MeInteractor me = client.Me();

IAsyncEnumerable<SubredditDetails> mySubreddits = me.GetSubredditsAsync();
        
await foreach (SubredditDetails subreddit in mySubreddits)
{            
    // Do something with subreddit
    ...
}
```

## Retrieving saved submissions/comments

```cs
MeInteractor me = client.Me(); 

IAsyncEnumerable<UserContentDetails> savedHistory = me.GetHistoryAsync(builder =>
    builder
        .WithType(UserHistoryType.Saved)                    
        .WithMaximumItems(100));

await foreach (UserContentDetails content in savedHistory)
{
    // Saved history can contain both submissions and comments.
    switch (content)
    {
        case CommentDetails comment:
            // Do something with comment
            ...
            break;

        case SubmissionDetails submission:
            // Do something with submission
            ...        
            break;
    }    
}
```

## Create a multireddit

```cs
MeInteractor me = client.Me(); 

MultiredditDetails multiredditDetails = await me.CreateMultiredditAsync(new MultiredditCreationDetails(
    name: "Ask X",
    subreddits: new string[] { "askreddit", "askscience" }
));

// Add another subreddit to the newly created multireddit
await multiredditDetails
    .Interact(client)
    .AddSubredditAsync("askcomputerscience");

await multiredditDetails.ReloadAsync(client);
```

## Voting on a submission/comment

> **Warning**: Votes must be cast by a human (see the [reddit API documentation](https://www.reddit.com/dev/api/oauth#POST_api_vote) for details).

```cs
// Obtain submission details e.g. by getting the submissions in a subreddit
SubmissionDetails submissionDetails = ...;

// Get an interactor for the submission
SubmissionInteractor submission = submissionDetails.Interact(client);

// There are equivalent methods for downvote/unvote.
await submission.UpvoteAsync();
```

## Create a submission

```cs
SubredditInteractor askReddit = client.Subreddit("askreddit");

SubmissionDetails submission = await subreddit.CreateSubmissionAsync(new LinkSubmissionCreationDetails(
    title: "Reddit.NET client",
    uri: new Uri("https://github.com/JedS6391/Reddit.NET"),
    resubmit: true));
```

## Navigating a comment thread

```cs
// Obtain a submission interactor e.g. by getting the submissions in a subreddit
SubmissionInteractor submission = ...;

CommentThreadNavigator comments = await submission.GetCommentsAsync(sort: SubmissionsCommentSort.Top);

// Navigate the replies of each top level thread on the submission
foreach (CommentThread topLevelThread in comments)
{
    foreach (CommentThread replyThread in topLevelThread.Replies)
    {
        // Do something with reply thread     
    }
}
```

## Streaming new subreddit submissions

```cs
SubredditInteractor askReddit = client.Subreddit("askreddit");

// No queries will be made until enumeration starts
IAsyncEnumerable<SubmissionDetails> newSubmissions = askReddit.Stream.SubmissionsAsync();

await foreach (SubmissionDetails submission in newSubmissions)
{
    // Do something with the new submission
    ...
}
```
