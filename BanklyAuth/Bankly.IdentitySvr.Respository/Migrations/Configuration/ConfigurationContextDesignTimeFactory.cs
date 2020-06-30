using System;
using Bankly.IdentitySvr.Respository.DBContextConfig;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Bankly.IdentitySvr.Respository.Migrations.Configuration
{
    public class ConfigurationContextDesignTimeFactory : DesignTimeDbContextFactoryBase<ConfigurationDbContext>
    {
        public ConfigurationContextDesignTimeFactory()
            : base("DefaultConnection", "Bankly.IdentitySvr.Respository")
        {
        }

        protected override ConfigurationDbContext CreateNewInstance(DbContextOptions<ConfigurationDbContext> options)
        {
            return new ConfigurationDbContext(options, new ConfigurationStoreOptions());
        }
    }
}
