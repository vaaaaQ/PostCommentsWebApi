using System;
using System.Runtime.Serialization;

namespace PostComments.Core.Exceptions
{
    public abstract class BaseNotExistsException : Exception
    {
        public BaseNotExistsException()
        {
        }

        public BaseNotExistsException(string message) : base(message)
        {
        }

        public BaseNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BaseNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}