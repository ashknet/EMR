using Microsoft.EntityFrameworkCore;
using HealthHistoryService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HealthHistoryService.Infrastructure.Data
{
    public class HealthHistoryDbContext : DbContext
    {
        public HealthHistoryDbContext(DbContextOptions<HealthHistoryDbContext> options) : base(options)
        {
        }

        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Immunization> Immunizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("health");

            // Condition Configuration
            modelBuilder.Entity<Condition>(entity =>
            {
                entity.ToTable("Conditions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.ConditionName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.ClinicalStatus);
                entity.HasIndex(e => e.ConditionCode);
            });

            // Allergy Configuration
            modelBuilder.Entity<Allergy>(entity =>
            {
                entity.ToTable("Allergies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.AllergenName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Criticality);
            });

            // Medication Configuration
            modelBuilder.Entity<Medication>(entity =>
            {
                entity.ToTable("Medications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.MedicationName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.StartDate);
            });

            // Immunization Configuration
            modelBuilder.Entity<Immunization>(entity =>
            {
                entity.ToTable("Immunizations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.VaccineName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.AdministrationDate);
                entity.HasIndex(e => e.VaccineCode);
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
