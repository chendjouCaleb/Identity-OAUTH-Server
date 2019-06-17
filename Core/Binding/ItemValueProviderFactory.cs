using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Everest.Identity.Core.Binding
{
    public class ItemValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var valueProvider = new RouteValueProvider(
                BindingSource.Path,
                context.ActionContext.RouteData.Values);

            context.ValueProviders.Add(valueProvider);

            return Task.CompletedTask;
        }
    }
}
