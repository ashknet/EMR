# 🚀 Quick Run Guide - Get All Services Running in 5 Minutes!

## Prerequisites Check ✅
```bash
dotnet --version  # Should be 8.0+
node --version    # Should be 18.0+
docker --version  # Optional
```

## Option 1: Docker Compose (Fastest! One Command!) 🐳

```bash
# From project root
docker-compose up -d

# Wait 30 seconds, then check logs
docker-compose logs -f
```

**Access Services:**
- Patient Portal: http://localhost:3000
- Patient API: http://localhost:5001/swagger
- Family API: http://localhost:5002/swagger
- Health History API: http://localhost:5003/swagger

## Option 2: Local Development (Step by Step) 💻

### Step 1: Setup Databases (2 minutes)

```bash
# Create and setup Patient Service database
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_Patient"
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/schemas/01_PatientService_Schema.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/stored-procedures/01_PatientService_Procedures.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/test-data/01_PatientService_TestData.sql

# Create Family Service database
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_Family"
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Family -i database/schemas/02_FamilyService_Schema.sql

# Create Health History Service database
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_HealthHistory"
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_HealthHistory -i database/schemas/03_HealthHistoryService_Schema.sql
```

### Step 2: Run APIs (Open 3 terminals)

**Terminal 1 - Patient Service:**
```bash
cd src/Services/PatientService/PatientService.API
dotnet restore
dotnet run
```
✅ Running on: **http://localhost:5001**

**Terminal 2 - Family Service:**
```bash
cd src/Services/FamilyService/FamilyService.API
dotnet restore
dotnet run  
```
✅ Running on: **http://localhost:5002**

**Terminal 3 - Health History Service:**
```bash
cd src/Services/HealthHistoryService/HealthHistoryService.API
dotnet restore
dotnet run
```
✅ Running on: **http://localhost:5003**

### Step 3: Run Frontend (Optional)

**Terminal 4 - React Portal:**
```bash
cd src/Frontend/PatientPortal
npm install
npm start
```
✅ Running on: **http://localhost:3000**

## Option 3: Visual Studio 2022 🎯

1. Open `HealthcarePlatform.sln`
2. Right-click Solution → Set Startup Projects → Multiple
3. Select:
   - ☑️ PatientService.API
   - ☑️ FamilyService.API
   - ☑️ HealthHistoryService.API
4. Press **F5**

All services will start with debugging attached!

## 🧪 Test the Services

### Test #1: Get All Patients (Includes Smith Family!)
```bash
curl http://localhost:5001/api/v1/patients
```

OR visit: **http://localhost:5001/swagger** and click "Try it out"

**Expected Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "...",
      "firstName": "John",
      "lastName": "Smith",
      "profileType": "Self",
      "isPrimaryAccountHolder": true
    },
    {
      "id": "...",
      "firstName": "Jane",
      "lastName": "Smith",
      "profileType": "Spouse"
    },
    // ... more family members
  ]
}
```

### Test #2: Create New Family Group
Visit: **http://localhost:5002/swagger**

POST `/api/v1/familygroups`:
```json
{
  "familyName": "Johnson Family",
  "primaryAccountHolderId": "{paste-patient-id-here}",
  "primaryEmail": "johnson.family@email.com",
  "city": "New York",
  "state": "NY"
}
```

### Test #3: Add Allergy for Patient
Visit: **http://localhost:5003/swagger**

POST `/api/v1/allergies`:
```json
{
  "patientId": "{paste-patient-id-here}",
  "allergenName": "Penicillin",
  "category": "medication",
  "criticality": "high",
  "severity": "severe",
  "reactionDescription": "Severe rash and difficulty breathing"
}
```

## 🎉 You're Now Running a Healthcare Platform!

### What You Can Do:
- ✅ Create patients and family members
- ✅ Manage family groups and relationships
- ✅ Track allergies and medications
- ✅ Search patients
- ✅ View comprehensive audit logs
- ✅ Explore Swagger UI for all endpoints

### Sample Data Available:
- **6 Patients** (Smith family: John, Jane, Emma, Oliver + Maria Garcia + Robert Johnson)
- **5 Documents** (insurance cards, lab results, vaccinations)
- **5 Notes** (allergies, reminders, conditions)
- **Audit Trail** (all data access logged)

## 🔍 Explore the APIs

### Patient Service (Port 5001)
```
GET    /api/v1/patients              - Get all patients
GET    /api/v1/patients/{id}         - Get patient by ID
POST   /api/v1/patients              - Create patient
PUT    /api/v1/patients/{id}         - Update patient
DELETE /api/v1/patients/{id}         - Delete patient
GET    /api/v1/patients/search       - Search patients
GET    /api/v1/patients/family/{id}  - Get family members
```

### Family Service (Port 5002)
```
GET    /api/v1/familygroups                           - Get all family groups
GET    /api/v1/familygroups/{id}                      - Get family group by ID
GET    /api/v1/familygroups/by-account-holder/{id}   - Get by account holder
POST   /api/v1/familygroups                           - Create family group
POST   /api/v1/familygroups/members                   - Add family member
DELETE /api/v1/familygroups/{id}                      - Delete family group
```

### Health History Service (Port 5003)
```
Allergies:
GET    /api/v1/allergies/patient/{patientId}  - Get patient allergies
GET    /api/v1/allergies/{id}                 - Get allergy by ID
POST   /api/v1/allergies                      - Create allergy
DELETE /api/v1/allergies/{id}                 - Delete allergy

Medications:
GET    /api/v1/medications/patient/{patientId}  - Get patient medications
POST   /api/v1/medications                      - Create medication
PUT    /api/v1/medications/{id}/discontinue     - Discontinue medication
```

## 🛠️ Troubleshooting

### Port Already in Use
```bash
# Change port in launchSettings.json or use:
dotnet run --urls "http://localhost:5099"
```

### Database Connection Issues
```bash
# Verify SQL Server is running
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT @@VERSION"

# Start LocalDB if needed
sqllocaldb start mssqllocaldb

# List databases
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT name FROM sys.databases"
```

### Docker Issues
```bash
# Reset everything
docker-compose down -v
docker-compose up -d --build

# View specific service logs
docker-compose logs -f patient-api
```

## 📊 Quick Stats Dashboard

After running for a few minutes, check:

### Patient Service
- Patients Created: Check `GET /api/v1/patients`
- Family Members: Check `GET /api/v1/patients/family/{id}`
- Database Records: 6 pre-loaded patients

### Family Service
- Family Groups: Create and manage via `/api/v1/familygroups`
- Relationships: Track family connections

### Health History Service
- Allergies: Add critical allergy information
- Medications: Track current and historical medications

## 🎯 Next Steps

1. **Explore Swagger UI** - Interactive API documentation
2. **Review Test Data** - See realistic patient scenarios
3. **Create New Records** - Test CRUD operations
4. **Check Audit Logs** - View HIPAA-compliant tracking
5. **Deploy to Azure** - Follow DEPLOYMENT_GUIDE.md

## 📞 Need Help?

- **Swagger Not Loading?** Make sure service is running and check console for errors
- **Database Errors?** Verify connection string in appsettings.json
- **Port Conflicts?** Change ports in launchSettings.json
- **Build Errors?** Run `dotnet clean` and `dotnet restore`

---

## ✨ Success!

You now have a running HIPAA-compliant healthcare platform with:
- ✅ 3 fully functional microservices
- ✅ Complete patient and family management
- ✅ Critical allergy and medication tracking
- ✅ Comprehensive audit logging
- ✅ Production-ready architecture

**Start building your healthcare solution today!** 🚀
