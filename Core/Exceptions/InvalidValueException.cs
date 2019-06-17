using Everest.Identity.Core.ExceptionTransformers;
using System;
using System.Runtime.Serialization;

namespace Everest.Identity.Core
{
    [ResponseStatusCode(StatusCode = 400)]
    public class InvalidValueException : ApplicationException
    {
        public InvalidValueException()
        {
        }

        public InvalidValueException(string message) : base(message)
        {
        }

        public InvalidValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
