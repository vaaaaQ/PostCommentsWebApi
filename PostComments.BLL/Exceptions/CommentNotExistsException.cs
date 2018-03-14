using System;

namespace PostComments.BLL.Exceptions
{
    public class CommentNotExistsException : BaseNotExistsException
    {
        public CommentNotExistsException(Guid commentId) : this($"Comment with id: {commentId} doesn't exist")
        {
        }

        public CommentNotExistsException(string message) : base(message)
        {
        }
    }
}