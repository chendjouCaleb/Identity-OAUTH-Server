using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Everest.Identity.Core.Binding
{
    public class ItemValueModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string name = bindingContext.FieldName;

            object value = bindingContext.HttpContext.Items[name];

            bindingContext.Result = ModelBindingResult.Success(value);

            return Task.CompletedTask;
        }
    }
}
