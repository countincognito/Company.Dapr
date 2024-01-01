using Company.Access.User.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Company.Access.User.Impl
{
    public class UserDbContext
        : DbContext
    {
        public DbSet<NameValueSet> NameValueSets { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        private static void ModelUsers(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<NameValueSet>()
                .HasKey(x => x.Name);
        }

        #region Overrides

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            ModelUsers(modelBuilder);
        }

        #endregion
    }
}
