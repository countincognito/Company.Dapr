using Company.iFX.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Company.Access.User.Impl
{
    public class DesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<UserContext>
    {
        public UserContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<UserContext>();
            builder.UseNpgsql(Configuration.Current.Setting<string>("ConnectionStrings:postgres_users"));
            return new UserContext(builder.Options);
        }
    }
}
