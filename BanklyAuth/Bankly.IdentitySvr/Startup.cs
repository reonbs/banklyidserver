using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bankly.IdentitySvr.Core.Config;
using Bankly.IdentitySvr.CustomConfiguration;
using Bankly.IdentitySvr.Respository;
using Bankly.IdentitySvr.Respository.Migrations.Users;
using Bankly.IdentitySvr.Respository.Models;
using IdentityServer4.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bankly.IdentitySvr
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            var appSettingsSection = Configuration.GetSection("AppSettings");

            services.Configure<AppSettings>(appSettingsSection);

            services.AddDbContext<AppDbContext>(options =>
              options.UseNpgsql(Configuration.GetConnectionString("Users")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            var migrationsAssembly = "Bankly.IdentitySvr.Respository";

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction = new UserInteractionOptions
                {
                    LogoutUrl = "/Account/Logout",
                    LoginUrl = "/Account/Login",
                    LoginReturnUrlParameter = "returnUrl"
                };
            })
               .AddAspNetIdentity<ApplicationUser>()
               // this adds the config data from DB (clients, resources, CORS)
               .AddConfigurationStore(options =>
               {
                   options.ConfigureDbContext = db =>
                       db.UseNpgsql(connectionString,
                           sql => sql.MigrationsAssembly(migrationsAssembly));
               })
               // this adds the operational data from DB (codes, tokens, consents)
               .AddOperationalStore(options =>
               {
                   options.ConfigureDbContext = db =>
                       db.UseNpgsql(connectionString,
                           sql => sql.MigrationsAssembly(migrationsAssembly));

                   // this enables automatic token cleanup. this is optional.
                   options.EnableTokenCleanup = true;
                   // options.TokenCleanupInterval = 15; // interval in seconds. 15 seconds useful for debugging
               });

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeIdentityServerDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseIdentityServer();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitializeIdentityServerDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                //see data
                if(!context.Clients.Any())
                {
                    foreach (var client in IdServerConfig.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }


                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdServerConfig.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in IdServerConfig.GetAllApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in IdServerConfig.ApiScopes)
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }

                    context.SaveChanges();
                }


                var appdbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                appdbContext.Database.Migrate();

            }
        }
    }
}
