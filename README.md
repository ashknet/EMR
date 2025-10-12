# Healthcare Platform - HIPAA-Compliant Healthcare Data Management System

## 🏥 Overview

A comprehensive, HIPAA-compliant healthcare platform for the US market, featuring:
- **Patient/Family Portal**: Multi-person health data management with digital sharing via QR codes
- **Hospital Agent**: Windows desktop application for automated patient intake and record transfer
- **Microservices Architecture**: 10+ specialized services built with .NET 8, Azure, and FHIR R4 compliance
- **Standards-Based Interoperability**: Full FHIR R4 and US Core compliance for seamless EMR integration

## 🎯 Key Features

### Patient/Family Portal
- ✅ Multi-person registration (self, spouse, children, parents, siblings, others)
- ✅ Comprehensive health history management (allergies, medications, conditions, immunizations)
- ✅ Insurance policy tracking and claims management
- ✅ Digital consent management with granular controls
- ✅ QR code generation for instant, time-limited data sharing
- ✅ Secure document vault with OCR processing
- ✅ Family relationship mapping and proxy/guardian management
- ✅ WCAG 2.1 AA accessible interface

### Hospital Agent (Windows Desktop)
- ✅ Lightweight, secure, auto-updating Windows application
- ✅ QR code scanning for instant patient data intake
- ✅ Automated EMR form filling (API, clipboard, or FHIR export)
- ✅ Email/eFax monitoring with OCR for incoming medical records
- ✅ Secure inter-hospital data transfer with audit trails
- ✅ No PHI stored locally (encrypted in-transit only)
- ✅ Least-privilege operation with comprehensive logging

### Microservices
1. **Patient Service**: Patient profile CRUD, family member management
2. **Family Service**: Relationship mapping, proxy authorizations, emergency contacts
3. **Health History Service**: Conditions, allergies, medications, immunizations, procedures, observations
4. **Insurance Service**: Policy management, claims tracking, eligibility verification
5. **Consent/Audit Service**: Granular consents, comprehensive audit logging, QR sharing
6. **Data Integration Service**: FHIR/HL7 adapters, OCR processing, EMR connectors
7. **Agent Management Service**: Agent deployment, updates, and communication
8. **Transfer/Routing Service**: Secure record transfers between facilities
9. **Notification Service**: Alerts, reminders, appointment notifications
10. **Security/Compliance Service**: Policy enforcement, BAA management, compliance monitoring

## 🏗️ Architecture

### Technology Stack
- **Backend**: .NET 8 (C#), ASP.NET Core Web API
- **Frontend**: React 18+ with TypeScript, Material-UI
- **Desktop Agent**: .NET 8 WPF (Windows Presentation Foundation)
- **Database**: Azure SQL Database (separate schema per service)
- **Cloud Platform**: Microsoft Azure (HIPAA-compliant regions)
- **Interoperability**: FHIR R4, HL7 v2, CCD/CDA
- **Security**: AES-256 encryption, TLS 1.3, Azure Key Vault, MFA

### System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Patient/Family Portal                     │
│                  (React + TypeScript)                        │
└─────────────────────────────────────────────────────────────┘
                             │
                             │ HTTPS/TLS 1.3
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                    API Gateway (Azure)                       │
│              Authentication & Authorization                   │
└─────────────────────────────────────────────────────────────┘
                             │
        ┌────────────────────┼────────────────────┐
        │                    │                    │
        ▼                    ▼                    ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│   Patient    │    │   Family     │    │Health History│
│   Service    │    │   Service    │    │   Service    │
│  (API+Func)  │    │  (API+Func)  │    │  (API+Func)  │
└──────┬───────┘    └──────┬───────┘    └──────┬───────┘
       │                   │                   │
       └───────────────────┴───────────────────┘
                           │
              ┌────────────┴────────────┐
              │    Azure SQL Database   │
              │  (Separate Schemas)     │
              └─────────────────────────┘
```

## 📁 Project Structure

```
healthcare-platform/
├── src/
│   ├── Services/
│   │   ├── PatientService/
│   │   │   ├── PatientService.API/          # REST API
│   │   │   ├── PatientService.Functions/    # Azure Functions
│   │   │   ├── PatientService.Domain/       # Entities & DTOs
│   │   │   └── PatientService.Infrastructure/ # Data access
│   │   ├── FamilyService/
│   │   ├── HealthHistoryService/
│   │   ├── InsuranceService/
│   │   ├── ConsentAuditService/
│   │   ├── DataIntegrationService/
│   │   ├── AgentManagementService/
│   │   ├── TransferRoutingService/
│   │   ├── NotificationService/
│   │   └── SecurityComplianceService/
│   ├── Shared/
│   │   ├── Common/                          # Shared utilities
│   │   ├── FHIR/                           # FHIR models & converters
│   │   └── Security/                       # Auth & encryption
│   ├── Frontend/
│   │   └── PatientPortal/                  # React application
│   └── Desktop/
│       └── HospitalAgent/                  # Windows WPF app
├── database/
│   ├── schemas/                            # SQL table definitions
│   ├── stored-procedures/                  # Stored procedures
│   └── test-data/                         # Sample data scripts
├── deployment/
│   └── azure/                             # ARM templates, scripts
└── docs/
    ├── API_CONTRACTS.md
    ├── FHIR_IMPLEMENTATION.md
    ├── HIPAA_COMPLIANCE.md
    └── DEPLOYMENT_GUIDE.md
```

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+ (for React frontend)
- SQL Server 2019+ or Azure SQL Database
- Azure subscription (for production deployment)
- Visual Studio 2022 or VS Code

### Local Development Setup

1. **Clone the Repository**
```bash
git clone https://github.com/your-org/healthcare-platform.git
cd healthcare-platform
```

2. **Setup Database**
```bash
# Create databases
sqlcmd -S localhost -i database/schemas/01_PatientService_Schema.sql
sqlcmd -S localhost -i database/schemas/02_FamilyService_Schema.sql
sqlcmd -S localhost -i database/schemas/03_HealthHistoryService_Schema.sql
# ... continue for all schemas

# Load test data
sqlcmd -S localhost -i database/test-data/01_PatientService_TestData.sql
```

3. **Configure Connection Strings**

Update `appsettings.Development.json` in each API project:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HealthcarePlatform_Patient;Trusted_Connection=True;"
  }
}
```

4. **Run Services**

**Option A: Run All Services (Recommended for development)**
```bash
# Terminal 1 - Patient Service
cd src/Services/PatientService/PatientService.API
dotnet run

# Terminal 2 - Family Service
cd src/Services/FamilyService/FamilyService.API
dotnet run

# Terminal 3 - Health History Service
cd src/Services/HealthHistoryService/HealthHistoryService.API
dotnet run

# ... continue for other services
```

**Option B: Use Docker Compose**
```bash
docker-compose up -d
```

5. **Run Azure Functions (Optional)**
```bash
cd src/Services/PatientService/PatientService.Functions
func start
```

6. **Run React Frontend**
```bash
cd src/Frontend/PatientPortal
npm install
npm start
```

7. **Access Swagger UI**
- Patient Service: http://localhost:5001
- Family Service: http://localhost:5002
- Health History Service: http://localhost:5003
- Frontend: http://localhost:3000

### Local Authentication Mode

In development, authentication is **disabled** for easier testing:
- All requests are automatically authenticated as a development user
- All API endpoints are accessible without tokens
- Swagger UI works without authentication

**⚠️ IMPORTANT**: Authentication is REQUIRED in production deployment.

## 🔐 Security & Compliance

### HIPAA Compliance

This platform implements comprehensive HIPAA security and privacy requirements:

#### Administrative Safeguards
- ✅ Security Management Process
- ✅ Assigned Security Responsibility
- ✅ Workforce Security & Training
- ✅ Information Access Management
- ✅ Security Awareness Training
- ✅ Contingency Planning
- ✅ Business Associate Agreements

#### Physical Safeguards
- ✅ Facility Access Controls (Azure datacenter compliance)
- ✅ Workstation Security (Hospital agent security controls)
- ✅ Device and Media Controls

#### Technical Safeguards
- ✅ Access Control (RBAC, MFA, session management)
- ✅ Audit Controls (comprehensive immutable audit logs)
- ✅ Integrity Controls (checksums, digital signatures)
- ✅ Transmission Security (TLS 1.3, AES-256 encryption)

### Encryption

**Data at Rest**: AES-256-GCM encryption for all PHI fields
**Data in Transit**: TLS 1.3 with certificate pinning
**Key Management**: Azure Key Vault with HSM backing

### Audit Logging

Every data access, modification, and sharing event is logged with:
- Timestamp (immutable)
- Actor (who performed the action)
- Action type and outcome
- Target resource
- Source IP, device, location
- Authorization method
- Consent validation

**Audit logs are immutable** and retained for 7+ years per HIPAA requirements.

## 📊 Database Schema

### Schema Organization

Each microservice has its own database schema for isolation:
- `patient.*` - Patient demographic and profile data
- `family.*` - Family relationships and authorizations
- `health.*` - Medical history, allergies, medications
- `insurance.*` - Insurance policies and claims
- `consent.*` - Consents and audit logs

### Key Tables

**Patient Service**
- `patient.Patients` - Patient demographics (encrypted PHI)
- `patient.PatientDocuments` - Document metadata
- `patient.PatientAccessLog` - HIPAA audit trail

**Family Service**
- `family.FamilyGroups` - Family/household units
- `family.FamilyMembers` - Member relationships
- `family.ProxyAuthorizations` - Guardian/proxy permissions
- `family.EmergencyContacts` - Emergency contact information

**Health History Service**
- `health.Conditions` - Diagnoses and chronic conditions
- `health.Allergies` - Allergies and intolerances
- `health.Medications` - Current and historical medications
- `health.Immunizations` - Vaccination records
- `health.Procedures` - Surgical and medical procedures
- `health.Observations` - Lab results and vital signs

**Insurance Service**
- `insurance.InsurancePolicies` - Coverage information
- `insurance.Claims` - Claim tracking
- `insurance.EligibilityVerifications` - Eligibility checks

**Consent/Audit Service**
- `consent.Consents` - Granular consent records
- `consent.AuditLog` - Comprehensive audit trail (immutable)
- `consent.DataSharingEvents` - QR code and transfer tracking

### Stored Procedures

Sample stored procedures provided for:
- CRUD operations with automatic audit logging
- Complex queries (family members, dashboard stats)
- HIPAA-compliant data access patterns

## 🔌 API Documentation

### Patient Service API

**Base URL**: `http://localhost:5001/api/v1`

#### Endpoints

**GET** `/patients` - Get all patients
**GET** `/patients/{id}` - Get patient by ID
**POST** `/patients` - Create new patient
**PUT** `/patients/{id}` - Update patient
**DELETE** `/patients/{id}` - Soft delete patient
**GET** `/patients/search?searchTerm={term}` - Search patients
**GET** `/patients/family/{primaryAccountHolderId}` - Get family members

#### Sample Request

```json
POST /api/v1/patients
{
  "firstName": "John",
  "middleName": "Michael",
  "lastName": "Smith",
  "dateOfBirth": "1985-06-15",
  "gender": "Male",
  "email": "john.smith@email.com",
  "phoneNumber": "+1-555-0101",
  "profileType": "Self"
}
```

#### Sample Response

```json
{
  "success": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "firstName": "John",
    "lastName": "Smith",
    "dateOfBirth": "1985-06-15",
    "isActive": true,
    "isVerified": false,
    "createdAt": "2025-10-12T10:30:00Z"
  },
  "message": "Patient created successfully",
  "timestamp": "2025-10-12T10:30:00Z"
}
```

### Swagger UI

All API services include interactive Swagger documentation:
- Patient Service: http://localhost:5001/swagger
- Family Service: http://localhost:5002/swagger
- Health History Service: http://localhost:5003/swagger

## 🧪 Testing

### Test Data

Sample test data includes:
- 6 patients (adults, minors, senior)
- 1 complete family (Smith family: John, Jane, Emma, Oliver)
- 2 independent accounts (Maria Garcia, Robert Johnson)
- Sample documents (insurance cards, lab results)
- Sample notes and allergies

### Running Tests

```bash
# Unit tests
dotnet test src/Services/PatientService/PatientService.Tests

# Integration tests
dotnet test src/Services/PatientService/PatientService.IntegrationTests

# E2E tests
cd src/Frontend/PatientPortal
npm test
```

## 🌐 FHIR Compliance

### US Core Implementation

This platform implements US Core Patient Profile requirements:
- Patient demographics (name, gender, birth date)
- US Core Race and Ethnicity extensions
- Contact information (telecom, address)
- Identifiers (MRN, SSN)

### FHIR Resources Supported

- **Patient** - Demographics and identifiers
- **AllergyIntolerance** - Allergies and adverse reactions
- **Condition** - Diagnoses and problems
- **MedicationStatement** - Medication history
- **Immunization** - Vaccination records
- **Procedure** - Surgical and medical procedures
- **Observation** - Lab results and vital signs
- **Coverage** - Insurance coverage
- **Consent** - Patient consent directives
- **DocumentReference** - Clinical documents

### FHIR API Endpoints

```
GET /fhir/Patient/{id}
GET /fhir/AllergyIntolerance?patient={id}
GET /fhir/Condition?patient={id}
GET /fhir/MedicationStatement?patient={id}
POST /fhir/Bundle - Submit transaction bundle
```

## 📱 Patient Portal Features

### Dashboard
- Insurance coverage summary with deductible tracking
- Upcoming appointments and visit history
- Family member roster with health status
- Recent activity and notifications
- Quick actions (Share QR, Add family member, Transfer records)

### Family Management
- Add/edit family members (spouse, children, parents, siblings)
- Manage relationships and proxy authorizations
- Emergency contact management
- Family-wide document sharing

### Health Records
- Conditions and chronic illnesses
- Allergies and intolerances
- Medications (current and historical)
- Immunization history
- Procedures and surgeries
- Lab results and vital signs

### Insurance Management
- Policy information and coverage details
- Deductible and out-of-pocket tracking
- Claims history and status
- Insurance card upload

### Consent & Sharing
- Generate time-limited QR codes for hospital intake
- Granular consent controls (what to share, with whom, for how long)
- View sharing history and audit logs
- Revoke access anytime

### Document Vault
- Upload medical documents, insurance cards, lab results
- OCR processing for automatic data extraction
- Secure encrypted storage
- Share documents with providers

## 🖥️ Hospital Agent

### Features
- QR code scanning for patient intake
- Auto-fill hospital EMR forms
- Email/eFax monitoring for incoming records
- OCR processing for scanned documents
- Secure inter-hospital transfers
- Real-time status updates

### Installation
1. Download installer from releases
2. Run as administrator
3. Enter hospital credentials
4. Agent registers with cloud service
5. Begins monitoring configured inboxes

### Security
- Runs with least privilege
- No PHI stored locally
- All communication encrypted (TLS 1.3)
- Automatic security updates
- Comprehensive audit logging
- Offline mode with secure queueing

## 🚢 Deployment

### Azure Deployment

1. **Provision Azure Resources**
```bash
cd deployment/azure
az login
./provision-resources.sh
```

2. **Deploy Microservices**
```bash
# Deploy each service to Azure App Service
az webapp deploy --resource-group healthcare-rg --name patient-api --src-path ./publish

# Or use Azure DevOps/GitHub Actions
# See deployment/azure/pipelines/
```

3. **Configure Managed Identity**
```bash
# Enable Managed Identity for each App Service
az webapp identity assign --name patient-api --resource-group healthcare-rg

# Grant access to Key Vault
az keyvault set-policy --name healthcare-kv --object-id {managed-identity-id} --secret-permissions get list
```

4. **Deploy Azure Functions**
```bash
func azure functionapp publish patient-functions --csharp
```

5. **Deploy Frontend**
```bash
cd src/Frontend/PatientPortal
npm run build
az storage blob upload-batch --destination '$web' --source ./build
```

### Environment Variables

**Production App Settings** (configure in Azure Portal):
```
SqlConnectionString=<Azure SQL connection string>
AzureKeyVaultUrl=https://healthcare-kv.vault.azure.net/
ApplicationInsights__InstrumentationKey=<instrumentation key>
AzureAd__TenantId=<tenant-id>
AzureAd__ClientId=<client-id>
AzureAd__ClientSecret=<client-secret>
AzureStorage__ConnectionString=<storage connection string>
```

## 📄 Documentation

- [API Contracts](docs/API_CONTRACTS.md) - Detailed API specifications
- [FHIR Implementation](docs/FHIR_IMPLEMENTATION.md) - FHIR resource mappings
- [HIPAA Compliance](docs/HIPAA_COMPLIANCE.md) - Compliance documentation
- [Deployment Guide](docs/DEPLOYMENT_GUIDE.md) - Production deployment
- [Agent Installation](docs/AGENT_INSTALLATION.md) - Hospital agent setup

## 🤝 Contributing

This is a production-ready healthcare platform. Contributions must adhere to:
- HIPAA compliance requirements
- Security best practices
- FHIR R4 standards
- Code review process

## 📝 License

Proprietary - All rights reserved

## 🆘 Support

For technical support or questions:
- Email: support@healthcare-platform.com
- Documentation: https://docs.healthcare-platform.com
- Status: https://status.healthcare-platform.com

## ⚠️ Important Notes

### Development vs Production
- **Development**: Authentication disabled, local database, sample data
- **Production**: Authentication REQUIRED, Azure SQL, encrypted PHI, audit logging

### HIPAA Compliance
- Sign Business Associate Agreements (BAAs) with all vendors
- Conduct regular security assessments
- Maintain comprehensive audit logs
- Implement breach notification procedures
- Train all users on HIPAA requirements

### Data Backup
- Automated daily backups to Azure Backup
- Point-in-time restore capability
- Geo-redundant storage for disaster recovery
- 7-year retention per HIPAA

---

**Built with ❤️ for better healthcare data management**
