using Everest.Core.Identity;
using Everest.Identity.Controllers;
using Everest.Identity.Core;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Everest.IdentityTest.Controlleurs
{
    public class AuthorizationControllerTest
    {
        private IConfiguration Configuration;
        private AuthorizationController controller;
        private ConnectionController connectionController;
        private AccountController accountController;
        private IRepository<Connection, long> connectionRepository;
        private IRepository<Client, string> clientRepository;
        private IRepository<Authorization, long> authorizationRepository;
        private AuthorizeModel model;
        private Account account;
        private Connection connection;
        private Client client;


        [SetUp]
        public void BeforeEach()
        {
            IServiceCollection serviceCollection = ServiceConfiguration.InitServiceCollection();
            IServiceProvider serviceProvider = ServiceConfiguration.BuildServiceProvider();

            Configuration = serviceProvider.GetRequiredService<IConfiguration>();

            controller = serviceProvider.GetRequiredService<AuthorizationController>();
            connectionController = serviceProvider.GetRequiredService<ConnectionController>();
            accountController = serviceProvider.GetRequiredService<AccountController>();

            connectionRepository = serviceProvider.GetRequiredService<IRepository<Connection, long>>();
            authorizationRepository = serviceProvider.GetRequiredService<IRepository<Authorization, long>>();
            clientRepository = serviceProvider.GetRequiredService<IRepository<Client, string>>();

            account = accountController.Create(new AddAccountModel
            {
                Name = "Chendjou",
                Surname = "Caleb",
                Email = "chendjou@email.com",
                Password = "password",
            });

            connection = connectionController.Create(new LoginModel
            {
                Email = account.Email,
                Password = "password"
            });

            client = clientRepository.Save(new Client
            {
                Name = "Identity Client",
                SecretCode = Guid.NewGuid().ToString()
            });


            model = new AuthorizeModel
            {
                ClientId = client.Id,
                SecretCode = client.SecretCode,
                ConnectionId = connection.Id
            };
        }

        [Test]
        public void Authorize()
        {
            Authorization authorization = controller.Create(model);

            Assert.True(authorizationRepository.Exists(authorization));

            Assert.AreEqual(client, authorization.Client);
            Assert.AreEqual(connection, authorization.Connection);

            Assert.NotNull(authorization.AccessToken);

            ValidateAccessToken(authorization);


        }

        [Test]
        public void Try_AuthorizationWithWrongSecretCode()
        {
            client.SecretCode = "wrong secret code";

            Assert.Throws<InvalidValueException>(() => controller.Create(model));
        }

        [Test]
        public void Try_AuthorizeWithClosedConnection()
        {
            connection.EndDate = DateTime.Now;

            Exception ex = Assert.Throws<InvalidOperationException>(() => controller.Create(model));

            Assert.AreEqual("La connexion de l'utilisateur est déjà fermée", ex.Message);
        }

        [Test]
        public void Try_SameAuthorizationTwoTime()
        {
            controller.Create(model);

            Exception ex = Assert.Throws<InvalidOperationException>(() => controller.Create(model));

            Assert.AreEqual("Cette application a déjà reçue une authorisation pour cette connexion", ex.Message);
        }


        private void ValidateAccessToken(Authorization authorization)
        {
            var handler = new JwtSecurityTokenHandler();

            TokenValidationParameters parameters = new TokenValidationParameters();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["authorization:secretKey"]));


            parameters = new TokenValidationParameters
            {
                IssuerSigningKey = secretKey,
                ValidAudience = Configuration["authorization:validAudience"],
                ValidIssuer = Configuration["authorization:validIssuer"],
                RequireSignedTokens = true
            };

            ClaimsPrincipal claims = handler.ValidateToken(authorization.AccessToken, parameters, out var _);

            Assert.AreEqual(account.Id.ToString(), claims.FindFirstValue(ClaimTypes.NameIdentifier));
            Assert.AreEqual(account.Email, claims.FindFirstValue(ClaimTypes.Email));
            Assert.AreEqual(account.Username, claims.FindFirstValue(EverestClaims.Username));
            Assert.AreEqual(client.Id.ToString(), claims.FindFirstValue(EverestClaims.ClientId));
            Assert.AreEqual(connection.Id.ToString(), claims.FindFirstValue(EverestClaims.ConnectionId));
        }
    }
}
