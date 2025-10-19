# Patient Intake System - Implementation Guide

## Overview
This document describes the comprehensive patient intake system that has been implemented across the EMRR stack, including database schema, API endpoints, and React UI components.

## Database Changes

### New Tables Added
1. **Metadata Lookup Tables** (`database/05_MetadataTables.sql`)
   - `Gender`, `MaritalStatus`, `Race`, `Language`, `RelationshipType`
   - `InsuranceProvider`, `AddressType`, `PhoneType`
   - `SmokingStatus`, `AlcoholUse`, `DrugUse`, `LivingSituation`, `CommunicationPreference`
   - `AllergyType`, `Medication`, `ChronicCondition`, `SurgeryType`, `ImmunizationType`, `Condition`

2. **Patient Detail Tables** (`database/06_PatientDetailTables.sql`)
   - `PatientAddresses` - Home/Mailing addresses
   - `PatientPhones` - Primary/Secondary phone numbers
   - `EmergencyContacts` - Emergency contact information
   - `InsurancePolicies` - Insurance provider details
   - `SocialHistory` - Smoking, alcohol, drug use, occupation
   - `LegalConsents` - HIPAA, consent to treat, advance directives

3. **Medical History Tables** (`database/07_MedicalHistoryTables.sql`)
   - `PatientAllergies` - Known allergies with severity
   - `PatientMedications` - Current medications with dosage
   - `PatientChronicConditions` - Chronic health conditions
   - `PatientSurgeries` - Past surgical procedures
   - `PatientHospitalizations` - Hospital admission history
   - `PatientImmunizations` - Vaccination records
   - `PatientTestResults` - Lab/test results with attachments
   - `FamilyMedicalHistory` - Family health conditions

### Enhanced Patient Core Table
Extended `Patients` table with new columns:
- `MiddleName`, `Suffix`, `DateOfBirth`
- `GenderId`, `MaritalStatusId`, `RaceId`
- `PrimaryLanguageId`, `PreferredLanguageId`
- `SSN` (encrypted), `InterpreterRequired`, `MobilityAssistance`
- `CommunicationPrefId`, `ReligiousConsiderations`

### Stored Procedures
- `usp_Metadata_Upsert` - Generic CRUD for lookup tables
- `usp_Metadata_Delete` - Delete metadata items
- `usp_Patient_Upsert` - Complete patient intake via JSON

## API Enhancements

### New Endpoints

#### Metadata Management (`/api/metadata/{table}`)
- `GET /api/metadata/{table}` - List all active metadata items (public)
- `POST /api/metadata/{table}` - Create/update metadata (Admin only)
- `DELETE /api/metadata/{table}/{id}` - Delete metadata (Admin only)

#### Patient Intake (`/api/patients/intake`)
- `PUT /api/patients/intake` - Complete patient intake with nested collections

### Authorization
- `RequireAdmin` policy for metadata management
- Role-based access control for patient data

### DTOs and Models
- `PatientIntakeDto` - Comprehensive patient data structure
- `MetadataDto` - Standardized lookup item format
- AutoMapper profiles for entity/DTO conversion

## Frontend Implementation

### New Pages
1. **Patient Intake Form** (`/patient/details`)
   - Multi-section form with dynamic dropdowns
   - Real-time validation and error handling
   - File upload support for documents
   - Signature pad for legal consents

2. **Admin Metadata Manager** (`/admin/metadata`)
   - CRUD interface for all lookup tables
   - Category-based organization
   - Real-time updates with React Query

### Technical Stack
- **React 18** with TypeScript
- **React Query** for server state management
- **Tailwind CSS** for styling
- **React Router** for navigation

## Deployment Steps

### 1. Database Migration
```sql
-- Run in order:
-- 01_Schemas.sql
-- 02_Tables.sql
-- 05_MetadataTables.sql
-- 06_PatientDetailTables.sql
-- 07_MedicalHistoryTables.sql
-- 08_StoredProcedures.sql
-- 09_SampleData.sql
```

### 2. Backend Deployment
```bash
cd PatientService/PatientService.API
dotnet ef migrations add PatientIntakeExpansion
dotnet ef database update
dotnet run
```

### 3. Frontend Deployment
```bash
cd PatientPortal
npm install
npm run build
# Deploy dist/ folder to web server
```

## Security Considerations

### Data Protection
- SSN encryption at rest using SQL Server encryption
- HIPAA-compliant audit logging
- Role-based access control

### API Security
- JWT token authentication
- Admin role enforcement for metadata management
- Input validation and sanitization

### File Upload Security
- File type validation
- Size limits
- Secure storage paths

## Testing

### Backend Tests
- Unit tests for service layer
- Integration tests for API endpoints
- Database migration tests

### Frontend Tests
- Component unit tests
- Integration tests for form submission
- E2E tests for complete user flows

## Configuration

### Environment Variables
```env
# API Configuration
VITE_API_URL=http://localhost:5100

# Database Connection
ConnectionStrings__DefaultConnection="Server=localhost;Database=EMRR;Trusted_Connection=true;"

# JWT Settings
JWT__SecretKey=your-secret-key
JWT__Issuer=EMRR-API
JWT__Audience=EMRR-Client
```

## Future Enhancements

### Planned Features
1. **Advanced Form Validation**
   - Real-time field validation
   - Cross-field validation rules
   - Custom validation messages

2. **Document Management**
   - PDF generation for patient summaries
   - Document versioning
   - Digital signature integration

3. **Reporting and Analytics**
   - Patient demographics reports
   - Metadata usage analytics
   - Compliance reporting

4. **Mobile Support**
   - Responsive design improvements
   - Progressive Web App features
   - Offline capability

## Troubleshooting

### Common Issues
1. **Metadata not loading**: Check API endpoint permissions
2. **Form submission fails**: Verify DTO mapping and validation
3. **File upload errors**: Check file size limits and types
4. **Database connection**: Verify connection string and permissions

### Support
For technical support or questions about this implementation, refer to the development team or create an issue in the project repository.
