using Everest.Identity.Models;
using Everest.Identity.Core.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System;
using Everest.Identity.Core;

namespace Everest.Identity.Controllers
{
    [Route("connections")]
    public class ConnectionController:Controller
    {
        private IRepository<Connection, long> connectionRepository;
        private IRepository<Account, string> accountRepository;
        private IPasswordHasher<Account> passwordHasher;

        public ConnectionController(IRepository<Connection, long> connectionRepository, IRepository<Account, string> accountRepository, 
            IPasswordHasher<Account> passwordHasher)
        {
            this.connectionRepository = connectionRepository;
            this.accountRepository = accountRepository;
            this.passwordHasher = passwordHasher;
        }

        [HttpGet("{connectionId}")]
        public Connection Find(long connectionId) => connectionRepository.Find(connectionId);

        [HttpGet]
        public IList<Connection> List(Account account)
        {
            return connectionRepository.List(c => c.Account.Equals(account));
        }

        [HttpPost]
        public Connection Create([FromBody] LoginModel model)
        {
            Account account = accountRepository.First(a => a.Email.Equals(model.Email));

            if(account == null)
            {
                throw new EntityNotFoundException($"Il n'existe aucun compte ayant pour email {model.Email}.");
            }

            if (PasswordVerificationResult.Success !=
                passwordHasher.VerifyHashedPassword(account, account.Password, model.Password))
            {
                throw new InvalidValueException("Mot de passe incorrect");
            }

            Connection connection = new Connection {
                Account = account,

                Navigator = model.Navigator,
                RemoteAddress = model.RemoteAddress,
                OS = model.OS,
                IsPersistent = model.Persisted,

                BeginDate = DateTime.Now,

            };

            connectionRepository.Save(connection);

            return connection;
        }

        

        [HttpPut("{connectionId}/close")]
        public StatusCodeResult Close(Connection connection)
        {
            if (connection.IsClosed)
            {
                throw new InvalidOperationException("Cette connexion a déjà été fermée");
            }
            connection.EndDate = DateTime.Now;
            return NoContent();
        }
    }
}
