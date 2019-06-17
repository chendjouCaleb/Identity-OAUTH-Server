using System;
using System.IO;
using System.Reflection;
using Everest.Identity;
using Everest.Identity.Core.ExceptionTransformers;
using Everest.Identity.Core.Persistence;
using Everest.Identity.Filters;
using Everest.Identity.Infrastruture;
using Everest.Identity.Models;
using Everest.Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddDbContext<DbContext, PersistenceContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(Configuration["Data:ConnectionStrings:Database"]);
            });

            services.AddExceptionTransformerFactory();

            services.AddTransient<IPasswordHasher<Account>, PasswordHasher<Account>>();
            services.AddTransient<ClientSeedData>();
            services.AddTransient<AccessTokenValidator>();

            services.AddRepositories();



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var clientSeed = app.ApplicationServices.GetRequiredService<ClientSeedData>();
            clientSeed.Seed();

            app.UseAccessTokenAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseExceptionTransformer();
            app.UseMvc();
        }
    }
}
