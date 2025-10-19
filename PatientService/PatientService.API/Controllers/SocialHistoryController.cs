using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/patients")]
public class SocialHistoryController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<SocialHistoryController> _logger;

    public SocialHistoryController(PatientDbContext context, ILogger<SocialHistoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/patients/{patientId}/social-history
    [HttpGet("{patientId}/social-history")]
    public async Task<ActionResult<SocialHistoryDto>> GetSocialHistory(Guid patientId)
    {
        try
        {
            var socialHistory = await _context.SocialHistories
                .FirstOrDefaultAsync(sh => sh.PatientId == patientId);

            if (socialHistory == null)
            {
                // Return empty object if no social history exists yet
                return Ok(new SocialHistoryDto { PatientId = patientId });
            }

            var dto = new SocialHistoryDto
            {
                PatientId = socialHistory.PatientId,
                SmokingStatus = GetSmokingStatusName(socialHistory.SmokingStatusId),
                AlcoholUse = GetAlcoholUseName(socialHistory.AlcoholUseId),
                DrugUse = GetDrugUseName(socialHistory.DrugUseId),
                Occupation = socialHistory.Occupation,
                LivingSituation = GetLivingSituationName(socialHistory.LivingSituationId),
                ExerciseFrequency = socialHistory.ExerciseFrequency,
                Diet = socialHistory.Diet,
                StressLevel = socialHistory.StressLevel,
                SleepHours = socialHistory.SleepHours
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving social history for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while retrieving social history" });
        }
    }

    // PUT: api/patients/{patientId}/social-history
    [HttpPut("{patientId}/social-history")]
    public async Task<IActionResult> UpdateSocialHistory(Guid patientId, [FromBody] SocialHistoryDto dto)
    {
        try
        {
            var existing = await _context.SocialHistories
                .FirstOrDefaultAsync(sh => sh.PatientId == patientId);

            if (existing == null)
            {
                // Create new if doesn't exist
                var newHistory = new SocialHistory
                {
                    PatientId = patientId,
                    SmokingStatusId = GetSmokingStatusId(dto.SmokingStatus),
                    AlcoholUseId = GetAlcoholUseId(dto.AlcoholUse),
                    DrugUseId = GetDrugUseId(dto.DrugUse),
                    Occupation = dto.Occupation,
                    LivingSituationId = GetLivingSituationId(dto.LivingSituation),
                    ExerciseFrequency = dto.ExerciseFrequency,
                    Diet = dto.Diet,
                    StressLevel = dto.StressLevel,
                    SleepHours = dto.SleepHours,
                    CreatedAt = DateTime.UtcNow
                };

                _context.SocialHistories.Add(newHistory);
            }
            else
            {
                // Update existing
                existing.SmokingStatusId = GetSmokingStatusId(dto.SmokingStatus);
                existing.AlcoholUseId = GetAlcoholUseId(dto.AlcoholUse);
                existing.DrugUseId = GetDrugUseId(dto.DrugUse);
                existing.Occupation = dto.Occupation;
                existing.LivingSituationId = GetLivingSituationId(dto.LivingSituation);
                existing.ExerciseFrequency = dto.ExerciseFrequency;
                existing.Diet = dto.Diet;
                existing.StressLevel = dto.StressLevel;
                existing.SleepHours = dto.SleepHours;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating social history for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while updating social history" });
        }
    }

    #region Helper Methods

    private string? GetSmokingStatusName(int? id)
    {
        if (!id.HasValue) return null;
        return id.Value switch
        {
            1 => "Never",
            2 => "Current",
            3 => "Former",
            _ => null
        };
    }

    private int? GetSmokingStatusId(string? name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return name switch
        {
            "Never" => 1,
            "Current" => 2,
            "Former" => 3,
            _ => null
        };
    }

    private string? GetAlcoholUseName(int? id)
    {
        if (!id.HasValue) return null;
        return id.Value switch
        {
            1 => "None",
            2 => "Occasional",
            3 => "Moderate",
            4 => "Heavy",
            _ => null
        };
    }

    private int? GetAlcoholUseId(string? name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return name switch
        {
            "None" => 1,
            "Occasional" => 2,
            "Moderate" => 3,
            "Heavy" => 4,
            _ => null
        };
    }

    private string? GetDrugUseName(int? id)
    {
        if (!id.HasValue) return null;
        return id.Value switch
        {
            1 => "None",
            2 => "Prescription",
            3 => "Recreational",
            _ => null
        };
    }

    private int? GetDrugUseId(string? name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return name switch
        {
            "None" => 1,
            "Prescription" => 2,
            "Recreational" => 3,
            _ => null
        };
    }

    private string? GetLivingSituationName(int? id)
    {
        if (!id.HasValue) return null;
        return id.Value switch
        {
            1 => "Alone",
            2 => "Family",
            3 => "Partner",
            4 => "Roommates",
            5 => "Assisted",
            6 => "Nursing",
            7 => "Other",
            _ => null
        };
    }

    private int? GetLivingSituationId(string? name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return name switch
        {
            "Alone" => 1,
            "Family" => 2,
            "Partner" => 3,
            "Roommates" => 4,
            "Assisted" => 5,
            "Nursing" => 6,
            "Other" => 7,
            _ => null
        };
    }

    #endregion
}

public class SocialHistoryDto
{
    public Guid PatientId { get; set; }
    public string? SmokingStatus { get; set; }
    public string? AlcoholUse { get; set; }
    public string? DrugUse { get; set; }
    public string? Occupation { get; set; }
    public string? LivingSituation { get; set; }
    public string? ExerciseFrequency { get; set; }
    public string? Diet { get; set; }
    public string? StressLevel { get; set; }
    public int? SleepHours { get; set; }
}

