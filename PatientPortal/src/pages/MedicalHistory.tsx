import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { API_BASE_URL } from '../config/apiConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';

interface Allergy {
  allergyId?: string;
  allergenName: string;
  allergenType: string;
  severity: string;
  reaction?: string;
  onsetDate?: string;
}

interface Medication {
  medicationName: string;
  dosage: string;
  frequency: string;
  prescriber?: string;
}

interface ChronicCondition {
  conditionId?: string;
  conditionName: string;
  diagnosedDate?: string;
  status: string;
}

interface Surgery {
  surgeryType: string;
  surgeryDate?: string;
  notes?: string;
}

interface Hospitalization {
  hospitalName: string;
  reason: string;
  admissionDate?: string;
  dischargeDate?: string;
}

interface Immunization {
  vaccineName: string;
  administeredDate: string;
  doseNumber?: number;
  provider?: string;
}

interface FamilyHistory {
  relative: string;
  condition: string;
  notes?: string;
}

export default function MedicalHistory() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [activeTab, setActiveTab] = useState('allergies');

  // State for new entries
  const [newAllergy, setNewAllergy] = useState<Allergy>({
    allergenName: '',
    allergenType: 'Drug',
    severity: 'Mild',
    reaction: '',
  });

  const [newMedication, setNewMedication] = useState<Medication>({
    medicationName: '',
    dosage: '',
    frequency: '',
    prescriber: '',
  });

  const [newCondition, setNewCondition] = useState<ChronicCondition>({
    conditionName: '',
    status: 'Active',
  });

  const [newSurgery, setNewSurgery] = useState<Surgery>({
    surgeryType: '',
    notes: '',
  });

  const [newHospitalization, setNewHospitalization] = useState<Hospitalization>({
    hospitalName: '',
    reason: '',
  });

  const [newImmunization, setNewImmunization] = useState<Immunization>({
    vaccineName: '',
    administeredDate: '',
    doseNumber: 1,
    provider: '',
  });

  const [newFamilyHistory, setNewFamilyHistory] = useState<FamilyHistory>({
    relative: '',
    condition: '',
    notes: '',
  });

  // Fetch allergies
  const { data: allergies = [] } = useQuery({
    queryKey: ['allergies', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/allergies/patient/${MOCK_PATIENT_ID}`);
      return response.data;
    },
  });

  // Fetch conditions
  const { data: conditions = [] } = useQuery({
    queryKey: ['conditions', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/chronicconditions/patient/${MOCK_PATIENT_ID}`);
      return response.data;
    },
  });

  // Fetch immunizations
  const { data: immunizations = [] } = useQuery({
    queryKey: ['immunizations', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/immunizations/patient/${MOCK_PATIENT_ID}`);
      return response.data;
    },
  });

  // Fetch medications
  const { data: medications = [] } = useQuery({
    queryKey: ['medications', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/medications/patient/${MOCK_PATIENT_ID}`);
      return response.data;
    },
  });

  // Fetch surgeries
  const { data: surgeries = [] } = useQuery({
    queryKey: ['surgeries', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/surgeries/patient/${MOCK_PATIENT_ID}`);
      return response.data;
    },
  });

  // Fetch hospitalizations
  const { data: hospitalizations = [] } = useQuery({
    queryKey: ['hospitalizations', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/hospitalizations/patient/${MOCK_PATIENT_ID}`);
      return response.data;
    },
  });

  // Fetch family history
  const { data: familyHistory = [] } = useQuery({
    queryKey: ['familyHistory', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/familyhistory/patient/${MOCK_PATIENT_ID}`);
      return response.data;
    },
  });

  // Mutations for adding entries
  const addAllergyMutation = useMutation({
    mutationFn: async (allergy: Allergy) => {
      const response = await axios.post(`${API_BASE_URL}/allergies`, {
        ...allergy,
        patientId: MOCK_PATIENT_ID,
        createdBy: 'Patient',
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['allergies', MOCK_PATIENT_ID] });
      setNewAllergy({ allergenName: '', allergenType: 'Drug', severity: 'Mild', reaction: '' });
    },
  });

  const addConditionMutation = useMutation({
    mutationFn: async (condition: ChronicCondition) => {
      const response = await axios.post(`${API_BASE_URL}/chronicconditions`, {
        ...condition,
        patientId: MOCK_PATIENT_ID,
        status: condition.status,
        createdBy: 'Patient',
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['conditions', MOCK_PATIENT_ID] });
      setNewCondition({ conditionName: '', status: 'Active' });
    },
  });

  const addImmunizationMutation = useMutation({
    mutationFn: async (immunization: Immunization) => {
      const response = await axios.post(`${API_BASE_URL}/immunizations`, {
        ...immunization,
        patientId: MOCK_PATIENT_ID,
        status: 'Completed',
        createdBy: 'Patient',
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['immunizations', MOCK_PATIENT_ID] });
      setNewImmunization({ vaccineName: '', administeredDate: '', doseNumber: 1, provider: '' });
    },
  });

  const addMedicationMutation = useMutation({
    mutationFn: async (medication: Medication) => {
      const response = await axios.post(`${API_BASE_URL}/medications`, {
        ...medication,
        patientId: MOCK_PATIENT_ID,
        isActive: true,
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['medications', MOCK_PATIENT_ID] });
      setNewMedication({ medicationName: '', dosage: '', frequency: '', prescriber: '' });
    },
  });

  const addSurgeryMutation = useMutation({
    mutationFn: async (surgery: Surgery) => {
      const response = await axios.post(`${API_BASE_URL}/surgeries`, {
        ...surgery,
        patientId: MOCK_PATIENT_ID,
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['surgeries', MOCK_PATIENT_ID] });
      setNewSurgery({ surgeryType: '', notes: '' });
    },
  });

  const addHospitalizationMutation = useMutation({
    mutationFn: async (hospitalization: Hospitalization) => {
      const response = await axios.post(`${API_BASE_URL}/hospitalizations`, {
        ...hospitalization,
        patientId: MOCK_PATIENT_ID,
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['hospitalizations', MOCK_PATIENT_ID] });
      setNewHospitalization({ hospitalName: '', reason: '' });
    },
  });

  const addFamilyHistoryMutation = useMutation({
    mutationFn: async (familyHistory: FamilyHistory) => {
      const response = await axios.post(`${API_BASE_URL}/familyhistory`, {
        ...familyHistory,
        patientId: MOCK_PATIENT_ID,
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['familyHistory', MOCK_PATIENT_ID] });
      setNewFamilyHistory({ relative: '', condition: '', notes: '' });
    },
  });

  const tabs = [
    { id: 'allergies', label: 'Allergies', icon: '🤧', count: allergies.length },
    { id: 'medications', label: 'Medications', icon: '💊', count: medications.length },
    { id: 'conditions', label: 'Chronic Conditions', icon: '🩺', count: conditions.length },
    { id: 'surgeries', label: 'Surgeries', icon: '🏥', count: surgeries.length },
    { id: 'hospitalizations', label: 'Hospitalizations', icon: '🛏️', count: hospitalizations.length },
    { id: 'immunizations', label: 'Immunizations', icon: '💉', count: immunizations.length },
    { id: 'family', label: 'Family History', icon: '👨‍👩‍👧‍👦', count: familyHistory.length },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-white rounded-xl shadow-sm border border-neutral-200 p-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 bg-gradient-to-br from-primary-500 to-accent-500 rounded-xl flex items-center justify-center">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <div>
              <h1 className="text-2xl font-bold text-neutral-900">Medical History</h1>
              <p className="text-sm text-neutral-600 mt-0.5">
                Manage your complete health history and records
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

      {/* Tabs */}
      <div className="bg-white rounded-xl shadow-sm border border-neutral-200 overflow-hidden">
        <div className="border-b border-neutral-200 overflow-x-auto">
          <nav className="flex min-w-max" aria-label="Medical history tabs">
            {tabs.map((tab) => (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id)}
                className={`flex items-center gap-2 px-6 py-4 text-sm font-semibold whitespace-nowrap border-b-2 transition-colors ${
                  activeTab === tab.id
                    ? 'border-primary-500 text-primary-600 bg-primary-50'
                    : 'border-transparent text-neutral-600 hover:text-primary-600 hover:bg-neutral-50'
                }`}
              >
                <span className="text-xl">{tab.icon}</span>
                {tab.label}
                {tab.count > 0 && (
                  <span className="ml-2 px-2 py-0.5 bg-primary-100 text-primary-700 text-xs font-bold rounded-full">
                    {tab.count}
                  </span>
                )}
              </button>
            ))}
          </nav>
        </div>

        <div className="p-6">
          {/* Allergies Tab */}
          {activeTab === 'allergies' && (
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold text-neutral-900">Your Allergies</h2>
                <button
                  onClick={() => {
                    if (newAllergy.allergenName.trim()) {
                      addAllergyMutation.mutate(newAllergy);
                    }
                  }}
                  disabled={!newAllergy.allergenName.trim() || addAllergyMutation.isPending}
                  className="px-4 py-2 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  {addAllergyMutation.isPending ? 'Adding...' : 'Add Allergy'}
                </button>
              </div>

              {/* Add New Allergy Form */}
              <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Allergen Name *
                    </label>
                    <input
                      type="text"
                      value={newAllergy.allergenName}
                      onChange={(e) => setNewAllergy({ ...newAllergy, allergenName: e.target.value })}
                      placeholder="e.g., Penicillin, Peanuts"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Type *
                    </label>
                    <select
                      value={newAllergy.allergenType}
                      onChange={(e) => setNewAllergy({ ...newAllergy, allergenType: e.target.value })}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    >
                      <option value="Drug">Drug</option>
                      <option value="Food">Food</option>
                      <option value="Environment">Environment</option>
                      <option value="Biologic">Biologic</option>
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Severity *
                    </label>
                    <select
                      value={newAllergy.severity}
                      onChange={(e) => setNewAllergy({ ...newAllergy, severity: e.target.value })}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    >
                      <option value="Mild">Mild</option>
                      <option value="Moderate">Moderate</option>
                      <option value="Severe">Severe</option>
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Reaction
                    </label>
                    <input
                      type="text"
                      value={newAllergy.reaction}
                      onChange={(e) => setNewAllergy({ ...newAllergy, reaction: e.target.value })}
                      placeholder="e.g., Hives, Rash"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>
              </div>

              {/* Allergies List */}
              <div className="space-y-3">
                {allergies.length === 0 ? (
                  <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
                    <p className="text-neutral-500">No allergies recorded. Add your first allergy above.</p>
                  </div>
                ) : (
                  allergies.map((allergy: any) => (
                    <div
                      key={allergy.allergyId}
                      className="flex items-center justify-between p-4 bg-white rounded-lg border border-neutral-200 hover:shadow-md transition-shadow"
                    >
                      <div className="flex-1">
                        <div className="flex items-center gap-2">
                          <h3 className="font-bold text-neutral-900">{allergy.allergenName}</h3>
                          <span className="px-2 py-0.5 bg-accent-100 text-accent-700 text-xs font-semibold rounded">
                            {allergy.allergenType}
                          </span>
                          <span className={`px-2 py-0.5 text-xs font-semibold rounded ${
                            allergy.severity === 'Severe'
                              ? 'bg-red-100 text-red-700'
                              : allergy.severity === 'Moderate'
                              ? 'bg-yellow-100 text-yellow-700'
                              : 'bg-green-100 text-green-700'
                          }`}>
                            {allergy.severity}
                          </span>
                        </div>
                        {allergy.reaction && (
                          <p className="text-sm text-neutral-600 mt-1">Reaction: {allergy.reaction}</p>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          )}

          {/* Medications Tab */}
          {activeTab === 'medications' && (
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold text-neutral-900">Current Medications</h2>
              </div>

              {/* Add New Medication Form */}
              <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Medication Name *
                    </label>
                    <input
                      type="text"
                      value={newMedication.medicationName}
                      onChange={(e) => setNewMedication({ ...newMedication, medicationName: e.target.value })}
                      placeholder="e.g., Lisinopril"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Dosage *
                    </label>
                    <input
                      type="text"
                      value={newMedication.dosage}
                      onChange={(e) => setNewMedication({ ...newMedication, dosage: e.target.value })}
                      placeholder="e.g., 10mg"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Frequency *
                    </label>
                    <input
                      type="text"
                      value={newMedication.frequency}
                      onChange={(e) => setNewMedication({ ...newMedication, frequency: e.target.value })}
                      placeholder="e.g., Once daily"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Prescriber
                    </label>
                    <input
                      type="text"
                      value={newMedication.prescriber}
                      onChange={(e) => setNewMedication({ ...newMedication, prescriber: e.target.value })}
                      placeholder="e.g., Dr. Smith"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>
                <button
                  onClick={() => {
                    if (newMedication.medicationName.trim() && newMedication.dosage.trim() && newMedication.frequency.trim()) {
                      addMedicationMutation.mutate(newMedication);
                    }
                  }}
                  disabled={!newMedication.medicationName.trim() || !newMedication.dosage.trim() || !newMedication.frequency.trim() || addMedicationMutation.isPending}
                  className="mt-4 px-4 py-2 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  {addMedicationMutation.isPending ? 'Adding...' : 'Add Medication'}
                </button>
              </div>

              {/* Medications List */}
              <div className="space-y-3">
                {medications.length === 0 ? (
                  <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
                    <p className="text-neutral-500">No medications recorded yet. Add your first medication above.</p>
                  </div>
                ) : (
                  medications.map((medication: any) => (
                    <div
                      key={medication.medicationId}
                      className="p-4 bg-white rounded-lg border border-neutral-200 hover:shadow-md transition-shadow"
                    >
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <h3 className="font-bold text-neutral-900">{medication.medicationName}</h3>
                          <p className="text-sm text-neutral-600 mt-1">
                            {medication.dosage} - {medication.frequency}
                          </p>
                          {medication.prescriber && (
                            <p className="text-sm text-neutral-500 mt-1">Prescribed by: {medication.prescriber}</p>
                          )}
                          {medication.notes && (
                            <p className="text-sm text-neutral-500 mt-1">{medication.notes}</p>
                          )}
                        </div>
                        {medication.isActive && (
                          <span className="px-3 py-1 bg-success-100 text-success-700 text-xs font-semibold rounded-full">
                            Active
                          </span>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          )}

          {/* Chronic Conditions Tab */}
          {activeTab === 'conditions' && (
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold text-neutral-900">Chronic Conditions</h2>
                <button
                  onClick={() => {
                    if (newCondition.conditionName.trim()) {
                      addConditionMutation.mutate(newCondition);
                    }
                  }}
                  disabled={!newCondition.conditionName.trim() || addConditionMutation.isPending}
                  className="px-4 py-2 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  {addConditionMutation.isPending ? 'Adding...' : 'Add Condition'}
                </button>
              </div>

              {/* Add New Condition Form */}
              <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Condition Name *
                    </label>
                    <input
                      type="text"
                      value={newCondition.conditionName}
                      onChange={(e) => setNewCondition({ ...newCondition, conditionName: e.target.value })}
                      placeholder="e.g., Type 2 Diabetes"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Status *
                    </label>
                    <select
                      value={newCondition.status}
                      onChange={(e) => setNewCondition({ ...newCondition, status: e.target.value })}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    >
                      <option value="Active">Active</option>
                      <option value="Inactive">Inactive</option>
                      <option value="Resolved">Resolved</option>
                    </select>
                  </div>
                </div>
              </div>

              {/* Conditions List */}
              <div className="space-y-3">
                {conditions.length === 0 ? (
                  <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
                    <p className="text-neutral-500">No chronic conditions recorded. Add your first condition above.</p>
                  </div>
                ) : (
                  conditions.map((condition: any) => (
                    <div
                      key={condition.conditionId}
                      className="flex items-center justify-between p-4 bg-white rounded-lg border border-neutral-200 hover:shadow-md transition-shadow"
                    >
                      <div className="flex-1">
                        <div className="flex items-center gap-2">
                          <h3 className="font-bold text-neutral-900">{condition.conditionName}</h3>
                          <span className={`px-2 py-0.5 text-xs font-semibold rounded ${
                            condition.status === 'Active'
                              ? 'bg-yellow-100 text-yellow-700'
                              : 'bg-green-100 text-green-700'
                          }`}>
                            {condition.status}
                          </span>
                        </div>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          )}

          {/* Immunizations Tab */}
          {activeTab === 'immunizations' && (
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold text-neutral-900">Immunization Records</h2>
                <button
                  onClick={() => {
                    if (newImmunization.vaccineName.trim() && newImmunization.administeredDate) {
                      addImmunizationMutation.mutate(newImmunization);
                    }
                  }}
                  disabled={!newImmunization.vaccineName.trim() || !newImmunization.administeredDate || addImmunizationMutation.isPending}
                  className="px-4 py-2 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  {addImmunizationMutation.isPending ? 'Adding...' : 'Add Immunization'}
                </button>
              </div>

              {/* Add New Immunization Form */}
              <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Vaccine Name *
                    </label>
                    <input
                      type="text"
                      value={newImmunization.vaccineName}
                      onChange={(e) => setNewImmunization({ ...newImmunization, vaccineName: e.target.value })}
                      placeholder="e.g., COVID-19, Flu"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Date Administered *
                    </label>
                    <input
                      type="date"
                      value={newImmunization.administeredDate}
                      onChange={(e) => setNewImmunization({ ...newImmunization, administeredDate: e.target.value })}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Dose Number
                    </label>
                    <input
                      type="number"
                      value={newImmunization.doseNumber}
                      onChange={(e) => setNewImmunization({ ...newImmunization, doseNumber: parseInt(e.target.value) })}
                      min="1"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Provider
                    </label>
                    <input
                      type="text"
                      value={newImmunization.provider}
                      onChange={(e) => setNewImmunization({ ...newImmunization, provider: e.target.value })}
                      placeholder="e.g., CVS Pharmacy"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>
              </div>

              {/* Immunizations List */}
              <div className="space-y-3">
                {immunizations.length === 0 ? (
                  <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
                    <p className="text-neutral-500">No immunizations recorded. Add your first immunization above.</p>
                  </div>
                ) : (
                  immunizations.map((immunization: any) => (
                    <div
                      key={immunization.immunizationId}
                      className="flex items-center justify-between p-4 bg-white rounded-lg border border-neutral-200 hover:shadow-md transition-shadow"
                    >
                      <div className="flex-1">
                        <div className="flex items-center gap-2">
                          <h3 className="font-bold text-neutral-900">{immunization.vaccineName}</h3>
                          {immunization.doseNumber && (
                            <span className="px-2 py-0.5 bg-primary-100 text-primary-700 text-xs font-semibold rounded">
                              Dose {immunization.doseNumber}
                            </span>
                          )}
                        </div>
                        <p className="text-sm text-neutral-600 mt-1">
                          {new Date(immunization.administeredDate).toLocaleDateString('en-US', {
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric',
                          })}
                        </p>
                        {immunization.providerName && (
                          <p className="text-xs text-neutral-500 mt-1">Provider: {immunization.providerName}</p>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          )}

          {/* Surgeries Tab */}
          {activeTab === 'surgeries' && (
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold text-neutral-900">Surgical History</h2>
              </div>

              {/* Add New Surgery Form */}
              <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Surgery Type *
                    </label>
                    <input
                      type="text"
                      value={newSurgery.surgeryType}
                      onChange={(e) => setNewSurgery({ ...newSurgery, surgeryType: e.target.value })}
                      placeholder="e.g., Appendectomy"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Surgery Date
                    </label>
                    <input
                      type="date"
                      value={newSurgery.surgeryDate || ''}
                      onChange={(e) => setNewSurgery({ ...newSurgery, surgeryDate: e.target.value })}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div className="md:col-span-2">
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Notes
                    </label>
                    <textarea
                      value={newSurgery.notes || ''}
                      onChange={(e) => setNewSurgery({ ...newSurgery, notes: e.target.value })}
                      placeholder="Additional details"
                      rows={2}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>
                <button
                  onClick={() => {
                    if (newSurgery.surgeryType.trim()) {
                      addSurgeryMutation.mutate(newSurgery);
                    }
                  }}
                  disabled={!newSurgery.surgeryType.trim() || addSurgeryMutation.isPending}
                  className="mt-4 px-4 py-2 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  {addSurgeryMutation.isPending ? 'Adding...' : 'Add Surgery'}
                </button>
              </div>

              {/* Surgeries List */}
              <div className="space-y-3">
                {surgeries.length === 0 ? (
                  <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
                    <p className="text-neutral-500">No surgeries recorded yet.</p>
                  </div>
                ) : (
                  surgeries.map((surgery: any) => (
                    <div
                      key={surgery.surgeryId}
                      className="p-4 bg-white rounded-lg border border-neutral-200 hover:shadow-md transition-shadow"
                    >
                      <h3 className="font-bold text-neutral-900">{surgery.surgeryType}</h3>
                      {surgery.surgeryDate && (
                        <p className="text-sm text-neutral-600 mt-1">
                          Date: {new Date(surgery.surgeryDate).toLocaleDateString()}
                        </p>
                      )}
                      {surgery.notes && (
                        <p className="text-sm text-neutral-500 mt-1">{surgery.notes}</p>
                      )}
                    </div>
                  ))
                )}
              </div>
            </div>
          )}

          {/* Hospitalizations Tab */}
          {activeTab === 'hospitalizations' && (
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold text-neutral-900">Hospitalization History</h2>
              </div>

              {/* Add New Hospitalization Form */}
              <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Hospital Name *
                    </label>
                    <input
                      type="text"
                      value={newHospitalization.hospitalName}
                      onChange={(e) => setNewHospitalization({ ...newHospitalization, hospitalName: e.target.value })}
                      placeholder="e.g., General Hospital"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Reason *
                    </label>
                    <input
                      type="text"
                      value={newHospitalization.reason}
                      onChange={(e) => setNewHospitalization({ ...newHospitalization, reason: e.target.value })}
                      placeholder="e.g., Pneumonia treatment"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Admission Date
                    </label>
                    <input
                      type="date"
                      value={newHospitalization.admissionDate || ''}
                      onChange={(e) => setNewHospitalization({ ...newHospitalization, admissionDate: e.target.value })}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Discharge Date
                    </label>
                    <input
                      type="date"
                      value={newHospitalization.dischargeDate || ''}
                      onChange={(e) => setNewHospitalization({ ...newHospitalization, dischargeDate: e.target.value })}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>
                <button
                  onClick={() => {
                    if (newHospitalization.hospitalName.trim() && newHospitalization.reason.trim()) {
                      addHospitalizationMutation.mutate(newHospitalization);
                    }
                  }}
                  disabled={!newHospitalization.hospitalName.trim() || !newHospitalization.reason.trim() || addHospitalizationMutation.isPending}
                  className="mt-4 px-4 py-2 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  {addHospitalizationMutation.isPending ? 'Adding...' : 'Add Hospitalization'}
                </button>
              </div>

              {/* Hospitalizations List */}
              <div className="space-y-3">
                {hospitalizations.length === 0 ? (
                  <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
                    <p className="text-neutral-500">No hospitalizations recorded yet.</p>
                  </div>
                ) : (
                  hospitalizations.map((hospitalization: any) => (
                    <div
                      key={hospitalization.hospitalizationId}
                      className="p-4 bg-white rounded-lg border border-neutral-200 hover:shadow-md transition-shadow"
                    >
                      <h3 className="font-bold text-neutral-900">{hospitalization.hospitalName}</h3>
                      <p className="text-sm text-neutral-600 mt-1">Reason: {hospitalization.reason}</p>
                      {hospitalization.admissionDate && (
                        <p className="text-sm text-neutral-500 mt-1">
                          Admitted: {new Date(hospitalization.admissionDate).toLocaleDateString()}
                        </p>
                      )}
                      {hospitalization.dischargeDate && (
                        <p className="text-sm text-neutral-500 mt-1">
                          Discharged: {new Date(hospitalization.dischargeDate).toLocaleDateString()}
                        </p>
                      )}
                    </div>
                  ))
                )}
              </div>
            </div>
          )}

          {/* Family History Tab */}
          {activeTab === 'family' && (
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold text-neutral-900">Family Medical History</h2>
              </div>

              {/* Add New Family History Form */}
              <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Relative *
                    </label>
                    <input
                      type="text"
                      value={newFamilyHistory.relative}
                      onChange={(e) => setNewFamilyHistory({ ...newFamilyHistory, relative: e.target.value })}
                      placeholder="e.g., Father, Mother, Sibling"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Condition *
                    </label>
                    <input
                      type="text"
                      value={newFamilyHistory.condition}
                      onChange={(e) => setNewFamilyHistory({ ...newFamilyHistory, condition: e.target.value })}
                      placeholder="e.g., Diabetes, Heart Disease"
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div className="md:col-span-2">
                    <label className="block text-sm font-semibold text-neutral-700 mb-1">
                      Notes
                    </label>
                    <textarea
                      value={newFamilyHistory.notes || ''}
                      onChange={(e) => setNewFamilyHistory({ ...newFamilyHistory, notes: e.target.value })}
                      placeholder="Additional details"
                      rows={2}
                      className="w-full px-3 py-2 border border-neutral-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>
                <button
                  onClick={() => {
                    if (newFamilyHistory.relative.trim() && newFamilyHistory.condition.trim()) {
                      addFamilyHistoryMutation.mutate(newFamilyHistory);
                    }
                  }}
                  disabled={!newFamilyHistory.relative.trim() || !newFamilyHistory.condition.trim() || addFamilyHistoryMutation.isPending}
                  className="mt-4 px-4 py-2 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  {addFamilyHistoryMutation.isPending ? 'Adding...' : 'Add Family History'}
                </button>
              </div>

              {/* Family History List */}
              <div className="space-y-3">
                {familyHistory.length === 0 ? (
                  <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
                    <p className="text-neutral-500">No family medical history recorded yet.</p>
                  </div>
                ) : (
                  familyHistory.map((history: any) => (
                    <div
                      key={history.familyHistoryId}
                      className="p-4 bg-white rounded-lg border border-neutral-200 hover:shadow-md transition-shadow"
                    >
                      <div className="flex items-start gap-3">
                        <div className="flex-1">
                          <h3 className="font-bold text-neutral-900">{history.relative}</h3>
                          <p className="text-sm text-neutral-600 mt-1">{history.condition}</p>
                          {history.notes && (
                            <p className="text-sm text-neutral-500 mt-1">{history.notes}</p>
                          )}
                        </div>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

