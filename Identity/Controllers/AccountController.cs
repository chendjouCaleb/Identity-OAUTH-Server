using Everest.Identity.Core;
using Everest.Identity.Models;
using Everest.Identity.Core.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Everest.Identity.Filters;
using Everest.Identity.Core.Binding;

namespace Everest.Identity.Controllers
{
    /// <summary>
    /// Controleur pour gérer les comptes utilisateurs dans les applications.
    /// <see cref="Account"/>
    /// </summary>
    [Route("api/accounts")]
    public class AccountController:Controller
    {
        public IRepository<Account, string> accountRepository { get; set; }
        private IPasswordHasher<Account> passwordHasher;

        public AccountController(IRepository<Account, string> accountRepository, IPasswordHasher<Account> passwordHasher)
        {
            this.accountRepository = accountRepository;
            this.passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Recherche un compte par un ID
        /// </summary>
        /// <param name="account">Le compte à chercher</param>
        /// <returns>Un Compte correspondant à l'ID spécifié</returns>
        [HttpGet("{accountId}")]
        [LoadAccount]
        public Account Find(Account account) => account;


        [HttpGet]
        public ActionResult<Account> List([FromQuery]string username)
        {
            if(username != null)
            {
                return Ok(accountRepository.First(a => a.Username == username));
            }else
            {
                return Ok(accountRepository.List());
            }
        }
        

        /// <summary>
        /// Permet de créer un compte utilisateur.
        ///
        /// </summary>
        /// <param name="model">Contient les informations sur l'utilisateur et dont sur le futur compte.</param>
        /// <remarks>L'email renseigné pour le compte ne doit pas être utilisé par un autre compte.</remarks>
        /// <returns>Le compte nouvellement crée</returns>
        ///
        [ValidModel]
        [HttpPost]
        public Account Create([FromBody] AddAccountModel model)
        {

            if(accountRepository.Exists(a => a.Email == model.Email))
            {
                ModelState.ThrowModelError("email", "Cet adresse électronique est déjà utilisée");
            }

            Account account = new Account
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email
            };

            string username = model.Name + model.Surname.Substring(0, 1).ToUpper() + model.Surname.Substring(1);
            long usernameUsage = accountRepository.Count(a => a.Name == model.Name && a.Surname == model.Surname);

            if (usernameUsage == 0)
            {
                account.Username = username;
            }
            else
            {
                account.Username = username + usernameUsage;
            }
            
            string password = passwordHasher.HashPassword(account, model.Password);

            account.Password = password;

            account = accountRepository.Save(account);

            return account;
        }


        /// <summary>
        /// Permet de modifier l'email de connexion d'un compte.
        /// </summary>
        /// <param name="account">Le compte dont on souhaite modifier l'email.</param>
        /// <param name="email">Le nouvel email du compte.</param>
        /// <returns>Un <see>StatusCodeResult</see> de code 201 indiquant que l'email a été modifié.</returns>
        /// <response code="201">Si l'email est mis à jour.</response>
        /// <exception cref="InvalidValueException">Si l'email renseigné est déjà utilisé par un autre compte.</exception>

        [Authorize]
        [LoadAccount]
        [RequireAccountOwner]
        [HttpPut("{accountId}/email")]
        public StatusCodeResult UpdateEmail(Account account, [FromQuery] string email)
        {
            if (accountRepository.Exists(a => a.Email == email))
            {
                throw new InvalidValueException("Cet adresse électronique est déjà utilisée");
            }

            account.Email = email;
            accountRepository.Update(account);

            return StatusCode(201);
        }

        /// <summary>
        /// Permet de modifier le numéro de téléphone d'un compte.
        /// </summary>
        /// <param name="account">Le compte dont on souhaite modifier le numéro de téléphone.</param>
        /// <param name="phoneNumber">Le nouveau numéro de téléphone du compte.</param>
        /// <returns>Un <see>StatusCodeResult</see> de code 201 indiquant que le numéro de 
        /// téléphone a été modifié.</returns>
        /// <response code="400"></response>
        /// <exception cref="InvalidValueException">Si le numéro de téléphone renseigné 
        /// est déjà utilisé par un autre compte.</exception>

        [Authorize]
        [RequireAccountOwner]
        [HttpPut("{accountId}/phone")]
        public StatusCodeResult UpdatePhoneNumber(Account account, [FromQuery] string phoneNumber)
        {
            if (accountRepository.Exists(a => a.PhoneNumber == phoneNumber))
            {
                throw new InvalidValueException("Ce numéro de téléphone est déjà utilisée");
            }

            account.PhoneNumber = phoneNumber;
            accountRepository.Update(account);

            return StatusCode(201);
        }

        /// <summary>
        /// Permet de modifier le nom d'utilisateur d'un compte.
        /// </summary>
        /// <param name="account">Le compte dont on souhaite modifier le nom d'utilisateur.</param>
        /// <param name="username">Le nouveau nom d'utilisateur du compte.</param>
        /// <returns>Un <see>StatusCodeResult</see> de code 201 indiquant que le nom d'utilisateur
        /// a été modifié.</returns>
        /// <response code="400"></response>
        /// <exception cref="InvalidValueException">Si le nom d'utilisateur renseigné 
        /// est déjà utilisé par un autre compte.</exception>

        [Authorize]
        [RequireAccountOwner]
        [HttpPut("{accountId}/username")]
        public StatusCodeResult UpdateUsername(Account account, [FromQuery] string username)
        {
            if(accountRepository.Exists(a => a.Username == username))
            {
                throw new InvalidValueException("Ce nom d'utilisateur est déjà utilisée");
            }

            account.Username = username;
            accountRepository.Update(account);

            return StatusCode(202);
        }

        /// <summary>
        /// Permet de modifier les informations d'état civil d'un compte.
        /// </summary>
        /// <param name="account">Le compte à modifier.</param>
        /// <param name="info">Les nouvelles informations du compte.</param>
        /// <returns>Le compte avec ses nouvelles informations.</returns>
        [Authorize]
        [RequireAccountOwner]
        [HttpPut("{accountId}/info")]
        public Account UpdateInfo(Account account, [FromBody] AccountInfo info)
        {
            account.Name = info.Name;
            account.Surname = info.Surname;
            account.BirthDate = info.BirthDate;
            account.NationalId = info.NationalId;
            account.Gender = info.Gender;

            accountRepository.Update(account);

            return account;
        }


        /// <summary>
        /// Permet de modifier l'adresse d'un compte.
        /// </summary>
        /// <param name="account">Le compte à modifier.</param>
        /// <param name="address">Les informations sur na nouvelle adresse du compte.</param>
        /// <returns>Le compte avec ses nouvelles informations d'adresse.</returns>
        [Authorize]
        [RequireAccountOwner]
        [HttpPut("{accountId}/address")]
        public Account UpdateAddress(Account account, [FromBody] Address address)
        {
            account.Country = address.Country;
            account.State = address.State;
            account.City = address.City;
            account.Street = address.Street;
            account.PostalCode = address.PostalCode;

            accountRepository.Update(account);

            return account;
        }

        /// <summary>
        /// Permet de modifier le mot de passe d'un compte.
        /// </summary>
        /// <param name="account">Le compte à modifier.</param>
        /// <param name="model">Les informations sur na nouvelle adresse du compte.</param>
        /// <returns>Un <see>StatusCodeResult</see> de code 201 indiquant que le mot de passe 
        /// a été modifié.</returns>
        [Authorize]
        [RequireAccountOwner]
        [HttpPut("{accountId}/password")]
        public StatusCodeResult ChangePassword(Account account, UpdatePasswordModel model)
        {
            if(PasswordVerificationResult.Success !=
                passwordHasher.VerifyHashedPassword(account, account.Password, model.CurrentPassword))
            {
                throw new InvalidValueException("Votre mot de passe actuel est incorrect");
            }
            string password = passwordHasher.HashPassword(account, model.NewPassword);

            account.Password = password;

            accountRepository.Update(account);

            return StatusCode(202);
        }

        /// <summary>
        /// Pour réinitialiser le mot de passe d'un compte.
        /// Celà peut arriver si le propriétaire du compte perd son mot de passe.
        /// </summary>
        /// <param name="account">Le compte à modifier.</param>
        /// <param name="model">Contient le code de réinitialisation et le nouveau mot de passe.</param>
        /// <exception cref="InvalidValueException">Si le code de réinitialisation renseigné
        /// n'est pas celui assigné au compte.</exception>
        /// 
        /// <exception cref="InvalidOperationException"> Si le code de réinitialisation est expiré soit 
        /// 10 minutes après sa création.</exception>
        /// 
        /// <returns>Un <see>StatusCodeResult</see> de code 201 indiquant que le mot de passe 
        /// a été modifié.</returns>
        /// 
        [HttpPut("{accountId}/password/reset")]
        public StatusCodeResult ResetPassword(Account account, ResetPasswordModel model)
        {
            if(model.Code != account.ResetPasswordCode)
            {
                throw new InvalidValueException("Le code de réinitialisation de mot de passe est incorrect");
            }

            if(DateTime.Now.Subtract(account.ResetPasswordCodeCreateTime).TotalMinutes > 9.99)
            {
                throw new InvalidOperationException("Le code de réinitialisation de mot de passe est expirée");
            }


            string password = passwordHasher.HashPassword(account, model.NewPassword);

            account.Password = password;

            accountRepository.Update(account);

            return StatusCode(202);
        }

        /// <summary>
        ///     Permet de vérifier que le mot de passe fourni est celui qui compte fourni.
        /// </summary>
        /// <param name="account">Le compte à vérifier.</param>
        /// <param name="password">Le mot de passe à vérifier.</param>
        /// <returns>
        ///     <code>true</code> Si le mot de passe est bien celui du compte.
        /// </returns>
        /// 
        
        [HttpPut("{accountId}/password/check")]
        public bool CheckPassword(Account account, [FromForm] string password)
        {
            return PasswordVerificationResult.Success ==
                passwordHasher.VerifyHashedPassword(account, account.Password, password);
        }

        /// <summary>
        /// Permet de télécharger l'image d'un compte.
        /// </summary>
        /// <param name="account">Le compte dont on souhaite obtenir l'image.</param>
        /// <returns>Le fichier qui est l'image du compte.</returns>
        [HttpGet("{accountId}/image")]
        public async Task<IActionResult> DownloadImage(Account account)
        {
            return null;
        }


        /// <summary>
        /// Permet de changer l'image d'un compte.
        /// </summary>
        /// <param name="account">Le compte à modifier.</param>
        /// <param name="image">Fichier image venant d'un formulaire et qui est la nouvelle image.</param>
        ///
        /// <returns>
        ///     Un <see>StatusCodeResult</see> de code 201 indiquant que l'image a été modifié.
        /// </returns>
        [HttpPut("{accountId}/image")]
        public async Task<StatusCodeResult> ChangeImage(Account account, IFormFile image)
        {
            string fileName = $"{account.Id.ToString()}.{image.Name.Split('.').Last()}";
            string path = Path.Combine(Constant.ACCOUNT_IMAGE_FOLDER, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            account.ImageName = fileName;

            accountRepository.Update(account);
            return Ok();
        }
    }

    
}
