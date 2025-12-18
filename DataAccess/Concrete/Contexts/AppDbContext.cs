using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
namespace DataAccess.Concrete.Contexts
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // مقداردهی اولیه نقش‌ها
            var now = DateTime.UtcNow;

            modelBuilder.Entity<Role>().HasData(
                new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin", Description = "مدیر سیستم", CreatedDate = now },
                new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "User", Description = "کاربر عادی", CreatedDate = now },
                new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Manager", Description = "مدیر مجموعه", CreatedDate = now },
                new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Secretary", Description = "منشی", CreatedDate = now },
                new { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Employee", Description = "کارمند", CreatedDate = now }
            );


            // Relation بین User و UserClaim
            modelBuilder.Entity<UserClaim>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Claims)   // Claims مجموعه‌ای از UserClaim ها
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ایندکس برای Email یکتا
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // می‌توان ایندکس برای PhoneNumber هم تعریف کرد
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique(false); // اگر می‌خوای شماره موبایل یکتا نباشد

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(u => u.RoleId).IsRequired();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Role>().ToTable("Roles");




        }
    }
}



