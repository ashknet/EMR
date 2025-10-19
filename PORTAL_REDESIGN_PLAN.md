# Patient Portal Redesign Plan

## Overview
This document outlines the comprehensive redesign of the Patient Portal to ensure consistency between the Intake Form and the main Portal tabs.

## 1. Profile Tab Redesign

### Current Sections
- Personal Information
- Medical Information (bloodType, height, weight)
- Emergency Contact (single)
- Insurance Information
- Additional Information
- Accessibility & Preferences

### Target Sections (Match Intake Form)
- **Personal Information**
  - First Name, Middle Name, Last Name, Suffix
  - Date of Birth
  - Gender (dropdown with lookup)
  - SSN (encrypted)
  - Marital Status (dropdown with lookup)
  - Race (dropdown with lookup)
  - Ethnicity
  - Primary Language (dropdown with lookup)
  - Preferred Language (dropdown with lookup)
  
- **Contact Information**
  - Email
  - Phone Number
  - Alternate Phone
  - Address Line 1
  - Address Line 2
  - City
  - State
  - Zip Code
  - Country

- **Emergency Contacts** (Multiple)
  - Name
  - Relationship Type (dropdown with lookup)
  - Phone
  - Alternate Phone

- **Accessibility & Preferences**
  - Interpreter Required (checkbox)
  - Mobility Assistance (checkbox)
  - Communication Preference (dropdown with lookup)
  - Religious Considerations (text)
  - Primary Care Physician
  - PCP Phone Number
  - Referring Physician
  - Preferred Pharmacy Name
  - Preferred Pharmacy Location
  - Photo ID Path

### Sections to Remove
- Medical Information (bloodType, height, weight) - moved to Medical History
- Insurance Information - moved to separate Insurance tab
- Additional Information (preferredLanguage, religion, organDonor, advanceDirective) - moved to appropriate sections

## 2. Medical History Tab Standardization

### Current Sections
- Allergies
- Current Medications
- Chronic Conditions
- Past Surgeries
- Past Hospitalizations
- Immunizations
- Family Medical History

### Target Sections (Match Intake Form + Add Missing)
- **Allergies**
  - Allergen Name
  - Allergen Type
  - Severity
  - Reaction
  - Onset Date

- **Current Medications**
  - Medication Name
  - Dosage
  - Frequency
  - Prescriber
  - Start Date

- **Chronic Conditions** (ADD TO INTAKE)
  - Condition Name
  - Diagnosed Date
  - Status (Active/Inactive/Resolved)

- **Past Surgeries**
  - Surgery Type
  - Surgery Date
  - Notes

- **Past Hospitalizations**
  - Hospital Name
  - Reason
  - Admission Date
  - Discharge Date
  - Notes

- **Immunizations** (ADD TO INTAKE)
  - Vaccine Name
  - Administered Date
  - Dose Number
  - Provider

- **Family Medical History**
  - Relative
  - Condition
  - Notes

## 3. Insurance Tab (Separate from Profile)

### Features
- Support multiple insurance policies
- Policy details:
  - Insurance Provider (dropdown with lookup)
  - Group Number
  - Policy Number
  - Policy Holder Name
  - Provider Phone
  - Medicare/Medicaid ID
  - Card Image Path

## 4. Provider Management

### Current State
- Single Primary Care Physician in intake

### Target State
- **Multiple Providers** with types:
  - Primary Care
  - Dental
  - Eye Care
  - Specialist
  - Other

- **Provider Details**:
  - Provider Type (dropdown)
  - Provider Name
  - Specialty
  - Phone Number
  - Address
  - Notes

## 5. Family Member Enhancement

### Current State (Intake)
- First Name
- Last Name
- Date of Birth
- Relationship
- Lives with Patient

### Target State
- **All Patient Fields**:
  - First Name, Middle Name, Last Name, Suffix
  - Date of Birth
  - Gender (dropdown)
  - Race (dropdown)
  - Ethnicity
  - Relationship to Patient (dropdown)
  - Lives with Patient (checkbox)
  
- **Contact Information**:
  - Email
  - Phone Number
  - Address (same as patient or different)
  
- **Emergency Contact**:
  - Emergency Contact Name
  - Emergency Contact Phone
  - Emergency Contact Relationship
  
- **Medical History** (Optional):
  - Allergies
  - Current Medications
  - Chronic Conditions
  - Family Medical History

## 6. Social History

### Ensure Complete Parity
- Smoking Status (dropdown with lookup)
- Alcohol Use (dropdown with lookup)
- Drug Use (dropdown with lookup)
- Occupation
- Living Situation (dropdown with lookup)

## 7. Legal Consents / Documents

### No Changes Needed
- Keep current upload/transfer functionality

---

## Database Changes Required

### New Tables
1. **ProviderTypes** (Lookup)
   - ProviderTypeId (PK)
   - TypeName
   - IsActive

2. **PatientProviders**
   - ProviderId (PK)
   - PatientId (FK)
   - ProviderTypeId (FK)
   - ProviderName
   - Specialty
   - PhoneNumber
   - Address
   - Notes
   - CreatedAt
   - IsActive

3. **ChronicConditions** (Enhanced)
   - Ensure exists with proper fields

4. **Immunizations** (Enhanced)
   - Ensure exists with proper fields

### Enhanced Tables
1. **FamilyMembers**
   - Add: MiddleName, Suffix, GenderId, RaceId, Ethnicity
   - Add: Email, PhoneNumber
   - Add: AddressLine1, AddressLine2, City, State, ZipCode, Country, SameAddressAsPatient
   - Add: EmergencyContactName, EmergencyContactPhone, EmergencyContactRelationship

2. **FamilyMemberMedicalHistory** (New)
   - FamilyMemberId (FK)
   - Allergies
   - Medications
   - ChronicConditions

---

## API Changes Required

### New Endpoints
1. **Providers**
   - GET /api/providers/patient/{patientId}
   - POST /api/providers
   - PUT /api/providers/{id}
   - DELETE /api/providers/{id}

2. **Chronic Conditions**
   - GET /api/conditions/patient/{patientId}
   - POST /api/conditions
   - PUT /api/conditions/{id}
   - DELETE /api/conditions/{id}

3. **Immunizations**
   - GET /api/immunizations/patient/{patientId}
   - POST /api/immunizations
   - PUT /api/immunizations/{id}
   - DELETE /api/immunizations/{id}

### Enhanced Endpoints
1. **Family Members**
   - Update POST/PUT to include new fields
   - Add medical history sub-resources

2. **Intake Form**
   - Add chronic conditions section
   - Add immunizations section
   - Add multiple providers section

---

## UI Changes Required

### Profile Page
- Complete redesign to match intake sections
- Remove insurance, medical info sections
- Add multiple emergency contacts support

### Medical History Page
- Already has most sections
- Ensure exact field parity with intake

### Insurance Page (New or Enhanced)
- Move from Profile to separate tab/section
- Support multiple policies

### Providers Page (New)
- Create new section for managing multiple providers

### Family Members Page
- Enhance to capture all patient-equivalent data
- Add medical history section

### Intake Form
- Add Chronic Conditions step
- Add Immunizations step
- Enhance Family Members step with new fields
- Add Providers step

---

## Implementation Priority

### Phase 1 (High Priority)
1. Profile Tab Redesign
2. Insurance Tab Separation
3. Medical History Sync (Add to Intake)

### Phase 2 (Medium Priority)
4. Provider Management
5. Social History Sync

### Phase 3 (Lower Priority)
6. Family Member Enhancement
7. Backend Unification & Optimization

---

## Testing Checklist

- [ ] Profile tab shows only intake-equivalent fields
- [ ] Multiple emergency contacts work
- [ ] Insurance moved to separate section
- [ ] Chronic conditions in both intake and medical history
- [ ] Immunizations in both intake and medical history
- [ ] Multiple providers can be added
- [ ] Family members have enhanced fields
- [ ] All data persists correctly
- [ ] APIs are unified and consistent
- [ ] No duplicate data entry required

