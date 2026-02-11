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
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<CarpoolTrip> CarpoolTrips { get; set; }
        public DbSet<CarpoolPassenger> CarpoolPassengers { get; set; }
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

            #region --- Equipment Configuration ---

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.ToTable("Equipment");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.OwnerId)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relationships
                entity.HasOne(e => e.Owner)
                    .WithMany(u => u.Equipment)
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.OwnerId);
                entity.HasIndex(e => e.Category);
            });

            #endregion

            #region --- Loan Configuration ---

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("Loans");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.EquipmentId)
                    .IsRequired();

                entity.Property(e => e.BorrowerId)
                    .IsRequired();

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();

                entity.Property(e => e.RequestedEndDate);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("PENDING");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ReturnedAt);

                // Relationships
                entity.HasOne(l => l.Equipment)
                    .WithMany(e => e.Loans)
                    .HasForeignKey(l => l.EquipmentId)
                    .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete loans if equipment is deleted

                entity.HasOne(l => l.Borrower)
                    .WithMany(u => u.Loans)
                    .HasForeignKey(l => l.BorrowerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(l => l.EquipmentId);
                entity.HasIndex(l => l.BorrowerId);
                entity.HasIndex(l => l.Status);
                entity.HasIndex(l => new { l.StartDate, l.EndDate });
            });

            #endregion

            #region --- CarpoolTrip Configuration ---

            modelBuilder.Entity<CarpoolTrip>(entity =>
            {
                entity.ToTable("CarpoolTrips");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DriverId)
                    .IsRequired();

                entity.Property(e => e.Departure)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Destination)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DepartureTime)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("PENDING");

                entity.Property(e => e.VehicleDescription)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasDefaultValue("");

                entity.Property(e => e.AvailableSeats)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relationships
                entity.HasOne(c => c.Driver)
                    .WithMany(u => u.CarpoolTripsAsDriver)
                    .HasForeignKey(c => c.DriverId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(c => c.DriverId);
                entity.HasIndex(c => c.Status);
                entity.HasIndex(c => c.DepartureTime);
                entity.HasIndex(c => new { c.Departure, c.Destination });
            });

            #endregion

            #region --- CarpoolPassenger Configuration ---

            modelBuilder.Entity<CarpoolPassenger>(entity =>
            {
                entity.ToTable("CarpoolPassengers");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CarpoolTripId)
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.JoinedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relationships
                entity.HasOne(cp => cp.CarpoolTrip)
                    .WithMany(c => c.Passengers)
                    .HasForeignKey(cp => cp.CarpoolTripId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.User)
                    .WithMany(u => u.CarpoolPassengers)
                    .HasForeignKey(cp => cp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(cp => cp.CarpoolTripId);
                entity.HasIndex(cp => cp.UserId);

                // Composite unique index - user can't join the same trip twice
                entity.HasIndex(cp => new { cp.CarpoolTripId, cp.UserId })
                    .IsUnique();
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

        }
    }
}