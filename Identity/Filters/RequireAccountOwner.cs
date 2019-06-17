using Everest.Identity.Core.Exceptions;
using Everest.Identity.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Everest.Identity.Filters
{
    /// <summary>
    /// Filtre pour vérifier que le compte authentifiée d'une requête
    /// HTTP est bien celui du compte présent dans les ressources.
    /// <see cref="Everest.Identity.Models.Account"/>
    /// </summary>
    public class RequireAccountOwner : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Account authAccount = context.HttpContext.Items["Authorization.Account"] as Account;
            Account account = context.HttpContext.Items["account"] as Account;
            System.Console.WriteLine($"Account Id = {account.Id}");
            System.Console.WriteLine($"Auth ccount Id = {authAccount.Id}");

            if (account.Id != authAccount.Id)
            {
                throw new UnauthorizedException("Le compte qui essaye d'accéder à la ressource n'est pas celui du compte de la ressource");
            }
        }
    }
}
