using Company.iFX.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Company.Access.User.Impl
{
    public class DesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<UserDbContext>();
            builder.UseNpgsql(Configuration.Current.Setting<string>("ConnectionStrings:postgres_users"));
            return new UserDbContext(builder.Options);
        }
    }
}
