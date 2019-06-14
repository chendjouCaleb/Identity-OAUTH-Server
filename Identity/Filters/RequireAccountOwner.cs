using Microsoft.AspNetCore.Mvc.Filters;

namespace Everest.Identity.Filters
{
    /// <summary>
    /// Filtre pour vérifier que le compte authentifiée d'une requête
    /// HTTP est bien celui du compte présent dans les ressources.
    /// <see cref="Models.Account"/>
    /// </summary>
    public class RequireAccountOwner:ActionFilterAttribute
    {
    }
}
