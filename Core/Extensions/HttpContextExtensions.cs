using Microsoft.AspNetCore.Http;
using System;

namespace Everest.Identity.Core.Extensions
{
    public static class HttpContextExtensions
    {
        public static T TryGetItem<T>(this HttpContext context, string key) where T : class
        {
            try
            {
                T item = context.Items[key] as T;

                return item;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static T GetItem<T>(this HttpContext context, string key) where T : class
        {

            var item = context.Items[key];
            if (item == null)
            {
                throw new ElementNotFoundException($"There are no request items with name '{key}'");
            }

            return (T)item;
        }

        public static object GetItem(this HttpContext context, string key)
         => context.Items[key];

        public static T GetResource<T>(this HttpContext context) where T : class
        {
            T item = context.Items["resource"] as T;

            return item;
        }

        public static object GetResource(this HttpContext context)
         => context.Items["resource"];

        public static void SetResource(this HttpContext context, Object resource)
        {
            context.Items["resource"] = resource;
        }

        public static string GetBearerToken(this HttpContext context)
        {
            string bearerToken = context.Request.Headers["Authorization"];

            if (bearerToken == null)
            {
                throw new ArgumentNullException("Aucun jeton d'authentification présent dans l'entête de requete");
            }

            string[] spitted = bearerToken.Split(" ");
            if (spitted.Length != 2)
            {
                throw new InvalidOperationException("Le jeton envoyé est malformé");
            }


            String jwtToken = bearerToken.Split(" ")[1];
            return jwtToken;
        }
    }
}
