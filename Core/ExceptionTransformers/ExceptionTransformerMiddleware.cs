using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Core.ExceptionTransformers
{
    public class ExceptionTransformerMiddleware
    {
        private RequestDelegate nextDelegate;
        private ExceptionTransformerFactory factory;

        public ExceptionTransformerMiddleware(RequestDelegate nextDelegate, ExceptionTransformerFactory factory)
        {
            this.nextDelegate = nextDelegate;
            this.factory = factory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await nextDelegate.Invoke(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                IExceptionTransformer transformer = factory.GetExceptionTransformer(e.GetType());
                ErrorResponseModel response = transformer.BuildErrorModel(e);
                transformer.EditContext(context, e);
                context.Response.StatusCode = response.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
        }
    }
}
