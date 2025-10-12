using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models;
using System;
using System.Threading.Tasks;

namespace SecurityComplianceService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ComplianceController : ControllerBase
    {
        private readonly ILogger<ComplianceController> _logger;

        public ComplianceController(ILogger<ComplianceController> logger)
        {
            _logger = logger;
        }

        [HttpGet("report/monthly")]
        [ProducesResponseType(typeof(ApiResponse<ComplianceReportDto>), 200)]
        public async Task<ActionResult<ApiResponse<ComplianceReportDto>>> GetMonthlyReport()
        {
            try
            {
                var report = new ComplianceReportDto
                {
                    ReportType = "Monthly",
                    ComplianceScore = 98.5m,
                    TotalIssues = 3,
                    CriticalIssues = 0,
                    MediumIssues = 2,
                    LowIssues = 1,
                    GeneratedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Generated monthly compliance report");
                return Ok(ApiResponse<ComplianceReportDto>.SuccessResponse(report));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating compliance report");
                return StatusCode(500, ApiResponse<ComplianceReportDto>.ErrorResponse("Error generating report"));
            }
        }

        [HttpPost("incident/report")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<ActionResult<ApiResponse<string>>> ReportIncident([FromBody] SecurityIncidentRequest request)
        {
            try
            {
                var incidentId = Guid.NewGuid();
                _logger.LogWarning($"Security incident reported: {request.IncidentType} - Severity: {request.Severity}");
                
                return Ok(ApiResponse<string>.SuccessResponse(
                    incidentId.ToString(), 
                    "Security incident reported and logged"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting security incident");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error reporting incident"));
            }
        }
    }

    public record ComplianceReportDto(
        string ReportType,
        decimal ComplianceScore,
        int TotalIssues,
        int CriticalIssues,
        int MediumIssues,
        int LowIssues,
        DateTime GeneratedAt);

    public record SecurityIncidentRequest(
        string IncidentType,
        string Severity,
        string Description);
}
