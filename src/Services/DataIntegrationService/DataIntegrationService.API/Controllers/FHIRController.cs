using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models;
using Shared.FHIR.Services;
using Hl7.Fhir.Model;
using System.Threading.Tasks;

namespace DataIntegrationService.API.Controllers
{
    [ApiController]
    [Route("api/v1/fhir")]
    [Produces("application/json")]
    public class FHIRController : ControllerBase
    {
        private readonly FHIRConverter _fhirConverter;
        private readonly ILogger<FHIRController> _logger;

        public FHIRController(ILogger<FHIRController> logger)
        {
            _logger = logger;
            _fhirConverter = new FHIRConverter();
        }

        /// <summary>
        /// Export patient data as FHIR Bundle
        /// </summary>
        [HttpGet("patient/{patientId}/export")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<ActionResult<ApiResponse<string>>> ExportPatientBundle(Guid patientId)
        {
            try
            {
                // Create FHIR Bundle with patient data
                var bundle = _fhirConverter.CreateBundle(Bundle.BundleType.Collection);
                
                // TODO: Fetch patient data from Patient Service
                // TODO: Add Patient, AllergyIntolerance, Medication, Condition resources
                
                var bundleJson = _fhirConverter.ToJson(bundle);
                
                _logger.LogInformation($"Exported FHIR bundle for patient: {patientId}");
                return Ok(ApiResponse<string>.SuccessResponse(bundleJson, "FHIR bundle exported successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error exporting FHIR bundle for patient: {patientId}");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error exporting FHIR bundle"));
            }
        }

        /// <summary>
        /// Import FHIR Bundle
        /// </summary>
        [HttpPost("import")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<ActionResult<ApiResponse<string>>> ImportBundle([FromBody] string bundleJson)
        {
            try
            {
                var bundle = _fhirConverter.FromJson<Bundle>(bundleJson);
                
                _logger.LogInformation($"Importing FHIR bundle with {bundle.Entry.Count} entries");
                
                // TODO: Process bundle entries and update respective services
                
                return Ok(ApiResponse<string>.SuccessResponse(
                    $"Imported {bundle.Entry.Count} resources", 
                    "FHIR bundle imported successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing FHIR bundle");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error importing FHIR bundle"));
            }
        }

        /// <summary>
        /// Validate FHIR resource
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public ActionResult<ApiResponse<bool>> ValidateResource([FromBody] string resourceJson)
        {
            try
            {
                // Attempt to parse as generic FHIR resource
                var resource = _fhirConverter.FromJson<Resource>(resourceJson);
                
                _logger.LogInformation($"Validated FHIR resource of type: {resource.TypeName}");
                return Ok(ApiResponse<bool>.SuccessResponse(true, "FHIR resource is valid"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FHIR resource validation failed");
                return Ok(ApiResponse<bool>.ErrorResponse($"Invalid FHIR resource: {ex.Message}"));
            }
        }
    }
}
