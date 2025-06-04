namespace QueueApp.Data
{
    using Microsoft.EntityFrameworkCore;
    using QueueApp.Shared.Models;
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<QueueItem> QueueItems { get; set; }
        public DbSet<QueueLink> QueueLinks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<QueueItem>()
                .HasOne(q => q.QueueLink)
                .WithMany(l => l.Items)
                .HasForeignKey(q => q.QueueLinkId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QueueItem>()
                .HasOne(q => q.User)
                .WithMany(u => u.QueueItems)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
