using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAgentService.API.Data;
using HospitalAgentService.API.Models;

namespace HospitalAgentService.API.Controllers;

/// <summary>
/// Hospital Agent Management API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly AgentDbContext _context;
    private readonly ILogger<AgentController> _logger;

    public AgentController(AgentDbContext context, ILogger<AgentController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Register new hospital agent
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AgentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AgentDto>> RegisterAgent([FromBody] RegisterAgentRequest request)
    {
        var agent = new HospitalAgent
        {
            AgentId = Guid.NewGuid(),
            MachineName = request.MachineName,
            HospitalName = request.HospitalName,
            HospitalNpi = request.HospitalNpi,
            Department = request.Department,
            Version = request.Version,
            RegisteredAt = DateTime.UtcNow,
            LastHeartbeat = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            IsActive = true
        };

        _context.Agents.Add(agent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Registered new agent {AgentId} for {HospitalName}", agent.AgentId, request.HospitalName);

        var dto = new AgentDto
        {
            AgentId = agent.AgentId,
            MachineName = agent.MachineName,
            HospitalName = agent.HospitalName,
            Status = agent.Status,
            RegisteredAt = agent.RegisteredAt
        };

        return CreatedAtAction(nameof(GetAgent), new { agentId = agent.AgentId }, dto);
    }

    /// <summary>
    /// Get agent by ID
    /// </summary>
    [HttpGet("{agentId}")]
    [ProducesResponseType(typeof(AgentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AgentDto>> GetAgent(Guid agentId)
    {
        var agent = await _context.Agents.FindAsync(agentId);

        if (agent == null)
        {
            return NotFound();
        }

        return Ok(new AgentDto
        {
            AgentId = agent.AgentId,
            MachineName = agent.MachineName,
            HospitalName = agent.HospitalName,
            Status = agent.Status,
            RegisteredAt = agent.RegisteredAt,
            LastHeartbeat = agent.LastHeartbeat
        });
    }

    /// <summary>
    /// Send heartbeat from agent
    /// </summary>
    [HttpPost("{agentId}/heartbeat")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Heartbeat(Guid agentId, [FromBody] HeartbeatRequest request)
    {
        var agent = await _context.Agents.FindAsync(agentId);

        if (agent == null)
        {
            return NotFound();
        }

        agent.LastHeartbeat = DateTime.UtcNow;
        agent.Version = request.Version;
        agent.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        await _context.SaveChangesAsync();

        return Ok(new { message = "Heartbeat received", serverTime = DateTime.UtcNow });
    }

    /// <summary>
    /// Process QR code scan for patient intake
    /// </summary>
    [HttpPost("{agentId}/scan-qr")]
    [ProducesResponseType(typeof(QrScanResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<QrScanResult>> ScanQr(Guid agentId, [FromBody] QrScanRequest request)
    {
        // In production, validate QR token with PatientService
        // For now, return mock data
        
        var intakeRequest = new IntakeRequest
        {
            RequestId = Guid.NewGuid(),
            AgentId = agentId,
            RequestType = "QR",
            QrToken = request.QrCode,
            Status = "Pending",
            RequestedAt = DateTime.UtcNow
        };

        _context.IntakeRequests.Add(intakeRequest);
        await _context.SaveChangesAsync();

        _logger.LogInformation("QR scan processed for agent {AgentId}", agentId);

        // Mock patient data
        return Ok(new QrScanResult
        {
            Success = true,
            PatientName = "John Doe",
            DateOfBirth = "1980-05-15",
            MemberId = "BCBS123456789",
            DataTypes = new[] { "Demographics", "Allergies", "Conditions", "Immunizations" }
        });
    }

    /// <summary>
    /// Get intake requests for agent
    /// </summary>
    [HttpGet("{agentId}/intake-requests")]
    [ProducesResponseType(typeof(List<IntakeRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<IntakeRequestDto>>> GetIntakeRequests(Guid agentId)
    {
        var requests = await _context.IntakeRequests
            .Where(r => r.AgentId == agentId)
            .OrderByDescending(r => r.RequestedAt)
            .Take(50)
            .Select(r => new IntakeRequestDto
            {
                RequestId = r.RequestId,
                RequestType = r.RequestType,
                Status = r.Status,
                RequestedAt = r.RequestedAt,
                ProcessedAt = r.ProcessedAt
            })
            .ToListAsync();

        return Ok(requests);
    }
}

#region DTOs

public class AgentDto
{
    public Guid AgentId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string HospitalName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public DateTime? LastHeartbeat { get; set; }
}

public class RegisterAgentRequest
{
    public string MachineName { get; set; } = string.Empty;
    public string HospitalName { get; set; } = string.Empty;
    public string? HospitalNpi { get; set; }
    public string? Department { get; set; }
    public string Version { get; set; } = string.Empty;
}

public class HeartbeatRequest
{
    public string Version { get; set; } = string.Empty;
}

public class QrScanRequest
{
    public string QrCode { get; set; } = string.Empty;
}

public class QrScanResult
{
    public bool Success { get; set; }
    public string? PatientName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? MemberId { get; set; }
    public string[]? DataTypes { get; set; }
}

public class IntakeRequestDto
{
    public Guid RequestId { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

#endregion

