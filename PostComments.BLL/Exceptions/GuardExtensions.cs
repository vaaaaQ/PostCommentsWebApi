using System;
using Ardalis.GuardClauses;
using PostComments.BLL.Entities.Comment;
using PostComments.BLL.Entities.Post;

namespace PostComments.BLL.Exceptions
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