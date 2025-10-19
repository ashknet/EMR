import { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config/apiConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';

interface Transfer {
  transferId: string;
  transferType: string;
  transferMethod: string;
  recipientOrganization?: string;
  recipientName?: string;
  senderOrganization?: string;
  purpose?: string;
  requestedDate: string;
  status: string;
  statusReason?: string;
  completedDate?: string;
  trackingNumber?: string;
  resourceCount?: number;
}

export default function Transfers() {
  const [transfers, setTransfers] = useState<Transfer[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [message, setMessage] = useState('');
  const [showRequestModal, setShowRequestModal] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [transferRequest, setTransferRequest] = useState({
    transferType: 'Outgoing',
    transferMethod: 'FHIR',
    recipientOrganization: '',
    recipientName: '',
    recipientEmail: '',
    purpose: '',
    includeAllRecords: true
  });

  useEffect(() => {
    loadTransfers();
  }, []);

  const loadTransfers = async () => {
    try {
      setIsLoading(true);
      const response = await axios.get(`${API_BASE_URL}/transfers/patient/${MOCK_PATIENT_ID}`);
      setTransfers(response.data || []);
    } catch (error) {
      console.error('Error loading transfers:', error);
      setMessage('✗ Error loading transfers');
    } finally {
      setIsLoading(false);
    }
  };

  const handleRequestTransfer = () => {
    setShowRequestModal(true);
    setMessage('');
  };

  const handleSubmitTransfer = async () => {
    try {
      setIsSubmitting(true);
      
      // Validate required fields
      if (!transferRequest.recipientOrganization && !transferRequest.recipientName) {
        setMessage('✗ Please provide either recipient organization or recipient name');
        return;
      }

      const requestData = {
        patientId: MOCK_PATIENT_ID,
        ...transferRequest,
        requestedDate: new Date().toISOString(),
        status: 'Requested'
      };

      await axios.post(`${API_BASE_URL}/transfers`, requestData);
      
      setMessage('✓ Transfer request submitted successfully');
      setShowRequestModal(false);
      
      // Reset form
      setTransferRequest({
        transferType: 'Outgoing',
        transferMethod: 'FHIR',
        recipientOrganization: '',
        recipientName: '',
        recipientEmail: '',
        purpose: '',
        includeAllRecords: true
      });
      
      // Reload transfers
      await loadTransfers();
    } catch (error) {
      console.error('Error submitting transfer request:', error);
      setMessage('✗ Error submitting transfer request');
    } finally {
      setIsSubmitting(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed':
        return 'bg-green-100 text-green-800';
      case 'Processing':
      case 'In Progress':
        return 'bg-yellow-100 text-yellow-800';
      case 'Requested':
        return 'bg-blue-100 text-blue-800';
      case 'Failed':
      case 'Cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading transfers...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Record Transfers</h1>
          <button
            onClick={handleRequestTransfer}
            className="bg-primary-600 text-white px-4 py-2 rounded-md hover:bg-primary-700 transition-colors"
            aria-label="Request new record transfer"
          >
            + Request Transfer
          </button>
        </div>

        {/* Message Banner */}
        {message && (
          <div className={`mb-6 p-4 rounded-lg ${
            message.startsWith('✓') ? 'bg-green-50 text-green-800 border border-green-200' : 
            'bg-red-50 text-red-800 border border-red-200'
          }`}>
            {message}
          </div>
        )}

        {transfers.length === 0 ? (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
            <div className="text-gray-400 text-6xl mb-4">📋</div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">No Record Transfers</h3>
            <p className="text-gray-600 mb-6">
              You haven't requested any record transfers yet. Click "Request Transfer" to get started.
            </p>
          </div>
        ) : (
          <div className="space-y-4" role="list" aria-label="Record transfers list">
            {transfers.map((transfer) => (
              <div
                key={transfer.transferId}
                className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow"
                role="listitem"
              >
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <div className="flex items-center gap-3">
                      <h3 className="text-lg font-semibold text-gray-900">
                        {transfer.recipientOrganization || transfer.recipientName || 'Unknown Recipient'}
                      </h3>
                      <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(transfer.status)}`}>
                        {transfer.status}
                      </span>
                    </div>
                    {transfer.purpose && (
                      <p className="text-sm text-gray-600 mt-1">{transfer.purpose}</p>
                    )}
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-4 mt-3 text-sm">
                      {transfer.trackingNumber && (
                        <div>
                          <span className="text-gray-600">Tracking Number:</span>
                          <p className="font-medium font-mono">{transfer.trackingNumber}</p>
                        </div>
                      )}
                      <div>
                        <span className="text-gray-600">Requested:</span>
                        <p className="font-medium">{new Date(transfer.requestedDate).toLocaleDateString()}</p>
                      </div>
                      {transfer.completedDate && (
                        <div>
                          <span className="text-gray-600">Completed:</span>
                          <p className="font-medium">{new Date(transfer.completedDate).toLocaleDateString()}</p>
                        </div>
                      )}
                      {transfer.resourceCount !== null && transfer.resourceCount !== undefined && (
                        <div>
                          <span className="text-gray-600">Resources:</span>
                          <p className="font-medium">{transfer.resourceCount} items</p>
                        </div>
                      )}
                      {transfer.transferMethod && (
                        <div>
                          <span className="text-gray-600">Method:</span>
                          <p className="font-medium">{transfer.transferMethod}</p>
                        </div>
                      )}
                    </div>
                    {transfer.statusReason && (
                      <div className="mt-2 text-sm">
                        <span className="text-gray-600">Status Reason:</span>
                        <p className="text-gray-700">{transfer.statusReason}</p>
                      </div>
                    )}
                  </div>
                  <button
                    className="ml-4 text-primary-600 hover:text-primary-700 text-sm"
                    aria-label={`View details for transfer ${transfer.trackingNumber || transfer.transferId}`}
                  >
                    View Details →
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Transfer Information */}
      <div className="bg-blue-50 border border-blue-200 rounded-lg p-6">
        <h2 className="text-lg font-semibold text-blue-900 mb-2">About Record Transfers</h2>
        <ul className="list-disc list-inside text-blue-800 text-sm space-y-1">
          <li>Transfers use FHIR R4 standard for interoperability</li>
          <li>All transfers require active consent</li>
          <li>Records are securely encrypted during transfer</li>
          <li>You'll receive notifications at each stage of the transfer process</li>
          <li>Track your transfers in real-time using the tracking number</li>
        </ul>
      </div>

      {/* Request Transfer Modal */}
      {showRequestModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">Request Record Transfer</h2>
                <button
                  onClick={() => setShowRequestModal(false)}
                  className="text-gray-400 hover:text-gray-600"
                  aria-label="Close modal"
                >
                  <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>

              <div className="space-y-4">
                {/* Transfer Type */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Transfer Type *
                  </label>
                  <select
                    value={transferRequest.transferType}
                    onChange={(e) => setTransferRequest({ ...transferRequest, transferType: e.target.value })}
                    className="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary-500"
                  >
                    <option value="Outgoing">Outgoing (Send my records)</option>
                    <option value="Incoming">Incoming (Request records)</option>
                  </select>
                </div>

                {/* Transfer Method */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Transfer Method *
                  </label>
                  <select
                    value={transferRequest.transferMethod}
                    onChange={(e) => setTransferRequest({ ...transferRequest, transferMethod: e.target.value })}
                    className="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary-500"
                  >
                    <option value="FHIR">FHIR API</option>
                    <option value="DirectMail">Direct Secure Messaging</option>
                    <option value="Physical">Physical Mail</option>
                    <option value="Fax">Fax</option>
                  </select>
                </div>

                {/* Recipient Organization */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Recipient Organization
                  </label>
                  <input
                    type="text"
                    value={transferRequest.recipientOrganization}
                    onChange={(e) => setTransferRequest({ ...transferRequest, recipientOrganization: e.target.value })}
                    placeholder="e.g., Springfield General Hospital"
                    className="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                {/* Recipient Name */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Recipient Name
                  </label>
                  <input
                    type="text"
                    value={transferRequest.recipientName}
                    onChange={(e) => setTransferRequest({ ...transferRequest, recipientName: e.target.value })}
                    placeholder="e.g., Dr. John Smith"
                    className="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                {/* Recipient Email */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Recipient Email
                  </label>
                  <input
                    type="email"
                    value={transferRequest.recipientEmail}
                    onChange={(e) => setTransferRequest({ ...transferRequest, recipientEmail: e.target.value })}
                    placeholder="recipient@healthcare.org"
                    className="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                {/* Purpose */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Purpose of Transfer
                  </label>
                  <textarea
                    value={transferRequest.purpose}
                    onChange={(e) => setTransferRequest({ ...transferRequest, purpose: e.target.value })}
                    placeholder="e.g., Referral for specialist consultation"
                    rows={3}
                    className="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                {/* Include All Records */}
                <div className="flex items-center">
                  <input
                    type="checkbox"
                    id="includeAllRecords"
                    checked={transferRequest.includeAllRecords}
                    onChange={(e) => setTransferRequest({ ...transferRequest, includeAllRecords: e.target.checked })}
                    className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                  />
                  <label htmlFor="includeAllRecords" className="ml-2 text-sm text-gray-700">
                    Include all medical records
                  </label>
                </div>

                {/* Info Banner */}
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                  <p className="text-sm text-blue-800">
                    <strong>Note:</strong> Record transfers require active consent. If you don't have an active consent for this recipient, you'll need to create one first.
                  </p>
                </div>
              </div>

              {/* Buttons */}
              <div className="flex gap-3 mt-6">
                <button
                  onClick={() => setShowRequestModal(false)}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
                  disabled={isSubmitting}
                >
                  Cancel
                </button>
                <button
                  onClick={handleSubmitTransfer}
                  disabled={isSubmitting}
                  className="flex-1 px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {isSubmitting ? 'Submitting...' : 'Submit Request'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
