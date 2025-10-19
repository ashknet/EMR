# Patient Portal & Intake Form Synchronization Checklist

## Goal
Ensure Patient Intake Form (wizard) and Main Portal Tabs display the SAME data with the SAME fields, using the SAME APIs and SAME database.

---

## Main Portal Tabs vs Intake Form Mapping

### 1. Dashboard Tab
**Status:** Keep as is (no sync needed)

---

### 2. Profile Tab (Main Portal) ↔️ Intake Form Steps 1-3

#### Current Profile Sections (TO REMOVE):
- ❌ Medical Info (bloodType, height, weight) → Move to Medical History
- ❌ Insurance → Separate Insurance tab
- ❌ Additional Information → Distribute to appropriate sections

#### Target Profile Sections (MATCH INTAKE):

**Section A: Personal Information**
- Intake Form: Step 1 - Personal Information
- Fields:
  - ✅ First Name, Middle Name, Last Name, Suffix
  - ✅ Date of Birth
  - ✅ Gender (dropdown with GenderId)
  - ✅ SSN (encrypted)
  - ✅ Marital Status (dropdown with MaritalStatusId)
  - ✅ Race (dropdown with RaceId)
  - ✅ Ethnicity
  - ✅ Primary Language (PrimaryLanguageId)
  - ✅ Preferred Language (PreferredLanguageId)

**Section B: Contact Information**
- Intake Form: Step 2 - Contact Information
- Fields:
  - ✅ Email
  - ✅ Phone Number
  - ✅ Alternate Phone
  - ✅ Address Line 1, Address Line 2
  - ✅ City, State, Zip Code
  - ✅ Country

**Section C: Emergency Contacts** (MULTIPLE SUPPORT)
- Intake Form: Step 3 - Emergency Contacts
- Fields (per contact):
  - ✅ Name
  - ✅ Relationship Type (RelationshipTypeId)
  - ✅ Phone
  - ✅ Alternate Phone
- **Change:** Support multiple emergency contacts (currently single)

**Section D: Accessibility**
- Intake Form: Step 9 - Accessibility
- Fields:
  - ✅ Interpreter Required
  - ✅ Mobility Assistance
  - ✅ Communication Preference (CommunicationPrefId)
  - ✅ Religious Considerations
  - ✅ Primary Care Physician
  - ✅ PCP Phone Number
  - ✅ Referring Physician
  - ✅ Preferred Pharmacy Name
  - ✅ Preferred Pharmacy Location

---

### 3. Medical History Tab (Main Portal) ↔️ Intake Form Step 7

#### Current Medical History Sections:
- ✅ Allergies
- ✅ Current Medications
- ✅ Chronic Conditions (EXISTS in portal, MISSING in intake) ⚠️
- ✅ Past Surgeries
- ✅ Past Hospitalizations
- ✅ Immunizations (EXISTS in portal, MISSING in intake) ⚠️
- ✅ Family Medical History

#### Required Changes:
1. **ADD to Intake Form:**
   - ➕ Chronic Conditions section
   - ➕ Immunizations section

2. **Ensure Field Parity:**

**Allergies:**
- AllergenName, AllergenType, Severity, Reaction, OnsetDate ✅

**Medications:**
- MedicationName, Dosage, Frequency, Prescriber, StartDate ✅

**Chronic Conditions:** ⚠️ ADD TO INTAKE
- ConditionName, DiagnosedDate, Status, Notes

**Surgeries:**
- SurgeryType, SurgeryDate, Notes ✅

**Hospitalizations:**
- HospitalName, Reason, AdmissionDate, DischargeDate, Notes ✅

**Immunizations:** ⚠️ ADD TO INTAKE
- VaccineName, AdministeredDate, DoseNumber, Provider, Site, Route, Notes

**Family Medical History:**
- Relative, Condition, Notes ✅

---

### 4. Insurance Tab (Main Portal) ↔️ Intake Form Step 5

#### Required Changes:
- ✅ Support MULTIPLE insurance policies (both places)
- ✅ Same fields in both places

**Fields:**
- Insurance Provider (ProviderId)
- Group Number
- Policy Number
- Policy Holder Name
- Provider Phone
- Medicare/Medicaid ID
- Card Image Path

---

### 5. Providers Tab (Main Portal) ↔️ Intake Form Step 6

#### Current Status:
- ❌ Intake: Single Primary Care Physician only
- ❌ Portal: No dedicated providers tab

#### Required Changes:
1. **Create Providers Tab** in main portal
2. **Enhance Intake Form** to support multiple providers
3. **Support Provider Types:**
   - Primary Care
   - Dental
   - Eye Care / Ophthalmology
   - Orthopedic
   - Pediatrics
   - Specialist
   - Other

**Fields (per provider):**
- Provider Type (dropdown)
- Provider Name
- Specialty
- Phone Number
- Address
- Email
- Notes

---

### 6. Family Members Tab (Main Portal) ↔️ Intake Form Step 4

#### Current Intake Fields:
- First Name, Last Name
- Date of Birth
- Relationship
- Lives with Patient

#### Required Fields (MATCH PATIENT):
**Personal Information:**
- ➕ First Name, Middle Name, Last Name, Suffix
- ✅ Date of Birth
- ➕ Gender (GenderId)
- ➕ Race (RaceId)
- ➕ Ethnicity
- ✅ Relationship to Patient (RelationshipTypeId)
- ✅ Lives with Patient

**Contact Information:**
- ➕ Email
- ➕ Phone Number
- ➕ Alternate Phone
- ➕ Address (checkbox: "Same as Patient" or enter different)
  - Address Line 1, Address Line 2
  - City, State, Zip Code, Country

**Emergency Contact:**
- ➕ Emergency Contact Name
- ➕ Emergency Contact Phone
- ➕ Emergency Contact Relationship

**Medical History (Optional):**
- ➕ Allergies
- ➕ Current Medications
- ➕ Chronic Conditions
- ➕ Past Surgeries
- ➕ Past Hospitalizations
- ➕ Family Medical History (inherited conditions)

---

### 7. Social History Tab (Main Portal) ↔️ Intake Form Step 8

#### Fields (MUST MATCH):
- ✅ Smoking Status (SmokingStatusId)
- ✅ Alcohol Use (AlcoholUseId)
- ✅ Drug Use (DrugUseId)
- ✅ Occupation
- ✅ Living Situation (LivingSituationId)

**Ensure:** Same dropdowns, same validation, same display

---

### 8. Documents Tab (Main Portal)
**Status:** Keep as is ✅

### 9. Transfers Tab (Main Portal)
**Status:** Keep as is ✅

---

## Database Changes Required

### New Tables:
1. ✅ ChronicConditions (Created)
2. ✅ Immunizations (Created)
3. ⏳ ProviderTypes (Lookup)
4. ⏳ PatientProviders

### Enhanced Tables:
1. ⏳ FamilyMembers (Add ~15 new columns)
2. ⏳ FamilyMemberAllergies (New)
3. ⏳ FamilyMemberMedications (New)
4. ⏳ FamilyMemberChronicConditions (New)

---

## API Changes Required

### New Endpoints:
1. ⏳ GET/POST/PUT/DELETE /api/conditions/patient/{patientId}
2. ⏳ GET/POST/PUT/DELETE /api/immunizations/patient/{patientId}
3. ⏳ GET/POST/PUT/DELETE /api/providers/patient/{patientId}
4. ⏳ GET/POST/PUT/DELETE /api/familymembers/{id}/medical-history

### Enhanced Endpoints:
1. ⏳ Update Family Members CRUD to include new fields
2. ⏳ Update step endpoints to include chronic conditions & immunizations

---

## UI Changes Required

### Profile Page (Profile.tsx):
- ⏳ Remove: Medical Info, Insurance, Additional sections
- ⏳ Redesign: Match intake form sections exactly
- ⏳ Add: Multiple emergency contacts support
- ⏳ Organize: Personal Info, Contact Info, Emergency Contacts, Accessibility

### Medical History Page (MedicalHistory.tsx):
- ⏳ Verify field parity with intake form
- ⏳ Already has most sections ✅

### Insurance Page:
- ⏳ Create/Enhance to support multiple policies
- ⏳ Move from Profile tab

### Providers Page (New):
- ⏳ Create new page/section
- ⏳ Support multiple providers

### Family Members Page:
- ⏳ Enhance form to capture all patient-equivalent fields
- ⏳ Add medical history section

### Intake Form (ComprehensiveIntake.tsx):
- ⏳ Add Chronic Conditions to Step 7
- ⏳ Add Immunizations to Step 7
- ⏳ Enhance Step 6 for multiple providers
- ⏳ Enhance Step 4 for complete family member data
- ⏳ Add step-specific data loading endpoints

---

## Implementation Order

### Phase 1A: Database & Backend (Priority 1)
1. ✅ Create ChronicConditions table
2. ✅ Create Immunizations table
3. ⏳ Create ProviderTypes lookup table
4. ⏳ Create PatientProviders table
5. ⏳ Enhance FamilyMembers table
6. ⏳ Create FamilyMember medical history tables

### Phase 1B: Backend Entities & APIs (Priority 1)
1. ⏳ Create ChronicCondition entity
2. ⏳ Create Immunization entity
3. ⏳ Create Provider entity
4. ⏳ Enhance FamilyMember entity
5. ⏳ Create CRUD endpoints for all new entities
6. ⏳ Enhance step endpoints

### Phase 1C: Intake Form Updates (Priority 1)
1. ⏳ Add Chronic Conditions to Step 7
2. ⏳ Add Immunizations to Step 7
3. ⏳ Enhance Family Members step (Step 4)
4. ⏳ Enhance Providers section (Step 6)

### Phase 1D: Portal Tab Updates (Priority 1)
1. ⏳ Redesign Profile page
2. ⏳ Verify Medical History page
3. ⏳ Create/Enhance Insurance page
4. ⏳ Create Providers page
5. ⏳ Enhance Family Members page

---

## Testing Checklist

- [ ] Can add chronic conditions in intake form
- [ ] Chronic conditions appear in Medical History tab
- [ ] Can add immunizations in intake form
- [ ] Immunizations appear in Medical History tab
- [ ] Profile tab matches intake form sections
- [ ] Multiple emergency contacts work in both places
- [ ] Insurance moved to separate section
- [ ] Can add multiple providers in both places
- [ ] Family members have all patient-equivalent fields
- [ ] Family members can have medical history
- [ ] All data syncs between intake and portal
- [ ] No duplicate data entry required
- [ ] Same APIs used everywhere
- [ ] Same database tables used everywhere

---

## Success Criteria

✅ **Single Source of Truth:** All data comes from the same database tables
✅ **Consistent UX:** Same fields, same order, same validation
✅ **No Duplication:** User enters data once, sees it everywhere
✅ **Complete Parity:** Intake Form ≡ Portal Tabs

