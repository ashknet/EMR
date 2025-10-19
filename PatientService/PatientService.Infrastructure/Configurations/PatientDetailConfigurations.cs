using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Domain.Entities;

namespace PatientService.Infrastructure.Configurations;

public class PatientAddressConfiguration : IEntityTypeConfiguration<PatientAddress>
{
    public void Configure(EntityTypeBuilder<PatientAddress> b)
    {
        b.ToTable("PatientAddresses");
        b.HasKey(e => e.Id);
        b.Property(e => e.Line1).HasMaxLength(200).IsRequired();
        b.Property(e => e.Line2).HasMaxLength(200);
        b.Property(e => e.City).HasMaxLength(100);
        b.Property(e => e.State).HasMaxLength(100);
        b.Property(e => e.PostalCode).HasMaxLength(20);
        b.Property(e => e.Country).HasMaxLength(100);
        b.HasIndex(e => new { e.PatientId, e.AddressTypeId });
        
        // Configure the relationship with Patient
        b.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PatientPhoneConfiguration : IEntityTypeConfiguration<PatientPhone>
{
    public void Configure(EntityTypeBuilder<PatientPhone> b)
    {
        b.ToTable("PatientPhones");
        b.HasKey(e => e.Id);
        b.Property(e => e.PhoneNumber).HasMaxLength(25).IsRequired();
        b.HasIndex(e => new { e.PatientId, e.PhoneTypeId });
        
        // Configure the relationship with Patient
        b.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmergencyContact> b)
    {
        b.ToTable("EmergencyContacts");
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).HasMaxLength(150).IsRequired();
        b.Property(e => e.Phone).HasMaxLength(25).IsRequired();
        b.Property(e => e.AltPhone).HasMaxLength(25);
        b.HasIndex(e => new { e.PatientId, e.RelationshipTypeId });
        
        // Configure the relationship with Patient
        b.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class InsurancePolicyConfiguration : IEntityTypeConfiguration<InsurancePolicy>
{
    public void Configure(EntityTypeBuilder<InsurancePolicy> b)
    {
        b.ToTable("InsurancePolicies");
        b.HasKey(e => e.Id);
        b.Property(e => e.GroupNumber).HasMaxLength(50);
        b.Property(e => e.PolicyNumber).HasMaxLength(50);
        b.Property(e => e.PolicyHolderName).HasMaxLength(150);
        b.Property(e => e.ProviderPhone).HasMaxLength(25);
        b.Property(e => e.MedicareMedicaidId).HasMaxLength(50);
        b.Property(e => e.CardImagePath).HasMaxLength(260);
        b.HasIndex(e => e.PatientId);
        b.HasIndex(e => e.ProviderId);
        
        // Configure the relationship with Patient
        b.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SocialHistoryConfiguration : IEntityTypeConfiguration<SocialHistory>
{
    public void Configure(EntityTypeBuilder<SocialHistory> b)
    {
        b.ToTable("SocialHistory"); // Map to singular table name
        b.HasKey(e => e.PatientId);
        b.Property(e => e.Occupation).HasMaxLength(150);
        b.Property(e => e.ExerciseFrequency).HasMaxLength(50);
        b.Property(e => e.Diet).HasMaxLength(100);
        b.Property(e => e.StressLevel).HasMaxLength(50);
        b.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("getutcdate()");
        b.Property(e => e.UpdatedAt);
        
        // Configure the relationship with Patient
        b.HasOne(e => e.Patient)
            .WithOne() // One-to-one relationship
            .HasForeignKey<SocialHistory>(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class LegalConsentConfiguration : IEntityTypeConfiguration<LegalConsent>
{
    public void Configure(EntityTypeBuilder<LegalConsent> b)
    {
        b.ToTable("LegalConsents"); // Map to plural table name
        b.HasKey(e => e.PatientId);
        b.Property(e => e.SignaturePath).HasMaxLength(260);
        b.Property(e => e.AdvanceDirectivesPath).HasMaxLength(500);
        
        // Ignore the ConsentId property as it's not in the database
        b.Ignore(e => e.ConsentId);
        
        // Configure the relationship with Patient
        b.HasOne(e => e.Patient)
            .WithOne() // One-to-one relationship
            .HasForeignKey<LegalConsent>(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
