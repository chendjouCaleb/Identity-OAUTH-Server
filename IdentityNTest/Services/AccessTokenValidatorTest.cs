using Everest.Identity.Controllers;
using Everest.Identity.Core.Exceptions;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Models;
using Everest.Identity.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Everest.IdentityTest.Services
{
    public class AccessTokenValidatorTest
    {
        private AuthorizationController controller;

        private IRepository<Client, string> clientRepository;
        private IRepository<Authorization, long> authorizationRepository;
        private IRepository<Connection, long> connectionRepository;
        private IRepository<Account, string> accountRepository;
        private AccessTokenValidator validator;
        private Client client;

        private IServiceCollection serviceCollection;
        private IServiceProvider provider;

        private IConfiguration configuration;

        [SetUp]
        public void BeforeEach()
        {
            serviceCollection = ServiceConfiguration.InitServiceCollection();
            provider = ServiceConfiguration.BuildServiceProvider();

            configuration = provider.GetRequiredService<IConfiguration>();
            clientRepository = provider.GetRequiredService<IRepository<Client, string>>();
            authorizationRepository = provider.GetRequiredService<IRepository<Authorization, long>>();
            connectionRepository = provider.GetRequiredService<IRepository<Connection, long>>();
            accountRepository = provider.GetRequiredService<IRepository<Account, string>>();


            validator = provider.GetRequiredService<AccessTokenValidator>();

            controller = provider.GetRequiredService<AuthorizationController>();

            client = clientRepository.Save(new Client { Name = "clientApp", Id = "d8g1fn" });

        }

        private Authorization CreateAuthorization()
        {
            Account account = accountRepository.Save(new Account {
                Name = "name",
                Surname = "surname",
                Username = "nameSurname",
                Email = "name@gmail.com"
            });
          
            Connection connection = connectionRepository.Save(new Connection { Account = account });

            AuthorizeModel model = new AuthorizeModel { ClientId = client.Id, ConnectionId = connection.Id, SecretCode = client.SecretCode };

            Authorization authorization = controller.Create(model);
            return authorization;
        }

        [Test]
        public void Validate_ValidToken()
        {
            String token = CreateAuthorization().AccessToken;
            validator.Validate(token);
        }

        [Test]
        public void Validate_ModifiedToken()
        {
            String token = CreateAuthorization().AccessToken;

            Assert.Throws<UnauthorizedException>(() => validator.Validate(token + "3"));
            Assert.Throws<UnauthorizedException>(() => validator.Validate("1" + token));
            Assert.Throws<UnauthorizedException>(() => validator.Validate(token.Substring(2)));
        }

        [Test]
        public void Validate_NotExistingAuthorization()
        {
            Authorization authorization = CreateAuthorization();
            String token = authorization.AccessToken;
            authorizationRepository.Delete(authorization);

            Assert.Throws<UnauthorizedException>(() => validator.Validate(token));
        }
    }
}
