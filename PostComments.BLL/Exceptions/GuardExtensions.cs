using System;
using System.Collections.Generic;
using System.Text;
using Ardalis.GuardClauses;
using PostComments.Core.Entities.Comment;
using PostComments.Core.Entities.Post;
using PostComments.Core.Services;

namespace PostComments.Core.Exceptions
{
    public static class GuardExtensions
    {
        public static void GuidEmpty(this IGuardClause guardClause, Guid input, string parameterName)
        {
            if (input == Guid.Empty)
                throw new ArgumentException("Should not have been empty!", parameterName);
        }

        public static void PostNotExists(this IGuardClause guardClause, Post post, Guid postId)
        {
            if (post is null)
                throw new PostNotExistsException(postId);
        }

        public static void CommentNotExists(this IGuardClause guardClause, Comment comment, Guid commentId)
        {
            if (comment is null)
                throw new CommentNotExistsException(commentId);
        }
    }
}
