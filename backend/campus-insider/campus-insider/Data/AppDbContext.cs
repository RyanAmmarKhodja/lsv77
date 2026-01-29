namespace campus_insider.Data
{
    using campus_insider.Models;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<CarpoolTrip> CarpoolTrips => Set<CarpoolTrip>();
        public DbSet<Equipment> Equipment => Set<Equipment>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // Users
            // =========================================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id");

                entity.Property(e => e.Email)
                      .HasColumnName("email")
                      .HasMaxLength(150)
                      .IsRequired();

                entity.HasIndex(e => e.Email)
                      .IsUnique();

                entity.Property(e => e.Password)
                      .HasColumnName("password")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.FirstName)
                      .HasColumnName("first_name")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.LastName)
                      .HasColumnName("last_name")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Role)
                      .HasColumnName("role")
                      .HasMaxLength(20)
                      .HasDefaultValue("USER")
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // =========================================================
            // Carpool Trips
            // =========================================================
            modelBuilder.Entity<CarpoolTrip >(entity =>
            {
                entity.ToTable("carpool_trips");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Departure)
                      .HasColumnName("departure")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Destination)
                      .HasColumnName("destination")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.DepartureTime)
                      .HasColumnName("departure_time")
                      .IsRequired();

                entity.Property(e => e.AvailableSeats)
                      .HasColumnName("available_seats")
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Driver)
                      .WithMany(u => u.CarpoolTrips)
                      .HasForeignKey(e => e.DriverId)
                      .HasConstraintName("fk_carpool_driver")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================================================
            // Equipment
            // =========================================================
            modelBuilder.Entity<Equipment >(entity =>
            {
                entity.ToTable("equipment");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Category)
                      .HasColumnName("category")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Description)
                      .HasColumnName("description")
                      .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Owner)
                      .WithMany(u => u.Equipment)
                      .HasForeignKey(e => e.OwnerId)
                      .HasConstraintName("fk_equipment_owner")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================================================
            // Loans
            // =========================================================
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("loans");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.StartDate)
                      .HasColumnName("start_date")
                      .IsRequired();

                entity.Property(e => e.EndDate)
                      .HasColumnName("end_date")
                      .IsRequired();

                entity.Property(e => e.Status)
                      .HasColumnName("status")
                      .HasMaxLength(20)
                      .HasDefaultValue("PENDING")
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Equipment)
                      .WithMany(eq => eq.Loans)
                      .HasForeignKey(e => e.EquipmentId)
                      .HasConstraintName("fk_loan_equipment")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Borrower)
                      .WithMany(u => u.Loans)
                      .HasForeignKey(e => e.BorrowerId)
                      .HasConstraintName("fk_loan_borrower")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================================================
            // Notifications
            // =========================================================
            modelBuilder.Entity<Notification >(entity =>
            {
                entity.ToTable("notifications");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                      .HasColumnName("type")
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(e => e.Content)
                      .HasColumnName("content")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.IsRead)
                      .HasColumnName("is_read")
                      .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(e => e.UserId)
                      .HasConstraintName("fk_notification_user")
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
