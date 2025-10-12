using Shared.Common.Models;
using System;

namespace PatientService.Domain.Entities
{
    /// <summary>
    /// Patient document entity for storing medical documents, ID cards, etc.
    /// Documents are stored in encrypted Azure Blob Storage
    /// </summary>
    public class PatientDocument : BaseEntity
    {
        public Guid PatientId { get; set; }
        public string DocumentType { get; set; } = string.Empty; // ID, Insurance, Medical, Lab, Imaging, etc.
        public string DocumentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string BlobStorageUrl { get; set; } = string.Empty; // Encrypted storage URL
        public string? ThumbnailUrl { get; set; }
        public string FileType { get; set; } = string.Empty; // PDF, JPG, PNG, TIFF, etc.
        public long FileSizeBytes { get; set; }
        public string? DocumentDate { get; set; }
        public bool IsEncrypted { get; set; } = true;
        public string? EncryptionKeyId { get; set; }
        public string? FHIRDocumentReferenceId { get; set; }
        public string? OCRText { get; set; } // Extracted text from OCR
        public bool OCRProcessed { get; set; }
        public DateTime? OCRProcessedAt { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public bool IsShared { get; set; }
        public DateTime? SharedUntil { get; set; }
        
        // Navigation
        public virtual Patient? Patient { get; set; }
    }
}
