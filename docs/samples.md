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

## Search for submissions

```cs
SubredditInteractor askReddit = client.Subreddit("askreddit");

IAsyncEnumerable<SubmissionDetails> searchResults = await subreddit.SearchSubmissionsAsync(
        query: "Reddit.NET client",
        builder => builder
            .WithSort(SubredditSearchSort.Relevance)
            .WithSyntax(SearchQuerySyntax.Lucene)
            .WithMaximumItems(50));

await foreach (SubmissionDetails submission in searchResults)
{
    // Do something with submission
    ...
}
```

## Subscribe to a subreddit

```cs
SubredditInteractor askReddit = client.Subreddit("askreddit");

await askReddit.SubscribeAsync();
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

## Award a submission/comment

_Note that currently only gold awards can be given._

```cs
// Obtain a submission interactor e.g. by getting the submissions in a subreddit
SubmissionInteractor submission = ...;

await submission.AwardAsync();
```

## Create a submission

```cs
// Link submission
SubredditInteractor subreddit = client.Subreddit("...");

SubmissionDetails submission = await subreddit.CreateSubmissionAsync(new LinkSubmissionCreationDetails(
    title: "Reddit.NET client",
    uri: new Uri("https://github.com/JedS6391/Reddit.NET"),
    resubmit: true));

// Text submission
SubmissionDetails submission = await subreddit.CreateSubmissionAsync(new TextSubmissionCreationDetails(
    title: $"Reddit.NET client",
    text: "Submission made using [Reddit.NET client](https://github.com/JedS6391/Reddit.NET)."););
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

## Streaming unread inbox messages

```cs
InboxInteractor inbox = client.Me().Inbox();

// No queries will be made until enumeration starts
IAsyncEnumerable<MessageDetails> newMessages = inbox.Stream.UnreadMessagesAsync();

await foreach (MessageDetails message in newMessages)
{
    // Do something with the new message
    ...
}
```

## Message another user

```cs
UserInteractor user = client.User("...");

var message = new PrivateMessageCreationDetails(
    subject: "Hi!",
    body: "I sent this message using Reddit.NET client.");

await user.SendMessageAsync(message);
```

## Reply to an inbox message

```cs
InboxInteractor inbox = client.Me().Inbox();

// Obtain a message e.g. by streaming new messages as they arrive
MessageDetails message = ...;

await inbox.ReplyAsync(message, "Replied using Reddit.NET client");
```

## Reloading data

_Subreddit, submission, and comment models provide the ability to be reloaded._

```cs
// Reload the details of a particular subreddit
SubredditDetails subredditDetails = ...;

SubredditInteractor subreddit = subredditDetails.Interact(client);

await subreddit.ReloadAsync();

// Reload the details of a particular submission
SubmissionDetails submissionDetails = ...;

SubmissionInteractor submission = submissionDetails.Interact(client);

await submission.ReloadAsync();

// Reload the details of a particular comment
CommentDetails commentDetails = ...;

CommentInteractor comment = commentDetails.Interact(client);

await comment.ReloadAsync();
```
