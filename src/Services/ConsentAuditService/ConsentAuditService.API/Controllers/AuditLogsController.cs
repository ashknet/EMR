using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsentAuditService.Domain.DTOs;
using ConsentAuditService.Infrastructure.Data;
using Shared.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsentAuditService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuditLogsController : ControllerBase
    {
        private readonly ConsentAuditDbContext _context;
        private readonly ILogger<AuditLogsController> _logger;

        public AuditLogsController(ConsentAuditDbContext context, ILogger<AuditLogsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get audit logs for a patient - HIPAA compliance requirement
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AuditLogDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetByPatient(
            Guid patientId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int limit = 100)
        {
            try
            {
                var query = _context.AuditLogs
                    .Where(a => a.PatientId == patientId);

                if (startDate.HasValue)
                    query = query.Where(a => a.EventTimestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(a => a.EventTimestamp <= endDate.Value);

                var logs = await query
                    .OrderByDescending(a => a.EventTimestamp)
                    .Take(limit)
                    .ToListAsync();

                var dtos = logs.Select(l => new AuditLogDto
                {
                    Id = l.Id,
                    EventTimestamp = l.EventTimestamp,
                    EventType = l.EventType,
                    EventAction = l.EventAction,
                    EventOutcome = l.EventOutcome,
                    ActorId = l.ActorId,
                    ActorName = l.ActorName,
                    TargetType = l.TargetType,
                    PatientId = l.PatientId,
                    IPAddress = l.IPAddress,
                    IsAuthorized = l.IsAuthorized,
                    IsAnomaly = l.IsAnomaly
                }).ToList();

                return Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving audit logs for patient: {patientId}");
                return StatusCode(500, ApiResponse<IEnumerable<AuditLogDto>>.ErrorResponse("Error retrieving audit logs"));
            }
        }

        /// <summary>
        /// Get recent system-wide audit logs (Admin only)
        /// </summary>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AuditLogDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetRecent([FromQuery] int hours = 24, [FromQuery] int limit = 100)
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-hours);
                
                var logs = await _context.AuditLogs
                    .Where(a => a.EventTimestamp >= cutoffTime)
                    .OrderByDescending(a => a.EventTimestamp)
                    .Take(limit)
                    .ToListAsync();

                var dtos = logs.Select(l => new AuditLogDto
                {
                    Id = l.Id,
                    EventTimestamp = l.EventTimestamp,
                    EventType = l.EventType,
                    EventAction = l.EventAction,
                    EventOutcome = l.EventOutcome,
                    ActorId = l.ActorId,
                    TargetType = l.TargetType,
                    PatientId = l.PatientId,
                    IsAuthorized = l.IsAuthorized,
                    IsAnomaly = l.IsAnomaly
                }).ToList();

                return Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent audit logs");
                return StatusCode(500, ApiResponse<IEnumerable<AuditLogDto>>.ErrorResponse("Error retrieving audit logs"));
            }
        }

        /// <summary>
        /// Get anomalies detected by security monitoring
        /// </summary>
        [HttpGet("anomalies")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AuditLogDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetAnomalies([FromQuery] int days = 7)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                
                var anomalies = await _context.AuditLogs
                    .Where(a => a.IsAnomaly && a.EventTimestamp >= cutoffDate)
                    .OrderByDescending(a => a.EventTimestamp)
                    .Take(100)
                    .ToListAsync();

                var dtos = anomalies.Select(l => new AuditLogDto
                {
                    Id = l.Id,
                    EventTimestamp = l.EventTimestamp,
                    EventType = l.EventType,
                    ActorId = l.ActorId,
                    PatientId = l.PatientId,
                    IsAnomaly = l.IsAnomaly
                }).ToList();

                return Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving anomalies");
                return StatusCode(500, ApiResponse<IEnumerable<AuditLogDto>>.ErrorResponse("Error retrieving anomalies"));
            }
        }

        private ConsentDto MapToDto(Consent consent)
        {
            return new ConsentDto
            {
                Id = consent.Id,
                PatientId = consent.PatientId,
                ConsentType = consent.ConsentType,
                ConsentScope = consent.ConsentScope,
                Status = consent.Status,
                EffectiveDate = consent.EffectiveDate,
                ExpirationDate = consent.ExpirationDate,
                CanView = consent.CanView,
                CanShare = consent.CanShare,
                CanExport = consent.CanExport,
                GrantedToName = consent.GrantedToName,
                GrantedToOrganization = consent.GrantedToOrganization,
                IsRevoked = consent.IsRevoked,
                QRCodeData = consent.QRCodeData,
                QRCodeExpiration = consent.QRCodeExpiration,
                CreatedAt = consent.CreatedAt
            };
        }
    }
}
