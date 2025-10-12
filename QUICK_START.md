# 🚀 Quick Start Guide - Healthcare Platform

Get the healthcare platform running in **5 minutes**!

## Prerequisites Check
```bash
# Verify you have these installed:
dotnet --version          # Should be 8.0 or higher
node --version            # Should be 18.0 or higher
npm --version
docker --version          # Optional, for Docker setup
sqlcmd -?                 # SQL Server command line
```

## Option 1: Docker Compose (Fastest) ⚡

**Start everything with one command:**

```bash
# From project root
docker-compose up -d

# Wait 30 seconds for SQL Server to initialize, then:
docker-compose logs -f
```

**Access:**
- Frontend: http://localhost:3000
- Patient API: http://localhost:5001/swagger
- Family API: http://localhost:5002/swagger
- Health History API: http://localhost:5003/swagger
- Insurance API: http://localhost:5004/swagger
- Consent API: http://localhost:5005/swagger

**Stop:**
```bash
docker-compose down
```

## Option 2: Local Setup (Recommended for Development) 💻

### Step 1: Setup Database (2 minutes)

```bash
# Create databases
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_Patient"

# Apply schema
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/schemas/01_PatientService_Schema.sql

# Add stored procedures
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/stored-procedures/01_PatientService_Procedures.sql

# Load test data
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/test-data/01_PatientService_TestData.sql
```

### Step 2: Run Patient API (1 minute)

```bash
cd src/Services/PatientService/PatientService.API
dotnet restore
dotnet run
```

**Visit:** http://localhost:5001

### Step 3: Run React Frontend (2 minutes)

```bash
cd src/Frontend/PatientPortal
npm install
npm start
```

**Visit:** http://localhost:3000

## 🎯 Try These Immediately

### 1. Test API with Swagger

Navigate to: http://localhost:5001/swagger

**Try this:**
```
GET /api/v1/patients
```

You'll see 6 test patients including the Smith family!

### 2. View Test Data

**Sample Patients:**
- John Smith (Primary account holder, male, 38)
- Jane Smith (Spouse, female, 36)
- Emma Smith (Daughter, minor, 8)
- Oliver Smith (Son, minor, 5)
- Maria Garcia (Independent account, female, 30)
- Robert Johnson (Senior, male, 73)

**Get Family Members:**
```
GET /api/v1/patients/family/{JohnSmithId}
```

### 3. Search Patients

```
GET /api/v1/patients/search?searchTerm=Smith
```

### 4. Create New Patient

```
POST /api/v1/patients
{
  "firstName": "Alice",
  "lastName": "Johnson",
  "dateOfBirth": "1995-08-20",
  "gender": "Female",
  "email": "alice.johnson@email.com",
  "phoneNumber": "+1-555-0999",
  "profileType": "Self"
}
```

## 🔍 Explore the Dashboard

Open http://localhost:3000 to see:
- Insurance coverage summary
- Family member roster
- Recent activity feed
- Quick actions (QR code, add family, documents)
- Upcoming appointments

**Note:** In development mode, authentication is disabled for easy testing!

## 📊 Database Exploration

```sql
-- View all patients
SELECT Id, FirstName, LastName, DateOfBirth, ProfileType 
FROM patient.Patients 
WHERE IsDeleted = 0

-- View family relationships
SELECT p.FirstName, p.LastName, p.ProfileType, p.RelationshipToPrimary
FROM patient.Patients p
WHERE p.PrimaryAccountHolderId IS NOT NULL

-- View audit log
SELECT TOP 10 AccessType, AccessedBy, AccessedAt, PatientId
FROM patient.PatientAccessLog
ORDER BY AccessedAt DESC

-- View patient documents
SELECT d.DocumentName, d.DocumentType, d.UploadedAt, p.FirstName, p.LastName
FROM patient.PatientDocuments d
INNER JOIN patient.Patients p ON d.PatientId = p.Id
WHERE d.IsDeleted = 0
```

## 🧪 Test Azure Functions (Optional)

```bash
cd src/Services/PatientService/PatientService.Functions
func start
```

**Functions Available:**
- `DocumentOCRFunction` - OCR processing for uploaded documents
- `FHIRSyncFunction` - Daily sync to FHIR resources
- `DataCleanupFunction` - Weekly data cleanup

## 🐛 Troubleshooting

### Database Connection Issues
```bash
# Verify SQL Server is running
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT @@VERSION"

# If fails, start localdb:
sqllocaldb start mssqllocaldb
```

### Port Already in Use
```bash
# Change ports in appsettings.json:
"urls": "http://localhost:5099"  # Pick any available port
```

### Docker Issues
```bash
# Reset everything:
docker-compose down -v
docker-compose up -d --build

# View logs:
docker-compose logs -f patient-api
```

## 📚 Next Steps

### 1. Understand the Architecture
- Read `README.md` for full overview
- Study `src/Services/PatientService/` as reference implementation
- Review database schemas in `database/schemas/`

### 2. Extend the Platform
```bash
# Copy Patient Service structure for new service:
cp -r src/Services/PatientService src/Services/FamilyService

# Update namespaces, controllers, and connection strings
# Follow same patterns for consistency
```

### 3. Deploy to Azure
Follow step-by-step guide in `docs/DEPLOYMENT_GUIDE.md`

## 🎓 Key Files to Review

### Backend
```
src/Services/PatientService/
├── PatientService.API/
│   ├── Program.cs              # Startup configuration
│   ├── Controllers/
│   │   └── PatientsController.cs  # REST API endpoints
│   └── appsettings.json        # Configuration
├── PatientService.Domain/
│   ├── Entities/Patient.cs     # Patient entity model
│   └── DTOs/PatientDto.cs      # Data transfer objects
├── PatientService.Infrastructure/
│   ├── Data/PatientDbContext.cs       # EF Core DbContext
│   └── Repositories/PatientRepository.cs  # Data access
└── PatientService.Functions/
    └── Functions/              # Azure Functions
```

### Frontend
```
src/Frontend/PatientPortal/
├── src/
│   ├── App.tsx                 # Main application
│   ├── components/
│   │   └── Layout.tsx          # Navigation & layout
│   └── pages/
│       └── Dashboard.tsx       # Dashboard page
└── package.json                # Dependencies
```

### Database
```
database/
├── schemas/
│   ├── 01_PatientService_Schema.sql       # Patient tables
│   ├── 02_FamilyService_Schema.sql        # Family tables
│   ├── 03_HealthHistoryService_Schema.sql # Health tables
│   ├── 04_InsuranceService_Schema.sql     # Insurance tables
│   └── 05_ConsentAuditService_Schema.sql  # Consent/Audit tables
├── stored-procedures/
│   └── 01_PatientService_Procedures.sql   # CRUD procedures
└── test-data/
    └── 01_PatientService_TestData.sql     # Sample data
```

## 🆘 Getting Help

### Documentation
- `README.md` - Complete project documentation
- `PROJECT_SUMMARY.md` - What's built and what's next
- `docs/DEPLOYMENT_GUIDE.md` - Azure deployment
- Inline code comments - Thorough explanations

### Sample Data
All test data uses realistic names and scenarios:
- Complete family (Smith family)
- Multiple document types
- Sample allergies and notes
- Comprehensive audit trail

### Development Mode
- **No authentication required** for API calls
- All Swagger endpoints work immediately
- Sample data pre-loaded
- Detailed error messages

## 🎉 You're Ready!

The healthcare platform is now running. Start by:
1. ✅ Exploring Swagger UI at http://localhost:5001/swagger
2. ✅ Viewing the dashboard at http://localhost:3000
3. ✅ Testing API endpoints with sample data
4. ✅ Reviewing code in `PatientService` for patterns
5. ✅ Extending with additional services

---

**Questions? Issues?**
- Review comprehensive README.md
- Check PROJECT_SUMMARY.md for what's complete
- All code is thoroughly commented
- Database schemas include descriptions

**Happy coding! 🚀**
