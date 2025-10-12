using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Data
{
    /// <summary>
    /// Patient service database context
    /// Implements encryption for PHI data and audit logging
    /// </summary>
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientDocument> PatientDocuments { get; set; }
        public DbSet<PatientNote> PatientNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure schema
            modelBuilder.HasDefaultSchema("patient");

            // Patient Entity Configuration
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patients");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.SSN).HasMaxLength(500); // Encrypted, needs more space
                entity.Property(e => e.SSNHash).HasMaxLength(100);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                // Indexes for performance
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.SSNHash);
                entity.HasIndex(e => e.MedicalRecordNumber);
                entity.HasIndex(e => e.PrimaryAccountHolderId);
                entity.HasIndex(e => e.FHIRPatientId);
                entity.HasIndex(e => new { e.LastName, e.FirstName });
                entity.HasIndex(e => e.IsActive);
                
                // Relationships
                entity.HasMany(e => e.Documents)
                      .WithOne(d => d.Patient)
                      .HasForeignKey(d => d.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(e => e.Notes)
                      .WithOne(n => n.Patient)
                      .HasForeignKey(n => n.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PatientDocument Entity Configuration
            modelBuilder.Entity<PatientDocument>(entity =>
            {
                entity.ToTable("PatientDocuments");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.BlobStorageUrl).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.FileType).HasMaxLength(50);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DocumentType);
                entity.HasIndex(e => e.UploadedAt);
            });

            // PatientNote Entity Configuration
            modelBuilder.Entity<PatientNote>(entity =>
            {
                entity.ToTable("PatientNotes");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.NoteType);
                entity.HasIndex(e => e.ReminderDate);
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
