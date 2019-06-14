using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Everest.Identity.Filters
{
    /// <summary>
    /// Filtre pour vérifier qu'une requête HTTP contient toutes les autorizations.
    /// </summary>
    public class AuthorizeAttribute: ActionFilterAttribute
    {

    }
}
