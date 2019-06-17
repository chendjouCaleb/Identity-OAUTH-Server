using Everest.Identity.Core.Extensions;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Everest.Identity.Filters
{
    public class LoadConnectionAttribute : Attribute, IResourceFilter
    {
        public string ItemName { get; set; } = "connection";
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            IRepository<Connection, long> repository =
                context.HttpContext.RequestServices.GetRequiredService<IRepository<Connection, long>>();
            string id = context.GetParameter("connectionId");

            Connection connection = repository.Find(long.Parse(id));

            context.HttpContext.Items[ItemName] = connection;

        }

      
    }
}
