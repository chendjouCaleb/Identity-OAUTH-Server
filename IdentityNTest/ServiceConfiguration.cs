using Everest.Identity;
using Everest.Identity.Controllers;
using Everest.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Everest.IdentityTest
{
    public class ServiceConfiguration
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static IServiceCollection ServiceCollection { get; set; }

        public static IServiceCollection InitServiceCollection()
        {
            ServiceCollection = new ServiceCollection();

            ServiceCollection.AddDbContext<DbContext, PersistenceContext>(options => {
                options.UseLazyLoadingProxies();
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            ServiceCollection.AddRepositories();


            ServiceCollection.AddTransient<AccountController>();
            ServiceCollection.AddTransient<ConnectionController>();
            ServiceCollection.AddTransient<AuthorizationController>();
            ServiceCollection.AddTransient<IPasswordHasher<Account>, PasswordHasher<Account>>();
            ServiceCollection.AddSingleton<IConfiguration, TestConfiguration>();


            return ServiceCollection;
        }

        public static IServiceProvider BuildServiceProvider()
        {
            ServiceProvider = ServiceCollection.BuildServiceProvider();

            return ServiceProvider;
        }

        
    }
}
