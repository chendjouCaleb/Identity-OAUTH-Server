using System;
using System.Collections.Generic;
using System.Text;

namespace Everest.Identity.Models
{
    public class AuthorizeModel
    {
        
        public string ClientId { get; set; }

        public string SecretCode { get; set; }

        public long ConnectionId { get; set; }
    }
}
