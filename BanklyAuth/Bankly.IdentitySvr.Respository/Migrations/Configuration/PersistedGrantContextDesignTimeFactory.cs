using System;
using Bankly.IdentitySvr.Respository.DBContextConfig;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Bankly.IdentitySvr.Respository.Migrations.Configuration
{
    public class PersistedGrantContextDesignTimeFactory : DesignTimeDbContextFactoryBase<PersistedGrantDbContext>
    {
        public PersistedGrantContextDesignTimeFactory()
            : base("DefaultConnection", "Bankly.IdentitySvr.Respository")
        {
        }

        protected override PersistedGrantDbContext CreateNewInstance(DbContextOptions<PersistedGrantDbContext> options)
        {
            return new PersistedGrantDbContext(options, new OperationalStoreOptions());
        }
    }
}
