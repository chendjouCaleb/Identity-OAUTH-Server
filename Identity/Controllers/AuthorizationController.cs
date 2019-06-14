using Everest.Core.Identity;
using Everest.Identity.Core;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Everest.Identity.Controllers
{
    [Route("authorizations")]
    public class AuthorizationController:Controller
    {
        private IRepository<Authorization, long> AuthorizationRepository;
        private IRepository<Connection, long> ConnectionRepository;
        private IRepository<Client, string> ClientRepository;
        private IConfiguration Configuration;

        public AuthorizationController(IRepository<Authorization, long> authorizationRepository, 
            IRepository<Connection, long> connectionRepository, 
            IRepository<Client, string> clientRepository, IConfiguration configuration)
        {
            AuthorizationRepository = authorizationRepository;
            ConnectionRepository = connectionRepository;
            ClientRepository = clientRepository;
            Configuration = configuration;
        }

        /// <summary>
        /// Pour obtenir une authorisation OAUTH2.
        /// Pour le moment seule les applications de confiance auront pourront obtenir des authorisations.
        /// Une authorisation entre une application entre une application cliente et un compte utilisateurs.
        /// 
        /// Une authorization est principalement une jeton JWT contenant des informations sur l'authorisation.
        /// Un jeton de rafraichissement est aussi fournit pour obtenir un nouveau jeton d'accès lorsque l'actuel
        /// est expiré.
        /// 
        /// </summary>
        /// <param name="model">Contient les informations sur l'authorisation.</param>
        /// <exception cref="InvalidValueException">
        ///     Si le code secret fournit n'est pas celui du client renseigné.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Si la connexion renseignée est déjà fermée. 
        ///     Ou si le client a déjà obtenu une authorisation de la connexion.
        /// </exception>
        /// <returns>L'authorisation nouvellement crée.</returns>
        [HttpPost]
        public Authorization Authorize([FromBody] AuthorizeModel model)
        {
            Client client = ClientRepository.Find(model.ClientId);
            if(model.SecretCode != client.SecretCode)
            {
                throw new InvalidValueException("Invalid client secret code");
            }

            Connection connection = ConnectionRepository.Find(model.ConnectionId);

            if(connection.IsClosed)
            {
                throw new InvalidOperationException("La connexion de l'utilisateur est déjà fermée");
            }

            if(AuthorizationRepository.Exists(a => a.Connection.Equals(connection) && a.Client.Equals(client)))
            {
                throw new InvalidOperationException("Cette application a déjà reçue une authorisation pour cette connexion");
            }

            Authorization authorization = new Authorization
            {
                Client = client,
                Connection = connection,
                RefreshToken = Guid.NewGuid().ToString()
            };
            Account account = connection.Account;

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id),
                new Claim(EverestClaims.Username, account.Username),
                new Claim(ClaimTypes.Email, account.Email),
            
                new Claim(EverestClaims.ConnectionData, JsonConvert.SerializeObject(connection)),

                new Claim(EverestClaims.ConnectionId, connection.Id.ToString()),
                new Claim(EverestClaims.ClientId, client.Id.ToString()),
                new Claim(EverestClaims.ClientData, JsonConvert.SerializeObject(client)),
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["authorization:secretKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha384);

            var tokenOptions = new JwtSecurityToken(
                issuer: Configuration["authorization:validIssuer"],
                audience: Configuration["authorization:validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(360),
                signingCredentials: signinCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            authorization.AccessToken = accessToken;
            authorization = AuthorizationRepository.Save(authorization);
            return authorization;
        }


        
    }
}
