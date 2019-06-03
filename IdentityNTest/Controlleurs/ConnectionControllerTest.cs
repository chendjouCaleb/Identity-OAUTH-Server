using Everest.Identity.Controllers;
using Everest.Identity.Core;
using Everest.Identity.Models;
using Everest.Identity.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using Microsoft.AspNetCore.Identity;

namespace Everest.IdentityTest.Controlleurs
{
    public class ConnectionControllerTest
    {
        private ConnectionController controller;
        private IRepository<Connection, long> connectionRepository;
        private IRepository<Account, string> accountRepository;
        private LoginModel model;
        private Account account;

        [SetUp]
        public void BeforeEach()
        {
            IServiceCollection serviceCollection = ServiceConfiguration.InitServiceCollection();
            IServiceProvider serviceProvider = ServiceConfiguration.BuildServiceProvider();


            controller = serviceProvider.GetRequiredService<ConnectionController>();
            accountRepository = serviceProvider.GetRequiredService<IRepository<Account, string>>();
            connectionRepository = serviceProvider.GetRequiredService<IRepository<Connection, long>>();
            IPasswordHasher<Account> passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<Account>>();

            account = new Account
            {
                Email = "account@email.com"
            };
            account.Password = passwordHasher.HashPassword(account, "password123");
            account = accountRepository.Save(account);

            model = new LoginModel
            {
                Email = "account@email.com",
                Password = "password123",
                OS = "Windows",
                Navigator = "Firefox"
            };
        }

        [Test]
        public void CreateConnection()
        {
            Connection connection = controller.Create(model);

            Assert.True(connectionRepository.Exists(connection));
            
            Assert.AreEqual(model.OS, connection.OS);
            Assert.AreEqual(model.Navigator, connection.Navigator);
            Assert.AreEqual(model.RemoteAddress, connection.RemoteAddress);
            Assert.AreEqual(model.Persisted, connection.IsPersistent);


            Assert.AreEqual(account, connection.Account);
            
            Assert.NotNull(connection.BeginDate);
            Assert.Null(connection.EndDate);
            Assert.False(connection.IsClosed);
        }

        [Test]
        public void Try_CreateConnection_WithUnusedEmail()
        {
            model.Email = "unusedEmail.com";

            Exception ex = Assert.Throws<EntityNotFoundException>(() => controller.Create(model));
        }

        [Test]
        public void Try_CreateConnection_WithNonAccountPassword()
        {
            model.Password = "fakePassword";

            Assert.Throws<InvalidValueException>(() => controller.Create(model));
        }

        [Test]
        public void CloseConnection()
        {
            Connection connection = controller.Create(model);

            controller.Close(connection);
            Assert.True(connection.IsClosed);
            Assert.NotNull(connection.BeginDate);
            Assert.NotNull(connection.EndDate);
        }

        [Test]
        public void Try_CloseClosedConnection()
        {
            Connection connection = controller.Create(model);

            controller.Close(connection);

            Assert.Throws<InvalidOperationException>(() => controller.Close(connection));
        }
    }
}
