using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

/// <summary>
/// Dashboard API - provides home dashboard with summary information
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(PatientDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard summary for a patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <returns>Dashboard summary with insurance, visits, family, and quick actions</returns>
    [HttpGet("{patientId}")]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DashboardResponse>> GetDashboard(Guid patientId)
    {
        var patient = await _context.Patients
            .Include(p => p.Insurances.Where(i => i.IsActive))
            .Include(p => p.FamilyRelations)
            .FirstOrDefaultAsync(p => p.PatientId == patientId && !p.IsDeleted);

        if (patient == null)
        {
            return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
        }

        var upcomingEncounters = await _context.Encounters
            .Where(e => e.PatientId == patientId && 
                       e.PlannedStartDate >= DateTime.UtcNow &&
                       e.Status == "Planned")
            .OrderBy(e => e.PlannedStartDate)
            .Take(5)
            .Select(e => new EncounterSummary
            {
                EncounterId = e.EncounterId,
                EncounterType = e.EncounterType,
                ProviderName = e.ProviderName,
                PlannedStartDate = e.PlannedStartDate,
                FacilityName = e.FacilityName
            })
            .ToListAsync();

        var recentDocuments = await _context.Documents
            .Where(d => d.PatientId == patientId && d.IsActive)
            .OrderByDescending(d => d.CreatedAt)
            .Take(5)
            .Select(d => new DocumentSummary
            {
                DocumentId = d.DocumentId,
                FileName = d.FileName,
                DocumentType = d.DocumentType,
                ServiceDate = d.ServiceDate,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync();

        var unreadNotifications = await _context.Notifications
            .Where(n => n.PatientId == patientId && !n.IsRead && !n.IsArchived)
            .CountAsync();

        var primaryInsurance = patient.Insurances
            .Where(i => i.IsPrimary && i.IsActive)
            .OrderBy(i => i.Priority)
            .FirstOrDefault();

        var response = new DashboardResponse
        {
            PatientId = patient.PatientId,
            PatientName = $"{patient.FirstName} {patient.LastName}",
            PrimaryInsurance = primaryInsurance != null ? new InsuranceSummary
            {
                InsuranceId = primaryInsurance.InsuranceId,
                PayerName = primaryInsurance.PayerName,
                PlanName = primaryInsurance.PlanName,
                MemberId = primaryInsurance.MemberId,
                Deductible = primaryInsurance.Deductible,
                DeductibleMet = primaryInsurance.DeductibleMet,
                OutOfPocketMax = primaryInsurance.OutOfPocketMax,
                OutOfPocketMet = primaryInsurance.OutOfPocketMet
            } : null,
            FamilyMemberCount = patient.FamilyRelations.Count(r => r.IsActive),
            UpcomingVisits = upcomingEncounters,
            RecentDocuments = recentDocuments,
            UnreadNotifications = unreadNotifications,
            QuickActions = new List<QuickAction>
            {
                new QuickAction { Label = "Schedule Appointment", Url = "/appointments/schedule", Icon = "calendar" },
                new QuickAction { Label = "Upload Document", Url = "/documents/upload", Icon = "upload" },
                new QuickAction { Label = "View Insurance", Url = "/insurance", Icon = "card" },
                new QuickAction { Label = "Manage Family", Url = "/family", Icon = "users" },
                new QuickAction { Label = "Request Records", Url = "/transfers/request", Icon = "share" }
            }
        };

        _logger.LogInformation("Dashboard loaded for patient {PatientId}", patientId);

        return Ok(response);
    }
}

#region DTOs

public class DashboardResponse
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public InsuranceSummary? PrimaryInsurance { get; set; }
    public int FamilyMemberCount { get; set; }
    public List<EncounterSummary> UpcomingVisits { get; set; } = new();
    public List<DocumentSummary> RecentDocuments { get; set; } = new();
    public int UnreadNotifications { get; set; }
    public List<QuickAction> QuickActions { get; set; } = new();
}

public class InsuranceSummary
{
    public Guid InsuranceId { get; set; }
    public string PayerName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public decimal? Deductible { get; set; }
    public decimal? DeductibleMet { get; set; }
    public decimal? OutOfPocketMax { get; set; }
    public decimal? OutOfPocketMet { get; set; }
}

public class EncounterSummary
{
    public Guid EncounterId { get; set; }
    public string EncounterType { get; set; } = string.Empty;
    public string? ProviderName { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public string? FacilityName { get; set; }
}

public class DocumentSummary
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public DateTime? ServiceDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class QuickAction
{
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

#endregion

