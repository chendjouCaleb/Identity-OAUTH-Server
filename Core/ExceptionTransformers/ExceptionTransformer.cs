using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Everest.Identity.Core.ExceptionTransformers
{
    public class ExceptionTransformer:IExceptionTransformer
    {
        public virtual Type[] ExceptionTypes => new Type[] { typeof(Exception) };

        public virtual ErrorResponseModel BuildErrorModel(Exception ex)
        {
            var statusCodeAttribute = ex.GetType().GetCustomAttributes(typeof(ResponseStatusCodeAttribute), true)
                .FirstOrDefault() as ResponseStatusCodeAttribute;

            int statusCode = 400;
            if (statusCodeAttribute != null)
            {
                statusCode = statusCodeAttribute.StatusCode;
            }
            return new ErrorResponseModel
            {
                StatusCode = statusCode,
                Message = ex.Message,
                Type = ex.GetType().Name
            };
        }

        public virtual void EditContext(HttpContext context, Exception ex)
        {

        }
    }
}
