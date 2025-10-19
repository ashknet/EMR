import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { API_BASE_URL } from '../config/apiConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';
import { US_STATES } from '../constants/states';

interface FamilyMember {
  relationId?: string;
  relationType: string;
  firstName: string;
  lastName: string;
  middleName?: string;
  dateOfBirth: string;
  gender?: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  isGuardian: boolean;
  isProxy: boolean;
  canViewRecords: boolean;
  canManageRecords: boolean;
  canGrantConsent: boolean;
  legalDocumentType?: string;
  legalDocumentNumber?: string;
  legalDocumentExpiryDate?: string;
}

export default function Family() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [showAddForm, setShowAddForm] = useState(false);
  const [editingMember, setEditingMember] = useState<FamilyMember | null>(null);
  const [formData, setFormData] = useState<FamilyMember>({
    relationType: 'Spouse',
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    email: '',
    isGuardian: false,
    isProxy: false,
    canViewRecords: false,
    canManageRecords: false,
    canGrantConsent: false,
  });

  // Fetch family members
  const { data: familyMembers = [], isLoading } = useQuery({
    queryKey: ['familyMembers', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/family/patient/${MOCK_PATIENT_ID}`);
      return response.data;
    },
  });

  // Add family member mutation
  const addFamilyMemberMutation = useMutation({
    mutationFn: async (data: FamilyMember) => {
      const response = await axios.post(`${API_BASE_URL}/family`, {
        ...data,
        patientId: MOCK_PATIENT_ID,
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['familyMembers', MOCK_PATIENT_ID] });
      resetForm();
      setShowAddForm(false);
    },
  });

  // Update family member mutation
  const updateFamilyMemberMutation = useMutation({
    mutationFn: async (data: FamilyMember) => {
      const response = await axios.put(`${API_BASE_URL}/family/${data.relationId}`, data);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['familyMembers', MOCK_PATIENT_ID] });
      resetForm();
      setEditingMember(null);
    },
  });

  // Delete family member mutation
  const deleteFamilyMemberMutation = useMutation({
    mutationFn: async (relationId: string) => {
      await axios.delete(`${API_BASE_URL}/family/${relationId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['familyMembers', MOCK_PATIENT_ID] });
    },
  });

  const resetForm = () => {
    setFormData({
      relationType: 'Spouse',
      firstName: '',
      lastName: '',
      dateOfBirth: '',
      email: '',
      isGuardian: false,
      isProxy: false,
      canViewRecords: false,
      canManageRecords: false,
      canGrantConsent: false,
    });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingMember) {
      updateFamilyMemberMutation.mutate({ ...formData, relationId: editingMember.relationId });
    } else {
      addFamilyMemberMutation.mutate(formData);
    }
  };

  const handleEdit = (member: any) => {
    setEditingMember(member);
    setFormData({
      relationId: member.relationId,
      relationType: member.relationType,
      firstName: member.firstName,
      lastName: member.lastName,
      middleName: member.middleName || '',
      dateOfBirth: member.dateOfBirth?.split('T')[0] || '',
      gender: member.gender || '',
      email: member.email,
      phone: member.phone || '',
      address: member.address || '',
      city: member.city || '',
      state: member.state || '',
      zipCode: member.zipCode || '',
      isGuardian: member.isGuardian,
      isProxy: member.isProxy,
      canViewRecords: member.canViewRecords,
      canManageRecords: member.canManageRecords,
      canGrantConsent: member.canGrantConsent,
      legalDocumentType: member.legalDocumentType || '',
      legalDocumentNumber: member.legalDocumentNumber || '',
      legalDocumentExpiryDate: member.legalDocumentExpiryDate?.split('T')[0] || '',
    });
    setShowAddForm(true);
  };

  const handleDelete = (relationId: string) => {
    if (window.confirm('Are you sure you want to remove this family member?')) {
      deleteFamilyMemberMutation.mutate(relationId);
    }
  };

  const relationTypes = ['Spouse', 'Child', 'Parent', 'Sibling', 'Guardian', 'Proxy', 'Other'];
  const genders = ['Male', 'Female', 'Other', 'Prefer not to say'];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-white rounded-xl shadow-sm border border-neutral-200 p-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 bg-gradient-to-br from-primary-500 to-accent-500 rounded-xl flex items-center justify-center">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <div>
              <h1 className="text-2xl font-bold text-neutral-900">Family Members</h1>
              <p className="text-sm text-neutral-600 mt-0.5">
                Manage your family members and their access permissions
              </p>
            </div>
          </div>
          <button
            onClick={() => navigate('/profile')}
            className="px-4 py-2 text-sm font-semibold text-primary-600 hover:bg-primary-50 rounded-lg transition-colors"
          >
            Back to Profile
          </button>
        </div>
      </div>

      {/* Add Family Member Button */}
      {!showAddForm && !editingMember && (
        <div className="flex justify-end">
          <button
            onClick={() => setShowAddForm(true)}
            className="px-6 py-3 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 transition-colors flex items-center gap-2"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
            </svg>
            Add Family Member
          </button>
        </div>
      )}

      {/* Add/Edit Form */}
      {(showAddForm || editingMember) && (
        <div className="bg-white rounded-xl shadow-sm border border-neutral-200 p-6">
          <h2 className="text-xl font-bold text-neutral-900 mb-6">
            {editingMember ? 'Edit Family Member' : 'Add Family Member'}
          </h2>

          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Basic Information */}
            <div className="border-b border-neutral-200 pb-6">
              <h3 className="text-lg font-semibold text-neutral-900 mb-4">Basic Information</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Relationship *
                  </label>
                  <select
                    value={formData.relationType}
                    onChange={(e) => setFormData({ ...formData, relationType: e.target.value })}
                    required
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  >
                    {relationTypes.map((type) => (
                      <option key={type} value={type}>{type}</option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    First Name *
                  </label>
                  <input
                    type="text"
                    value={formData.firstName}
                    onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                    required
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Middle Name
                  </label>
                  <input
                    type="text"
                    value={formData.middleName || ''}
                    onChange={(e) => setFormData({ ...formData, middleName: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Last Name *
                  </label>
                  <input
                    type="text"
                    value={formData.lastName}
                    onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                    required
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Date of Birth *
                  </label>
                  <input
                    type="date"
                    value={formData.dateOfBirth}
                    onChange={(e) => setFormData({ ...formData, dateOfBirth: e.target.value })}
                    required
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Gender
                  </label>
                  <select
                    value={formData.gender || ''}
                    onChange={(e) => setFormData({ ...formData, gender: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  >
                    <option value="">Select...</option>
                    {genders.map((gender) => (
                      <option key={gender} value={gender}>{gender}</option>
                    ))}
                  </select>
                </div>
              </div>
            </div>

            {/* Contact Information */}
            <div className="border-b border-neutral-200 pb-6">
              <h3 className="text-lg font-semibold text-neutral-900 mb-4">Contact Information</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Email *
                  </label>
                  <input
                    type="email"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    required
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Phone
                  </label>
                  <input
                    type="tel"
                    value={formData.phone || ''}
                    onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div className="md:col-span-2">
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Address
                  </label>
                  <input
                    type="text"
                    value={formData.address || ''}
                    onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    City
                  </label>
                  <input
                    type="text"
                    value={formData.city || ''}
                    onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    State
                  </label>
                  <select
                    value={formData.state || ''}
                    onChange={(e) => setFormData({ ...formData, state: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  >
                    <option value="">Select State...</option>
                    {US_STATES.map((state) => (
                      <option key={state.code} value={state.code}>
                        {state.code} - {state.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    ZIP Code
                  </label>
                  <input
                    type="text"
                    value={formData.zipCode || ''}
                    onChange={(e) => setFormData({ ...formData, zipCode: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>
              </div>
            </div>

            {/* Permissions */}
            <div className="border-b border-neutral-200 pb-6">
              <h3 className="text-lg font-semibold text-neutral-900 mb-4">Permissions & Access</h3>
              <div className="space-y-3">
                <label className="flex items-center gap-3 p-3 bg-neutral-50 rounded-lg cursor-pointer hover:bg-neutral-100 transition-colors">
                  <input
                    type="checkbox"
                    checked={formData.isGuardian}
                    onChange={(e) => setFormData({ ...formData, isGuardian: e.target.checked })}
                    className="w-5 h-5 text-primary-600 rounded focus:ring-2 focus:ring-primary-500"
                  />
                  <div>
                    <span className="font-semibold text-neutral-900">Guardian</span>
                    <p className="text-sm text-neutral-600">Legal guardian with full decision-making authority</p>
                  </div>
                </label>

                <label className="flex items-center gap-3 p-3 bg-neutral-50 rounded-lg cursor-pointer hover:bg-neutral-100 transition-colors">
                  <input
                    type="checkbox"
                    checked={formData.isProxy}
                    onChange={(e) => setFormData({ ...formData, isProxy: e.target.checked })}
                    className="w-5 h-5 text-primary-600 rounded focus:ring-2 focus:ring-primary-500"
                  />
                  <div>
                    <span className="font-semibold text-neutral-900">Healthcare Proxy</span>
                    <p className="text-sm text-neutral-600">Can make healthcare decisions on your behalf</p>
                  </div>
                </label>

                <label className="flex items-center gap-3 p-3 bg-neutral-50 rounded-lg cursor-pointer hover:bg-neutral-100 transition-colors">
                  <input
                    type="checkbox"
                    checked={formData.canViewRecords}
                    onChange={(e) => setFormData({ ...formData, canViewRecords: e.target.checked })}
                    className="w-5 h-5 text-primary-600 rounded focus:ring-2 focus:ring-primary-500"
                  />
                  <div>
                    <span className="font-semibold text-neutral-900">View Records</span>
                    <p className="text-sm text-neutral-600">Can view your medical records</p>
                  </div>
                </label>

                <label className="flex items-center gap-3 p-3 bg-neutral-50 rounded-lg cursor-pointer hover:bg-neutral-100 transition-colors">
                  <input
                    type="checkbox"
                    checked={formData.canManageRecords}
                    onChange={(e) => setFormData({ ...formData, canManageRecords: e.target.checked })}
                    className="w-5 h-5 text-primary-600 rounded focus:ring-2 focus:ring-primary-500"
                  />
                  <div>
                    <span className="font-semibold text-neutral-900">Manage Records</span>
                    <p className="text-sm text-neutral-600">Can update and manage your medical records</p>
                  </div>
                </label>

                <label className="flex items-center gap-3 p-3 bg-neutral-50 rounded-lg cursor-pointer hover:bg-neutral-100 transition-colors">
                  <input
                    type="checkbox"
                    checked={formData.canGrantConsent}
                    onChange={(e) => setFormData({ ...formData, canGrantConsent: e.target.checked })}
                    className="w-5 h-5 text-primary-600 rounded focus:ring-2 focus:ring-primary-500"
                  />
                  <div>
                    <span className="font-semibold text-neutral-900">Grant Consent</span>
                    <p className="text-sm text-neutral-600">Can provide consent for medical procedures</p>
                  </div>
                </label>
              </div>
            </div>

            {/* Legal Documentation */}
            <div>
              <h3 className="text-lg font-semibold text-neutral-900 mb-4">Legal Documentation (Optional)</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Document Type
                  </label>
                  <input
                    type="text"
                    value={formData.legalDocumentType || ''}
                    onChange={(e) => setFormData({ ...formData, legalDocumentType: e.target.value })}
                    placeholder="e.g., Power of Attorney"
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Document Number
                  </label>
                  <input
                    type="text"
                    value={formData.legalDocumentNumber || ''}
                    onChange={(e) => setFormData({ ...formData, legalDocumentNumber: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-neutral-700 mb-1">
                    Expiry Date
                  </label>
                  <input
                    type="date"
                    value={formData.legalDocumentExpiryDate || ''}
                    onChange={(e) => setFormData({ ...formData, legalDocumentExpiryDate: e.target.value })}
                    className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                </div>
              </div>
            </div>

            {/* Form Actions */}
            <div className="flex gap-3 pt-4">
              <button
                type="submit"
                disabled={addFamilyMemberMutation.isPending || updateFamilyMemberMutation.isPending}
                className="px-6 py-3 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                {addFamilyMemberMutation.isPending || updateFamilyMemberMutation.isPending
                  ? 'Saving...'
                  : editingMember
                  ? 'Update Family Member'
                  : 'Add Family Member'}
              </button>
              <button
                type="button"
                onClick={() => {
                  setShowAddForm(false);
                  setEditingMember(null);
                  resetForm();
                }}
                className="px-6 py-3 bg-neutral-200 text-neutral-700 rounded-lg font-semibold hover:bg-neutral-300 transition-colors"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Family Members List */}
      <div className="bg-white rounded-xl shadow-sm border border-neutral-200 p-6">
        <h2 className="text-xl font-bold text-neutral-900 mb-4">Family Members ({familyMembers.length})</h2>

        {isLoading ? (
          <div className="text-center py-8">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
            <p className="text-neutral-600 mt-2">Loading family members...</p>
          </div>
        ) : familyMembers.length === 0 ? (
          <div className="text-center py-12 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
            <svg className="w-16 h-16 text-neutral-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            <p className="text-neutral-500 text-lg mb-2">No family members added yet</p>
            <p className="text-neutral-400 text-sm">Click "Add Family Member" to get started</p>
          </div>
        ) : (
          <div className="space-y-4">
            {familyMembers.map((member: any) => (
              <div
                key={member.relationId}
                className="p-6 border border-neutral-200 rounded-lg hover:shadow-md transition-shadow"
              >
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-2">
                      <h3 className="text-lg font-bold text-neutral-900">
                        {member.firstName} {member.middleName && `${member.middleName} `}{member.lastName}
                      </h3>
                      <span className="px-3 py-1 bg-primary-100 text-primary-700 text-sm font-semibold rounded-full">
                        {member.relationType}
                      </span>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-2 text-sm">
                      <div>
                        <span className="text-neutral-600">Date of Birth:</span>{' '}
                        <span className="text-neutral-900 font-medium">
                          {new Date(member.dateOfBirth).toLocaleDateString()}
                        </span>
                      </div>
                      {member.gender && (
                        <div>
                          <span className="text-neutral-600">Gender:</span>{' '}
                          <span className="text-neutral-900 font-medium">{member.gender}</span>
                        </div>
                      )}
                      <div>
                        <span className="text-neutral-600">Email:</span>{' '}
                        <span className="text-neutral-900 font-medium">{member.email}</span>
                      </div>
                      {member.phone && (
                        <div>
                          <span className="text-neutral-600">Phone:</span>{' '}
                          <span className="text-neutral-900 font-medium">{member.phone}</span>
                        </div>
                      )}
                      {member.address && (
                        <div className="md:col-span-2">
                          <span className="text-neutral-600">Address:</span>{' '}
                          <span className="text-neutral-900 font-medium">
                            {member.address}
                            {member.city && `, ${member.city}`}
                            {member.state && `, ${member.state}`}
                            {member.zipCode && ` ${member.zipCode}`}
                          </span>
                        </div>
                      )}
                    </div>

                    {/* Permissions */}
                    {(member.isGuardian || member.isProxy || member.canViewRecords || member.canManageRecords || member.canGrantConsent) && (
                      <div className="mt-3 pt-3 border-t border-neutral-200">
                        <p className="text-sm text-neutral-600 mb-2">Permissions:</p>
                        <div className="flex flex-wrap gap-2">
                          {member.isGuardian && (
                            <span className="px-2 py-1 bg-success-100 text-success-700 text-xs font-semibold rounded">
                              Guardian
                            </span>
                          )}
                          {member.isProxy && (
                            <span className="px-2 py-1 bg-success-100 text-success-700 text-xs font-semibold rounded">
                              Healthcare Proxy
                            </span>
                          )}
                          {member.canViewRecords && (
                            <span className="px-2 py-1 bg-info-100 text-info-700 text-xs font-semibold rounded">
                              View Records
                            </span>
                          )}
                          {member.canManageRecords && (
                            <span className="px-2 py-1 bg-info-100 text-info-700 text-xs font-semibold rounded">
                              Manage Records
                            </span>
                          )}
                          {member.canGrantConsent && (
                            <span className="px-2 py-1 bg-warning-100 text-warning-700 text-xs font-semibold rounded">
                              Grant Consent
                            </span>
                          )}
                        </div>
                      </div>
                    )}

                    {/* Legal Documentation */}
                    {member.legalDocumentType && (
                      <div className="mt-3 pt-3 border-t border-neutral-200">
                        <p className="text-sm text-neutral-600">Legal Document: {member.legalDocumentType}</p>
                        {member.legalDocumentNumber && (
                          <p className="text-sm text-neutral-600">Document #: {member.legalDocumentNumber}</p>
                        )}
                        {member.legalDocumentExpiryDate && (
                          <p className="text-sm text-neutral-600">
                            Expires: {new Date(member.legalDocumentExpiryDate).toLocaleDateString()}
                          </p>
                        )}
                      </div>
                    )}
                  </div>

                  <div className="flex gap-2 ml-4">
                    <button
                      onClick={() => handleEdit(member)}
                      className="p-2 text-primary-600 hover:bg-primary-50 rounded-lg transition-colors"
                      title="Edit"
                    >
                      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                      </svg>
                    </button>
                    <button
                      onClick={() => handleDelete(member.relationId)}
                      className="p-2 text-error-600 hover:bg-error-50 rounded-lg transition-colors"
                      title="Delete"
                      disabled={deleteFamilyMemberMutation.isPending}
                    >
                      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
