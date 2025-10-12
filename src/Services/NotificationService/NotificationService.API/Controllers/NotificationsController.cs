using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ILogger<NotificationsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("send")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<ActionResult<ApiResponse<string>>> SendNotification([FromBody] SendNotificationRequest request)
        {
            try
            {
                _logger.LogInformation($"Sending {request.NotificationType} notification to patient: {request.PatientId}");
                
                var notificationId = Guid.NewGuid();
                
                // TODO: Implement actual notification sending (email, SMS, push)
                
                return Ok(ApiResponse<string>.SuccessResponse(
                    notificationId.ToString(), 
                    "Notification sent successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error sending notification"));
            }
        }

        [HttpPost("appointment-reminder")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<ActionResult<ApiResponse<string>>> SendAppointmentReminder([FromBody] AppointmentReminderRequest request)
        {
            try
            {
                _logger.LogInformation($"Sending appointment reminder to patient: {request.PatientId}");
                
                return Ok(ApiResponse<string>.SuccessResponse(
                    Guid.NewGuid().ToString(), 
                    "Appointment reminder scheduled"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending appointment reminder");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error sending reminder"));
            }
        }
    }

    public record SendNotificationRequest(
        [Required] Guid PatientId,
        [Required] string NotificationType,
        [Required] string Subject,
        [Required] string Message,
        bool SendEmail = true,
        bool SendSMS = false);

    public record AppointmentReminderRequest(
        [Required] Guid PatientId,
        [Required] DateTime AppointmentDate,
        [Required] string ProviderName,
        string Location = "");
}
