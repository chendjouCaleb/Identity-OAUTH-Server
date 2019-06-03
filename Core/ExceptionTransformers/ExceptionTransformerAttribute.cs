using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Identity.Core.ExceptionTransformers
{
    public class ExceptionTransformerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ExceptionTransformerFactory factory = context.HttpContext.RequestServices.
                GetService(typeof(ExceptionTransformerFactory)) as ExceptionTransformerFactory;
            Exception ex = context.Exception;

            Console.WriteLine(ex);

            IExceptionTransformer transformer = factory.GetExceptionTransformer(ex.GetType());

            transformer.EditContext(context.HttpContext, ex);
            ErrorResponseModel error = transformer.BuildErrorModel(ex);
            context.Result = new ObjectResult(error)
            {
                StatusCode = error.StatusCode
            };
        }
    }
}
