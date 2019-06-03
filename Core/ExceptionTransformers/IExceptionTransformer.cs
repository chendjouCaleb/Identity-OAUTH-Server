using Microsoft.AspNetCore.Http;
using System;

namespace Identity.Core.ExceptionTransformers
{
    public interface IExceptionTransformer
    {
        Type[] ExceptionTypes { get; }
        void EditContext(HttpContext context, Exception ex);
        ErrorResponseModel BuildErrorModel(Exception ex);
    }
}
