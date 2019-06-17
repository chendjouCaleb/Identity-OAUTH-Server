using Everest.Identity.Controllers;
using Everest.Identity.Core;
using Everest.Identity.Models;
using Everest.Identity.Core.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Everest.Identity.Core.Binding;

namespace Everest.IdentityTest.Controlleurs
{
    public class AccountControllerTest
    {
        private AccountController controller;
        private IRepository<Account, string> accountRepository;
        private IPasswordHasher<Account> passwordHasher;
        private AddAccountModel model;

        [SetUp]
        public void BeforeEach()
        {
            IServiceCollection serviceCollection = ServiceConfiguration.InitServiceCollection();
            IServiceProvider serviceProvider = ServiceConfiguration.BuildServiceProvider();


            controller = serviceProvider.GetRequiredService<AccountController>();
            accountRepository = serviceProvider.GetRequiredService<IRepository<Account, string>>();
            passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<Account>>();

            model = new AddAccountModel
            {
                Email = "chendjou@email.com",
                Name = "caleb",
                Surname = "deGrace",
                Password = "password"
            };
        }

        [Test]
        public void CreateAccount()
        {
            Account account = controller.Create(model);
            Assert.True(accountRepository.Exists(account));

            Assert.True(account.Id.Length > 10);
            Assert.AreEqual(model.Name, account.Name);
            Assert.AreEqual(model.Surname, account.Surname);
            Assert.AreEqual(model.Name + model.Surname.Substring(0, 1).ToUpper() + model.Surname.Substring(1), account.Username);
            Assert.AreEqual(model.Email, account.Email);

            Assert.AreEqual(PasswordVerificationResult.Success, passwordHasher.VerifyHashedPassword(account, account.Password, model.Password));
        }

        [Test]
        public void UseSameUsernameMultipleTime()
        {
            Account account = controller.Create(model);
            for (int i = 1; i < 10; i++)
            {
                model.Email = "chendjou@email.com" + i;
                account = controller.Create(model);

                string username = model.Name + model.Surname.Substring(0, 1).ToUpper() + model.Surname.Substring(1) + i;
                Assert.AreEqual(username, account.Username);

            }
        }

        [Test]
        public void Try_UseSameEmailTwoTime()
        {
            controller.Create(model);

            InvalidModelException ex = Assert.Throws<InvalidModelException>(() => controller.Create(model));

            Assert.AreEqual("Cet adresse électronique est déjà utilisée", ex.ModelState["email"].Errors[0].ErrorMessage);
        }


        [Test]
        public void UpdateEmail()
        {
            Account account = controller.Create(model);
            string email = "newEmail@mail.com";

            controller.UpdateEmail(account, email);
            accountRepository.Refresh(account);

            Assert.AreEqual(email, account.Email);
        }

        [Test]
        public void Try_UpdateEmail_WithUsedEmail()
        {
            Account account = controller.Create(model);

            model.Email = "newEmail@mail.com";

            controller.Create(model);

            Assert.Throws<InvalidValueException>(() => controller.UpdateEmail(account, model.Email));

        }



        [Test]
        public void UpdatePhoneNumber()
        {
            Account account = controller.Create(model);
            string phoneNumber = "369852147";

            controller.UpdatePhoneNumber(account, phoneNumber);
            accountRepository.Refresh(account);

            Assert.AreEqual(phoneNumber, account.PhoneNumber);
        }

        [Test]
        public void Try_UpdatePhoneNumber_WithUsedPhoneNumber()
        {
            string phoneNumber = "741258963";

            Account account1 = controller.Create(model);
            account1.PhoneNumber = phoneNumber;
            accountRepository.Update(account1);

            model.Email = "newEmail@mail.com";
            Account account2 = controller.Create(model);

            Assert.Throws<InvalidValueException>(() => controller.UpdatePhoneNumber(account2, phoneNumber));

        }



        [Test]
        public void UpdateUsername()
        {
            Account account = controller.Create(model);
            string username = "newUsername";

            controller.UpdateUsername(account, username);
            accountRepository.Refresh(account);

            Assert.AreEqual(username, account.Username);
        }

        [Test]
        public void Try_UpdateUsername_WithUsedUsername()
        {
            string username = "userName";

            Account account1 = controller.Create(model);
            account1.Username = username;
            accountRepository.Update(account1);

            model.Email = "newEmail@mail.com";
            Account account2 = controller.Create(model);

            Assert.Throws<InvalidValueException>(() => controller.UpdateUsername(account2, username));

        }

        [Test]
        public void UpdateAccountInfo()
        {
            
            AccountInfo info = new AccountInfo
            {
                Name = "new name",
                Surname = "new surname",
                BirthDate = DateTime.Now.AddYears(-22),
                Gender = "M",
                NationalId = "398562475"
            };

            Account account = controller.Create(model);

            account = controller.UpdateInfo(account, info);
            accountRepository.Refresh(account);


            Assert.AreEqual(info.Name, account.Name);
            Assert.AreEqual(info.Surname, account.Surname);
            Assert.AreEqual(info.BirthDate, account.BirthDate);
            Assert.AreEqual(info.NationalId, account.NationalId);
            Assert.AreEqual(info.Gender, account.Gender);
        }



        [Test]
        public void UpdateAddress()
        {
            Address address = new Address
            {
                Country = "Cameroun",
                State = "Centre",
                City = "Yaoundé",
                Street = "Tsoungi Akoa",
                PostalCode = "922"
            };

            Account account = controller.Create(model);

            account = controller.UpdateAddress(account, address);
            accountRepository.Refresh(account);

            Assert.AreEqual(address.Country, account.Country);
            Assert.AreEqual(address.State, account.State);
            Assert.AreEqual(address.City, account.City);
            Assert.AreEqual(address.Street, account.Street);
            Assert.AreEqual(address.PostalCode, account.PostalCode);
        }


        [Test]
        public void ChangePassword()
        {
            Account account = controller.Create(model);
            UpdatePasswordModel passwordModel = new UpdatePasswordModel
            {
                CurrentPassword = model.Password,
                NewPassword = "new password"
            };

            controller.ChangePassword(account, passwordModel);

            accountRepository.Refresh(account);
            Assert.AreEqual(PasswordVerificationResult.Success, 
                passwordHasher.VerifyHashedPassword(account, account.Password, passwordModel.NewPassword));
        }


        [Test]
        public void Try_ChangePassword_WithWrongCurrentPassword()
        {
            Account account = controller.Create(model);
            UpdatePasswordModel passwordModel = new UpdatePasswordModel
            {
                CurrentPassword = "wrong password"
            };

            Assert.Throws<InvalidValueException> (() => controller.ChangePassword(account, passwordModel));
        }


        [Test]
        public void ResetPassword()
        {
            Account account = controller.Create(model);
            account.ResetPasswordCode = "z6ef5z";
            account.ResetPasswordCodeCreateTime = DateTime.Now;
            accountRepository.Update(account);

            ResetPasswordModel passwordModel = new ResetPasswordModel
            {
                Code = "z6ef5z",
                NewPassword = "new password"
            };

            controller.ResetPassword(account, passwordModel);

            accountRepository.Refresh(account);
            Assert.AreEqual(PasswordVerificationResult.Success,
                passwordHasher.VerifyHashedPassword(account, account.Password, passwordModel.NewPassword));
        }


        [Test]
        public void Try_ResetPassword_WithWrongCode()
        {
            Account account = controller.Create(model);
            //Le code sur le compte est déjà nul plus besoin de l'assigné.

            ResetPasswordModel passwordModel = new ResetPasswordModel
            {
                Code = "z6ef5z",
                NewPassword = "new password"
            };

            Assert.Throws<InvalidValueException>(() => controller.ResetPassword(account, passwordModel));
        }

        [Test]
        public void Try_ResetPassword_WithExpiredCode()
        {
            Account account = controller.Create(model);
            account.ResetPasswordCode = "z6ef5z";
            account.ResetPasswordCodeCreateTime = DateTime.Now.AddMinutes(-10);
            accountRepository.Update(account);

            ResetPasswordModel passwordModel = new ResetPasswordModel
            {
                Code = "z6ef5z"
            };


            Assert.Throws<InvalidOperationException>(() => controller.ResetPassword(account, passwordModel));
        }


        [Test]

        public async Task ChangeImageAsync()
        {
            Account account = controller.Create(model);
            FileStream image = File.Open("E:/Lab/static/TestImage/heic0515a.jpg", FileMode.Open);
            


            FormFile formImage = new FormFile(image, 100, image.Length, image.Name, image.Name)
            {
                Headers = new HeaderDictionary()
            };
            formImage.ContentType = "image/jpeg";

            await controller.ChangeImage(account, formImage);

            Assert.AreEqual(account.Id + ".jpg", account.ImageName);
        }
    }

}
