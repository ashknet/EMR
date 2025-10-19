import { useState, FormEvent, useEffect } from 'react';
import { API_BASE } from '../config/apiConfig';
import { MOCK_USER } from '../config/mockUser';

interface PatientCoreDto {
  id: string;
  // Personal Information
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
  dateOfBirth: string;
  gender: string;
  genderId?: number;
  ssnEncrypted?: string;
  maritalStatus?: string;
  maritalStatusId?: number;
  race?: string;
  raceId?: number;
  ethnicity?: string;
  primaryLanguageId?: number;
  preferredLanguageId?: number;
  preferredLanguage?: string;
  
  // Contact Information
  email: string;
  phoneNumber: string;
  alternatePhone?: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  
  // Emergency Contact (legacy, consider using EmergencyContacts table)
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  emergencyContactRelationship?: string;
  
  // Accessibility & Preferences
  interpreterRequired: boolean;
  mobilityAssistance: boolean;
  communicationPrefId?: number;
  religiousConsiderations?: string;
  
  // Provider Information
  primaryCarePhysician?: string;
  pcpPhoneNumber?: string;
  referringPhysician?: string;
  
  // Pharmacy Information
  preferredPharmacyName?: string;
  preferredPharmacyLocation?: string;
  
  // Administrative
  photoIdPath?: string;
  
  // Electronic Signature (stored here for UI, but sent to legalConsents on submit)
  electronicSignature?: string;
}

interface AddressDto {
  addressType: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  isPrimary: boolean;
}

interface PhoneDto {
  phoneType: string;
  phoneNumber: string;
  isPrimary: boolean;
}

interface EmergencyContactDto {
  name: string;
  relationshipTypeId: number;
  phone: string;
  altPhone?: string;
}

interface InsurancePolicyDto {
  insuranceCompany: string;
  policyNumber: string;
  groupNumber?: string;
  policyHolderName: string;
  policyHolderDob?: string;
  relationToPolicyholder?: string;
  isPrimary: boolean;
  effectiveDate?: string;
  expirationDate?: string;
  copay?: number;
  insurancePhoneNumber?: string;
}

interface SocialHistoryDto {
  smokingStatusId?: number;
  alcoholUseId?: number;
  drugUseId?: number;
  occupation?: string;
  livingSituationId?: number;
  exerciseFrequency?: string;
  diet?: string;
  stressLevel?: string;
  sleepHours?: number;
}

interface LegalConsentDto {
  hipaaAgreed: boolean;
  consentToTreat: boolean;
  advanceDirectives: boolean;
  advanceDirectivesPath?: string;
  assignmentOfBenefits: boolean;
  financialResponsibility: boolean;
  signedOnUtc?: string;
  signaturePath?: string;
}

interface AllergyDto {
  allergyName: string;
  allergyType: string;
  reaction?: string;
  severity?: string;
  onsetDate?: string;
}

interface MedicationDto {
  medicationName: string;
  dosage?: string;
  frequency?: string;
  prescriber?: string;
  startDate?: string;
}

interface ChronicConditionDto {
  conditionName: string;
  diagnosedDate?: string;
  status?: string;
  notes?: string;
}

interface ImmunizationDto {
  vaccineName: string;
  administeredDate: string;
  doseNumber?: number;
  provider?: string;
  lotNumber?: string;
  expirationDate?: string;
  site?: string;
  route?: string;
  notes?: string;
}

interface SurgeryDto {
  surgeryType: string;
  surgeryDate?: string;
  notes?: string;
}

interface HospitalizationDto {
  hospitalName?: string;
  reason?: string;
  admissionDate?: string;
  dischargeDate?: string;
  notes?: string;
}

interface FamilyHistoryDto {
  relative: string;
  condition: string;
  notes?: string;
}

interface FamilyMemberDto {
  firstName: string;
  lastName: string;
  dateOfBirth?: string;
  relationship: string;
  phoneNumber?: string;
  email?: string;
  address?: string;
}

interface PatientIntakeDto {
  patient: PatientCoreDto;
  addresses: AddressDto[];
  phones: PhoneDto[];
  emergencyContacts: EmergencyContactDto[];
  familyMembers: FamilyMemberDto[];
  insurancePolicies: InsurancePolicyDto[];
  socialHistory?: SocialHistoryDto;
  legalConsents?: LegalConsentDto;
  allergies: AllergyDto[];
  medications: MedicationDto[];
  chronicConditions: ChronicConditionDto[];
  immunizations: ImmunizationDto[];
  surgeries: SurgeryDto[];
  hospitalizations: HospitalizationDto[];
  familyMedicalHistory: FamilyHistoryDto[];
}

export default function ComprehensiveIntake() {
  const [currentStep, setCurrentStep] = useState(1);
  const [loading, setLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(true);
  const [message, setMessage] = useState('');

  const [formData, setFormData] = useState<PatientIntakeDto>({
    patient: {
      id: '',
      firstName: '',
      lastName: '',
      dateOfBirth: '',
      gender: 'Male',
      email: '',
      phoneNumber: '',
      addressLine1: '',
      city: '',
      state: '',
      zipCode: '',
      country: 'USA',
      interpreterRequired: false,
      mobilityAssistance: false,
    },
    addresses: [],
    phones: [],
    emergencyContacts: [],
    familyMembers: [],
    insurancePolicies: [],
    allergies: [],
    medications: [],
    chronicConditions: [],
    immunizations: [],
    surgeries: [],
    hospitalizations: [],
    familyMedicalHistory: [],
  });

  const totalSteps = 11;

  // Load basic patient data on component mount (no duplicate calls)
  useEffect(() => {
    const loadBasicPatientData = async () => {
      try {
        setLoadingData(true);
        
        // Fetch only basic patient data from database using logged-in user ID
        const patientResponse = await fetch(`${API_BASE}/api/patients/${MOCK_USER.patientId}`);
        if (!patientResponse.ok) {
          console.error('Failed to load patient data');
          setLoadingData(false);
          return;
        }
        
        const patientData = await patientResponse.json();
        
        // Map API response to form structure
        setFormData({
          patient: {
            id: patientData.patientId || MOCK_USER.patientId,
            firstName: patientData.firstName || '',
            middleName: patientData.middleName || '',
            lastName: patientData.lastName || '',
            suffix: patientData.suffix || '',
            dateOfBirth: patientData.dateOfBirth ? patientData.dateOfBirth.split('T')[0] : '',
            gender: patientData.gender || 'Male',
            genderId: patientData.genderId,
            ssnEncrypted: patientData.ssnEncrypted || '',
            maritalStatus: patientData.maritalStatus || '',
            maritalStatusId: patientData.maritalStatusId,
            race: patientData.race || '',
            raceId: patientData.raceId,
            ethnicity: patientData.ethnicity || '',
            primaryLanguageId: patientData.primaryLanguageId,
            preferredLanguageId: patientData.preferredLanguageId,
            preferredLanguage: patientData.preferredLanguage || '',
            email: patientData.email || '',
            phoneNumber: patientData.phoneNumber || '',
            alternatePhone: patientData.alternatePhone || '',
            addressLine1: patientData.addressLine1 || '',
            addressLine2: patientData.addressLine2 || '',
            city: patientData.city || '',
            state: patientData.state || '',
            zipCode: patientData.zipCode || '',
            country: patientData.country || 'USA',
            emergencyContactName: patientData.emergencyContactName || '',
            emergencyContactPhone: patientData.emergencyContactPhone || '',
            emergencyContactRelationship: patientData.emergencyContactRelationship || '',
            interpreterRequired: patientData.interpreterRequired || false,
            mobilityAssistance: patientData.mobilityAssistance || false,
            communicationPrefId: patientData.communicationPrefId,
            religiousConsiderations: patientData.religiousConsiderations || '',
            primaryCarePhysician: patientData.primaryCarePhysician || '',
            pcpPhoneNumber: patientData.pcpPhoneNumber || '',
            referringPhysician: patientData.referringPhysician || '',
            preferredPharmacyName: patientData.preferredPharmacyName || '',
            preferredPharmacyLocation: patientData.preferredPharmacyLocation || '',
            photoIdPath: patientData.photoIdPath || '',
            electronicSignature: patientData.electronicSignature || '',
          },
          addresses: [],
          phones: [],
          emergencyContacts: [],
          familyMembers: [],
          insurancePolicies: [],
          socialHistory: {
            smokingStatusId: undefined,
            alcoholUseId: undefined,
            drugUseId: undefined,
            occupation: '',
            livingSituationId: undefined,
          },
          legalConsents: {
            hipaaAgreed: false,
            consentToTreat: false,
            advanceDirectives: false,
            advanceDirectivesPath: '',
            assignmentOfBenefits: false,
            financialResponsibility: false,
            signedOnUtc: '',
            signaturePath: '',
          },
          allergies: [],
          medications: [],
          chronicConditions: [],
          immunizations: [],
          surgeries: [],
          hospitalizations: [],
          familyMedicalHistory: [],
        });
        
      } catch (error) {
        console.error('Error loading patient data:', error);
        setMessage('Error loading patient data. Please try again.');
      } finally {
        setLoadingData(false);
      }
    };
    
    loadBasicPatientData();
  }, []);

  // Step-specific save functions
  const saveStep1 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      firstName: formData.patient.firstName,
      lastName: formData.patient.lastName,
      middleName: formData.patient.middleName,
      suffix: formData.patient.suffix,
      dateOfBirth: formData.patient.dateOfBirth,
      genderId: formData.patient.genderId,
      ssnEncrypted: formData.patient.ssnEncrypted,
      maritalStatusId: formData.patient.maritalStatusId,
      raceId: formData.patient.raceId,
      ethnicity: formData.patient.ethnicity,
      primaryLanguageId: formData.patient.primaryLanguageId,
      preferredLanguageId: formData.patient.preferredLanguageId,
      communicationPrefId: formData.patient.communicationPrefId,
      interpreterRequired: formData.patient.interpreterRequired,
      mobilityAssistance: formData.patient.mobilityAssistance,
      religiousConsiderations: formData.patient.religiousConsiderations,
      primaryCarePhysician: formData.patient.primaryCarePhysician,
      pcpPhoneNumber: formData.patient.pcpPhoneNumber,
      referringPhysician: formData.patient.referringPhysician,
      preferredPharmacyName: formData.patient.preferredPharmacyName,
      preferredPharmacyLocation: formData.patient.preferredPharmacyLocation,
      photoIdPath: formData.patient.photoIdPath,
      electronicSignature: formData.patient.electronicSignature,
      signedDate: undefined, // signedDate is not part of patient entity
    };

    const response = await fetch(`${API_BASE}/api/patients/step1`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 1');
    }
  };

  const saveStep2 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      email: formData.patient.email,
      phoneNumber: formData.patient.phoneNumber,
      alternatePhone: formData.patient.alternatePhone,
      emergencyContactName: formData.patient.emergencyContactName,
      emergencyContactPhone: formData.patient.emergencyContactPhone,
      emergencyContactRelationship: formData.patient.emergencyContactRelationship,
    };

    const response = await fetch(`${API_BASE}/api/patients/step2`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 2');
    }
  };

  const saveStep3 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      addressLine1: formData.patient.addressLine1,
      addressLine2: formData.patient.addressLine2,
      city: formData.patient.city,
      state: formData.patient.state,
      zipCode: formData.patient.zipCode,
      country: formData.patient.country,
    };

    const response = await fetch(`${API_BASE}/api/patients/step3`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 3');
    }
  };

  const saveStep4 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      emergencyContacts: formData.emergencyContacts
        .filter(contact => contact.name && contact.name.trim() !== '' && contact.phone && contact.phone.trim() !== '')
        .map(contact => ({
          Name: contact.name,
          RelationshipTypeId: contact.relationshipTypeId,
          Phone: contact.phone,
          AltPhone: contact.altPhone,
        })),
    };

    const response = await fetch(`${API_BASE}/api/patients/step4`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 4');
    }
  };

  const saveStep5 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      smokingStatusId: formData.socialHistory?.smokingStatusId,
      alcoholUseId: formData.socialHistory?.alcoholUseId,
      drugUseId: formData.socialHistory?.drugUseId,
      occupation: formData.socialHistory?.occupation,
      livingSituationId: formData.socialHistory?.livingSituationId,
    };

    const response = await fetch(`${API_BASE}/api/patients/step5`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 5');
    }
  };

  const saveStep6 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      allergies: formData.allergies
        .filter(a => a.allergyName && a.allergyName.trim() !== '')
        .map(a => ({
          AllergenName: a.allergyName,
          AllergenType: a.allergyType || 'Drug',
          Severity: a.severity || 'Mild',
          Reaction: a.reaction || '',
          OnsetDate: a.onsetDate || null,
        })),
    };

    const response = await fetch(`${API_BASE}/api/patients/step6`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 6');
    }
  };

  const saveStep7 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      medications: formData.medications
        .filter(m => m.medicationName && m.medicationName.trim() !== '')
        .map(m => ({
          MedicationName: m.medicationName,
          Dosage: m.dosage,
          Frequency: m.frequency,
          Prescriber: m.prescriber,
          StartDate: m.startDate || null,
        })),
      chronicConditions: formData.chronicConditions
        .filter(c => c.conditionName && c.conditionName.trim() !== '')
        .map(c => ({
          ConditionName: c.conditionName,
          DiagnosedDate: c.diagnosedDate || null,
          Status: c.status || 'Active',
          Notes: c.notes,
        })),
      immunizations: formData.immunizations
        .filter(i => i.vaccineName && i.vaccineName.trim() !== '')
        .map(i => ({
          VaccineName: i.vaccineName,
          AdministeredDate: i.administeredDate,
          DoseNumber: i.doseNumber,
          Provider: i.provider,
          LotNumber: i.lotNumber,
          ExpirationDate: i.expirationDate || null,
          Site: i.site,
          Route: i.route,
          Notes: i.notes,
        })),
    };

    const response = await fetch(`${API_BASE}/api/patients/step7`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 7');
    }
  };

  const saveStep8 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      surgeries: formData.surgeries
        .filter(s => s.surgeryType && s.surgeryType.trim() !== '')
        .map(s => ({
          SurgeryType: s.surgeryType,
          SurgeryDate: s.surgeryDate || null,
          Notes: s.notes,
        })),
      hospitalizations: formData.hospitalizations
        .filter(h => h.hospitalName && h.hospitalName.trim() !== '')
        .map(h => ({
          HospitalName: h.hospitalName,
          Reason: h.reason,
          AdmissionDate: h.admissionDate || null,
          DischargeDate: h.dischargeDate || null,
          Notes: h.notes,
        })),
      familyMedicalHistory: formData.familyMedicalHistory
        .filter(f => f.relative && f.relative.trim() !== '' && f.condition && f.condition.trim() !== '')
        .map(f => ({
          Relative: f.relative,
          Condition: f.condition,
          Notes: f.notes,
        })),
      };

    const response = await fetch(`${API_BASE}/api/patients/step8`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 8');
    }
  };

  const saveStep9 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      insurancePolicies: formData.insurancePolicies
        .filter(ip => ip.insuranceCompany && ip.insuranceCompany.trim() !== '')
        .map(ip => ({
          insuranceCompany: ip.insuranceCompany,
          policyNumber: ip.policyNumber,
          groupNumber: ip.groupNumber,
          policyHolderName: ip.policyHolderName,
          policyHolderDob: ip.policyHolderDob,
          relationToPolicyholder: ip.relationToPolicyholder,
          isPrimary: ip.isPrimary,
          effectiveDate: ip.effectiveDate,
          expirationDate: ip.expirationDate,
        })),
    };

    const response = await fetch(`${API_BASE}/api/patients/step9`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 9');
    }
  };

  const saveStep10 = async (patientId: string) => {
    const stepData = {
      patientId: patientId,
      hipaaAgreed: formData.legalConsents?.hipaaAgreed,
      consentToTreat: formData.legalConsents?.consentToTreat,
      advanceDirectives: formData.legalConsents?.advanceDirectives,
      advanceDirectivesPath: formData.legalConsents?.advanceDirectivesPath,
      assignmentOfBenefits: formData.legalConsents?.assignmentOfBenefits,
      financialResponsibility: formData.legalConsents?.financialResponsibility,
      signedOnUtc: formData.legalConsents?.signedOnUtc || new Date().toISOString(),
      signaturePath: formData.patient.electronicSignature || '',
    };

    const response = await fetch(`${API_BASE}/api/patients/step10`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(stepData),
    });

    if (!response.ok) {
      throw new Error('Failed to save step 10');
    }
  };

  // Auto-save function - saves current step data
  const autoSave = async () => {
    try {
      const patientId = formData.patient.id || MOCK_USER.patientId;
      
      switch (currentStep) {
        case 1:
          await saveStep1(patientId);
          break;
        case 2:
          await saveStep2(patientId);
          break;
        case 3:
          await saveStep3(patientId);
          break;
        case 4:
          await saveStep4(patientId);
          break;
        case 5:
          await saveStep5(patientId);
          break;
        case 6:
          await saveStep6(patientId);
          break;
        case 7:
          await saveStep7(patientId);
          break;
        case 8:
          await saveStep8(patientId);
          break;
        case 9:
          await saveStep9(patientId);
          break;
        case 10:
          await saveStep10(patientId);
          break;
      }

        setMessage('✓ Progress saved');
      setTimeout(() => setMessage(''), 2000);
        return true;
    } catch (err) {
      console.error('Auto-save error:', err);
      setMessage('⚠ Could not save progress');
      setTimeout(() => setMessage(''), 3000);
      return false;
    }
  };

  // Load step-specific data
  const loadStepData = async (stepNumber: number) => {
    const stepNames = [
      '', // Step 0 - not used
      'basicinfo',
      'contactinfo', 
      'address',
      'emergencycontacts',
      'insurance',
      'providerinfo',
      'medicalhistory',
      'socialhistory',
      'accessibility',
      'legalconsents'
    ];

    const stepName = stepNames[stepNumber];
    if (!stepName) return;

    try {
      const response = await fetch(`${API_BASE}/api/patients/${MOCK_USER.patientId}/step/${stepName}`);
      if (response.ok) {
        const stepData = await response.json();
        
        // Update form data based on step
        setFormData(prev => {
          switch (stepName) {
            case 'basicinfo':
              return {
                ...prev,
                patient: {
                  ...prev.patient,
                  firstName: stepData.firstName || prev.patient.firstName,
                  lastName: stepData.lastName || prev.patient.lastName,
                  middleName: stepData.middleName || prev.patient.middleName,
                  suffix: stepData.suffix || prev.patient.suffix,
                  dateOfBirth: stepData.dateOfBirth ? stepData.dateOfBirth.split('T')[0] : prev.patient.dateOfBirth,
                  genderId: stepData.genderId || prev.patient.genderId,
                  ssnEncrypted: stepData.ssnEncrypted || prev.patient.ssnEncrypted,
                  maritalStatusId: stepData.maritalStatusId || prev.patient.maritalStatusId,
                  raceId: stepData.raceId || prev.patient.raceId,
                  ethnicity: stepData.ethnicity || prev.patient.ethnicity,
                  primaryLanguageId: stepData.primaryLanguageId || prev.patient.primaryLanguageId,
                  preferredLanguageId: stepData.preferredLanguageId || prev.patient.preferredLanguageId,
                  communicationPrefId: stepData.communicationPrefId || prev.patient.communicationPrefId,
                  interpreterRequired: stepData.interpreterRequired || prev.patient.interpreterRequired,
                  mobilityAssistance: stepData.mobilityAssistance || prev.patient.mobilityAssistance,
                  religiousConsiderations: stepData.religiousConsiderations || prev.patient.religiousConsiderations,
                  primaryCarePhysician: stepData.primaryCarePhysician || prev.patient.primaryCarePhysician,
                  pcpPhoneNumber: stepData.pcpPhoneNumber || prev.patient.pcpPhoneNumber,
                  referringPhysician: stepData.referringPhysician || prev.patient.referringPhysician,
                  preferredPharmacyName: stepData.preferredPharmacyName || prev.patient.preferredPharmacyName,
                  preferredPharmacyLocation: stepData.preferredPharmacyLocation || prev.patient.preferredPharmacyLocation,
                  photoIdPath: stepData.photoIdPath || prev.patient.photoIdPath,
                  electronicSignature: stepData.electronicSignature || prev.patient.electronicSignature,
                }
              };
            case 'contactinfo':
              return {
                ...prev,
                patient: {
                  ...prev.patient,
                  email: stepData.email || prev.patient.email,
                  phoneNumber: stepData.phoneNumber || prev.patient.phoneNumber,
                  alternatePhone: stepData.alternatePhone || prev.patient.alternatePhone,
                  emergencyContactName: stepData.emergencyContactName || prev.patient.emergencyContactName,
                  emergencyContactPhone: stepData.emergencyContactPhone || prev.patient.emergencyContactPhone,
                  emergencyContactRelationship: stepData.emergencyContactRelationship || prev.patient.emergencyContactRelationship,
                }
              };
            case 'address':
              return {
                ...prev,
                patient: {
                  ...prev.patient,
                  addressLine1: stepData.addressLine1 || prev.patient.addressLine1,
                  addressLine2: stepData.addressLine2 || prev.patient.addressLine2,
                  city: stepData.city || prev.patient.city,
                  state: stepData.state || prev.patient.state,
                  zipCode: stepData.zipCode || prev.patient.zipCode,
                  country: stepData.country || prev.patient.country,
                }
              };
            case 'emergencycontacts':
              return {
                ...prev,
                emergencyContacts: stepData.emergencyContacts || prev.emergencyContacts
              };
            case 'socialhistory':
              return {
                ...prev,
                socialHistory: {
                  smokingStatusId: stepData.smokingStatusId || prev.socialHistory?.smokingStatusId,
                  alcoholUseId: stepData.alcoholUseId || prev.socialHistory?.alcoholUseId,
                  drugUseId: stepData.drugUseId || prev.socialHistory?.drugUseId,
                  occupation: stepData.occupation || prev.socialHistory?.occupation,
                  livingSituationId: stepData.livingSituationId || prev.socialHistory?.livingSituationId,
                }
              };
            case 'medicalhistory':
              return {
                ...prev,
                allergies: stepData.allergies?.map((a: any) => ({
                  allergyName: a.allergenName || a.AllergenName || '',
                  allergyType: a.allergenType || a.AllergenType || 'Drug',
                  reaction: a.reaction || a.Reaction || '',
                  severity: a.severity || a.Severity || 'Mild',
                  onsetDate: a.onsetDate ? a.onsetDate.split('T')[0] : (a.OnsetDate ? a.OnsetDate.split('T')[0] : ''),
                })) || prev.allergies,
                medications: stepData.medications?.map((m: any) => ({
                  medicationName: m.medicationName || '',
                  dosage: m.dosage || '',
                  frequency: m.frequency || '',
                  prescriber: m.prescriber || '',
                  startDate: m.startDate ? m.startDate.split('T')[0] : '',
                })) || prev.medications,
                chronicConditions: stepData.chronicConditions?.map((c: any) => ({
                  conditionName: c.conditionName || '',
                  diagnosedDate: c.diagnosedDate ? c.diagnosedDate.split('T')[0] : '',
                  status: c.status || 'Active',
                  notes: c.notes || '',
                })) || prev.chronicConditions,
                immunizations: stepData.immunizations?.map((i: any) => ({
                  vaccineName: i.vaccineName || '',
                  administeredDate: i.administeredDate ? i.administeredDate.split('T')[0] : '',
                  doseNumber: i.doseNumber || undefined,
                  provider: i.provider || '',
                  lotNumber: i.lotNumber || '',
                  expirationDate: i.expirationDate ? i.expirationDate.split('T')[0] : '',
                  site: i.site || '',
                  route: i.route || '',
                  notes: i.notes || '',
                })) || prev.immunizations,
                surgeries: stepData.surgeries?.map((s: any) => ({
                  surgeryType: s.surgeryType || '',
                  surgeryDate: s.surgeryDate ? s.surgeryDate.split('T')[0] : '',
                  notes: s.notes || '',
                })) || prev.surgeries,
                hospitalizations: stepData.hospitalizations?.map((h: any) => ({
                  hospitalName: h.hospitalName || '',
                  reason: h.reason || '',
                  admissionDate: h.admissionDate ? h.admissionDate.split('T')[0] : '',
                  dischargeDate: h.dischargeDate ? h.dischargeDate.split('T')[0] : '',
                  notes: h.notes || '',
                })) || prev.hospitalizations,
                familyMedicalHistory: stepData.familyMedicalHistory?.map((f: any) => ({
                  relative: f.relative || '',
                  condition: f.condition || '',
                  notes: f.notes || '',
                })) || prev.familyMedicalHistory,
              };
            case 'insurance':
              return {
                ...prev,
                insurancePolicies: stepData.insurancePolicies?.map((ip: any) => ({
                  insuranceCompany: ip.insuranceCompany || '',
                  policyNumber: ip.policyNumber || '',
                  groupNumber: ip.groupNumber || '',
                  policyHolderName: ip.policyHolderName || '',
                  policyHolderDob: ip.policyHolderDob ? ip.policyHolderDob.split('T')[0] : '',
                  relationToPolicyholder: ip.relationToPolicyholder || '',
                  isPrimary: ip.isPrimary || false,
                  effectiveDate: ip.effectiveDate ? ip.effectiveDate.split('T')[0] : '',
                  expirationDate: ip.expirationDate ? ip.expirationDate.split('T')[0] : '',
                })) || prev.insurancePolicies
              };
            case 'legalconsents':
              return {
                ...prev,
                legalConsents: {
                  hipaaAgreed: stepData.hipaaAgreed || prev.legalConsents?.hipaaAgreed || false,
                  consentToTreat: stepData.consentToTreat || prev.legalConsents?.consentToTreat || false,
                  advanceDirectives: stepData.advanceDirectives || prev.legalConsents?.advanceDirectives || false,
                  advanceDirectivesPath: stepData.advanceDirectivesPath || prev.legalConsents?.advanceDirectivesPath || '',
                  assignmentOfBenefits: stepData.assignmentOfBenefits || prev.legalConsents?.assignmentOfBenefits || false,
                  financialResponsibility: stepData.financialResponsibility || prev.legalConsents?.financialResponsibility || false,
                  signedOnUtc: stepData.signedOnUtc || prev.legalConsents?.signedOnUtc || '',
                  signaturePath: stepData.signaturePath || prev.legalConsents?.signaturePath || '',
                }
              };
            default:
              return prev;
          }
        });
      }
    } catch (error) {
      console.error(`Error loading ${stepName} data:`, error);
    }
  };

  const handleNext = async () => {
    if (currentStep < totalSteps) {
      setLoading(true);
      await autoSave();
      setLoading(false);
      const nextStep = currentStep + 1;
      setCurrentStep(nextStep);
      // Load data for the next step
      await loadStepData(nextStep);
    }
  };

  const handlePrev = async () => {
    if (currentStep > 1) {
      setLoading(true);
      await autoSave();
      setLoading(false);
      const prevStep = currentStep - 1;
      setCurrentStep(prevStep);
      // Load data for the previous step
      await loadStepData(prevStep);
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage('');

    try {
      // Clean the data before final submit - API expects PatientIntakeDto structure
      const cleanedData = {
        patient: {
          ...formData.patient,
          // Remove signedDate from patient object - it's not part of the patient entity
          signedDate: undefined,
        },
        // TODO: Backend needs to be updated to support familyMembers
        // For now, exclude familyMembers from submission
        familyMembers: undefined,
        // Map emergency contacts to API format
        emergencyContacts: formData.emergencyContacts
          .filter(contact => contact.name && contact.name.trim() !== '' && contact.phone && contact.phone.trim() !== '')
          .map(contact => ({
            name: contact.name,
            relationshipTypeId: contact.relationshipTypeId,
            phone: contact.phone,
            altPhone: contact.altPhone,
          })),
        // Map social history to API format
        socialHistory: formData.socialHistory ? {
          smokingStatusId: formData.socialHistory.smokingStatusId,
          alcoholUseId: formData.socialHistory.alcoholUseId,
          drugUseId: formData.socialHistory.drugUseId,
          occupation: formData.socialHistory.occupation,
          livingSituationId: formData.socialHistory.livingSituationId,
        } : undefined,
        legalConsents: formData.legalConsents ? {
          ...formData.legalConsents,
          signedOnUtc: formData.legalConsents.signedOnUtc || new Date().toISOString(),
          // signaturePath can be the electronic signature
          signaturePath: formData.patient.electronicSignature || '',
        } : {
          hipaaAgreed: false,
          consentToTreat: false,
          advanceDirectives: false,
          advanceDirectivesPath: '',
          assignmentOfBenefits: false,
          financialResponsibility: false,
          signedOnUtc: new Date().toISOString(),
          signaturePath: formData.patient.electronicSignature || '',
        },
        allergies: formData.allergies
          .filter(a => a.allergyName && a.allergyName.trim() !== '') // Only include allergies with names
          .map(a => ({
            AllergenName: a.allergyName,
            AllergenType: a.allergyType || 'Drug',
            Severity: a.severity || 'Mild',
            Reaction: a.reaction || '',
            OnsetDate: a.onsetDate || null,
          })),
        medications: formData.medications
          .filter(m => m.medicationName && m.medicationName.trim() !== '')
          .map(m => ({
          ...m,
          startDate: m.startDate || null,
        })),
        surgeries: formData.surgeries
          .filter(s => s.surgeryType && s.surgeryType.trim() !== '')
          .map(s => ({
          ...s,
          surgeryDate: s.surgeryDate || null,
        })),
        hospitalizations: formData.hospitalizations
          .filter(h => h.hospitalName && h.hospitalName.trim() !== '')
          .map(h => ({
          ...h,
          admissionDate: h.admissionDate || null,
          dischargeDate: h.dischargeDate || null,
        })),
        familyMedicalHistory: formData.familyMedicalHistory
          .filter(f => f.relative && f.relative.trim() !== '' && f.condition && f.condition.trim() !== '')
          .map(f => ({
            ...f,
        })),
      };

      console.log('Submitting payload:', JSON.stringify(cleanedData, null, 2));

      const response = await fetch(`${API_BASE}/api/patients/intake`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(cleanedData),
      });

      if (response.ok) {
        const result = await response.json();
        setMessage(`✓ Success! All information saved. Patient ID: ${result.patientId}`);
        // Optionally redirect to profile or dashboard after successful save
        setTimeout(() => {
          window.location.href = '/profile';
        }, 2000);
      } else {
        const error = await response.json();
        console.error('Submit error:', error);
        setMessage(`Error: ${error.title || 'Failed to save data'}\n${JSON.stringify(error.errors || {})}`);
      }
    } catch (err) {
      console.error('Submit error:', err);
      setMessage(`Error: ${err}`);
    } finally {
      setLoading(false);
    }
  };

  const renderStep = () => {
    switch (currentStep) {
      case 1:
        return <PersonalInformation formData={formData} setFormData={setFormData} />;
      case 2:
        return <ContactInformation formData={formData} setFormData={setFormData} />;
      case 3:
        return <EmergencyContacts formData={formData} setFormData={setFormData} />;
      case 4:
        return <FamilyMembers formData={formData} setFormData={setFormData} />;
      case 5:
        return <InsuranceInformation formData={formData} setFormData={setFormData} />;
      case 6:
        return <ProviderInformation formData={formData} setFormData={setFormData} />;
      case 7:
        return <MedicalHistory formData={formData} setFormData={setFormData} />;
      case 8:
        return <SocialHistorySection formData={formData} setFormData={setFormData} />;
      case 9:
        return <AccessibilityNeeds formData={formData} setFormData={setFormData} />;
      case 10:
        return <LegalConsents formData={formData} setFormData={setFormData} />;
      case 11:
        return <Review formData={formData} />;
      default:
        return null;
    }
  };

  // Show loading spinner while data is being fetched
  if (loadingData) {
    return (
      <div className="max-w-4xl mx-auto p-6">
        <div className="flex flex-col items-center justify-center min-h-[400px]">
          <div className="animate-spin rounded-full h-16 w-16 border-b-4 border-blue-600 mb-4"></div>
          <p className="text-gray-600 text-lg">Loading your information...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-900 mb-6">Patient Intake Form</h1>
      
      {/* Progress Bar */}
      <div className="mb-8">
        <div className="flex justify-between mb-2">
          {[...Array(totalSteps)].map((_, i) => (
            <div
              key={i}
              className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-semibold ${
                i + 1 <= currentStep ? 'bg-blue-600 text-white' : 'bg-gray-300 text-gray-600'
              }`}
            >
              {i + 1}
            </div>
          ))}
        </div>
        <div className="text-sm text-gray-600 text-center">
          Step {currentStep} of {totalSteps}
        </div>
      </div>

      <form onSubmit={handleSubmit}>
        {renderStep()}

        {/* Navigation Buttons */}
        <div className="flex justify-between mt-8">
          <button
            type="button"
            onClick={handlePrev}
            disabled={currentStep === 1}
            className="px-6 py-2 bg-gray-300 text-gray-700 rounded-lg disabled:opacity-50"
          >
            Previous
          </button>
          
          {currentStep < totalSteps ? (
            <button
              type="button"
              onClick={handleNext}
              disabled={loading}
              className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 flex items-center gap-2"
            >
              {loading ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                  Saving...
                </>
              ) : (
                'Save & Next'
              )}
            </button>
          ) : (
            <button
              type="submit"
              disabled={loading}
              className="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 flex items-center gap-2"
            >
              {loading ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                  Saving...
                </>
              ) : (
                'Save & Complete'
              )}
            </button>
          )}
        </div>

        {message && (
          <div className={`mt-4 p-4 rounded-lg flex items-center gap-2 ${
            message.includes('✓') || message.includes('Success') 
              ? 'bg-green-100 text-green-800 border border-green-300' 
              : message.includes('⚠') 
              ? 'bg-yellow-100 text-yellow-800 border border-yellow-300' 
              : 'bg-red-100 text-red-800 border border-red-300'
          }`}>
            {message.includes('✓') && (
              <svg className="w-5 h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            )}
            {message.includes('⚠') && (
              <svg className="w-5 h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
            )}
            <span>{message}</span>
          </div>
        )}
      </form>
    </div>
  );
}

// Step 1: Personal Information
function PersonalInformation({ formData, setFormData }: any) {
  const updatePatient = (field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      patient: { ...prev.patient, [field]: value },
    }));
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Personal Information</h2>
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <input
          type="text"
          placeholder="First Name *"
          required
          value={formData.patient.firstName}
          onChange={(e) => updatePatient('firstName', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Middle Name"
          value={formData.patient.middleName || ''}
          onChange={(e) => updatePatient('middleName', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Last Name *"
          required
          value={formData.patient.lastName}
          onChange={(e) => updatePatient('lastName', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Suffix (Jr., Sr., III)"
          value={formData.patient.suffix || ''}
          onChange={(e) => updatePatient('suffix', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="date"
          placeholder="Date of Birth *"
          required
          value={formData.patient.dateOfBirth}
          onChange={(e) => updatePatient('dateOfBirth', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <select
          value={formData.patient.gender}
          onChange={(e) => updatePatient('gender', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        >
          <option value="Male">Male</option>
          <option value="Female">Female</option>
          <option value="Other">Other</option>
          <option value="Unknown">Prefer not to say</option>
        </select>
        <input
          type="text"
          placeholder="Social Security Number (optional)"
          value={formData.patient.ssnEncrypted || ''}
          onChange={(e) => updatePatient('ssnEncrypted', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <select
          value={formData.patient.maritalStatus || ''}
          onChange={(e) => updatePatient('maritalStatus', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        >
          <option value="">Marital Status</option>
          <option value="Single">Single</option>
          <option value="Married">Married</option>
          <option value="Divorced">Divorced</option>
          <option value="Widowed">Widowed</option>
          <option value="Other">Other</option>
        </select>
        <input
          type="text"
          placeholder="Race/Ethnicity"
          value={formData.patient.race || ''}
          onChange={(e) => updatePatient('race', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Ethnicity"
          value={formData.patient.ethnicity || ''}
          onChange={(e) => updatePatient('ethnicity', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Preferred Language"
          value={formData.patient.preferredLanguage || ''}
          onChange={(e) => updatePatient('preferredLanguage', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
      </div>
    </div>
  );
}

// Step 2: Contact Information
function ContactInformation({ formData, setFormData }: any) {
  const updatePatient = (field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      patient: { ...prev.patient, [field]: value },
    }));
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Contact Information</h2>
      
      <div className="grid grid-cols-1 gap-4">
        <input
          type="text"
          placeholder="Address Line 1 *"
          required
          value={formData.patient.addressLine1}
          onChange={(e) => updatePatient('addressLine1', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Address Line 2"
          value={formData.patient.addressLine2 || ''}
          onChange={(e) => updatePatient('addressLine2', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <input
            type="text"
            placeholder="City *"
            required
            value={formData.patient.city}
            onChange={(e) => updatePatient('city', e.target.value)}
            className="border border-gray-300 rounded-lg p-3"
          />
          <input
            type="text"
            placeholder="State *"
            required
            value={formData.patient.state}
            onChange={(e) => updatePatient('state', e.target.value)}
            className="border border-gray-300 rounded-lg p-3"
          />
          <input
            type="text"
            placeholder="ZIP Code *"
            required
            value={formData.patient.zipCode}
            onChange={(e) => updatePatient('zipCode', e.target.value)}
            className="border border-gray-300 rounded-lg p-3"
          />
        </div>
        <input
          type="email"
          placeholder="Email Address *"
          required
          value={formData.patient.email}
          onChange={(e) => updatePatient('email', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="tel"
          placeholder="Primary Phone Number *"
          required
          value={formData.patient.phoneNumber}
          onChange={(e) => updatePatient('phoneNumber', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="tel"
          placeholder="Alternate Phone Number"
          value={formData.patient.alternatePhone || ''}
          onChange={(e) => updatePatient('alternatePhone', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
      </div>
    </div>
  );
}

// Step 3: Emergency Contacts
function EmergencyContacts({ formData, setFormData }: any) {
  const addContact = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      emergencyContacts: [...prev.emergencyContacts, {
        name: '',
        relationshipTypeId: 0,
        phone: '',
        altPhone: '',
      }],
    }));
  };

  const updateContact = (index: number, field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      emergencyContacts: prev.emergencyContacts.map((contact, i) =>
        i === index ? { ...contact, [field]: value } : contact
      ),
    }));
  };

  const removeContact = (index: number) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      emergencyContacts: prev.emergencyContacts.filter((_, i) => i !== index),
    }));
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Emergency Contacts</h2>
      
      {formData.emergencyContacts.map((contact: EmergencyContactDto, index: number) => (
        <div key={index} className="border border-gray-300 rounded-lg p-4 space-y-3">
          <div className="flex justify-between items-center">
            <h3 className="font-semibold text-gray-700">Contact {index + 1}</h3>
            <button
              type="button"
              onClick={() => removeContact(index)}
              className="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
          <input
            type="text"
            placeholder="Contact Name *"
            required
            value={contact.name}
            onChange={(e) => updateContact(index, 'name', e.target.value)}
            className="border border-gray-300 rounded-lg p-3 w-full"
          />
          <select
            value={contact.relationshipTypeId}
            onChange={(e) => updateContact(index, 'relationshipTypeId', e.target.value ? parseInt(e.target.value) : 0)}
            className="border border-gray-300 rounded-lg p-3 w-full"
            required
          >
            <option value="">Select Relationship *</option>
            <option value="1">Spouse</option>
            <option value="2">Parent</option>
            <option value="3">Child</option>
            <option value="4">Sibling</option>
            <option value="5">Friend</option>
            <option value="6">Other</option>
          </select>
          <input
            type="tel"
            placeholder="Phone Number *"
            required
            value={contact.phone}
            onChange={(e) => updateContact(index, 'phone', e.target.value)}
            className="border border-gray-300 rounded-lg p-3 w-full"
          />
          <input
            type="tel"
            placeholder="Alternate Phone Number"
            value={contact.altPhone || ''}
            onChange={(e) => updateContact(index, 'altPhone', e.target.value)}
            className="border border-gray-300 rounded-lg p-3 w-full"
          />
        </div>
      ))}
      
      <button
        type="button"
        onClick={addContact}
        className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        + Add Emergency Contact
      </button>
    </div>
  );
}

// Step 4: Family Members
function FamilyMembers({ formData, setFormData }: any) {
  const addFamilyMember = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      familyMembers: [...prev.familyMembers, {
        firstName: '',
        lastName: '',
        dateOfBirth: '',
        relationship: '',
        phoneNumber: '',
        email: '',
        address: '',
      }],
    }));
  };

  const updateFamilyMember = (index: number, field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      familyMembers: prev.familyMembers.map((member, i) =>
        i === index ? { ...member, [field]: value } : member
      ),
    }));
  };

  const removeFamilyMember = (index: number) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      familyMembers: prev.familyMembers.filter((_, i) => i !== index),
    }));
  };

  return (
    <div className="space-y-4">
      <div className="mb-4">
        <h2 className="text-2xl font-semibold text-gray-800 mb-2">Family Members</h2>
        <p className="text-sm text-gray-600">
          Add family members who may need care or should have access to your records. 
          You can skip this step and add family members later from the Family tab.
        </p>
      </div>
      
      {formData.familyMembers.map((member: FamilyMemberDto, index: number) => (
        <div key={index} className="border-2 border-blue-200 rounded-lg p-4 bg-blue-50 space-y-3">
          <div className="flex justify-between items-center mb-3">
            <h3 className="font-semibold text-gray-800 text-lg">
              Family Member {index + 1}
            </h3>
            <button
              type="button"
              onClick={() => removeFamilyMember(index)}
              className="text-red-600 hover:text-red-800 font-semibold px-3 py-1 rounded hover:bg-red-100"
            >
              Remove
            </button>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <input
              type="text"
              placeholder="First Name *"
              required
              value={member.firstName}
              onChange={(e) => updateFamilyMember(index, 'firstName', e.target.value)}
              className="border border-gray-300 rounded-lg p-3 w-full bg-white"
            />
            <input
              type="text"
              placeholder="Last Name *"
              required
              value={member.lastName}
              onChange={(e) => updateFamilyMember(index, 'lastName', e.target.value)}
              className="border border-gray-300 rounded-lg p-3 w-full bg-white"
            />
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <input
              type="date"
              placeholder="Date of Birth"
              value={member.dateOfBirth || ''}
              onChange={(e) => updateFamilyMember(index, 'dateOfBirth', e.target.value)}
              className="border border-gray-300 rounded-lg p-3 w-full bg-white"
            />
            <select
              value={member.relationship}
              onChange={(e) => updateFamilyMember(index, 'relationship', e.target.value)}
              className="border border-gray-300 rounded-lg p-3 w-full bg-white"
              required
            >
              <option value="">Select Relationship *</option>
              <option value="Spouse">Spouse</option>
              <option value="Partner">Partner</option>
              <option value="Parent">Parent</option>
              <option value="Child">Child</option>
              <option value="Sibling">Sibling</option>
              <option value="Grandparent">Grandparent</option>
              <option value="Grandchild">Grandchild</option>
              <option value="Other">Other</option>
            </select>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <input
              type="tel"
              placeholder="Phone Number"
              value={member.phoneNumber || ''}
              onChange={(e) => updateFamilyMember(index, 'phoneNumber', e.target.value)}
              className="border border-gray-300 rounded-lg p-3 w-full bg-white"
            />
            <input
              type="email"
              placeholder="Email Address"
              value={member.email || ''}
              onChange={(e) => updateFamilyMember(index, 'email', e.target.value)}
              className="border border-gray-300 rounded-lg p-3 w-full bg-white"
            />
          </div>
          
          <textarea
            placeholder="Address (Optional)"
            value={member.address || ''}
            onChange={(e) => updateFamilyMember(index, 'address', e.target.value)}
            className="border border-gray-300 rounded-lg p-3 w-full bg-white"
            rows={2}
          />
          
          <div className="mt-3 p-3 bg-white rounded border border-blue-200">
            <p className="text-sm text-gray-600">
              💡 <strong>Note:</strong> For comprehensive health information for this family member, 
              use the <strong>Family</strong> tab after completing this form.
            </p>
          </div>
        </div>
      ))}
      
      <button
        type="button"
        onClick={addFamilyMember}
        className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 font-semibold"
      >
        + Add Family Member
      </button>
      
      {formData.familyMembers.length === 0 && (
        <div className="text-center py-8 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300">
          <p className="text-gray-600 mb-3">No family members added yet</p>
          <p className="text-sm text-gray-500">Click "Add Family Member" to add family information</p>
        </div>
      )}
    </div>
  );
}

// Step 5: Insurance Information (was Step 4)
function InsuranceInformation({ formData, setFormData }: any) {
  const addInsurance = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      insurancePolicies: [...prev.insurancePolicies, {
        insuranceCompany: '',
        policyNumber: '',
        policyHolderName: '',
        isPrimary: prev.insurancePolicies.length === 0,
      }],
    }));
  };

  const updateInsurance = (index: number, field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      insurancePolicies: prev.insurancePolicies.map((ins, i) =>
        i === index ? { ...ins, [field]: value } : ins
      ),
    }));
  };

  const removeInsurance = (index: number) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      insurancePolicies: prev.insurancePolicies.filter((_, i) => i !== index),
    }));
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Insurance Information</h2>
      
      {formData.insurancePolicies.map((insurance: InsurancePolicyDto, index: number) => (
        <div key={index} className="border border-gray-300 rounded-lg p-4 space-y-3">
          <div className="flex justify-between items-center">
            <h3 className="font-semibold text-gray-700">
              {insurance.isPrimary ? 'Primary Insurance' : `Secondary Insurance ${index}`}
            </h3>
            <button
              type="button"
              onClick={() => removeInsurance(index)}
              className="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
          <input
            type="text"
            placeholder="Insurance Company *"
            required
            value={insurance.insuranceCompany}
            onChange={(e) => updateInsurance(index, 'insuranceCompany', e.target.value)}
            className="border border-gray-300 rounded-lg p-3 w-full"
          />
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <input
              type="text"
              placeholder="Policy Number *"
              required
              value={insurance.policyNumber}
              onChange={(e) => updateInsurance(index, 'policyNumber', e.target.value)}
              className="border border-gray-300 rounded-lg p-3"
            />
            <input
              type="text"
              placeholder="Group Number"
              value={insurance.groupNumber || ''}
              onChange={(e) => updateInsurance(index, 'groupNumber', e.target.value)}
              className="border border-gray-300 rounded-lg p-3"
            />
          </div>
          <input
            type="text"
            placeholder="Policy Holder Name *"
            required
            value={insurance.policyHolderName}
            onChange={(e) => updateInsurance(index, 'policyHolderName', e.target.value)}
            className="border border-gray-300 rounded-lg p-3 w-full"
          />
          <input
            type="tel"
            placeholder="Insurance Phone Number"
            value={insurance.insurancePhoneNumber || ''}
            onChange={(e) => updateInsurance(index, 'insurancePhoneNumber', e.target.value)}
            className="border border-gray-300 rounded-lg p-3 w-full"
          />
        </div>
      ))}
      
      <button
        type="button"
        onClick={addInsurance}
        className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        + Add Insurance Policy
      </button>
    </div>
  );
}

// Step 6: Provider Information (was Step 5)
function ProviderInformation({ formData, setFormData }: any) {
  const updatePatient = (field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      patient: { ...prev.patient, [field]: value },
    }));
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Provider & Pharmacy Information</h2>
      
      <div className="grid grid-cols-1 gap-4">
        <input
          type="text"
          placeholder="Primary Care Physician"
          value={formData.patient.primaryCarePhysician || ''}
          onChange={(e) => updatePatient('primaryCarePhysician', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="tel"
          placeholder="PCP Phone Number"
          value={formData.patient.pcpPhoneNumber || ''}
          onChange={(e) => updatePatient('pcpPhoneNumber', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Referring Physician"
          value={formData.patient.referringPhysician || ''}
          onChange={(e) => updatePatient('referringPhysician', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Preferred Pharmacy Name"
          value={formData.patient.preferredPharmacyName || ''}
          onChange={(e) => updatePatient('preferredPharmacyName', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
        <input
          type="text"
          placeholder="Preferred Pharmacy Location"
          value={formData.patient.preferredPharmacyLocation || ''}
          onChange={(e) => updatePatient('preferredPharmacyLocation', e.target.value)}
          className="border border-gray-300 rounded-lg p-3"
        />
      </div>
    </div>
  );
}

// Step 7: Medical History (was Step 6)
function MedicalHistory({ formData, setFormData }: any) {
  const addAllergy = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      allergies: [...prev.allergies, { allergyName: '', allergyType: 'Drug', reaction: '', severity: 'Mild' }],
    }));
  };

  const addMedication = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      medications: [...prev.medications, { medicationName: '', dosage: '', frequency: '' }],
    }));
  };

  const addChronicCondition = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      chronicConditions: [...prev.chronicConditions, { conditionName: '', diagnosedDate: '', status: 'Active', notes: '' }],
    }));
  };

  const addImmunization = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      immunizations: [...prev.immunizations, { vaccineName: '', administeredDate: '', doseNumber: undefined, provider: '', site: '', route: '', notes: '' }],
    }));
  };

  const addSurgery = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      surgeries: [...prev.surgeries, { surgeryType: '', surgeryDate: '', notes: '' }],
    }));
  };

  const addHospitalization = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      hospitalizations: [...prev.hospitalizations, { hospitalName: '', reason: '', admissionDate: '' }],
    }));
  };

  const addFamilyHistory = () => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      familyMedicalHistory: [...prev.familyMedicalHistory, { relative: '', condition: '', notes: '' }],
    }));
  };

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Medical History</h2>
      
      {/* Allergies */}
      <div>
        <h3 className="font-semibold text-gray-700 mb-2">Allergies</h3>
        {formData.allergies.map((allergy: AllergyDto, index: number) => (
          <div key={index} className="border border-gray-300 rounded-lg p-3 mb-2 grid grid-cols-1 md:grid-cols-4 gap-2">
            <input
              type="text"
              placeholder="Allergy Name"
              value={allergy.allergyName}
              onChange={(e) => {
                const updated = [...formData.allergies];
                updated[index].allergyName = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, allergies: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <select
              value={allergy.allergyType}
              onChange={(e) => {
                const updated = [...formData.allergies];
                updated[index].allergyType = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, allergies: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            >
              <option value="Drug">Drug</option>
              <option value="Food">Food</option>
              <option value="Environmental">Environmental</option>
              <option value="Other">Other</option>
            </select>
            <input
              type="text"
              placeholder="Reaction"
              value={allergy.reaction || ''}
              onChange={(e) => {
                const updated = [...formData.allergies];
                updated[index].reaction = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, allergies: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <button
              type="button"
              onClick={() => setFormData((prev: PatientIntakeDto) => ({
                ...prev,
                allergies: prev.allergies.filter((_, i) => i !== index),
              }))}
              className="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
        ))}
        <button
          type="button"
          onClick={addAllergy}
          className="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
        >
          + Add Allergy
        </button>
      </div>

      {/* Medications */}
      <div>
        <h3 className="font-semibold text-gray-700 mb-2">Current Medications</h3>
        {formData.medications.map((medication: MedicationDto, index: number) => (
          <div key={index} className="border border-gray-300 rounded-lg p-3 mb-2 grid grid-cols-1 md:grid-cols-4 gap-2">
            <input
              type="text"
              placeholder="Medication Name"
              value={medication.medicationName}
              onChange={(e) => {
                const updated = [...formData.medications];
                updated[index].medicationName = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, medications: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="text"
              placeholder="Dosage"
              value={medication.dosage || ''}
              onChange={(e) => {
                const updated = [...formData.medications];
                updated[index].dosage = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, medications: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="text"
              placeholder="Frequency"
              value={medication.frequency || ''}
              onChange={(e) => {
                const updated = [...formData.medications];
                updated[index].frequency = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, medications: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <button
              type="button"
              onClick={() => setFormData((prev: PatientIntakeDto) => ({
                ...prev,
                medications: prev.medications.filter((_, i) => i !== index),
              }))}
              className="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
        ))}
        <button
          type="button"
          onClick={addMedication}
          className="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
        >
          + Add Medication
        </button>
      </div>

      {/* Chronic Conditions */}
      <div>
        <h3 className="font-semibold text-gray-700 mb-2">Chronic Conditions</h3>
        {formData.chronicConditions.map((condition: ChronicConditionDto, index: number) => (
          <div key={index} className="border border-gray-300 rounded-lg p-3 mb-2 grid grid-cols-1 md:grid-cols-5 gap-2">
            <input
              type="text"
              placeholder="Condition Name"
              value={condition.conditionName}
              onChange={(e) => {
                const updated = [...formData.chronicConditions];
                updated[index].conditionName = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, chronicConditions: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="date"
              placeholder="Diagnosed Date"
              value={condition.diagnosedDate || ''}
              onChange={(e) => {
                const updated = [...formData.chronicConditions];
                updated[index].diagnosedDate = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, chronicConditions: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <select
              value={condition.status || 'Active'}
              onChange={(e) => {
                const updated = [...formData.chronicConditions];
                updated[index].status = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, chronicConditions: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            >
              <option value="Active">Active</option>
              <option value="Inactive">Inactive</option>
              <option value="Resolved">Resolved</option>
            </select>
            <input
              type="text"
              placeholder="Notes"
              value={condition.notes || ''}
              onChange={(e) => {
                const updated = [...formData.chronicConditions];
                updated[index].notes = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, chronicConditions: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <button
              type="button"
              onClick={() => setFormData((prev: PatientIntakeDto) => ({
                ...prev,
                chronicConditions: prev.chronicConditions.filter((_, i) => i !== index),
              }))}
              className="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
        ))}
        <button
          type="button"
          onClick={addChronicCondition}
          className="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
        >
          + Add Chronic Condition
        </button>
      </div>

      {/* Immunizations */}
      <div>
        <h3 className="font-semibold text-gray-700 mb-2">Immunizations</h3>
        {formData.immunizations.map((immunization: ImmunizationDto, index: number) => (
          <div key={index} className="border border-gray-300 rounded-lg p-3 mb-2">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-2 mb-2">
              <input
                type="text"
                placeholder="Vaccine Name"
                value={immunization.vaccineName}
                onChange={(e) => {
                  const updated = [...formData.immunizations];
                  updated[index].vaccineName = e.target.value;
                  setFormData((prev: PatientIntakeDto) => ({ ...prev, immunizations: updated }));
                }}
                className="border border-gray-300 rounded p-2"
              />
              <input
                type="date"
                placeholder="Administered Date"
                value={immunization.administeredDate}
                onChange={(e) => {
                  const updated = [...formData.immunizations];
                  updated[index].administeredDate = e.target.value;
                  setFormData((prev: PatientIntakeDto) => ({ ...prev, immunizations: updated }));
                }}
                className="border border-gray-300 rounded p-2"
              />
              <input
                type="number"
                placeholder="Dose Number"
                value={immunization.doseNumber || ''}
                onChange={(e) => {
                  const updated = [...formData.immunizations];
                  updated[index].doseNumber = e.target.value ? parseInt(e.target.value) : undefined;
                  setFormData((prev: PatientIntakeDto) => ({ ...prev, immunizations: updated }));
                }}
                className="border border-gray-300 rounded p-2"
              />
            </div>
            <div className="grid grid-cols-1 md:grid-cols-4 gap-2 mb-2">
              <input
                type="text"
                placeholder="Provider"
                value={immunization.provider || ''}
                onChange={(e) => {
                  const updated = [...formData.immunizations];
                  updated[index].provider = e.target.value;
                  setFormData((prev: PatientIntakeDto) => ({ ...prev, immunizations: updated }));
                }}
                className="border border-gray-300 rounded p-2"
              />
              <input
                type="text"
                placeholder="Lot Number"
                value={immunization.lotNumber || ''}
                onChange={(e) => {
                  const updated = [...formData.immunizations];
                  updated[index].lotNumber = e.target.value;
                  setFormData((prev: PatientIntakeDto) => ({ ...prev, immunizations: updated }));
                }}
                className="border border-gray-300 rounded p-2"
              />
              <input
                type="text"
                placeholder="Site (e.g., Left deltoid)"
                value={immunization.site || ''}
                onChange={(e) => {
                  const updated = [...formData.immunizations];
                  updated[index].site = e.target.value;
                  setFormData((prev: PatientIntakeDto) => ({ ...prev, immunizations: updated }));
                }}
                className="border border-gray-300 rounded p-2"
              />
              <input
                type="text"
                placeholder="Route (e.g., Intramuscular)"
                value={immunization.route || ''}
                onChange={(e) => {
                  const updated = [...formData.immunizations];
                  updated[index].route = e.target.value;
                  setFormData((prev: PatientIntakeDto) => ({ ...prev, immunizations: updated }));
                }}
                className="border border-gray-300 rounded p-2"
              />
            </div>
            <div className="grid grid-cols-1 gap-2">
              <textarea
                placeholder="Notes"
                value={immunization.notes || ''}
                onChange={(e) => {
                  const updated = [...formData.immunizations];
                  updated[index].notes = e.target.value;
                  setFormData((prev: PatientIntakeDto) => ({ ...prev, immunizations: updated }));
                }}
                className="border border-gray-300 rounded p-2"
                rows={2}
              />
              <button
                type="button"
                onClick={() => setFormData((prev: PatientIntakeDto) => ({
                  ...prev,
                  immunizations: prev.immunizations.filter((_, i) => i !== index),
                }))}
                className="text-red-600 hover:text-red-800 text-left"
              >
                Remove
              </button>
            </div>
          </div>
        ))}
        <button
          type="button"
          onClick={addImmunization}
          className="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
        >
          + Add Immunization
        </button>
      </div>

      {/* Surgeries */}
      <div>
        <h3 className="font-semibold text-gray-700 mb-2">Past Surgeries</h3>
        {formData.surgeries.map((surgery: SurgeryDto, index: number) => (
          <div key={index} className="border border-gray-300 rounded-lg p-3 mb-2 grid grid-cols-1 md:grid-cols-4 gap-2">
            <input
              type="text"
              placeholder="Surgery Type"
              value={surgery.surgeryType}
              onChange={(e) => {
                const updated = [...formData.surgeries];
                updated[index].surgeryType = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, surgeries: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="date"
              placeholder="Surgery Date"
              value={surgery.surgeryDate || ''}
              onChange={(e) => {
                const updated = [...formData.surgeries];
                updated[index].surgeryDate = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, surgeries: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="text"
              placeholder="Notes"
              value={surgery.notes || ''}
              onChange={(e) => {
                const updated = [...formData.surgeries];
                updated[index].notes = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, surgeries: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <button
              type="button"
              onClick={() => setFormData((prev: PatientIntakeDto) => ({
                ...prev,
                surgeries: prev.surgeries.filter((_, i) => i !== index),
              }))}
              className="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
        ))}
        <button
          type="button"
          onClick={addSurgery}
          className="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
        >
          + Add Surgery
        </button>
      </div>

      {/* Hospitalizations */}
      <div>
        <h3 className="font-semibold text-gray-700 mb-2">Past Hospitalizations</h3>
        {formData.hospitalizations.map((hosp: HospitalizationDto, index: number) => (
          <div key={index} className="border border-gray-300 rounded-lg p-3 mb-2 grid grid-cols-1 md:grid-cols-4 gap-2">
            <input
              type="text"
              placeholder="Hospital Name"
              value={hosp.hospitalName || ''}
              onChange={(e) => {
                const updated = [...formData.hospitalizations];
                updated[index].hospitalName = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, hospitalizations: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="text"
              placeholder="Reason"
              value={hosp.reason || ''}
              onChange={(e) => {
                const updated = [...formData.hospitalizations];
                updated[index].reason = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, hospitalizations: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="date"
              placeholder="Admission Date"
              value={hosp.admissionDate || ''}
              onChange={(e) => {
                const updated = [...formData.hospitalizations];
                updated[index].admissionDate = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, hospitalizations: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <button
              type="button"
              onClick={() => setFormData((prev: PatientIntakeDto) => ({
                ...prev,
                hospitalizations: prev.hospitalizations.filter((_, i) => i !== index),
              }))}
              className="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
        ))}
        <button
          type="button"
          onClick={addHospitalization}
          className="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
        >
          + Add Hospitalization
        </button>
      </div>

      {/* Family Medical History */}
      <div>
        <h3 className="font-semibold text-gray-700 mb-2">Family Medical History</h3>
        {formData.familyMedicalHistory.map((fam: FamilyHistoryDto, index: number) => (
          <div key={index} className="border border-gray-300 rounded-lg p-3 mb-2 grid grid-cols-1 md:grid-cols-4 gap-2">
            <input
              type="text"
              placeholder="Relative"
              value={fam.relative}
              onChange={(e) => {
                const updated = [...formData.familyMedicalHistory];
                updated[index].relative = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, familyMedicalHistory: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="text"
              placeholder="Condition"
              value={fam.condition}
              onChange={(e) => {
                const updated = [...formData.familyMedicalHistory];
                updated[index].condition = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, familyMedicalHistory: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <input
              type="text"
              placeholder="Notes"
              value={fam.notes || ''}
              onChange={(e) => {
                const updated = [...formData.familyMedicalHistory];
                updated[index].notes = e.target.value;
                setFormData((prev: PatientIntakeDto) => ({ ...prev, familyMedicalHistory: updated }));
              }}
              className="border border-gray-300 rounded p-2"
            />
            <button
              type="button"
              onClick={() => setFormData((prev: PatientIntakeDto) => ({
                ...prev,
                familyMedicalHistory: prev.familyMedicalHistory.filter((_, i) => i !== index),
              }))}
              className="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
        ))}
        <button
          type="button"
          onClick={addFamilyHistory}
          className="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
        >
          + Add Family History
        </button>
      </div>
    </div>
  );
}

// Step 8: Social History (was Step 7)
function SocialHistorySection({ formData, setFormData }: any) {
  const updateSocialHistory = (field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      socialHistory: { ...prev.socialHistory, [field]: value },
    }));
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Social History</h2>
      
      <select
        value={formData.socialHistory?.smokingStatusId || ''}
        onChange={(e) => updateSocialHistory('smokingStatusId', e.target.value ? parseInt(e.target.value) : null)}
        className="border border-gray-300 rounded-lg p-3 w-full"
      >
        <option value="">Smoking Status</option>
        <option value="1">Never</option>
        <option value="2">Current</option>
        <option value="3">Former</option>
        <option value="4">Unknown</option>
      </select>

      <select
        value={formData.socialHistory?.alcoholUseId || ''}
        onChange={(e) => updateSocialHistory('alcoholUseId', e.target.value ? parseInt(e.target.value) : null)}
        className="border border-gray-300 rounded-lg p-3 w-full"
      >
        <option value="">Alcohol Use</option>
        <option value="1">None</option>
        <option value="2">Occasional</option>
        <option value="3">Moderate</option>
        <option value="4">Heavy</option>
      </select>

      <select
        value={formData.socialHistory?.drugUseId || ''}
        onChange={(e) => updateSocialHistory('drugUseId', e.target.value ? parseInt(e.target.value) : null)}
        className="border border-gray-300 rounded-lg p-3 w-full"
      >
        <option value="">Drug Use</option>
        <option value="1">None</option>
        <option value="2">Prescription</option>
        <option value="3">Recreational</option>
      </select>

      <input
        type="text"
        placeholder="Occupation"
        value={formData.socialHistory?.occupation || ''}
        onChange={(e) => updateSocialHistory('occupation', e.target.value)}
        className="border border-gray-300 rounded-lg p-3 w-full"
      />

      <select
        value={formData.socialHistory?.livingSituationId || ''}
        onChange={(e) => updateSocialHistory('livingSituationId', e.target.value ? parseInt(e.target.value) : null)}
        className="border border-gray-300 rounded-lg p-3 w-full"
      >
        <option value="">Living Situation</option>
        <option value="1">Alone</option>
        <option value="2">With Family</option>
        <option value="3">Assisted Living</option>
        <option value="4">Other</option>
      </select>
    </div>
  );
}

// Step 9: Accessibility Needs (was Step 8)
function AccessibilityNeeds({ formData, setFormData }: any) {
  const updatePatient = (field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      patient: { ...prev.patient, [field]: value },
    }));
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Accessibility Needs & Preferences</h2>
      
      <div className="space-y-3">
        <label className="flex items-center space-x-3">
          <input
            type="checkbox"
            checked={formData.patient.interpreterRequired}
            onChange={(e) => updatePatient('interpreterRequired', e.target.checked)}
            className="w-5 h-5"
          />
          <span>Interpreter Required</span>
        </label>

        <label className="flex items-center space-x-3">
          <input
            type="checkbox"
            checked={formData.patient.mobilityAssistance}
            onChange={(e) => updatePatient('mobilityAssistance', e.target.checked)}
            className="w-5 h-5"
          />
          <span>Mobility Assistance Needed</span>
        </label>
      </div>

      <textarea
        placeholder="Religious/Cultural Considerations"
        value={formData.patient.religiousConsiderations || ''}
        onChange={(e) => updatePatient('religiousConsiderations', e.target.value)}
        className="border border-gray-300 rounded-lg p-3 w-full h-24"
      />
    </div>
  );
}

// Step 10: Legal Consents (was Step 9)
function LegalConsents({ formData, setFormData }: any) {
  const updateConsent = (field: string, value: any) => {
    setFormData((prev: PatientIntakeDto) => ({
      ...prev,
      legalConsents: { ...prev.legalConsents, [field]: value },
    }));
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Legal Consents</h2>
      
      <div className="space-y-3">
        <label className="flex items-center space-x-3">
          <input
            type="checkbox"
            checked={formData.legalConsents?.hipaaAgreed || false}
            onChange={(e) => updateConsent('hipaaAgreed', e.target.checked)}
            className="w-5 h-5"
            required
          />
          <span>I agree to the HIPAA Privacy Agreement *</span>
        </label>

        <label className="flex items-center space-x-3">
          <input
            type="checkbox"
            checked={formData.legalConsents?.consentToTreat || false}
            onChange={(e) => updateConsent('consentToTreat', e.target.checked)}
            className="w-5 h-5"
            required
          />
          <span>I consent to treatment *</span>
        </label>

        <label className="flex items-center space-x-3">
          <input
            type="checkbox"
            checked={formData.legalConsents?.advanceDirectives || false}
            onChange={(e) => updateConsent('advanceDirectives', e.target.checked)}
            className="w-5 h-5"
          />
          <span>I have Advance Directives</span>
        </label>

        <label className="flex items-center space-x-3">
          <input
            type="checkbox"
            checked={formData.legalConsents?.assignmentOfBenefits || false}
            onChange={(e) => updateConsent('assignmentOfBenefits', e.target.checked)}
            className="w-5 h-5"
          />
          <span>I agree to Assignment of Benefits</span>
        </label>

        <label className="flex items-center space-x-3">
          <input
            type="checkbox"
            checked={formData.legalConsents?.financialResponsibility || false}
            onChange={(e) => updateConsent('financialResponsibility', e.target.checked)}
            className="w-5 h-5"
            required
          />
          <span>I acknowledge Financial Responsibility *</span>
        </label>
      </div>

      <div className="mt-6">
        <label className="block text-sm font-medium text-gray-700 mb-2">Electronic Signature *</label>
        <input
          type="text"
          placeholder="Type your full name to sign"
          required
          value={formData.patient.electronicSignature || ''}
          onChange={(e) => {
            setFormData((prev: PatientIntakeDto) => ({
              ...prev,
              patient: { ...prev.patient, electronicSignature: e.target.value },
              legalConsents: { 
                ...prev.legalConsents, 
                signedOnUtc: new Date().toISOString(),
                signaturePath: e.target.value 
              },
            }));
          }}
          className="border border-gray-300 rounded-lg p-3 w-full"
        />
      </div>
    </div>
  );
}

// Step 11: Review (was Step 10)
function Review({ formData }: { formData: PatientIntakeDto }) {
  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold text-gray-800 mb-4">Review Your Information</h2>
      
      <div className="bg-gray-50 rounded-lg p-4 space-y-3">
        <h3 className="font-semibold text-gray-700">Personal Information</h3>
        <p><strong>Name:</strong> {formData.patient.firstName} {formData.patient.middleName} {formData.patient.lastName} {formData.patient.suffix}</p>
        <p><strong>DOB:</strong> {formData.patient.dateOfBirth}</p>
        <p><strong>Gender:</strong> {formData.patient.gender}</p>
        <p><strong>Email:</strong> {formData.patient.email}</p>
        <p><strong>Phone:</strong> {formData.patient.phoneNumber}</p>
        
        <h3 className="font-semibold text-gray-700 mt-4">Address</h3>
        <p>{formData.patient.addressLine1} {formData.patient.addressLine2}</p>
        <p>{formData.patient.city}, {formData.patient.state} {formData.patient.zipCode}</p>
        
        <h3 className="font-semibold text-gray-700 mt-4">Emergency Contacts</h3>
        <p>{formData.emergencyContacts.length} contact(s) added</p>
        
        <h3 className="font-semibold text-gray-700 mt-4">Family Members</h3>
        <p>{formData.familyMembers.length} family member(s) added</p>
        
        <h3 className="font-semibold text-gray-700 mt-4">Insurance</h3>
        <p>{formData.insurancePolicies.length} policy(ies) added</p>
        
        <h3 className="font-semibold text-gray-700 mt-4">Medical History</h3>
        <p>Allergies: {formData.allergies.length}</p>
        <p>Medications: {formData.medications.length}</p>
        <p>Surgeries: {formData.surgeries.length}</p>
        <p>Hospitalizations: {formData.hospitalizations.length}</p>
        <p>Family History: {formData.familyMedicalHistory.length}</p>
      </div>
      
      <p className="text-sm text-gray-600 mt-4">
        Please review all information carefully before submitting. By submitting this form, you confirm that all information provided is accurate and complete.
      </p>
    </div>
  );
}

