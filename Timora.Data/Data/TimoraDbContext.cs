using Microsoft.EntityFrameworkCore;
using Timora.Data.Entities;

namespace Timora.Data.Data
{
    /// <summary>
    /// Entity Framework Core database context for the Timora application.
    /// Provides access to Users, HolidayRequests, and Notices entities with proper relationship configuration.
    /// </summary>
    public class TimoraDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the TimoraDbContext with the specified options.
        /// </summary>
        /// <param name="options">The options to configure the context.</param>
        public TimoraDbContext(DbContextOptions<TimoraDbContext> options)
            : base(options) { }

        /// <summary>
        /// Gets or sets the Users entity set for managing user accounts and authentication.
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Notices entity set for managing announcements and notifications.
        /// </summary>
        public DbSet<Notice> Notices { get; set; } = null!;

        /// <summary>
        /// Gets or sets the HolidayRequests entity set for managing employee holiday requests and approvals.
        /// </summary>
        public DbSet<HolidayRequest> HolidayRequests { get; set; } = null!;

        /// <summary>
        /// Configures the model relationships, constraints, and database mappings using Fluent API.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the entities.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUserEntity(modelBuilder);
            ConfigureHolidayRequestEntity(modelBuilder);
            ConfigureNoticeEntity(modelBuilder);
        }

        /// <summary>
        /// Configures the User entity with constraints and default values.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the entity.</param>
        private static void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // Configure string properties
                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(255).IsRequired();
                entity.Property(u => u.UserName).HasMaxLength(50).IsRequired();
                entity.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();

                // Configure enum storage
                entity
                    .Property(u => u.Role)
                    .HasConversion<string>()
                    .HasDefaultValue(UserRole.Employee);

                // Configure indexes for performance
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Users_Email");
                entity.HasIndex(u => u.UserName).IsUnique().HasDatabaseName("IX_Users_UserName");
            });
        }

        /// <summary>
        /// Configures the HolidayRequest entity with relationships, constraints, and default values.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the entity.</param>
        private static void ConfigureHolidayRequestEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HolidayRequest>(entity =>
            {
                // Configure string properties
                entity.Property(hr => hr.Reason).HasMaxLength(500).IsRequired();
                entity.Property(hr => hr.ResolverComment).HasMaxLength(500);

                // Configure enum storage
                entity
                    .Property(hr => hr.Status)
                    .HasConversion<string>()
                    .HasDefaultValue(HolidayRequestStatus.Pending);

                // Configure relationship: HolidayRequest -> User (Requester)
                entity
                    .HasOne(hr => hr.User)
                    .WithMany(u => u.HolidayRequests)
                    .HasForeignKey(hr => hr.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_HolidayRequests_Users_UserId");

                // Configure relationship: HolidayRequest -> User (Resolver)
                entity
                    .HasOne(hr => hr.ResolvedBy)
                    .WithMany(u => u.ResolvedHolidayRequests)
                    .HasForeignKey(hr => hr.ResolvedByUserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_HolidayRequests_Users_ResolvedByUserId");

                // Configure indexes for performance
                entity.HasIndex(hr => hr.UserId).HasDatabaseName("IX_HolidayRequests_UserId");
                entity.HasIndex(hr => hr.Status).HasDatabaseName("IX_HolidayRequests_Status");
                entity.HasIndex(hr => hr.StartDate).HasDatabaseName("IX_HolidayRequests_StartDate");
            });
        }

        /// <summary>
        /// Configures the Notice entity with relationships and constraints.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the entity.</param>
        private static void ConfigureNoticeEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notice>(entity =>
            {
                // Configure string properties
                entity.Property(n => n.Title).HasMaxLength(200).IsRequired();
                entity.Property(n => n.Content).HasMaxLength(2000).IsRequired();

                // Configure relationship: Notice -> User
                entity
                    .HasOne(n => n.User)
                    .WithMany(u => u.Notices)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Notices_Users_UserId");

                // Configure indexes for performance
                entity.HasIndex(n => n.UserId).HasDatabaseName("IX_Notices_UserId");
                entity.HasIndex(n => n.CreatedAt).HasDatabaseName("IX_Notices_CreatedAt");
            });
        }
    }
}
