using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models;
using System;
using System.Threading.Tasks;

namespace AgentManagementService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(ILogger<AgentsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<ActionResult<ApiResponse<string>>> RegisterAgent([FromBody] AgentRegistrationRequest request)
        {
            try
            {
                var apiKey = Guid.NewGuid().ToString("N");
                _logger.LogInformation($"Registered agent for hospital: {request.HospitalName}");
                
                return Ok(ApiResponse<string>.SuccessResponse(apiKey, "Agent registered successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering agent");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error registering agent"));
            }
        }

        [HttpPost("{agentId}/heartbeat")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<ActionResult<ApiResponse<bool>>> Heartbeat(Guid agentId)
        {
            try
            {
                _logger.LogInformation($"Received heartbeat from agent: {agentId}");
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Heartbeat received"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing heartbeat: {agentId}");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Error processing heartbeat"));
            }
        }
    }

    public record AgentRegistrationRequest(string HospitalName, string DeviceId, string AgentVersion);
}
