using Everest.Identity.Core;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Everest.Identity.Infrastruture
{
    public class ClientSeedData
    {
        private IRepository<Client, string> clientRepository;

        public ClientSeedData(IRepository<Client, string> clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public void Seed()
        {
            foreach (var client in Clients)
            {
                if(!clientRepository.Exists(c => c.Name == client.Name))
                {
                    clientRepository.Save(client);
                }
                
            }
        }

        public Client[] Clients { get; set; } =
        {
             new Client
            {
                Name = "Identity Web", SecretCode = Guid.NewGuid().ToString(),
                RedirectURL = "http://localhost:7000/authentication/callback"
            },
             new Client
            {
                Name = "Postman", SecretCode = Guid.NewGuid().ToString(),
                RedirectURL = "http://postman/authentication/callback"
            },

             new Client
            {
                Name = "Academic", SecretCode = Guid.NewGuid().ToString(),
                RedirectURL = "http://localhost:7100/authentication/callback"
            },

        };
    }
}
