using System;

namespace Identity.Core.ExceptionTransformers
{
    public class ResponseStatusCodeAttribute: Attribute
    {
        public int StatusCode { get; set; }
    }
}
