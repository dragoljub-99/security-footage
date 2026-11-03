using Microsoft.EntityFrameworkCore;
using SecurityFootage.Domain.Entities;

namespace SecurityFootage.Infrastructure.Persistence
{
    public class SecurityFootageDbContext : DbContext
    {
        public SecurityFootageDbContext(DbContextOptions<SecurityFootageDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<EventQueue> EventQueues => Set<EventQueue>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<UserLocationAccess> UserAccesses => Set<UserLocationAccess>();
        public DbSet<FootageInformation> FootagesInfo => Set<FootageInformation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);
                                   
                entity.HasIndex(x => x.Nickname)
                      .IsUnique();

                entity.Property(x => x.Nickname)
                      .HasMaxLength(100)
                      .IsRequired();
                      

                entity.Property(x => x.CreatedAtUtc)
                     .IsRequired();
                      
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Action)                    
                      .IsRequired()
                      .HasConversion<string>();

                entity.Property(x => x.Result)
                      .IsRequired()
                      .HasConversion<string>();

                entity.Property(x => x.CreatedAtUtc)
                       .IsRequired();

                entity.Property(x => x.Nickname)
                .IsRequired(false);

                entity.Property(x => x.RequestedCountry)
                      .HasMaxLength(150);

                 entity.Property(x => x.RequestedCity)
                       .HasMaxLength(150);

                entity.HasOne(x => x.User)
                .WithMany(x => x.AuditLogs)
                .HasForeignKey(u => u.UserId)
                .IsRequired(false);

                entity.HasOne(l => l.Location)
                      .WithMany()
                      .HasForeignKey(l => l.LocationId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<EventQueue>(entity =>
            {
                entity.ToTable("EventQueues");

                entity.HasKey(x => x.Id);
                
                entity.Property(x => x.Status)
                    .IsRequired();

                entity.Property(x => x.CreatedAtUtc)
                      .IsRequired();

                entity.HasOne(x => x.User)
                      .WithMany(e => e.EventQueues)
                      .HasForeignKey(u => u.UserId)
                      .IsRequired();
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Locations");

                entity.HasKey(k => k.Id);

                entity.Property(k => k.Country)
                      .HasMaxLength(150);

                entity.Property(x => x.City)
                      .HasMaxLength(150);
                
                entity.HasIndex(x => new
                {
                      x.Country,
                      x.City
                })
                .IsUnique();
                      
            });

            modelBuilder.Entity<UserLocationAccess>(entity =>
            {
                entity.ToTable("LocationAccesses");

                entity.HasKey(x => x.Id);

                entity.HasIndex(x => new
                {
                    x.UserId,
                    x.LocationId                 
                })
                .IsUnique();

                entity.HasOne(x => x.User)
                      .WithMany(x => x.LocationsAccess)
                      .HasForeignKey(x => x.UserId);

                entity.HasOne(x => x.Location)
                      .WithMany(x => x.UserAccesses)
                      .HasForeignKey(x => x.LocationId);
            });

            modelBuilder.Entity<FootageInformation>(entity =>
            {
                  entity.ToTable("FootageInformation");

                  entity.HasKey(k => k.Id);
                  
                  entity.Property(f => f.BlobName)
                        .HasMaxLength(150)
                        .IsRequired();
                  
                  entity.HasOne(x => x.Location)
                        .WithMany(x => x.FootagesInfo)
                        .HasForeignKey(x => x.LocationId);
                        
            });
        }
    }
}