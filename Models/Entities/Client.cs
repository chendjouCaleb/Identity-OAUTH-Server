using Everest.Identity.Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Everest.Identity.Models
{
    public class Client:Entity<string>
    {
        public string Name { get; set; }

        public string RedirectURL { get; set; }

        public string SecretCode { get; set; }

        public string ImageName { get; set; }

        public string ImageURL { get; set; }

        [JsonIgnore]
        public virtual List<Authorization> Authorizations { get; set; }
    }
}
