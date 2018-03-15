using System;
using PostComments.BLL.Entities.Post;

namespace PostComments.BLL.Entities.Comment
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