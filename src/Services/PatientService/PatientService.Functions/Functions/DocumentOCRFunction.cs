using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientService.Infrastructure.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PatientService.Functions.Functions
{
    /// <summary>
    /// Azure Function for processing uploaded documents with OCR
    /// Extracts text from medical documents, insurance cards, and prescriptions
    /// Triggered when documents are uploaded to blob storage
    /// </summary>
    public class DocumentOCRFunction
    {
        private readonly ILogger<DocumentOCRFunction> _logger;
        private readonly PatientDbContext _dbContext;
        private readonly BlobServiceClient _blobServiceClient;

        public DocumentOCRFunction(
            ILogger<DocumentOCRFunction> logger,
            PatientDbContext dbContext,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _dbContext = dbContext;
            _blobServiceClient = blobServiceClient;
        }

        /// <summary>
        /// Process document when uploaded to blob storage
        /// </summary>
        [Function("DocumentOCRFunction")]
        public async Task Run(
            [BlobTrigger("patient-documents/{name}", Connection = "AzureWebJobsStorage")] Stream documentStream,
            string name)
        {
            _logger.LogInformation($"Processing document: {name}");

            try
            {
                // Extract document ID from blob name (format: patientId_documentId.ext)
                var parts = Path.GetFileNameWithoutExtension(name).Split('_');
                if (parts.Length < 2)
                {
                    _logger.LogWarning($"Invalid document name format: {name}");
                    return;
                }

                var documentIdString = parts[1];
                if (!Guid.TryParse(documentIdString, out var documentId))
                {
                    _logger.LogWarning($"Invalid document ID in name: {name}");
                    return;
                }

                // Get document record from database
                var document = await _dbContext.PatientDocuments
                    .FirstOrDefaultAsync(d => d.Id == documentId);

                if (document == null)
                {
                    _logger.LogWarning($"Document not found in database: {documentId}");
                    return;
                }

                // Perform OCR based on document type
                string extractedText = await PerformOCR(documentStream, document.FileType);

                // Update document record with OCR results
                document.OCRText = extractedText;
                document.OCRProcessed = true;
                document.OCRProcessedAt = DateTime.UtcNow;
                document.OCRConfidenceScore = CalculateConfidenceScore(extractedText);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"OCR completed for document: {documentId}, extracted {extractedText.Length} characters");

                // Trigger additional processing based on document type
                await ProcessExtractedData(document, extractedText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing document: {name}");
                throw;
            }
        }

        private async Task<string> PerformOCR(Stream documentStream, string fileType)
        {
            // Simulate OCR processing
            // In production, use Azure Form Recognizer or Azure Computer Vision
            
            _logger.LogInformation($"Performing OCR on {fileType} document");

            // For demonstration purposes, return sample extracted text
            // Real implementation would use:
            // - Azure Form Recognizer for structured documents
            // - Azure Computer Vision OCR for unstructured documents
            
            await Task.Delay(100); // Simulate processing

            return @"Sample OCR extracted text:
                Patient Name: John Smith
                Date of Birth: 06/15/1985
                MRN: MRN-001-2023
                Document Date: " + DateTime.Now.ToString("MM/dd/yyyy") + @"
                
                [Additional document content would appear here]";
        }

        private decimal CalculateConfidenceScore(string extractedText)
        {
            // Simple confidence calculation based on extracted text quality
            // In production, use actual OCR confidence scores
            
            if (string.IsNullOrWhiteSpace(extractedText))
                return 0;

            if (extractedText.Length < 50)
                return 50.0m;

            if (extractedText.Length > 500)
                return 95.0m;

            return 75.0m;
        }

        private async Task ProcessExtractedData(Domain.Entities.PatientDocument document, string extractedText)
        {
            _logger.LogInformation($"Processing extracted data for document type: {document.DocumentType}");

            // Based on document type, extract structured data
            switch (document.DocumentType.ToLower())
            {
                case "insurance":
                    await ProcessInsuranceCard(document, extractedText);
                    break;
                case "lab":
                    await ProcessLabResults(document, extractedText);
                    break;
                case "prescription":
                    await ProcessPrescription(document, extractedText);
                    break;
                default:
                    _logger.LogInformation($"No specific processing for document type: {document.DocumentType}");
                    break;
            }
        }

        private async Task ProcessInsuranceCard(Domain.Entities.PatientDocument document, string extractedText)
        {
            // Extract insurance information and update Insurance Service
            _logger.LogInformation("Processing insurance card data");
            // TODO: Call Insurance Service API to create/update insurance record
            await Task.CompletedTask;
        }

        private async Task ProcessLabResults(Domain.Entities.PatientDocument document, string extractedText)
        {
            // Extract lab results and update Health History Service
            _logger.LogInformation("Processing lab results");
            // TODO: Call Health History Service API to add lab observations
            await Task.CompletedTask;
        }

        private async Task ProcessPrescription(Domain.Entities.PatientDocument document, string extractedText)
        {
            // Extract prescription information and update Health History Service
            _logger.LogInformation("Processing prescription");
            // TODO: Call Health History Service API to add medication records
            await Task.CompletedTask;
        }
    }
}
