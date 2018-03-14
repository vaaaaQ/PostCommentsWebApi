using System;
using PostComments.Core.Entities.Post;

namespace PostComments.Core.Entities.Comment
{
    public class Comment : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid FromId { get; }

        public Content Content { get; }

        public Comment(Guid fromId, Guid postId, Content content)
        {
            FromId = fromId;
            Content = content;
            PostId = postId;
        }

        protected Comment()
        {
        }
    }
}