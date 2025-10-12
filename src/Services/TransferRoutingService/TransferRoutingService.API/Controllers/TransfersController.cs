using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TransferRoutingService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransfersController : ControllerBase
    {
        private readonly ILogger<TransfersController> _logger;

        public TransfersController(ILogger<TransfersController> logger)
        {
            _logger = logger;
        }

        [HttpPost("initiate")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<ActionResult<ApiResponse<string>>> InitiateTransfer([FromBody] InitiateTransferRequest request)
        {
            try
            {
                var transferId = Guid.NewGuid();
                _logger.LogInformation($"Initiated transfer {transferId} for patient: {request.PatientId}");
                
                return Ok(ApiResponse<string>.SuccessResponse(
                    transferId.ToString(), 
                    "Transfer initiated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating transfer");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error initiating transfer"));
            }
        }

        [HttpGet("{transferId}/status")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public ActionResult<ApiResponse<string>> GetTransferStatus(Guid transferId)
        {
            try
            {
                _logger.LogInformation($"Checking transfer status: {transferId}");
                return Ok(ApiResponse<string>.SuccessResponse("InProgress", "Transfer is in progress"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting transfer status: {transferId}");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error getting transfer status"));
            }
        }
    }

    public record InitiateTransferRequest(
        [Required] Guid PatientId,
        [Required] string SourceFacility,
        [Required] string DestinationFacility,
        string TransferType = "Full");
}
