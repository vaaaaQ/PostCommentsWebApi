using System;
using System.Runtime.Serialization;

namespace PostComments.BLL.Exceptions
{
    [Serializable]
    public class PostNotExistsException : BaseNotExistsException
    {
        public PostNotExistsException(Guid id) : this($"Post with id: {id} doesn't exist")
        {
        }

        public PostNotExistsException(string message) : base(message)
        {
        }

        public PostNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PostNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}