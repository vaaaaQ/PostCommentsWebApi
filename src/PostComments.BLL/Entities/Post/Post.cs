using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace PostComments.BLL.Entities.Post
{
    public class Post : BaseEntity
    {
        public Content Content { get; }
        public Title Title { get; }
        public DateTimeOffset CreatedOn { get; } = DateTimeOffset.Now;
        public Guid FromId { get; }

        protected Post()
        {
        }

        public Post(Content content, Title title, Guid from)
        {
            Guard.Against.Null(content, nameof(content));
            Guard.Against.Null(title, nameof(title));
            Guard.Against.NullOrEmpty(content.Text, nameof(content.Text));
            Guard.Against.NullOrEmpty(title.Text, nameof(title.Text));

            Content = content;
            Title = title;
            FromId = from;
        }
    }
}