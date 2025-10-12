using Microsoft.EntityFrameworkCore;
using InsuranceService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InsuranceService.Infrastructure.Data
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options)
        {
        }

        public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("insurance");

            // InsurancePolicy Configuration
            modelBuilder.Entity<InsurancePolicy>(entity =>
            {
                entity.ToTable("InsurancePolicies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.PolicyNumber).IsRequired().HasMaxLength(500);
                entity.Property(e => e.InsuranceCompanyName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsPrimary);
            });

            // Claim Configuration
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.ToTable("Claims");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.ClaimNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RowVersion).IsRowVersion();
                
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.InsurancePolicyId);
                entity.HasIndex(e => e.ClaimNumber);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ServiceDate);
                
                entity.HasOne(e => e.InsurancePolicy)
                      .WithMany()
                      .HasForeignKey(e => e.InsurancePolicyId)
                      .OnDelete(DeleteBehavior.Restrict);
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
