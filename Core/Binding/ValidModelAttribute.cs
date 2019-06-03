﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace Identity.Core.Binding
{
    public class ValidModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new InvalidModelException(context.ModelState);
            }
        }
    }
}
