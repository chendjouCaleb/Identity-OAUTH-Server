using Everest.Identity.Models;
using Everest.Identity.Core.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System;
using Everest.Identity.Core;
using Everest.Identity.Filters;
using Everest.Identity.Core.Binding;

namespace Everest.Identity.Controllers
{
    [Route("api/connections")]
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
        [LoadConnection]
        public Connection Find(Connection connection) => connection;

        /// <summary>
        /// Pour obtenir toutes les connexions d'un compte.
        /// </summary>
        /// <param name="account">Le compte dont on souhaite obtenir les connexions.</param>
        /// <returns>Une liste contenant les connexions du compte.</returns>
        [HttpGet]
        [LoadAccount]
        public IList<Connection> List(Account account)
        {
            return connectionRepository.List(c => c.Account.Equals(account));
        }


        /// <summary>
        /// Pour créer une connexion. Autrement dit, pour d'authentifier.
        /// Obtenir une authentification n'est pas suffisante pour accéder 
        /// aux ressources protégées de l'application. Il faut encore 
        /// obtenir une authorization pour l'application que le compte utilise
        /// pour accéder à l'application.
        /// </summary>
        /// <param name="model">Contient les informations sur la connection à créer.</param>
        /// <exception cref="EntityNotFoundException">
        ///     Si l'email renseigné pour la connexion n'est utilisé par aucun compte.
        /// </exception>
        /// <exception cref="InvalidValueException">
        ///     Si le mot de passe renseigné n'est pas celui du compte
        /// </exception>
        /// <returns>La connexion nouvellement crée.</returns>
        [HttpPost]
        [ValidModel]
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


        /// <summary>
        /// Pour fermer une connexion. Autrement dit, déconnecter un compte.
        /// Fermer une connexion entrainement de facto l'invalidation de toutes
        /// les authorisations liées à la connexion.
        /// </summary>
        /// <param name="connection">La connexion à fermer.</param>
        /// <exception cref="InvalidOperationException">
        ///     Si on essaye de fermer une connexion déjà fermée.
        /// </exception>
        /// <returns>
        ///     Un <see>StatusCodeResult</see> de code 204 si la connexion est fermée.
        /// </returns>
        [HttpPut("{connectionId}/close")]
        [LoadConnection]
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
