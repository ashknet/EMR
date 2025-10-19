import { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config/apiConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';

interface PersonalInfo {
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
  dateOfBirth: string;
  gender: string;
  genderId?: number;
  maritalStatus?: string;
  maritalStatusId?: number;
  race?: string;
  raceId?: number;
  ethnicity?: string;
  primaryLanguageId?: number;
  preferredLanguageId?: number;
  ssnEncrypted?: string;
}

interface ContactInfo {
  email: string;
  phoneNumber: string;
  alternatePhone?: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

interface EmergencyContact {
  emergencyContactId?: string;
  name: string;
  relationshipTypeId: number;
  relationshipType?: string;
  phone: string;
  altPhone?: string;
}

interface Accessibility {
  interpreterRequired: boolean;
  mobilityAssistance: boolean;
  communicationPrefId?: number;
  religiousConsiderations?: string;
  primaryCarePhysician?: string;
  pcpPhoneNumber?: string;
  referringPhysician?: string;
  preferredPharmacyName?: string;
  preferredPharmacyLocation?: string;
}

export default function ProfileRedesigned() {
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [activeSection, setActiveSection] = useState('personal');
  const [message, setMessage] = useState('');

  const [personalInfo, setPersonalInfo] = useState<PersonalInfo>({
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    gender: 'Male',
  });

  const [contactInfo, setContactInfo] = useState<ContactInfo>({
    email: '',
    phoneNumber: '',
    addressLine1: '',
    city: '',
    state: '',
    zipCode: '',
    country: 'USA',
  });

  const [emergencyContacts, setEmergencyContacts] = useState<EmergencyContact[]>([]);

  const [accessibility, setAccessibility] = useState<Accessibility>({
    interpreterRequired: false,
    mobilityAssistance: false,
  });

  // Load patient data
  useEffect(() => {
    const loadPatientData = async () => {
      try {
        setIsLoading(true);
        
        // Load basic patient info
        const patientResponse = await axios.get(`${API_BASE_URL}/api/patients/${MOCK_PATIENT_ID}`);
        const patientData = patientResponse.data;

        setPersonalInfo({
          firstName: patientData.firstName || '',
          middleName: patientData.middleName || '',
          lastName: patientData.lastName || '',
          suffix: patientData.suffix || '',
          dateOfBirth: patientData.dateOfBirth ? patientData.dateOfBirth.split('T')[0] : '',
          gender: patientData.gender || 'Male',
          genderId: patientData.genderId,
          maritalStatus: patientData.maritalStatus,
          maritalStatusId: patientData.maritalStatusId,
          race: patientData.race,
          raceId: patientData.raceId,
          ethnicity: patientData.ethnicity,
          primaryLanguageId: patientData.primaryLanguageId,
          preferredLanguageId: patientData.preferredLanguageId,
          ssnEncrypted: patientData.ssnEncrypted,
        });

        setContactInfo({
          email: patientData.email || '',
          phoneNumber: patientData.phoneNumber || '',
          alternatePhone: patientData.alternatePhone || '',
          addressLine1: patientData.addressLine1 || '',
          addressLine2: patientData.addressLine2 || '',
          city: patientData.city || '',
          state: patientData.state || '',
          zipCode: patientData.zipCode || '',
          country: patientData.country || 'USA',
        });

        setAccessibility({
          interpreterRequired: patientData.interpreterRequired || false,
          mobilityAssistance: patientData.mobilityAssistance || false,
          communicationPrefId: patientData.communicationPrefId,
          religiousConsiderations: patientData.religiousConsiderations,
          primaryCarePhysician: patientData.primaryCarePhysician,
          pcpPhoneNumber: patientData.pcpPhoneNumber,
          referringPhysician: patientData.referringPhysician,
          preferredPharmacyName: patientData.preferredPharmacyName,
          preferredPharmacyLocation: patientData.preferredPharmacyLocation,
        });

        // Load emergency contacts
        // Note: For now using the single emergency contact from patient record
        // TODO: Load from EmergencyContacts table when available
        if (patientData.emergencyContactName) {
          setEmergencyContacts([{
            name: patientData.emergencyContactName,
            relationshipTypeId: 1, // Default to Spouse
            relationshipType: patientData.emergencyContactRelationship || 'Spouse',
            phone: patientData.emergencyContactPhone || '',
          }]);
        }

      } catch (error) {
        console.error('Error loading patient data:', error);
        setMessage('Error loading profile data');
      } finally {
        setIsLoading(false);
      }
    };

    loadPatientData();
  }, []);

  const handleSave = async () => {
    try {
      setIsSaving(true);
      setMessage('');

      // Save personal and contact information
      await axios.put(`${API_BASE_URL}/api/patients/${MOCK_PATIENT_ID}`, {
        ...personalInfo,
        ...contactInfo,
        ...accessibility,
      });

      // TODO: Save emergency contacts to EmergencyContacts table

      setMessage('✓ Profile saved successfully');
      setIsEditing(false);
      
      setTimeout(() => setMessage(''), 3000);
    } catch (error) {
      console.error('Error saving profile:', error);
      setMessage('✗ Error saving profile');
    } finally {
      setIsSaving(false);
    }
  };

  const addEmergencyContact = () => {
    setEmergencyContacts([...emergencyContacts, {
      name: '',
      relationshipTypeId: 1,
      phone: '',
    }]);
  };

  const removeEmergencyContact = (index: number) => {
    setEmergencyContacts(emergencyContacts.filter((_, i) => i !== index));
  };

  const updateEmergencyContact = (index: number, field: keyof EmergencyContact, value: any) => {
    const updated = [...emergencyContacts];
    updated[index] = { ...updated[index], [field]: value };
    setEmergencyContacts(updated);
  };

  const sections = [
    { id: 'personal', label: 'Personal Information', icon: '👤' },
    { id: 'contact', label: 'Contact Information', icon: '📧' },
    { id: 'emergency', label: 'Emergency Contacts', icon: '🚨' },
    { id: 'accessibility', label: 'Accessibility', icon: '♿' },
  ];

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading profile...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">My Profile</h1>
          <p className="mt-2 text-gray-600">Manage your personal information and preferences</p>
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

        {/* Edit/Save Button */}
        <div className="mb-6 flex justify-end gap-4">
          {!isEditing ? (
            <button
              onClick={() => setIsEditing(true)}
              className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              Edit Profile
            </button>
          ) : (
            <>
              <button
                onClick={() => {
                  setIsEditing(false);
                  setMessage('');
                }}
                className="px-6 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleSave}
                disabled={isSaving}
                className="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50"
              >
                {isSaving ? 'Saving...' : 'Save Changes'}
              </button>
            </>
          )}
        </div>

        {/* Section Tabs */}
        <div className="mb-6 border-b border-gray-200">
          <nav className="flex space-x-8">
            {sections.map((section) => (
              <button
                key={section.id}
                onClick={() => setActiveSection(section.id)}
                className={`pb-4 px-1 border-b-2 font-medium text-sm transition-colors ${
                  activeSection === section.id
                    ? 'border-blue-600 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                <span className="mr-2">{section.icon}</span>
                {section.label}
              </button>
            ))}
          </nav>
        </div>

        {/* Personal Information Section */}
        {activeSection === 'personal' && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-6">Personal Information</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  First Name *
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={personalInfo.firstName}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, firstName: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{personalInfo.firstName}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Middle Name
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={personalInfo.middleName || ''}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, middleName: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{personalInfo.middleName || '-'}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Last Name *
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={personalInfo.lastName}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, lastName: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{personalInfo.lastName}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Suffix
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={personalInfo.suffix || ''}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, suffix: e.target.value })}
                    placeholder="Jr., Sr., III, etc."
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{personalInfo.suffix || '-'}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Date of Birth *
                </label>
                {isEditing ? (
                  <input
                    type="date"
                    value={personalInfo.dateOfBirth}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, dateOfBirth: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">
                    {personalInfo.dateOfBirth ? new Date(personalInfo.dateOfBirth).toLocaleDateString() : '-'}
                  </p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Gender *
                </label>
                {isEditing ? (
                  <select
                    value={personalInfo.gender}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, gender: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  >
                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                    <option value="Other">Other</option>
                    <option value="Prefer not to say">Prefer not to say</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{personalInfo.gender}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Marital Status
                </label>
                {isEditing ? (
                  <select
                    value={personalInfo.maritalStatus || ''}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, maritalStatus: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  >
                    <option value="">Select...</option>
                    <option value="Single">Single</option>
                    <option value="Married">Married</option>
                    <option value="Divorced">Divorced</option>
                    <option value="Widowed">Widowed</option>
                    <option value="Separated">Separated</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{personalInfo.maritalStatus || '-'}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Race
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={personalInfo.race || ''}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, race: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{personalInfo.race || '-'}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Ethnicity
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={personalInfo.ethnicity || ''}
                    onChange={(e) => setPersonalInfo({ ...personalInfo, ethnicity: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{personalInfo.ethnicity || '-'}</p>
                )}
              </div>
            </div>
          </div>
        )}

        {/* Contact Information Section */}
        {activeSection === 'contact' && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-6">Contact Information</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Email *
                </label>
                {isEditing ? (
                  <input
                    type="email"
                    value={contactInfo.email}
                    onChange={(e) => setContactInfo({ ...contactInfo, email: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.email}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Phone Number *
                </label>
                {isEditing ? (
                  <input
                    type="tel"
                    value={contactInfo.phoneNumber}
                    onChange={(e) => setContactInfo({ ...contactInfo, phoneNumber: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.phoneNumber}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Alternate Phone
                </label>
                {isEditing ? (
                  <input
                    type="tel"
                    value={contactInfo.alternatePhone || ''}
                    onChange={(e) => setContactInfo({ ...contactInfo, alternatePhone: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.alternatePhone || '-'}</p>
                )}
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Address Line 1 *
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={contactInfo.addressLine1}
                    onChange={(e) => setContactInfo({ ...contactInfo, addressLine1: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.addressLine1}</p>
                )}
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Address Line 2
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={contactInfo.addressLine2 || ''}
                    onChange={(e) => setContactInfo({ ...contactInfo, addressLine2: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.addressLine2 || '-'}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  City *
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={contactInfo.city}
                    onChange={(e) => setContactInfo({ ...contactInfo, city: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.city}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  State *
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={contactInfo.state}
                    onChange={(e) => setContactInfo({ ...contactInfo, state: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.state}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  ZIP Code *
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={contactInfo.zipCode}
                    onChange={(e) => setContactInfo({ ...contactInfo, zipCode: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.zipCode}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Country *
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    value={contactInfo.country}
                    onChange={(e) => setContactInfo({ ...contactInfo, country: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                ) : (
                  <p className="text-gray-900">{contactInfo.country}</p>
                )}
              </div>
            </div>
          </div>
        )}

        {/* Emergency Contacts Section */}
        {activeSection === 'emergency' && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-xl font-semibold text-gray-900">Emergency Contacts</h2>
              {isEditing && (
                <button
                  onClick={addEmergencyContact}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm"
                >
                  + Add Contact
                </button>
              )}
            </div>

            {emergencyContacts.length === 0 ? (
              <p className="text-gray-500 text-center py-8">
                No emergency contacts added yet. {isEditing && 'Click "Add Contact" to add one.'}
              </p>
            ) : (
              <div className="space-y-6">
                {emergencyContacts.map((contact, index) => (
                  <div key={index} className="border border-gray-200 rounded-lg p-4">
                    <div className="flex items-center justify-between mb-4">
                      <h3 className="font-medium text-gray-900">Contact {index + 1}</h3>
                      {isEditing && emergencyContacts.length > 1 && (
                        <button
                          onClick={() => removeEmergencyContact(index)}
                          className="text-red-600 hover:text-red-800 text-sm"
                        >
                          Remove
                        </button>
                      )}
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          Name *
                        </label>
                        {isEditing ? (
                          <input
                            type="text"
                            value={contact.name}
                            onChange={(e) => updateEmergencyContact(index, 'name', e.target.value)}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                          />
                        ) : (
                          <p className="text-gray-900">{contact.name}</p>
                        )}
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          Relationship *
                        </label>
                        {isEditing ? (
                          <select
                            value={contact.relationshipTypeId}
                            onChange={(e) => updateEmergencyContact(index, 'relationshipTypeId', parseInt(e.target.value))}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                          >
                            <option value={1}>Spouse</option>
                            <option value={2}>Parent</option>
                            <option value={3}>Child</option>
                            <option value={4}>Sibling</option>
                            <option value={5}>Friend</option>
                            <option value={6}>Other</option>
                          </select>
                        ) : (
                          <p className="text-gray-900">{contact.relationshipType || 'Spouse'}</p>
                        )}
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          Phone *
                        </label>
                        {isEditing ? (
                          <input
                            type="tel"
                            value={contact.phone}
                            onChange={(e) => updateEmergencyContact(index, 'phone', e.target.value)}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                          />
                        ) : (
                          <p className="text-gray-900">{contact.phone}</p>
                        )}
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          Alternate Phone
                        </label>
                        {isEditing ? (
                          <input
                            type="tel"
                            value={contact.altPhone || ''}
                            onChange={(e) => updateEmergencyContact(index, 'altPhone', e.target.value)}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                          />
                        ) : (
                          <p className="text-gray-900">{contact.altPhone || '-'}</p>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {/* Accessibility Section */}
        {activeSection === 'accessibility' && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-6">Accessibility & Preferences</h2>
            <div className="space-y-6">
              {/* Checkboxes */}
              <div className="flex items-center space-x-6">
                <label className="flex items-center">
                  {isEditing ? (
                    <input
                      type="checkbox"
                      checked={accessibility.interpreterRequired}
                      onChange={(e) => setAccessibility({ ...accessibility, interpreterRequired: e.target.checked })}
                      className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                    />
                  ) : (
                    <span className={`w-4 h-4 inline-block rounded border ${
                      accessibility.interpreterRequired ? 'bg-blue-600 border-blue-600' : 'border-gray-300'
                    }`}></span>
                  )}
                  <span className="ml-2 text-sm text-gray-700">Interpreter Required</span>
                </label>

                <label className="flex items-center">
                  {isEditing ? (
                    <input
                      type="checkbox"
                      checked={accessibility.mobilityAssistance}
                      onChange={(e) => setAccessibility({ ...accessibility, mobilityAssistance: e.target.checked })}
                      className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                    />
                  ) : (
                    <span className={`w-4 h-4 inline-block rounded border ${
                      accessibility.mobilityAssistance ? 'bg-blue-600 border-blue-600' : 'border-gray-300'
                    }`}></span>
                  )}
                  <span className="ml-2 text-sm text-gray-700">Mobility Assistance</span>
                </label>
              </div>

              {/* Text fields */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Religious Considerations
                  </label>
                  {isEditing ? (
                    <textarea
                      value={accessibility.religiousConsiderations || ''}
                      onChange={(e) => setAccessibility({ ...accessibility, religiousConsiderations: e.target.value })}
                      rows={3}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                  ) : (
                    <p className="text-gray-900">{accessibility.religiousConsiderations || '-'}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Primary Care Physician
                  </label>
                  {isEditing ? (
                    <input
                      type="text"
                      value={accessibility.primaryCarePhysician || ''}
                      onChange={(e) => setAccessibility({ ...accessibility, primaryCarePhysician: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                  ) : (
                    <p className="text-gray-900">{accessibility.primaryCarePhysician || '-'}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    PCP Phone Number
                  </label>
                  {isEditing ? (
                    <input
                      type="tel"
                      value={accessibility.pcpPhoneNumber || ''}
                      onChange={(e) => setAccessibility({ ...accessibility, pcpPhoneNumber: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                  ) : (
                    <p className="text-gray-900">{accessibility.pcpPhoneNumber || '-'}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Referring Physician
                  </label>
                  {isEditing ? (
                    <input
                      type="text"
                      value={accessibility.referringPhysician || ''}
                      onChange={(e) => setAccessibility({ ...accessibility, referringPhysician: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                  ) : (
                    <p className="text-gray-900">{accessibility.referringPhysician || '-'}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Preferred Pharmacy Name
                  </label>
                  {isEditing ? (
                    <input
                      type="text"
                      value={accessibility.preferredPharmacyName || ''}
                      onChange={(e) => setAccessibility({ ...accessibility, preferredPharmacyName: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                  ) : (
                    <p className="text-gray-900">{accessibility.preferredPharmacyName || '-'}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Preferred Pharmacy Location
                  </label>
                  {isEditing ? (
                    <input
                      type="text"
                      value={accessibility.preferredPharmacyLocation || ''}
                      onChange={(e) => setAccessibility({ ...accessibility, preferredPharmacyLocation: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                  ) : (
                    <p className="text-gray-900">{accessibility.preferredPharmacyLocation || '-'}</p>
                  )}
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

