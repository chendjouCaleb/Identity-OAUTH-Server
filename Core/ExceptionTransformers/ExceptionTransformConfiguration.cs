using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Core.ExceptionTransformers
{
    public static class ExceptionTransformConfiguration
    {
        public static void AddExceptionTransformerFactory(this IServiceCollection services)
        {
            services.AddSingleton<ExceptionTransformerFactory>();
            services.AddSingleton<IExceptionTransformer, ExceptionTransformer>();
            services.AddSingleton<IExceptionTransformer, InvalidModelExceptionHandler>();
        }

        public static void AddExceptionTransformer<T>(this IServiceCollection services) where T : class, IExceptionTransformer
        {
            services.AddSingleton<IExceptionTransformer, T>();
        }

        public static void UseExceptionTransformer(this IApplicationBuilder builder)
        {
            ExceptionTransformerFactory factory = builder
                .ApplicationServices.GetService<ExceptionTransformerFactory>();

            List<IExceptionTransformer> transformers = builder.ApplicationServices.GetServices<IExceptionTransformer>().ToList();

            foreach (var transformer in transformers)
            {
                factory.AddTransformer(transformer);
            }

            builder.UseMiddleware<ExceptionTransformerMiddleware>();
        }

    }
}
