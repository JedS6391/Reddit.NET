using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    public class CommentDetails
    {
        public CommentDetails(Thing<Comment.Details> comment)
        {
            Body = comment.Data.Body;
        }

        public string Body { get; internal set; } 
    }
}