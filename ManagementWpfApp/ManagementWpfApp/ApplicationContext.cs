using Microsoft.EntityFrameworkCore;
using ManagementWpfApp.Models;

namespace ManagementWpfApp
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Mission> Missions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=management.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mission>()
                .HasOne(m => m.Author)
                .WithMany(u => u.AuthoredMissions)
                .HasForeignKey(m => m.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mission>()
                .HasOne(m => m.Performer)
                .WithMany(u => u.AssignedMissions)
                .HasForeignKey(m => m.PerformerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            base.OnModelCreating(modelBuilder);
        }
    }
}