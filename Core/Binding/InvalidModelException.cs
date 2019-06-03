using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Core.Binding
{
    public class InvalidModelException:ApplicationException
    {
        public ModelStateDictionary ModelState { get; set; }
        public InvalidModelException(ModelStateDictionary ModelState)
            : base(ExceptionMessage(ModelState))
        {
            this.ModelState = ModelState;
        }

        public InvalidModelException(ModelStateDictionary ModelState, string message) : base(message)
        {
            this.ModelState = ModelState;
        }

        public static string ExceptionMessage(ModelStateDictionary modelState)
        {
            StringBuilder message = new StringBuilder();
            message.Append("Les données du formulaire sont invalides\n");

            foreach (KeyValuePair<string, ModelStateEntry> keyValue in modelState)
            {
                foreach (var m in keyValue.Value.Errors)
                {
                    message.AppendLine($"\t{keyValue.Key} : {m.ErrorMessage}");
                }
            }
            return message.ToString();
        }
    }
}
