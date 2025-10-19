# Development Setup Guide

## Setting Up Mock User for Local Development

To test the Patient Portal locally without Azure AD authentication, follow these steps:

### Step 1: Get Patient ID from Database

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance
3. Run the query from `database/GetPatientId.sql`:

```sql
USE HealthcarePlatform;
GO

SELECT TOP 1 
    PatientId,
    FirstName,
    LastName,
    Email
FROM pt.Patients 
WHERE IsActive = 1 
  AND IsDeleted = 0
ORDER BY CreatedAt ASC;
```

4. Copy the `PatientId` value (it will be a GUID like `A1234567-89AB-CDEF-0123-456789ABCDEF`)

### Step 2: Update Mock User Configuration

1. Open `PatientPortal/src/config/mockUser.ts`
2. Replace the `MOCK_PATIENT_ID` value with your actual Patient ID:

```typescript
export const MOCK_PATIENT_ID = 'YOUR-PATIENT-ID-HERE'; // Replace with actual GUID
```

3. Save the file

### Step 3: Restart Development Server

```bash
cd PatientPortal
npm run dev
```

### Step 4: Test the Application

Open your browser and navigate to `http://localhost:5173` (or whatever port Vite assigns).

You should now see the dashboard with actual data from your database!

---

## Troubleshooting

### Issue: "Patient not found" error
- Make sure you have run the sample data script (`database/03_SampleData.sql`)
- Verify the Patient ID is correct in `mockUser.ts`
- Check that your API is running on `https://localhost:58069`

### Issue: CORS errors
- Make sure your PatientService API has CORS configured to allow `http://localhost:5173`

### Issue: SSL Certificate errors
- Your API is using HTTPS. You may need to trust the development certificate
- Run in PowerShell: `dotnet dev-certs https --trust`

---

## API Endpoint Configuration

The API endpoint is configured in `src/config/apiConfig.ts`:
- Default: `https://localhost:58069/api`
- You can override this with environment variable `VITE_API_BASE_URL`

---

## Production Notes

⚠️ **Important**: This mock user configuration is only for development!

In production, the actual patient ID will come from:
- Azure AD authentication
- User session/token claims
- Secure backend authentication

