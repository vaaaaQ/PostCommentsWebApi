using System;
using PostComments.Core.Entities.Post;

namespace PostComments.Core.Entities.Comment
{
    public class Comment: BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid FromId { get; private set; }

        public Content Content { get; private set; }

        public Comment(Guid fromId, Content content)
        {
            FromId = fromId;
            Content = content;
        }

        protected Comment(){}
    }
}
