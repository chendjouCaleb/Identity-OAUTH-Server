using Everest.Identity.Core.ExceptionTransformers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Everest.Identity.Core.Binding
{
    public class BindingErrorModel:ErrorResponseModel
    {
        public IDictionary<string, List<string>> FieldErrors { get; set; }
            = new Dictionary<string, List<string>>();
        public IList<string> GlobalErrors { get; set; } = new List<string>();

        public BindingErrorModel()
        { }

        public BindingErrorModel(ModelStateDictionary modelState)
        {
            foreach (KeyValuePair<string, ModelStateEntry> v in modelState)
            {
                if (v.Key != "")
                {
                    string key = v.Key[0].ToString().ToLower() + v.Key.Substring(1);
                    FieldErrors.Add(key, new List<string>());
                    foreach (ModelError error in v.Value.Errors)
                    {

                        FieldErrors[key].Add(error.ErrorMessage);
                    }
                }
                else
                {
                    foreach (ModelError error in v.Value.Errors)
                    {
                        GlobalErrors.Add(error.ErrorMessage);
                    }
                }
            }
        }
    }
}
