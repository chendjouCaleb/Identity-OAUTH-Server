﻿using Everest.Identity.Core;
using Everest.Identity.Models;
using Identity.Core.Binding;
using Everest.Identity.Core.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Everest.Identity.Controllers
{
    [Route("api/accounts")]
    public class AccountController:Controller
    {
        public static string ACCOUNT_IMAGE_FOLDER = "E:/Lab/static/Identity/Account_Image";
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
        /// <param name="id">L'ID du compte à chercher</param>
        /// <returns>Un Compte correspondant à l'ID spécifié</returns>
        [HttpGet("{accountId}")]
        public Account Find(string accountId) => accountRepository.Find(accountId);


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

        public StatusCodeResult UpdateEmail(Account account, [FromQuery] string email)
        {
            if (accountRepository.Exists(a => a.Email == email))
            {
                throw new InvalidValueException("Cet adresse électronique est déjà utilisée");
            }

            account.Email = email;
            accountRepository.Update(account);

            return StatusCode(202);
        }

        public StatusCodeResult UpdatePhoneNumber(Account account, [FromQuery] string phoneNumber)
        {
            if (accountRepository.Exists(a => a.PhoneNumber == phoneNumber))
            {
                throw new InvalidValueException("Ce numéro de téléphone est déjà utilisée");
            }

            account.PhoneNumber = phoneNumber;
            accountRepository.Update(account);

            return StatusCode(202);
        }

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


        [HttpPut("{accountId}/info")]
        public Account UpdateInfo(Account account, [FromBody] AccountInfo info)
        {
            account.Name = info.Name;
            account.Surname = info.Surname;
            account.BirthDate = info.BirthDate;
            account.NationalIDNumber = info.NationalIDNumber;
            account.Gender = info.Gender;

            accountRepository.Update(account);

            return account;
        }

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


        public bool CheckPassword([FromQuery]string accountId, [FromForm] string password)
        {
            Account account = accountRepository.Find(accountId);
            return PasswordVerificationResult.Success ==
                passwordHasher.VerifyHashedPassword(account, account.Password, password);
        }


        public async Task<StatusCodeResult> ChangeImage(Account account, IFormFile image)
        {
            string fileName = $"{account.Id.ToString()}.{image.Name.Split('.').Last()}";
            string path = Path.Combine(ACCOUNT_IMAGE_FOLDER, fileName);

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