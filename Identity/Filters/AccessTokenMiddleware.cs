using Everest.Identity.Core.Extensions;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Infrastruture;
using Everest.Identity.Models;
using Everest.Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Everest.Identity.Filters
{
    public class AccessTokenMiddleware
    {
        private RequestDelegate nextDelegate;
        private IRepository<Authorization, long> authorizationRepository;
        private AccessTokenValidator accessTokenValidator;

        public AccessTokenMiddleware(RequestDelegate nextDelegate, IRepository<Authorization, long> authorizationRepository, 
            AccessTokenValidator accessTokenValidator)
        {
            this.nextDelegate = nextDelegate;
            this.authorizationRepository = authorizationRepository;
            this.accessTokenValidator = accessTokenValidator;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            AuthorizationResult result = new AuthorizationResult();
            httpContext.Items["Authorization.Result"] = result;
            try
            {
                string accessToken = httpContext.GetBearerToken();

                accessTokenValidator.Validate(accessToken);

                Authorization authorization = authorizationRepository.First(a => a.AccessToken == accessToken);

                httpContext.Items["Authorization"] = authorization;
                httpContext.Items["Authorization.Client"] = authorization.Client;
                httpContext.Items["Authorization.Connection"] = authorization.Connection;
                httpContext.Items["Authorization.Account"] = authorization.Connection.Account;

                result.Successed = true;
                result.Result = authorization;


            }
            catch (Exception e)
            {
                result.Successed = false;
                result.Exception = e;
                while(result.Exception.InnerException != null)
                {
                    result.Exception = result.Exception.InnerException;
                }
                Console.Error.WriteLine("Access token is absent or is not valid");
            }

            await nextDelegate.Invoke(httpContext);
        }
    }


    public static class AccessTokenMiddlewareExtension
    {
        public static void UseAccessTokenAuthorization(this IApplicationBuilder app)
        {
            app.UseMiddleware<AccessTokenMiddleware>();
        }
    }
}
