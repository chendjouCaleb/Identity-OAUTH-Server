using Everest.Identity.Core.Models;

namespace Everest.Identity.Models
{
    public class Authorization:Entity<string>
    {
        public virtual Client Client { get; set; }

        public string ClientId { get; set; }

        public virtual Connection Connection { get; set; }
        public string ConnectionId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

    }
}
