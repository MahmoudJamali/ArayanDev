using Entities.Concrete;
using Entities.Concrete.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
namespace DataAccess.Concrete.Contexts
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserOtp> UserOtps { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollment { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var now = DateTime.UtcNow;


            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.HasIndex(u => u.PhoneNumber)
                      .IsUnique();

                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserClaim>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Claims)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserOtp>(entity =>
            {
                entity.HasOne(o => o.User)
                      .WithMany()
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);



                entity.HasIndex(o => o.UserId);
            });
            modelBuilder.Entity<CourseEnrollment>()
    .HasOne(e => e.User)
    .WithMany(u => u.Enrollments)
    .HasForeignKey(e => e.UserId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseEnrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseEnrollment>()
                .HasIndex(e => new { e.UserId, e.CourseId })
                .IsUnique();



            modelBuilder.Entity<UserProfile>()
    .HasOne(p => p.User)
    .WithOne(u => u.Profile)
    .HasForeignKey<UserProfile>(p => p.UserId)
    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}



