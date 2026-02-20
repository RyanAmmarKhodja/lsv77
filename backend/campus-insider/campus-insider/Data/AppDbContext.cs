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

        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatMessageRead> ChatMessageReads { get; set; }

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
                entity.Property(e => e.Category)
                    .HasMaxLength(100)
                    .HasConversion<string>();
                entity.Property(e => e.PostType)
                    .HasMaxLength(100)
                    .HasConversion<string>();
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
               
            });

            #endregion

            #region --- ChatConversation Configuration ---

            modelBuilder.Entity<ChatConversation>(entity =>
            {
                entity.ToTable("ChatConversations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("DIRECT");

                entity.Property(e => e.Name)
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastMessageAt);

                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.LastMessageAt);
            });

            #endregion

            #region --- ChatParticipant Configuration ---

            modelBuilder.Entity<ChatParticipant>(entity =>
            {
                entity.ToTable("ChatParticipants");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.JoinedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastReadAt);

                entity.Property(e => e.IsMuted)
                    .HasDefaultValue(false);

                entity.HasOne(cp => cp.Conversation)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(cp => cp.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.User)
                    .WithMany(u => u.ChatParticipants)
                    .HasForeignKey(cp => cp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // User can only be in a conversation once
                entity.HasIndex(cp => new { cp.ConversationId, cp.UserId })
                    .IsUnique();

                entity.HasIndex(cp => cp.UserId);
            });

            #endregion

            #region --- ChatMessage Configuration ---

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessages");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.AttachmentUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.IsEdited)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EditedAt);

                entity.HasOne(m => m.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(m => m.ConversationId);
                entity.HasIndex(m => m.CreatedAt);
                entity.HasIndex(m => m.IsDeleted);
            });

            #endregion

            #region --- ChatMessageRead Configuration ---

            modelBuilder.Entity<ChatMessageRead>(entity =>
            {
                entity.ToTable("ChatMessageReads");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ReadAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(mr => mr.Message)
                    .WithMany(m => m.ReadReceipts)
                    .HasForeignKey(mr => mr.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mr => mr.User)
                    .WithMany()
                    .HasForeignKey(mr => mr.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // User can only read a message once
                entity.HasIndex(mr => new { mr.MessageId, mr.UserId })
                    .IsUnique();
            });

            #endregion
        }
    }
}