import { useState, useEffect } from 'react';

// Note: This is a placeholder. Documents API endpoint needs to be implemented in the backend.
// For now, showing a message that documents feature is coming soon.

export default function Documents() {
  return (
    <div className="space-y-6">
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Medical Documents</h1>
          <button
            className="bg-gray-400 text-white px-4 py-2 rounded-md cursor-not-allowed"
            aria-label="Upload new document"
            disabled
          >
            + Upload Document
          </button>
        </div>

        {/* Coming Soon Message */}
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-12 text-center">
          <div className="text-blue-400 text-6xl mb-4">📄</div>
          <h3 className="text-xl font-semibold text-blue-900 mb-2">Documents Feature Coming Soon</h3>
          <p className="text-blue-800 mb-4">
            The document management system is currently under development. This feature will allow you to:
          </p>
          <ul className="text-left inline-block text-blue-800 text-sm space-y-2 mb-6">
            <li>✓ Upload and store medical documents securely</li>
            <li>✓ View lab results, imaging reports, and discharge summaries</li>
            <li>✓ Download and share documents with your healthcare providers</li>
            <li>✓ Automatic OCR processing and FHIR resource mapping</li>
            <li>✓ HIPAA-compliant encryption for all stored documents</li>
          </ul>
          <div className="text-sm text-blue-700 bg-blue-100 rounded-lg p-4 mt-4">
            <p><strong>For Developers:</strong> A DocumentsController needs to be implemented in the backend API to support:</p>
            <ul className="list-disc list-inside mt-2 text-left">
              <li>File upload with multipart/form-data support</li>
              <li>Document metadata storage (type, category, date, etc.)</li>
              <li>File storage (Azure Blob Storage or similar)</li>
              <li>OCR processing pipeline</li>
              <li>FHIR DocumentReference resource generation</li>
              <li>Download and sharing capabilities</li>
            </ul>
          </div>
        </div>
      </div>

      {/* Upload Instructions */}
      <div className="bg-gray-50 border border-gray-200 rounded-lg p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-2">Planned Upload Guidelines</h2>
        <ul className="list-disc list-inside text-gray-700 text-sm space-y-1">
          <li>Supported formats: PDF, JPEG, PNG, DICOM</li>
          <li>Maximum file size: 25 MB</li>
          <li>Documents will be automatically OCR processed and mapped to FHIR resources</li>
          <li>All documents will be encrypted and HIPAA compliant</li>
          <li>Version control and audit trail for all document operations</li>
        </ul>
      </div>
    </div>
  );
}
