using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.UnitTests.Shared;

namespace Reddit.NET.Client.UnitTests
{
    public class CommentThreadNavigatorTests
    {
        private static readonly Random s_random = new Random();
        private static readonly SubmissionsCommentSort s_defaultSort = SubmissionsCommentSort.Confidence;

        [Test]
        public void Parent_NavigatorWithNoParent_ReturnsNull()
        {
            var submission = RandomSubmission();
            var replies = new List<IThing<IHasParent>>()
            {
                RandomComment(),
                RandomComment(),
                RandomComment(),
                new MoreComments()
            };

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort);

            Assert.IsNull(navigator.Parent);
        }

        [Test]
        public void Parent_NavigatorWithParent_ReturnsParentCommentThread()
        {
            var submission = RandomSubmission();
            var replies = new List<IThing<IHasParent>>()
            {
                RandomComment(),
                RandomComment(),
                RandomComment(),
                new MoreComments()
            };
            var parent = RandomComment();

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort, parent);

            Assert.IsNotNull(navigator.Parent);
            Assert.AreEqual(parent.Data.Id, navigator.Parent.Details.Id);
            Assert.AreEqual(submission.Data.Id, navigator.Parent.Submission.Id);
        }

        [Test]
        public void Count_TopLevelCommentThread_ShouldReturnCorrectCount()
        {
            var submission = RandomSubmission();
            var replies = new List<IThing<IHasParent>>()
            {
                RandomComment(),
                RandomComment(),
                RandomComment(),
                new MoreComments()
            };

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort);

            // 'more comments' aren't counted as comments
            Assert.AreEqual(3, navigator.Count);
        }

        [Test]
        public void Count_ChildCommentThread_ShouldReturnCorrectCount()
        {
            var submission = RandomSubmission();
            var replies = new List<IThing<IHasParent>>()
            {
                RandomComment(),
                RandomComment(),
                RandomComment(),
                RandomComment(),
                RandomComment(),
                new MoreComments()
            };
            var parent = new Comment();

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort, parent);

            // 'more comments' aren't counted as comments
            Assert.AreEqual(5, navigator.Count);
        }

        [Test]
        public void IndexOperator_ValidIndex_ShouldReturnComment()
        {
            var submission = RandomSubmission();
            var replies = new List<IThing<IHasParent>>()
            {
                RandomComment(),
                RandomComment(),
                RandomComment(),
                new MoreComments()
            };
            var secondComment = replies[1] as Comment;

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort);

            var actualComment = navigator[1];

            Assert.IsNotNull(actualComment);
            Assert.AreEqual(secondComment.Data.Id, actualComment.Details.Id);
            Assert.AreEqual(submission.Data.Id, actualComment.Submission.Id);
        }

        [Test]
        public void IndexOperator_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            var submission = RandomSubmission();
            var replies = new List<IThing<IHasParent>>()
            {
                RandomComment(),
                RandomComment(),
                RandomComment(),
                new MoreComments()
            };

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var commnet = navigator[5];
            });
        }

        [Test]
        public void GetEnumerator_NavigatorWithThreeComments_ShouldEnumerateThreeComments()
        {
            var submission = RandomSubmission();
            var replies = new List<IThing<IHasParent>>()
            {
                RandomComment(),
                RandomComment(),
                RandomComment(),
                new MoreComments()
            };

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort);

            var count = 0;

            foreach (var commentThread in navigator)
            {
                var expectedComment = replies[count] as Comment;

                Assert.IsNotNull(commentThread);
                Assert.AreEqual(expectedComment.Data.Id, commentThread.Details.Id);
                Assert.AreEqual(submission.Data.Id, commentThread.Submission.Id);

                count++;
            }

            Assert.AreEqual(3, count);
        }

        [Test]
        public void GetCommentThreadReplies_TwoLevelCommentThread_ShouldGetReplies()
        {
            var submission = RandomSubmission();
            var replies = new List<IThing<IHasParent>>()
            {
                RandomComment(generateRandomReplies: true),
                RandomComment(generateRandomReplies: true),
                new MoreComments()
            };

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort);

            var i = 0;

            foreach (var topLevelThread in navigator)
            {
                var expectedComment = replies[i] as Comment;

                Assert.IsNotNull(topLevelThread);
                Assert.AreEqual(expectedComment.Data.Id, topLevelThread.Details.Id);
                Assert.AreEqual(submission.Data.Id, topLevelThread.Submission.Id);

                var j = 0;

                foreach (var replyThread in topLevelThread.Replies)
                {
                    var expectedReply = expectedComment.Data.Replies.Children[j] as Comment;

                    Assert.IsNotNull(replyThread);
                    Assert.AreEqual(expectedReply.Data.Id, replyThread.Details.Id);
                    Assert.AreEqual(submission.Data.Id, replyThread.Submission.Id);

                    j++;
                }

                i++;
            }
        }

        [Test]
        public void Flatten_MultiLevelCommentThread_ShouldProduceCorrectOrder()
        {
            var submission = RandomSubmission();
            var comment1 = RandomComment(generateRandomReplies: true);
            var comment2 = RandomComment(generateRandomReplies: true);
            var replies = new List<IThing<IHasParent>>()
            {
                comment1,
                comment2,
                new MoreComments()
            };
            var expectedFlattenedCommentIds = new List<string>();

            expectedFlattenedCommentIds.Add(comment1.Data.Id);
            expectedFlattenedCommentIds.AddRange(comment1.Data.Replies.Children.OfType<Comment>().Select(c => c.Data.Id));
            expectedFlattenedCommentIds.Add(comment2.Data.Id);
            expectedFlattenedCommentIds.AddRange(comment2.Data.Replies.Children.OfType<Comment>().Select(c => c.Data.Id));

            var navigator = new CommentThreadNavigator(submission, replies, s_defaultSort);

            var flattenedComments = navigator.Flatten();

            foreach (var (comment, expectedCommentId) in flattenedComments.Zip(expectedFlattenedCommentIds))
            {
                Assert.AreEqual(expectedCommentId, comment.Details.Id);
            }
        }

        private static Submission RandomSubmission()
        {
            var submission = new Submission();
            var details = new Submission.Details();

            details.SetProperty(d => d.Id, Guid.NewGuid().ToString());

            submission.SetProperty(s => s.Kind, "t3");
            submission.SetProperty(s => s.Data, details);

            return submission;
        }

        private static Comment RandomComment(bool generateRandomReplies = false)
        {
            var comment = new Comment();
            var details = new Comment.Details();
            var listing = new Listing<IHasParent>();
            var children = new List<IThing<IHasParent>>();

            details.SetProperty(d => d.Id, Guid.NewGuid().ToString());
            details.SetProperty(d => d.Body, "Test comment body");

            listing.SetProperty(l => l.Data, new ListingData<IHasParent>());
            listing.Data.SetProperty(l => l.Children, children);
            details.SetProperty(d => d.Replies, listing);

            if (generateRandomReplies)
            {
                var replyCount = s_random.Next(1, 10);

                for (var i = 0; i < replyCount; i++)
                {
                    children.Add(RandomComment());
                }

                listing.Data.SetProperty(l => l.Children, children);
                details.SetProperty(d => d.Replies, listing);
            }

            comment.SetProperty(c => c.Kind, "t1");
            comment.SetProperty(c => c.Data, details);

            return comment;
        }
    }
}
