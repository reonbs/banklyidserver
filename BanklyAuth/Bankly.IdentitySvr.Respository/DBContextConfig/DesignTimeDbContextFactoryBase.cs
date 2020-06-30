using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Bankly.IdentitySvr.Respository.DBContextConfig
{
    public abstract class DesignTimeDbContextFactoryBase<TContext> :
   IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {
        protected string ConnectionStringName { get; }
        protected String MigrationsAssemblyName { get; }
        public DesignTimeDbContextFactoryBase(string connectionStringName, string migrationsAssemblyName)
        {
            ConnectionStringName = connectionStringName;
            MigrationsAssemblyName = migrationsAssemblyName;
        }

        public TContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"reo >>>>>>>>>>>>>>>>> (1) ran");
            return Create(
                Directory.GetCurrentDirectory(),
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                ConnectionStringName, MigrationsAssemblyName);
        }
        protected abstract TContext CreateNewInstance(
            DbContextOptions<TContext> options);

        public TContext CreateWithConnectionStringName(string connectionStringName, string migrationsAssemblyName)
        {
            var environmentName =
                Environment.GetEnvironmentVariable(
                    "ASPNETCORE_ENVIRONMENT");

            Console.WriteLine($"reo >>>>>>>>>>>>>>>>> (2) ran");


            var basePath = "/Users/ren-ekene/Documents/reoproj/banklyidserver/BanklyAuth/Bankly.IdentitySvr"; //AppContext.BaseDirectory;

            return Create(basePath, environmentName, connectionStringName, migrationsAssemblyName);
        }

        private TContext Create(string basePath, string environmentName, string connectionStringName, string migrationsAssemblyName)
        {
            Console.WriteLine($"reo >>>>>>>>>>>>>>>>> (3) ran");

            var basePath2 = "/Users/ren-ekene/Documents/reoproj/banklyidserver/BanklyAuth/Bankly.IdentitySvr";

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath2)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config.GetConnectionString(connectionStringName);

            if (String.IsNullOrWhiteSpace(connstr) == true)
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'default'.");
            }
            else
            {
                return CreateWithConnectionString(connstr, migrationsAssemblyName);
            }
        }

        private TContext CreateWithConnectionString(string connectionString, string migrationsAssemblyName)
        {
            Console.WriteLine($"reo >>>>>>>>>>>>>>>>> (4) ran");

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(
             $"{nameof(connectionString)} is null or empty.",
             nameof(connectionString));

            var optionsBuilder =
                 new DbContextOptionsBuilder<TContext>();

            Console.WriteLine(
                "MyDesignTimeDbContextFactory.Create(string): Connection string: {0}",
                connectionString);

            optionsBuilder.UseNpgsql(connectionString, sqlServerOptions => sqlServerOptions.MigrationsAssembly(migrationsAssemblyName));

            DbContextOptions<TContext> options = optionsBuilder.Options;

            return CreateNewInstance(options);
        }
    }
}
