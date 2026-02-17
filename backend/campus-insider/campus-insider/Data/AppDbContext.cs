using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Coride> Corides { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region --- User Configuration ---

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("USER");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            #endregion

            #region --- Notifications Configuration ---
            // In AppDbContext.cs, add to OnModelCreating:

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.IsRead)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ReadAt);

                entity.Property(e => e.EntityType)
                    .HasMaxLength(50);

                entity.Property(e => e.EntityId);

                entity.Property(e => e.ActionUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.ActionText)
                    .HasMaxLength(100);

                // Relationships
                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes for performance
                entity.HasIndex(n => n.UserId);
                entity.HasIndex(n => new { n.UserId, n.IsRead });
                entity.HasIndex(n => n.CreatedAt);
            });
            #endregion

            #region --- Post Configuration ---

            // Configure Post base
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.ImageUrl);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                // Relationship with User
                entity.HasOne(p => p.Author)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index for feed queries
                entity.HasIndex(e => new { e.IsActive, e.CreatedAt });
            });

            // Configure Coride-specific properties
            modelBuilder.Entity<Coride>(entity =>
            {
                entity.Property(e => e.DepartureLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DestinationLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AvailableSeats).IsRequired();
            });

            // Configure Equipment-specific properties
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Category).HasMaxLength(100);
            });

            #endregion
        }
    }
}