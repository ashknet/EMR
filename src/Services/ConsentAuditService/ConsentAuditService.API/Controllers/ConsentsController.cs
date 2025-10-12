using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsentAuditService.Domain.DTOs;
using ConsentAuditService.Domain.Entities;
using ConsentAuditService.Infrastructure.Data;
using Shared.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConsentAuditService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ConsentsController : ControllerBase
    {
        private readonly ConsentAuditDbContext _context;
        private readonly ILogger<ConsentsController> _logger;

        public ConsentsController(ConsentAuditDbContext context, ILogger<ConsentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ConsentDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ConsentDto>>>> GetByPatient(Guid patientId)
        {
            try
            {
                var consents = await _context.Consents
                    .Where(c => c.PatientId == patientId && !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var dtos = consents.Select(MapToDto).ToList();
                
                // Log audit event
                await LogAuditEvent("ConsentView", patientId, "Success");
                
                return Ok(ApiResponse<IEnumerable<ConsentDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving consents for patient: {patientId}");
                return StatusCode(500, ApiResponse<IEnumerable<ConsentDto>>.ErrorResponse("Error retrieving consents"));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ConsentDto>), 201)]
        public async Task<ActionResult<ApiResponse<ConsentDto>>> Create([FromBody] CreateConsentRequest request)
        {
            try
            {
                var consent = new Consent
                {
                    PatientId = request.PatientId,
                    ConsentType = request.ConsentType,
                    ConsentScope = request.ConsentScope,
                    EffectiveDate = DateTime.UtcNow,
                    ExpirationDate = request.ExpirationDate,
                    IsIndefinite = request.ExpirationDate == null,
                    CanView = request.CanView,
                    CanShare = request.CanShare,
                    CanExport = request.CanExport,
                    GrantedToOrganization = request.GrantedToOrganization,
                    Status = "active",
                    CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system"
                };

                // Generate QR code if requested
                if (request.GenerateQRCode)
                {
                    consent.QRCodeData = GenerateQRCodeData(consent);
                    consent.QRCodeExpiration = DateTime.UtcNow.AddHours(24);
                    consent.QRCodeMaxUsage = 5;
                }

                _context.Consents.Add(consent);
                await _context.SaveChangesAsync();

                // Log audit event
                await LogAuditEvent("ConsentCreate", request.PatientId, "Success", consent.Id);

                _logger.LogInformation($"Created consent: {consent.Id} for patient: {request.PatientId}");
                return CreatedAtAction(nameof(GetByPatient), new { patientId = request.PatientId }, 
                    ApiResponse<ConsentDto>.SuccessResponse(MapToDto(consent), "Consent created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating consent");
                return StatusCode(500, ApiResponse<ConsentDto>.ErrorResponse("Error creating consent"));
            }
        }

        [HttpPut("{id}/revoke")]
        [ProducesResponseType(typeof(ApiResponse<ConsentDto>), 200)]
        public async Task<ActionResult<ApiResponse<ConsentDto>>> RevokeConsent(Guid id, [FromBody] string reason)
        {
            try
            {
                var consent = await _context.Consents.FindAsync(id);
                if (consent == null || consent.IsDeleted)
                    return NotFound(ApiResponse<ConsentDto>.ErrorResponse("Consent not found"));

                consent.IsRevoked = true;
                consent.RevokedAt = DateTime.UtcNow;
                consent.RevokedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
                consent.RevocationReason = reason;
                consent.Status = "inactive";
                
                await _context.SaveChangesAsync();
                
                // Log audit event
                await LogAuditEvent("ConsentRevoke", consent.PatientId, "Success", id);
                
                _logger.LogInformation($"Revoked consent: {id}");
                
                return Ok(ApiResponse<ConsentDto>.SuccessResponse(MapToDto(consent), "Consent revoked successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error revoking consent: {id}");
                return StatusCode(500, ApiResponse<ConsentDto>.ErrorResponse("Error revoking consent"));
            }
        }

        [HttpPost("qr-code/generate")]
        [ProducesResponseType(typeof(ApiResponse<ConsentDto>), 200)]
        public async Task<ActionResult<ApiResponse<ConsentDto>>> GenerateQRCode([FromBody] Guid patientId)
        {
            try
            {
                // Create time-limited consent for QR sharing
                var consent = new Consent
                {
                    PatientId = patientId,
                    ConsentType = "QRCodeSharing",
                    ConsentScope = "EmergencyIntake",
                    EffectiveDate = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow.AddHours(24),
                    CanView = true,
                    CanShare = false,
                    CanExport = false,
                    Status = "active",
                    QRCodeData = GenerateQRCodeData(new Consent { PatientId = patientId }),
                    QRCodeExpiration = DateTime.UtcNow.AddHours(24),
                    QRCodeMaxUsage = 5,
                    CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system"
                };

                _context.Consents.Add(consent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Generated QR code consent for patient: {patientId}");
                return Ok(ApiResponse<ConsentDto>.SuccessResponse(MapToDto(consent), "QR code generated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code");
                return StatusCode(500, ApiResponse<ConsentDto>.ErrorResponse("Error generating QR code"));
            }
        }

        private string GenerateQRCodeData(Consent consent)
        {
            // Generate unique token for QR code
            var token = $"HC-{consent.PatientId:N}-{Guid.NewGuid():N}".Substring(0, 50);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        }

        private async Task LogAuditEvent(string eventType, Guid patientId, string outcome, Guid? targetId = null)
        {
            var auditLog = new AuditLog
            {
                EventType = eventType,
                EventCategory = "Consent",
                EventAction = eventType.Contains("View") ? "View" : eventType.Contains("Create") ? "Create" : "Update",
                EventOutcome = outcome,
                ActorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system",
                ActorType = "User",
                ActorName = User.FindFirst(ClaimTypes.Name)?.Value,
                TargetType = "Consent",
                TargetId = targetId,
                PatientId = patientId,
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsAuthorized = true,
                HasValidConsent = true
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
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
