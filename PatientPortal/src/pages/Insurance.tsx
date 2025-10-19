import { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config/apiConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';

interface InsurancePolicy {
  insuranceId?: string;
  patientId?: string;
  payerName: string;
  planName: string;
  planType: string;
  memberId: string;
  groupNumber?: string;
  groupName?: string;
  effectiveDate: string;
  expirationDate?: string;
  isPrimary: boolean;
  priority: number;
  subscriberRelationship: string;
  subscriberName?: string;
  payerPhone?: string;
  payerWebsite?: string;
  deductible?: number;
  deductibleMet?: number;
  outOfPocketMax?: number;
  outOfPocketMet?: number;
  copay?: number;
  coinsurancePercentage?: number;
  status?: string;
  isVerified?: boolean;
  lastVerifiedDate?: string;
}

export default function Insurance() {
  const [insurances, setInsurances] = useState<InsurancePolicy[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  // isEditing and editingId removed as they're not used
  const [message, setMessage] = useState('');
  const [showAddForm, setShowAddForm] = useState(false);

  const emptyInsurance: InsurancePolicy = {
    payerName: '',
    planName: '',
    planType: 'Medical',
    memberId: '',
    groupNumber: '',
    effectiveDate: new Date().toISOString().split('T')[0],
    isPrimary: false,
    priority: 1,
    subscriberRelationship: 'Self',
  };

  const [newInsurance, setNewInsurance] = useState<InsurancePolicy>(emptyInsurance);

  // Load insurance policies
  useEffect(() => {
    loadInsurances();
  }, []);

  const loadInsurances = async () => {
    try {
      setIsLoading(true);
      const response = await axios.get(`${API_BASE_URL}/insurance/patient/${MOCK_PATIENT_ID}`);
      setInsurances(response.data || []);
    } catch (error) {
      console.error('Error loading insurance policies:', error);
      setMessage('✗ Error loading insurance policies');
    } finally {
      setIsLoading(false);
    }
  };

  const handleAdd = async () => {
    try {
      setIsSaving(true);
      setMessage('');

      // Set priority based on existing policies
      const maxPriority = insurances.length > 0 ? Math.max(...insurances.map(i => i.priority)) : 0;
      
      const payload = {
        patientId: MOCK_PATIENT_ID,
        ...newInsurance,
        priority: newInsurance.isPrimary ? 1 : maxPriority + 1,
        effectiveDate: new Date(newInsurance.effectiveDate).toISOString(),
        expirationDate: newInsurance.expirationDate ? new Date(newInsurance.expirationDate).toISOString() : null,
      };

      await axios.post(`${API_BASE_URL}/insurance`, payload);
      
      setMessage('✓ Insurance policy added successfully');
      setShowAddForm(false);
      setNewInsurance(emptyInsurance);
      await loadInsurances();
      
      setTimeout(() => setMessage(''), 3000);
    } catch (error: any) {
      console.error('Error adding insurance:', error);
      setMessage(`✗ Error adding insurance: ${error.response?.data?.message || error.message}`);
    } finally {
      setIsSaving(false);
    }
  };

  // handleUpdate function removed as it's not used

  const handleDelete = async (insuranceId: string) => {
    if (!confirm('Are you sure you want to delete this insurance policy?')) {
      return;
    }

    try {
      setIsSaving(true);
      setMessage('');

      await axios.delete(`${API_BASE_URL}/insurance/${insuranceId}`);
      
      setMessage('✓ Insurance policy deleted successfully');
      await loadInsurances();
      
      setTimeout(() => setMessage(''), 3000);
    } catch (error: any) {
      console.error('Error deleting insurance:', error);
      setMessage(`✗ Error deleting insurance: ${error.response?.data?.message || error.message}`);
    } finally {
      setIsSaving(false);
    }
  };

  const updateNewInsurance = (field: keyof InsurancePolicy, value: any) => {
    setNewInsurance(prev => ({ ...prev, [field]: value }));
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading insurance policies...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Insurance Management</h1>
          <p className="mt-2 text-gray-600">Manage your insurance policies and coverage information</p>
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

        {/* Add New Button */}
        <div className="mb-6 flex justify-end">
          <button
            onClick={() => setShowAddForm(!showAddForm)}
            className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            {showAddForm ? 'Cancel' : '+ Add Insurance'}
          </button>
        </div>

        {/* Add Insurance Form */}
        {showAddForm && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-6">Add New Insurance Policy</h2>
            
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Insurance Provider *
                </label>
                <input
                  type="text"
                  value={newInsurance.payerName}
                  onChange={(e) => updateNewInsurance('payerName', e.target.value)}
                  placeholder="e.g., Blue Cross Blue Shield"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Plan Name *
                </label>
                <input
                  type="text"
                  value={newInsurance.planName}
                  onChange={(e) => updateNewInsurance('planName', e.target.value)}
                  placeholder="e.g., PPO Gold"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Plan Type *
                </label>
                <select
                  value={newInsurance.planType}
                  onChange={(e) => updateNewInsurance('planType', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                >
                  <option value="Medical">Medical</option>
                  <option value="Dental">Dental</option>
                  <option value="Vision">Vision</option>
                  <option value="Medicare">Medicare</option>
                  <option value="Medicaid">Medicaid</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Member ID *
                </label>
                <input
                  type="text"
                  value={newInsurance.memberId}
                  onChange={(e) => updateNewInsurance('memberId', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Group Number
                </label>
                <input
                  type="text"
                  value={newInsurance.groupNumber || ''}
                  onChange={(e) => updateNewInsurance('groupNumber', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Effective Date *
                </label>
                <input
                  type="date"
                  value={newInsurance.effectiveDate}
                  onChange={(e) => updateNewInsurance('effectiveDate', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Expiration Date
                </label>
                <input
                  type="date"
                  value={newInsurance.expirationDate || ''}
                  onChange={(e) => updateNewInsurance('expirationDate', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Subscriber Relationship *
                </label>
                <select
                  value={newInsurance.subscriberRelationship}
                  onChange={(e) => updateNewInsurance('subscriberRelationship', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                >
                  <option value="Self">Self</option>
                  <option value="Spouse">Spouse</option>
                  <option value="Child">Child</option>
                  <option value="Parent">Parent</option>
                  <option value="Other">Other</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Phone Number
                </label>
                <input
                  type="tel"
                  value={newInsurance.payerPhone || ''}
                  onChange={(e) => updateNewInsurance('payerPhone', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div className="flex items-center mt-8">
                <input
                  type="checkbox"
                  id="isPrimary"
                  checked={newInsurance.isPrimary}
                  onChange={(e) => updateNewInsurance('isPrimary', e.target.checked)}
                  className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                />
                <label htmlFor="isPrimary" className="ml-2 text-sm text-gray-700">
                  Primary Insurance
                </label>
              </div>
            </div>

            <div className="mt-6 flex justify-end gap-4">
              <button
                onClick={() => {
                  setShowAddForm(false);
                  setNewInsurance(emptyInsurance);
                }}
                className="px-6 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleAdd}
                disabled={isSaving || !newInsurance.payerName || !newInsurance.planName || !newInsurance.memberId}
                className="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isSaving ? 'Adding...' : 'Add Insurance'}
              </button>
            </div>
          </div>
        )}

        {/* Insurance Policies List */}
        {insurances.length === 0 ? (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
            <div className="text-gray-400 text-6xl mb-4">💳</div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">No Insurance Policies</h3>
            <p className="text-gray-600 mb-6">
              You haven't added any insurance policies yet. Click "Add Insurance" to get started.
            </p>
          </div>
        ) : (
          <div className="space-y-6">
            {insurances.map((insurance) => (
              <div key={insurance.insuranceId} className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center gap-3">
                    <div className="text-3xl">💳</div>
                    <div>
                      <h3 className="text-xl font-semibold text-gray-900">
                        {insurance.payerName}
                        {insurance.isPrimary && (
                          <span className="ml-2 px-2 py-1 text-xs font-medium bg-blue-100 text-blue-800 rounded">
                            Primary
                          </span>
                        )}
                      </h3>
                      <p className="text-gray-600">{insurance.planName} - {insurance.planType}</p>
                    </div>
                  </div>
                  
                  <div className="flex items-center gap-2">
                    {insurance.isVerified && (
                      <span className="px-2 py-1 text-xs font-medium bg-green-100 text-green-800 rounded">
                        ✓ Verified
                      </span>
                    )}
                    <button
                      onClick={() => handleDelete(insurance.insuranceId!)}
                      disabled={isSaving}
                      className="text-red-600 hover:text-red-800 px-3 py-1 rounded hover:bg-red-50 transition-colors disabled:opacity-50"
                    >
                      Delete
                    </button>
                  </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 text-sm">
                  <div>
                    <span className="text-gray-500">Member ID:</span>
                    <p className="font-medium text-gray-900">{insurance.memberId}</p>
                  </div>
                  
                  {insurance.groupNumber && (
                    <div>
                      <span className="text-gray-500">Group Number:</span>
                      <p className="font-medium text-gray-900">{insurance.groupNumber}</p>
                    </div>
                  )}
                  
                  <div>
                    <span className="text-gray-500">Effective Date:</span>
                    <p className="font-medium text-gray-900">
                      {new Date(insurance.effectiveDate).toLocaleDateString()}
                    </p>
                  </div>
                  
                  {insurance.expirationDate && (
                    <div>
                      <span className="text-gray-500">Expiration Date:</span>
                      <p className="font-medium text-gray-900">
                        {new Date(insurance.expirationDate).toLocaleDateString()}
                      </p>
                    </div>
                  )}
                  
                  <div>
                    <span className="text-gray-500">Subscriber:</span>
                    <p className="font-medium text-gray-900">{insurance.subscriberRelationship}</p>
                  </div>
                  
                  {insurance.payerPhone && (
                    <div>
                      <span className="text-gray-500">Phone:</span>
                      <p className="font-medium text-gray-900">{insurance.payerPhone}</p>
                    </div>
                  )}
                  
                  {insurance.copay && (
                    <div>
                      <span className="text-gray-500">Copay:</span>
                      <p className="font-medium text-gray-900">${insurance.copay}</p>
                    </div>
                  )}
                  
                  {insurance.deductible && (
                    <div>
                      <span className="text-gray-500">Deductible:</span>
                      <p className="font-medium text-gray-900">
                        ${insurance.deductibleMet || 0} / ${insurance.deductible}
                      </p>
                    </div>
                  )}
                </div>

                {insurance.status && (
                  <div className="mt-4 pt-4 border-t border-gray-200">
                    <span className={`px-2 py-1 text-xs font-medium rounded ${
                      insurance.status === 'Active' ? 'bg-green-100 text-green-800' :
                      insurance.status === 'Cancelled' ? 'bg-red-100 text-red-800' :
                      'bg-gray-100 text-gray-800'
                    }`}>
                      Status: {insurance.status}
                    </span>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
