using Identity.Core.ExceptionTransformers;
using System;

namespace Everest.Identity.Core
{
    [ResponseStatusCode(StatusCode = 404)]
    public class ElementNotFoundException : ApplicationException
    {
        public ElementNotFoundException() : base("L'élément demandé est introuvable")
        { }
        public ElementNotFoundException(string message) : base(message)
        { }
    }
}
