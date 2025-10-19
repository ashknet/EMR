# Debugging Large Symbols Issue

## Quick Checks:

### 1. Check if API is Running
Open in browser: https://localhost:58069/api/dashboard/3e35157e-ec48-4e9a-9456-23961eea2de0

You should see JSON data. If you get an error, the API isn't running or there's a database issue.

### 2. Check Browser Console
1. Press F12 in your browser
2. Go to Console tab
3. Look for red errors
4. Share any error messages you see

### 3. Check Browser Zoom
1. Press Ctrl+0 (Windows) or Cmd+0 (Mac) to reset zoom to 100%
2. Make sure your browser window isn't too narrow

### 4. Try Clearing Cache
1. Press Ctrl+Shift+R (Windows) or Cmd+Shift+R (Mac) for hard refresh
2. Or clear browser cache completely

### 5. Check if Data is Loading
Open DevTools (F12) > Network tab > reload page
- Look for a request to `/api/dashboard/...`
- Check if it returns 200 OK or an error

## Common Issues:

### Issue: "Only see one big symbol per page"
**Possible Causes:**
- Browser zoom is very high (> 200%)
- CSS is broken/not loading
- Page is stuck in loading/error state
- Data not loading from API

### Issue: API not responding
**Fix:**
1. Make sure PatientService is running
2. Trust SSL certificate: `dotnet dev-certs https --trust`
3. Check database has patient data

### Issue: Patient ID not found
**Fix:**
Run this SQL to get your patient ID:
```sql
USE HealthcarePlatform;
SELECT TOP 1 PatientId FROM pt.Patients WHERE IsActive = 1;
```

Then update `PatientPortal/src/config/mockUser.ts` with that ID.

