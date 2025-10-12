using Microsoft.EntityFrameworkCore;
using FamilyService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyService.Infrastructure.Data
{
    /// <summary>
    /// Family service database context
    /// Manages family relationships, proxies, and emergency contacts
    /// </summary>
    public class FamilyDbContext : DbContext
    {
        public FamilyDbContext(DbContextOptions<FamilyDbContext> options) : base(options)
        {
        }

        public DbSet<FamilyGroup> FamilyGroups { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<ProxyAuthorization> ProxyAuthorizations { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("family");

            // FamilyGroup Configuration
            modelBuilder.Entity<FamilyGroup>(entity =>
            {
                entity.ToTable("FamilyGroups");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.FamilyName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PrimaryAccountHolderId);
                entity.HasIndex(e => e.IsActive);
                
                entity.HasMany(e => e.Members)
                      .WithOne(m => m.FamilyGroup)
                      .HasForeignKey(m => m.FamilyGroupId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // FamilyMember Configuration
            modelBuilder.Entity<FamilyMember>(entity =>
            {
                entity.ToTable("FamilyMembers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.RelationshipType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.FamilyGroupId);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.RelationshipType);
            });

            // ProxyAuthorization Configuration
            modelBuilder.Entity<ProxyAuthorization>(entity =>
            {
                entity.ToTable("ProxyAuthorizations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.AuthorizationType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.ProxyPatientId);
                entity.HasIndex(e => e.Status);
            });

            // EmergencyContact Configuration
            modelBuilder.Entity<EmergencyContact>(entity =>
            {
                entity.ToTable("EmergencyContacts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.ContactName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(500);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.ContactPriority);
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
