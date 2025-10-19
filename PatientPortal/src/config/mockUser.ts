/**
 * Mock User Configuration for Development
 * 
 * To get your Patient ID from the database, run this SQL query:
 * SELECT TOP 1 PatientId, FirstName, LastName, Email FROM pt.Patients WHERE IsActive = 1
 * 
 * Then update the MOCK_PATIENT_ID below with the actual GUID from your database
 */

// Update this with an actual patient ID from your database
export const MOCK_PATIENT_ID = '3e35157e-ec48-4e9a-9456-23961eea2de0'; // Replace with actual GUID

export const MOCK_USER = {
  patientId: MOCK_PATIENT_ID,
  name: 'Ashok Thotakura',
  email: 'athotakura@err.com',
};

/**
 * Instructions:
 * 1. Run the SQL query above in SQL Server Management Studio (SSMS)
 * 2. Copy the PatientId GUID value
 * 3. Replace the MOCK_PATIENT_ID value above with your actual Patient ID
 * 4. Save this file
 * 5. Restart your React development server
 */

