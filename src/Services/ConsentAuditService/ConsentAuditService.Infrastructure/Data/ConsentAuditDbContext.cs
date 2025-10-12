using Microsoft.EntityFrameworkCore;
using ConsentAuditService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsentAuditService.Infrastructure.Data
{
    public class ConsentAuditDbContext : DbContext
    {
        public ConsentAuditDbContext(DbContextOptions<ConsentAuditDbContext> options) : base(options)
        {
        }

        public DbSet<Consent> Consents { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<DataSharingEvent> DataSharingEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("consent");

            // Consent Configuration
            modelBuilder.Entity<Consent>(entity =>
            {
                entity.ToTable("Consents");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.ConsentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ConsentScope).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.EffectiveDate);
                entity.HasIndex(e => e.IsRevoked);
            });

            // AuditLog Configuration (Immutable)
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLog");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.EventTimestamp).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ActorId).IsRequired().HasMaxLength(255);
                entity.Property(e => e.TargetType).IsRequired().HasMaxLength(100);
                
                // Make immutable - no updates or deletes via EF
                entity.Metadata.SetIsReadOnly(false); // Allow inserts only
                
                entity.HasIndex(e => e.EventTimestamp);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.ActorId);
                entity.HasIndex(e => e.EventType);
                entity.HasIndex(e => e.IsAnomaly);
            });

            // DataSharingEvent Configuration
            modelBuilder.Entity<DataSharingEvent>(entity =>
            {
                entity.ToTable("DataSharingEvents");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.ShareMethod).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SharedWith).IsRequired().HasMaxLength(255);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.ConsentId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ShareInitiatedAt);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is Shared.Common.Models.BaseEntity entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entity.CreatedAt = DateTime.UtcNow;
                            entity.IsDeleted = false;
                            break;
                        case EntityState.Modified:
                            entity.UpdatedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
