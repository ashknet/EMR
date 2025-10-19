using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;

namespace PatientService.Infrastructure.Data;

/// <summary>
/// Database context for Patient Service (pt schema)
/// </summary>
public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<FamilyRelation> FamilyRelations { get; set; }
    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<Condition> Conditions { get; set; }
    public DbSet<Immunization> Immunizations { get; set; }
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<InsuranceDocument> InsuranceDocuments { get; set; }
    public DbSet<Consent> Consents { get; set; }
    public DbSet<ConsentAudit> ConsentAudits { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Encounter> Encounters { get; set; }
    public DbSet<RecordTransfer> RecordTransfers { get; set; }
    public DbSet<TransferAudit> TransferAudits { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<PatientAddress> PatientAddresses { get; set; }
    public DbSet<PatientPhone> PatientPhones { get; set; }
    public DbSet<EmergencyContact> EmergencyContacts { get; set; }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
    public DbSet<SocialHistory> SocialHistories { get; set; }
    public DbSet<LegalConsent> LegalConsents { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<PatientSurgery> PatientSurgeries { get; set; }
    public DbSet<PatientHospitalization> PatientHospitalizations { get; set; }
    public DbSet<FamilyMedicalHistory> FamilyMedicalHistories { get; set; }
    public DbSet<PatientChronicCondition> PatientChronicConditions { get; set; }
    public DbSet<ChronicCondition> ChronicConditions { get; set; }
    public DbSet<PatientProvider> PatientProviders { get; set; }
    public DbSet<ProviderType> ProviderTypes { get; set; }

    public DbSet<Gender> Genders { get; set; }
    public DbSet<MaritalStatus> MaritalStatuses { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<RelationshipType> RelationshipTypes { get; set; }
    public DbSet<AddressType> AddressTypes { get; set; }
    public DbSet<PhoneType> PhoneTypes { get; set; }
    public DbSet<InsuranceProvider> InsuranceProviders { get; set; }
    public DbSet<SmokingStatus> SmokingStatuses { get; set; }
    public DbSet<AlcoholUse> AlcoholUses { get; set; }
    public DbSet<DrugUse> DrugUses { get; set; }
    public DbSet<LivingSituation> LivingSituations { get; set; }
    public DbSet<CommunicationPreference> CommunicationPreferences { get; set; }
    public DbSet<ChronicConditionLookup> ChronicConditionsLookup { get; set; }
    public DbSet<AllergyType> AllergyTypes { get; set; }
    public DbSet<MedicationLookup> MedicationLookups { get; set; }
    public DbSet<SurgeryType> SurgeryTypes { get; set; }
    public DbSet<ImmunizationType> ImmunizationTypes { get; set; }
    public DbSet<ConditionLookup> ConditionLookups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema to 'pt'
        modelBuilder.HasDefaultSchema("pt");

        // Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId);
            entity.Property(e => e.PatientId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.SsnHash);
            entity.HasIndex(e => new { e.LastName, e.FirstName, e.DateOfBirth });
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.SsnEncrypted).HasMaxLength(500);
            entity.Property(e => e.SsnHash).HasMaxLength(128);
        });

        // FamilyRelation
        modelBuilder.Entity<FamilyRelation>(entity =>
        {
            entity.HasKey(e => e.RelationId);
            entity.Property(e => e.RelationId).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.PatientId, e.RelatedPatientId });
            entity.Property(e => e.RelationType).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.FamilyRelations)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RelatedPatient)
                .WithMany(p => p.RelatedToPatients)
                .HasForeignKey(e => e.RelatedPatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Allergy
        modelBuilder.Entity<Allergy>(entity =>
        {
            entity.HasKey(e => e.AllergyId);
            entity.Property(e => e.AllergyId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.AllergenName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.AllergenType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Severity).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Allergies)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Condition
        modelBuilder.Entity<Condition>(entity =>
        {
            entity.HasKey(e => e.ConditionId);
            entity.Property(e => e.ConditionId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.ConditionName).HasMaxLength(300).IsRequired();
            entity.Property(e => e.ClinicalStatus).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Conditions)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Insurance
        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.HasKey(e => e.InsuranceId);
            entity.Property(e => e.InsuranceId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.MemberId);
            entity.Property(e => e.PayerName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.PlanName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.MemberId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Deductible).HasPrecision(18, 2);
            entity.Property(e => e.DeductibleMet).HasPrecision(18, 2);
            entity.Property(e => e.OutOfPocketMax).HasPrecision(18, 2);
            entity.Property(e => e.OutOfPocketMet).HasPrecision(18, 2);

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Insurances)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // InsuranceDocument
        modelBuilder.Entity<InsuranceDocument>(entity =>
        {
            entity.HasKey(e => e.InsuranceDocumentId);
            entity.Property(e => e.InsuranceDocumentId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.InsuranceId);
            entity.Property(e => e.FileName).HasMaxLength(500).IsRequired();
            entity.Property(e => e.DocumentType).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Insurance)
                .WithMany(i => i.Documents)
                .HasForeignKey(e => e.InsuranceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Consent
        modelBuilder.Entity<Consent>(entity =>
        {
            entity.HasKey(e => e.ConsentId);
            entity.Property(e => e.ConsentId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.ShareToken).IsUnique();
            entity.Property(e => e.ConsentType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Consents)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ConsentAudit
        modelBuilder.Entity<ConsentAudit>(entity =>
        {
            entity.HasKey(e => e.AuditId);
            entity.Property(e => e.AuditId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.ConsentId);
            entity.HasIndex(e => e.ActionDate);
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Consent)
                .WithMany(c => c.ConsentAudits)
                .HasForeignKey(e => e.ConsentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Document
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId);
            entity.Property(e => e.DocumentId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.EncounterId);
            entity.Property(e => e.FileName).HasMaxLength(500).IsRequired();
            entity.Property(e => e.DocumentType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Documents)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Encounter)
                .WithMany(enc => enc.Documents)
                .HasForeignKey(e => e.EncounterId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Encounter
        modelBuilder.Entity<Encounter>(entity =>
        {
            entity.HasKey(e => e.EncounterId);
            entity.Property(e => e.EncounterId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.ActualStartDate);
            entity.Property(e => e.EncounterType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Encounters)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RecordTransfer
        modelBuilder.Entity<RecordTransfer>(entity =>
        {
            entity.HasKey(e => e.TransferId);
            entity.Property(e => e.TransferId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.TrackingNumber).IsUnique();
            entity.Property(e => e.TransferType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Consent)
                .WithMany()
                .HasForeignKey(e => e.ConsentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // TransferAudit
        modelBuilder.Entity<TransferAudit>(entity =>
        {
            entity.HasKey(e => e.AuditId);
            entity.Property(e => e.AuditId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.TransferId);
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Transfer)
                .WithMany(t => t.TransferAudits)
                .HasForeignKey(e => e.TransferId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.NotificationId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => new { e.PatientId, e.IsRead });
            entity.Property(e => e.NotificationType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId);
            entity.Property(e => e.AuditLogId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.EventTimestamp);
            entity.HasIndex(e => new { e.EventType, e.EntityType });
            entity.Property(e => e.EventType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ActorId).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.AuditLogs)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Medication
        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.MedicationId);
            entity.Property(e => e.MedicationId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.MedicationName).HasMaxLength(200).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PatientSurgery
        modelBuilder.Entity<PatientSurgery>(entity =>
        {
            entity.HasKey(e => e.SurgeryId);
            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.SurgeryType).HasMaxLength(200).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PatientHospitalization
        modelBuilder.Entity<PatientHospitalization>(entity =>
        {
            entity.HasKey(e => e.HospitalizationId);
            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.HospitalName).HasMaxLength(200);
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FamilyMedicalHistory
        modelBuilder.Entity<FamilyMedicalHistory>(entity =>
        {
            entity.ToTable("FamilyMedicalHistory", "pt"); // Singular table name to match database
            entity.HasKey(e => e.FamilyHistoryId);
            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.Relative).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Condition).HasMaxLength(200).IsRequired();

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PatientChronicCondition
        modelBuilder.Entity<PatientChronicCondition>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.ConditionId });
            entity.HasIndex(e => e.PatientId);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Condition)
                .WithMany()
                .HasForeignKey(e => e.ConditionId)
                .OnDelete(DeleteBehavior.NoAction); // Changed from Restrict to NoAction to match SQL script
        });

        // ChronicCondition
        modelBuilder.Entity<ChronicCondition>(entity =>
        {
            entity.ToTable("ChronicConditions", "pt");
            entity.HasKey(e => e.ConditionId);
            entity.HasIndex(e => e.PatientId);

            entity.Property(e => e.ConditionName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Active");
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Immunization
        modelBuilder.Entity<Immunization>(entity =>
        {
            entity.ToTable("Immunizations", "pt");
            entity.HasKey(e => e.ImmunizationId);
            entity.Property(e => e.ImmunizationId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.AdministeredDate);

            entity.Property(e => e.VaccineName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AdministeredDate).IsRequired();
            entity.Property(e => e.ProviderName).HasMaxLength(200);
            entity.Property(e => e.AdministeredBy).HasMaxLength(200);
            entity.Property(e => e.FacilityName).HasMaxLength(200);
            entity.Property(e => e.LotNumber).HasMaxLength(50);
            entity.Property(e => e.Site).HasMaxLength(100);
            entity.Property(e => e.Route).HasMaxLength(100);
            entity.Property(e => e.DoseQuantity).HasMaxLength(50);
            entity.Property(e => e.StatusReason).HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Completed");
            entity.Property(e => e.FhirImmunizationId).HasMaxLength(100);
            entity.Property(e => e.VaccineCode).HasMaxLength(50);
            entity.Property(e => e.CodeSystem).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Immunizations)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PatientProvider
        modelBuilder.Entity<PatientProvider>(entity =>
        {
            entity.ToTable("PatientProviders", "pt");
            entity.HasKey(e => e.PatientProviderId);
            entity.Property(e => e.PatientProviderId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => new { e.ProviderTypeId, e.PatientId });
            entity.HasIndex(e => e.NPI).HasFilter("NPI IS NOT NULL");

            entity.Property(e => e.ProviderTypeName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProviderName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Specialty).HasMaxLength(200);
            entity.Property(e => e.NPI).HasMaxLength(10);
            entity.Property(e => e.Phone).HasMaxLength(25);
            entity.Property(e => e.Fax).HasMaxLength(25);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.AddressLine2).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.ZipCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100).HasDefaultValue("USA");
            entity.Property(e => e.PracticeName).HasMaxLength(200);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Active");
            entity.Property(e => e.FhirPractitionerId).HasMaxLength(100);
            entity.Property(e => e.FhirOrganizationId).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProviderType
        modelBuilder.Entity<ProviderType>(entity =>
        {
            entity.ToTable("ProviderTypes", "pt");
            entity.HasKey(e => e.ProviderTypeId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // SocialHistory
        modelBuilder.Entity<SocialHistory>(entity =>
        {
            entity.ToTable("SocialHistories", "pt");
            entity.HasKey(e => e.PatientId);
            entity.Property(e => e.Occupation).HasMaxLength(200);
            entity.Property(e => e.ExerciseFrequency).HasMaxLength(100);
            entity.Property(e => e.Diet).HasMaxLength(100);
            entity.Property(e => e.StressLevel).HasMaxLength(50);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Apply all IEntityTypeConfiguration implementations automatically
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientDbContext).Assembly);
    }
}

