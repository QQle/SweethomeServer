using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sweethome.Domain;


namespace SweetHome.DAL
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Problem> Problem { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
            modelBuilder.Entity<User>(builder =>
            {
                builder.Property(x => x.Surname).HasMaxLength(100);
                builder.Property(x => x.LastName).HasMaxLength(100);
                builder.Property(x => x.Address).HasMaxLength(200);

                builder.HasMany(x => x.Problem)  
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);
            });
            modelBuilder.Entity<Problem>().ToTable("Problems");
            modelBuilder.Entity<Problem>(builder =>
            {
                builder.Property(x => x.Id).ValueGeneratedOnAdd();
                builder.Property(x => x.Problems).HasMaxLength(100);
                builder.Property(x => x.Description).HasMaxLength(700);
            });

        }
    }
}
