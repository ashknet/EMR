import { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config/apiConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';

interface Provider {
  patientProviderId?: string;
  providerTypeId: number;
  providerTypeName: string;
  providerName: string;
  specialty?: string;
  npi?: string;
  phone?: string;
  fax?: string;
  email?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  practiceName?: string;
  website?: string;
  notes?: string;
  isPrimary: boolean;
  isAcceptingPatients: boolean;
  status?: string;
  firstVisitDate?: string;
  lastVisitDate?: string;
}

interface ProviderType {
  providerTypeId: number;
  name: string;
  description?: string;
  displayOrder: number;
}

export default function Providers() {
  const [providers, setProviders] = useState<Provider[]>([]);
  const [providerTypes, setProviderTypes] = useState<ProviderType[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState('');
  const [showAddForm, setShowAddForm] = useState(false);

  const emptyProvider: Provider = {
    providerTypeId: 1,
    providerTypeName: 'Primary Care',
    providerName: '',
    phone: '',
    isPrimary: false,
    isAcceptingPatients: true,
    country: 'USA',
  };

  const [newProvider, setNewProvider] = useState<Provider>(emptyProvider);

  // Load providers and provider types
  useEffect(() => {
    loadProviders();
    loadProviderTypes();
  }, []);

  const loadProviders = async () => {
    try {
      setIsLoading(true);
      const response = await axios.get(`${API_BASE_URL}/providers/patient/${MOCK_PATIENT_ID}`);
      setProviders(response.data || []);
    } catch (error) {
      console.error('Error loading providers:', error);
      setMessage('✗ Error loading providers');
    } finally {
      setIsLoading(false);
    }
  };

  const loadProviderTypes = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/providers/types`);
      setProviderTypes(response.data || []);
    } catch (error) {
      console.error('Error loading provider types:', error);
    }
  };

  const handleAdd = async () => {
    try {
      setIsSaving(true);
      setMessage('');

      // Find provider type name
      const selectedType = providerTypes.find(t => t.providerTypeId === newProvider.providerTypeId);
      
      const payload = {
        patientId: MOCK_PATIENT_ID,
        ...newProvider,
        providerTypeName: selectedType?.name || newProvider.providerTypeName,
        firstVisitDate: newProvider.firstVisitDate ? new Date(newProvider.firstVisitDate).toISOString() : null,
      };

      await axios.post(`${API_BASE_URL}/providers`, payload);
      
      setMessage('✓ Provider added successfully');
      setShowAddForm(false);
      setNewProvider(emptyProvider);
      await loadProviders();
      
      setTimeout(() => setMessage(''), 3000);
    } catch (error: any) {
      console.error('Error adding provider:', error);
      setMessage(`✗ Error adding provider: ${error.response?.data?.message || error.message}`);
    } finally {
      setIsSaving(false);
    }
  };

  const handleDelete = async (providerId: string) => {
    if (!confirm('Are you sure you want to delete this provider?')) {
      return;
    }

    try {
      setIsSaving(true);
      setMessage('');

      await axios.delete(`${API_BASE_URL}/providers/${providerId}`);
      
      setMessage('✓ Provider deleted successfully');
      await loadProviders();
      
      setTimeout(() => setMessage(''), 3000);
    } catch (error: any) {
      console.error('Error deleting provider:', error);
      setMessage(`✗ Error deleting provider: ${error.response?.data?.message || error.message}`);
    } finally {
      setIsSaving(false);
    }
  };

  const updateNewProvider = (field: keyof Provider, value: any) => {
    setNewProvider(prev => {
      const updated = { ...prev, [field]: value };
      
      // If provider type changed, update the name
      if (field === 'providerTypeId') {
        const selectedType = providerTypes.find(t => t.providerTypeId === value);
        if (selectedType) {
          updated.providerTypeName = selectedType.name;
        }
      }
      
      return updated;
    });
  };

  const getProviderIcon = (typeName: string) => {
    const iconMap: { [key: string]: string } = {
      'Primary Care': '🩺',
      'Dental': '🦷',
      'Eye Care': '👁️',
      'Specialist': '🔬',
      'Orthopedic': '🦴',
      'Pediatrics': '👶',
      'OB/GYN': '🤰',
      'Mental Health': '🧠',
      'Physical Therapy': '💪',
      'Urgent Care': '🚑',
      'Hospital': '🏥',
      'Other': '⚕️',
    };
    return iconMap[typeName] || '⚕️';
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading providers...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Healthcare Providers</h1>
          <p className="mt-2 text-gray-600">Manage your healthcare team and provider information</p>
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
            {showAddForm ? 'Cancel' : '+ Add Provider'}
          </button>
        </div>

        {/* Add Provider Form */}
        {showAddForm && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-6">Add New Provider</h2>
            
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Provider Type *
                </label>
                <select
                  value={newProvider.providerTypeId}
                  onChange={(e) => updateNewProvider('providerTypeId', parseInt(e.target.value))}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  required
                >
                  {providerTypes.map((type) => (
                    <option key={type.providerTypeId} value={type.providerTypeId}>
                      {type.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Provider Name *
                </label>
                <input
                  type="text"
                  value={newProvider.providerName}
                  onChange={(e) => updateNewProvider('providerName', e.target.value)}
                  placeholder="Dr. John Smith"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Specialty
                </label>
                <input
                  type="text"
                  value={newProvider.specialty || ''}
                  onChange={(e) => updateNewProvider('specialty', e.target.value)}
                  placeholder="e.g., Cardiology"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Practice/Clinic Name
                </label>
                <input
                  type="text"
                  value={newProvider.practiceName || ''}
                  onChange={(e) => updateNewProvider('practiceName', e.target.value)}
                  placeholder="Medical Center Name"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Phone *
                </label>
                <input
                  type="tel"
                  value={newProvider.phone || ''}
                  onChange={(e) => updateNewProvider('phone', e.target.value)}
                  placeholder="(555) 123-4567"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Fax
                </label>
                <input
                  type="tel"
                  value={newProvider.fax || ''}
                  onChange={(e) => updateNewProvider('fax', e.target.value)}
                  placeholder="(555) 123-4568"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Email
                </label>
                <input
                  type="email"
                  value={newProvider.email || ''}
                  onChange={(e) => updateNewProvider('email', e.target.value)}
                  placeholder="office@example.com"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  NPI Number
                </label>
                <input
                  type="text"
                  value={newProvider.npi || ''}
                  onChange={(e) => updateNewProvider('npi', e.target.value)}
                  placeholder="1234567890"
                  maxLength={10}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Website
                </label>
                <input
                  type="url"
                  value={newProvider.website || ''}
                  onChange={(e) => updateNewProvider('website', e.target.value)}
                  placeholder="https://example.com"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Address Line 1
                </label>
                <input
                  type="text"
                  value={newProvider.addressLine1 || ''}
                  onChange={(e) => updateNewProvider('addressLine1', e.target.value)}
                  placeholder="123 Main Street"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Address Line 2
                </label>
                <input
                  type="text"
                  value={newProvider.addressLine2 || ''}
                  onChange={(e) => updateNewProvider('addressLine2', e.target.value)}
                  placeholder="Suite 100"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  City
                </label>
                <input
                  type="text"
                  value={newProvider.city || ''}
                  onChange={(e) => updateNewProvider('city', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  State
                </label>
                <input
                  type="text"
                  value={newProvider.state || ''}
                  onChange={(e) => updateNewProvider('state', e.target.value)}
                  placeholder="CA"
                  maxLength={2}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  ZIP Code
                </label>
                <input
                  type="text"
                  value={newProvider.zipCode || ''}
                  onChange={(e) => updateNewProvider('zipCode', e.target.value)}
                  placeholder="12345"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div className="md:col-span-3">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Notes
                </label>
                <textarea
                  value={newProvider.notes || ''}
                  onChange={(e) => updateNewProvider('notes', e.target.value)}
                  rows={3}
                  placeholder="Additional notes or special instructions"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div className="flex items-center">
                <input
                  type="checkbox"
                  id="isPrimary"
                  checked={newProvider.isPrimary}
                  onChange={(e) => updateNewProvider('isPrimary', e.target.checked)}
                  className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                />
                <label htmlFor="isPrimary" className="ml-2 text-sm text-gray-700">
                  Primary Provider of this type
                </label>
              </div>

              <div className="flex items-center">
                <input
                  type="checkbox"
                  id="isAcceptingPatients"
                  checked={newProvider.isAcceptingPatients}
                  onChange={(e) => updateNewProvider('isAcceptingPatients', e.target.checked)}
                  className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                />
                <label htmlFor="isAcceptingPatients" className="ml-2 text-sm text-gray-700">
                  Currently accepting patients
                </label>
              </div>
            </div>

            <div className="mt-6 flex justify-end gap-4">
              <button
                onClick={() => {
                  setShowAddForm(false);
                  setNewProvider(emptyProvider);
                }}
                className="px-6 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleAdd}
                disabled={isSaving || !newProvider.providerName || !newProvider.phone}
                className="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isSaving ? 'Adding...' : 'Add Provider'}
              </button>
            </div>
          </div>
        )}

        {/* Providers List */}
        {providers.length === 0 ? (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
            <div className="text-gray-400 text-6xl mb-4">⚕️</div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">No Healthcare Providers</h3>
            <p className="text-gray-600 mb-6">
              You haven't added any healthcare providers yet. Click "Add Provider" to get started.
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {providers.map((provider) => (
              <div key={provider.patientProviderId} className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center gap-3">
                    <div className="text-4xl">{getProviderIcon(provider.providerTypeName)}</div>
                    <div>
                      <h3 className="text-lg font-semibold text-gray-900">
                        {provider.providerName}
                        {provider.isPrimary && (
                          <span className="ml-2 px-2 py-1 text-xs font-medium bg-blue-100 text-blue-800 rounded">
                            Primary
                          </span>
                        )}
                      </h3>
                      <p className="text-sm text-gray-600">{provider.providerTypeName}</p>
                      {provider.specialty && (
                        <p className="text-sm text-gray-500">{provider.specialty}</p>
                      )}
                    </div>
                  </div>
                </div>

                {provider.practiceName && (
                  <div className="mb-3">
                    <p className="text-sm font-medium text-gray-900">{provider.practiceName}</p>
                  </div>
                )}

                <div className="space-y-2 text-sm">
                  {provider.phone && (
                    <div className="flex items-center gap-2">
                      <span className="text-gray-500">📞</span>
                      <a href={`tel:${provider.phone}`} className="text-blue-600 hover:underline">
                        {provider.phone}
                      </a>
                    </div>
                  )}
                  
                  {provider.email && (
                    <div className="flex items-center gap-2">
                      <span className="text-gray-500">✉️</span>
                      <a href={`mailto:${provider.email}`} className="text-blue-600 hover:underline">
                        {provider.email}
                      </a>
                    </div>
                  )}
                  
                  {provider.addressLine1 && (
                    <div className="flex items-start gap-2">
                      <span className="text-gray-500">📍</span>
                      <div className="text-gray-700">
                        <p>{provider.addressLine1}</p>
                        {provider.addressLine2 && <p>{provider.addressLine2}</p>}
                        {provider.city && provider.state && (
                          <p>{provider.city}, {provider.state} {provider.zipCode}</p>
                        )}
                      </div>
                    </div>
                  )}

                  {provider.website && (
                    <div className="flex items-center gap-2">
                      <span className="text-gray-500">🌐</span>
                      <a href={provider.website} target="_blank" rel="noopener noreferrer" className="text-blue-600 hover:underline">
                        Visit Website
                      </a>
                    </div>
                  )}
                </div>

                {provider.notes && (
                  <div className="mt-3 pt-3 border-t border-gray-200">
                    <p className="text-sm text-gray-600">{provider.notes}</p>
                  </div>
                )}

                <div className="mt-4 pt-4 border-t border-gray-200 flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    {provider.isAcceptingPatients && (
                      <span className="px-2 py-1 text-xs font-medium bg-green-100 text-green-800 rounded">
                        Accepting Patients
                      </span>
                    )}
                    <span className={`px-2 py-1 text-xs font-medium rounded ${
                      provider.status === 'Active' ? 'bg-green-100 text-green-800' :
                      'bg-gray-100 text-gray-800'
                    }`}>
                      {provider.status}
                    </span>
                  </div>
                  <button
                    onClick={() => handleDelete(provider.patientProviderId!)}
                    disabled={isSaving}
                    className="text-red-600 hover:text-red-800 text-sm px-3 py-1 rounded hover:bg-red-50 transition-colors disabled:opacity-50"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
