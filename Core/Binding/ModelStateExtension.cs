using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Identity.Core.Binding
{
    public static class ModelStateExtension
    {
        public static void ThrowModelError(
        this ModelStateDictionary ModelState, string key, string message)
        {
            ModelState.AddModelError(key, message);
            throw new InvalidModelException(ModelState);
        }


        public static void Validate(this ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException(ModelState);
            }

        }
    }
}
