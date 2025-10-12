using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientService.Infrastructure.Data;
using Shared.FHIR.Models;
using Shared.FHIR.Services;
using Hl7.Fhir.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatientService.Functions.Functions
{
    /// <summary>
    /// Azure Function for syncing patient data to FHIR resources
    /// Runs periodically to ensure all patient records have FHIR representations
    /// </summary>
    public class FHIRSyncFunction
    {
        private readonly ILogger<FHIRSyncFunction> _logger;
        private readonly PatientDbContext _dbContext;
        private readonly FHIRConverter _fhirConverter;

        public FHIRSyncFunction(
            ILogger<FHIRSyncFunction> logger,
            PatientDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _fhirConverter = new FHIRConverter();
        }

        /// <summary>
        /// Timer-triggered function that runs daily to sync patient data to FHIR
        /// Schedule: Daily at 2 AM UTC
        /// </summary>
        [Function("FHIRSyncFunction")]
        public async Task Run([TimerTrigger("0 0 2 * * *")] TimerInfo timerInfo)
        {
            _logger.LogInformation($"FHIR sync started at: {DateTime.UtcNow}");

            try
            {
                // Get patients without FHIR resources or with outdated resources
                var patientsToSync = await _dbContext.Patients
                    .Where(p => !p.IsDeleted && p.IsActive)
                    .Where(p => p.FHIRPatientId == null || 
                               p.UpdatedAt > p.CreatedAt) // Updated after creation
                    .Take(100) // Process in batches
                    .ToListAsync();

                _logger.LogInformation($"Found {patientsToSync.Count} patients to sync");

                int syncedCount = 0;
                int errorCount = 0;

                foreach (var patient in patientsToSync)
                {
                    try
                    {
                        // Convert patient to FHIR resource
                        var fhirPatient = ConvertToFHIRPatient(patient);

                        // Serialize to JSON
                        var fhirJson = _fhirConverter.ToJson(fhirPatient);

                        // Update patient record with FHIR data
                        patient.FHIRPatientId = fhirPatient.Id;
                        patient.FHIRResourceJson = fhirJson;

                        syncedCount++;
                        _logger.LogInformation($"Synced patient to FHIR: {patient.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error syncing patient to FHIR: {patient.Id}");
                        errorCount++;
                    }
                }

                // Save all changes
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"FHIR sync completed. Synced: {syncedCount}, Errors: {errorCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FHIR sync function");
                throw;
            }
        }

        private Patient ConvertToFHIRPatient(Domain.Entities.Patient patient)
        {
            var builder = new FHIRPatientBuilder()
                .WithIdentifier(patient.MedicalRecordNumber ?? patient.Id.ToString())
                .WithName(patient.LastName, patient.FirstName, patient.MiddleName)
                .WithBirthDate(patient.DateOfBirth.ToString("yyyy-MM-dd"));

            // Gender mapping
            var gender = patient.Gender.ToLower() switch
            {
                "male" => AdministrativeGender.Male,
                "female" => AdministrativeGender.Female,
                "other" => AdministrativeGender.Other,
                _ => AdministrativeGender.Unknown
            };
            builder.WithGender(gender);

            // Contact information
            if (!string.IsNullOrEmpty(patient.PhoneNumber))
            {
                builder.WithTelecom(patient.PhoneNumber, ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Home);
            }

            // Address
            if (!string.IsNullOrEmpty(patient.AddressLine1))
            {
                builder.WithAddress(
                    patient.AddressLine1,
                    patient.City ?? "",
                    patient.State ?? "",
                    patient.ZipCode ?? "",
                    patient.Country ?? "US"
                );
            }

            // Race (US Core requirement)
            if (!string.IsNullOrEmpty(patient.Race))
            {
                var raceCode = MapRaceToCode(patient.Race);
                builder.WithRace(raceCode, patient.Race);
            }

            // Ethnicity (US Core requirement)
            if (!string.IsNullOrEmpty(patient.Ethnicity))
            {
                var ethnicityCode = MapEthnicityToCode(patient.Ethnicity);
                builder.WithEthnicity(ethnicityCode, patient.Ethnicity);
            }

            return builder.Build();
        }

        private string MapRaceToCode(string race)
        {
            // CDC Race codes from OMB standards
            return race.ToLower() switch
            {
                "white" => "2106-3",
                "black or african american" => "2054-5",
                "asian" => "2028-9",
                "native hawaiian or other pacific islander" => "2076-8",
                "american indian or alaska native" => "1002-5",
                _ => "2131-1" // Other Race
            };
        }

        private string MapEthnicityToCode(string ethnicity)
        {
            // CDC Ethnicity codes
            return ethnicity.ToLower().Contains("hispanic") ? "2135-2" : "2186-5";
        }
    }
}
