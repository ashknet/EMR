# Medical History Features - Implementation Plan

## Current Status

### ✅ Already Working (with fixes)
1. **Allergies** - Fully functional
2. **Chronic Conditions** - Fully functional  
3. **Immunizations** - Entity exists, needs POST fix
4. **Providers** - Fixed SortOrder → DisplayOrder issue ✅

### ❌ Missing Implementations
The following features have database tables and entities but NO API controllers:

1. **Social History** - Has table, entity, DbSet but NO endpoints
2. **Medications** - Has table, entity, DbSet but NO endpoints  
3. **Surgeries** - Has table, entity, DbSet but NO endpoints
4. **Hospitalizations** - Has table, entity, DbSet but NO endpoints
5. **Family Medical History** - Has table, entity, DbSet but NO endpoints

## Database Schema Summary

### pt.SocialHistory
```sql
- PatientId (PK, FK to Patients)
- SmokingStatusId, AlcoholUseId, DrugUseId  
- Occupation, ExerciseFrequency, Diet
- StressLevel, SleepHours
- LivingSituationId
```

###pt.Medications
```sql
- MedicationId (PK)
- PatientId (FK)
- MedicationName, Dosage, Frequency
- Prescriber, StartDate, EndDate
- IsActive, Notes
```

### pt.PatientSurgeries
```sql
- SurgeryId (PK, IDENTITY)
- PatientId (FK)
- SurgeryType, SurgeryDate
- Notes
```

### pt.PatientHospitalizations
```sql
- HospitalizationId (PK, IDENTITY)
- PatientId (FK)
- HospitalName, Reason
- AdmissionDate, DischargeDate
- Notes
```

### pt.FamilyMedicalHistory
```sql
- FamilyHistoryId (PK, IDENTITY)
- PatientId (FK)
- Relative, Condition
- Notes
```

## Implementation Required

### 1. Backend API Controllers Needed

Create these controllers in `PatientService.API/Controllers/`:

1. **SocialHistoryController.cs**
   - GET /api/patients/{patientId}/social-history
   - PUT /api/patients/{patientId}/social-history
   - POST /api/patients/{patientId}/social-history

2. **MedicationsController.cs**
   - GET /api/medications/patient/{patientId}
   - POST /api/medications
   - PUT /api/medications/{id}
   - DELETE /api/medications/{id}

3. **SurgeriesController.cs**
   - GET /api/surgeries/patient/{patientId}
   - POST /api/surgeries
   - PUT /api/surgeries/{id}
   - DELETE /api/surgeries/{id}

4. **HospitalizationsController.cs**
   - GET /api/hospitalizations/patient/{patientId}
   - POST /api/hospitalizations
   - PUT /api/hospitalizations/{id}
   - DELETE /api/hospitalizations/{id}

5. **FamilyMedicalHistoryController.cs**
   - GET /api/family-medical-history/patient/{patientId}
   - POST /api/family-medical-history
   - PUT /api/family-medical-history/{id}
   - DELETE /api/family-medical-history/{id}

### 2. Frontend Updates Needed

Update `PatientPortal/src/pages/MedicalHistory.tsx`:
- Connect medications tab to real API
- Connect surgeries tab to real API  
- Connect hospitalizations tab to real API
- Connect family history tab to real API

## Priority Order

1. **CRITICAL** - Fix immunization POST error
2. **HIGH** - Implement Social History endpoints (currently failing)
3. **HIGH** - Implement Medications API and frontend
4. **MEDIUM** - Implement Surgeries API and frontend
5. **MEDIUM** - Implement Hospitalizations API and frontend
6. **MEDIUM** - Implement Family Medical History API and frontend

## Estimated LOC

- Backend Controllers: ~1,500 lines (5 controllers × ~300 lines each)
- Frontend Updates: ~500 lines  
- Total: ~2,000 lines of code

## Time Estimate

- Backend: 2-3 hours per controller = 10-15 hours
- Frontend: 1-2 hours  
- Testing: 2-3 hours
- **Total: 13-20 hours of development**

## Next Steps

Due to the large scope, I will:
1. Fix the immediate errors (Providers, Immunization)
2. Implement the most critical missing feature (Social History)
3. Create template controllers that can be adapted for the remaining features
4. Provide detailed instructions for completing the remaining implementations

