using Company.Access.User.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Company.Access.User.Impl
{
    public class UserContext
        : DbContext
    {
        public DbSet<NameValuePair> NameValuePairs { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        private static void ModelUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NameValuePair>()
                .HasKey(x => x.Name);
        }

        #region Overrides

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ModelUsers(modelBuilder);
        }

        #endregion
    }
}
