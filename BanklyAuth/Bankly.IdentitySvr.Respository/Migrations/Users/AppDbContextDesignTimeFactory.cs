using System;
using Bankly.IdentitySvr.Respository.DBContextConfig;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Bankly.IdentitySvr.Respository.Migrations.Users
{
    public class AppDbContextDesignTimeFactory : DesignTimeDbContextFactoryBase<AppDbContext>
    {
        public AppDbContextDesignTimeFactory()
            : base("Users", "Bankly.IdentitySvr.Respository")
        {
        }

        protected override AppDbContext CreateNewInstance(DbContextOptions<AppDbContext> options)
        {
            return new AppDbContext(options);
        }
    }
}
