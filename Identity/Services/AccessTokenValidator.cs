using Everest.Core.Identity;
using Everest.Identity.Core.Exceptions;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Everest.Identity.Services
{
    public class AccessTokenValidator
    {
        private IRepository<Authorization, long> authorizationRepository;
        private IRepository<Connection, long> connectionRepository;
        private IConfiguration configuration;

        public AccessTokenValidator(IRepository<Authorization, long> authorizationRepository, 
            IRepository<Connection, long> connectionRepository, IConfiguration configuration)
        {
            this.authorizationRepository = authorizationRepository;
            this.connectionRepository = connectionRepository;
            this.configuration = configuration;
        }






        /// <summary>
        /// Méthode pour valider un jeton d'accès.
        /// Un jeton est invalide quand ce dernier est périmé.
        /// Il peut aussi être invalide si la connexion à laquelle 
        /// il est liée est déjà fermée.
        /// Si l'autorisation liée au jeton a déjà été fermée.
        /// </summary>
        /// <param name="accessToken">Le jeton d'accès à valider.</param>
        public void Validate(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();

            TokenValidationParameters parameters = new TokenValidationParameters();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["authorization:secretKey"]));


            parameters = new TokenValidationParameters
            {
                IssuerSigningKey = secretKey,
                ValidAudience = configuration["authorization:validAudience"],
                ValidIssuer = configuration["authorization:validIssuer"],
                RequireSignedTokens = true
            };


            ClaimsPrincipal claims;
            try
            {
                claims = handler.ValidateToken(accessToken, parameters, out var _);
            }
            catch (Exception e)
            {
                throw new UnauthorizedException("Erreur lors de la validation du jeton d'accès", e);
            }

            long connectionId = long.Parse(claims.FindFirstValue(EverestClaims.ConnectionId));

            Connection connection = connectionRepository.Find(connectionId);

            if(connection.IsClosed)
            {
                throw new UnauthorizedException("La connexion liée à cette authorisation a déjà été fermée.");
            }

            if(!authorizationRepository.Exists(a => a.AccessToken == accessToken))
            {
                throw new UnauthorizedException("Aucune authorisation n'est liée à ce jeton ou bien " +
                    "l'authorisation ont déjà été supprimée.");
            }

            
        }
    }
}
