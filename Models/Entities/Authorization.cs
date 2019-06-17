using Everest.Identity.Core.Binding;
using Everest.Identity.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Everest.Identity.Models
{
    [ModelBinder(BinderType = typeof(ItemValueModelBinder))]
    public class Authorization:Entity<long>
    {
        public virtual Client Client { get; set; }

        public string ClientId { get; set; }

        public virtual Connection Connection { get; set; }
        public string ConnectionId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

    }
}
