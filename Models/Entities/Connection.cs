using Everest.Identity.Core.Binding;
using Everest.Identity.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Everest.Identity.Models
{
    [ModelBinder(BinderType = typeof(ItemValueModelBinder))]
    public class Connection:Entity<long>
    { 
        public string Navigator { get; set; }

        public string RemoteAddress { get; set; }

        public string OS { get; set; }

        public bool IsPersistent { get; set; }


        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsClosed { get => EndDate != null; }


        public virtual Account Account { get; set; }
        public string AccountId { get; set; }

        [JsonIgnore]
        public virtual List<Authorization> Authorizations { get; set; }
    }
}
