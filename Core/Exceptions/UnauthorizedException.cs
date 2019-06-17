using Everest.Identity.Core.ExceptionTransformers;
using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.Serialization;

namespace Everest.Identity.Core.Exceptions
{
    [ResponseStatusCode(StatusCode = StatusCodes.Status401Unauthorized)]
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException()
        {
        }

        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
