using Everest.Identity.Core.Extensions;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Everest.Identity.Filters
{
    public class LoadAccountAttribute: Attribute, IResourceFilter
    {
        public string ItemName { get; set; } = "account";
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            IRepository<Account, string> repository =
                context.HttpContext.RequestServices.GetRequiredService<IRepository<Account, string>>();
            string id = context.GetParameter("accountId");

            Account account = repository.Find(id);

            context.HttpContext.Items[ItemName] = account;

        }
    }
}
