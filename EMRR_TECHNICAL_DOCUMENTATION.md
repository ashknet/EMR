# EMRR Platform - Complete Technical Documentation

## 📋 **Table of Contents**

1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Technology Stack](#technology-stack)
4. [Database Design](#database-design)
5. [Authentication & Security](#authentication--security)
6. [API Architecture](#api-architecture)
7. [Frontend Architecture](#frontend-architecture)
8. [Backend Services](#backend-services)
9. [Deployment Architecture](#deployment-architecture)
10. [Development Environment](#development-environment)
11. [Configuration Management](#configuration-management)
12. [Testing Strategy](#testing-strategy)
13. [Performance Considerations](#performance-considerations)
14. [Security Implementation](#security-implementation)
15. [Monitoring & Logging](#monitoring--logging)
16. [Future Enhancements](#future-enhancements)

---

## 🏥 **Project Overview**

### **EMRR (Electronic Medical Record & Registry) Platform**
A comprehensive healthcare management system designed to streamline patient care, provider workflows, and administrative processes through intelligent automation and modern technology.

### **Core Objectives**
- **Patient-Centric Care**: Seamless patient experience with secure access to medical records
- **Provider Efficiency**: Streamlined workflows for healthcare professionals
- **Data Integrity**: HIPAA-compliant data management and security
- **Interoperability**: Integration with existing healthcare systems
- **Scalability**: Support for multiple healthcare facilities and growing patient bases

### **Key Features**
- Patient Portal with comprehensive medical record access
- Hospital/Provider management system
- AI-powered EMR agent for automated patient intake
- Real-time data synchronization
- Multi-tenant architecture
- Advanced analytics and reporting

---

## 🏗️ **System Architecture**

### **High-Level Architecture**

```
┌─────────────────────────────────────────────────────────────────┐
│                        EMRR Platform                           │
├─────────────────────────────────────────────────────────────────┤
│  Frontend Layer (React/TypeScript)                             │
│  ├── Patient Portal (React SPA)                                │
│  ├── Hospital Agent (WPF Desktop)                              │
│  └── Admin Dashboard (React)                                   │
├─────────────────────────────────────────────────────────────────┤
│  API Gateway & Authentication                                  │
│  ├── Microsoft Entra ID (Azure AD)                             │
│  ├── Google OAuth 2.0                                          │
│  └── JWT Token Management                                      │
├─────────────────────────────────────────────────────────────────┤
│  Backend Services (.NET Core)                                  │
│  ├── Patient Service API                                       │
│  ├── Hospital Agent Service API                                │
│  └── Authentication Service                                    │
├─────────────────────────────────────────────────────────────────┤
│  Data Layer                                                    │
│  ├── SQL Server Database                                       │
│  ├── Entity Framework Core                                     │
│  └── Data Access Layer                                         │
└─────────────────────────────────────────────────────────────────┘
```

### **Microservices Architecture**

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Patient Portal │    │ Hospital Agent  │    │ Admin Portal    │
│  (React SPA)    │    │ (WPF Desktop)   │    │ (React SPA)     │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          └──────────────────────┼──────────────────────┘
                                 │
                    ┌─────────────┴─────────────┐
                    │     API Gateway           │
                    │  (Load Balancer/Routing)  │
                    └─────────────┬─────────────┘
                                  │
        ┌─────────────────────────┼─────────────────────────┐
        │                         │                         │
┌───────▼────────┐    ┌──────────▼──────────┐    ┌────────▼────────┐
│ Patient Service│    │ Hospital Agent      │    │ Authentication  │
│ API            │    │ Service API         │    │ Service         │
│ (.NET Core)    │    │ (.NET Core)         │    │ (.NET Core)     │
└───────┬────────┘    └──────────┬──────────┘    └────────┬────────┘
        │                        │                        │
        └────────────────────────┼────────────────────────┘
                                 │
                    ┌─────────────▼─────────────┐
                    │    SQL Server Database   │
                    │   (Primary Data Store)   │
                    └───────────────────────────┘
```

---

## 💻 **Technology Stack**

### **Frontend Technologies**

#### **Patient Portal (React/TypeScript)**
```typescript
// Core Technologies
- React 19.2.0 (Latest with new JSX transform)
- TypeScript 5.7.2 (Type safety and modern features)
- Vite 6.0.1 (Fast build tool and dev server)
- React Router DOM 6.28.0 (Client-side routing)

// UI & Styling
- Tailwind CSS 3.4.17 (Utility-first CSS framework)
- Custom Design System (Healthcare-focused components)
- Responsive Design (Mobile-first approach)
- Accessibility (WCAG AA compliant)

// State Management & Data Fetching
- React Query (TanStack Query) 5.59.0 (Server state management)
- React Hooks (Local state management)
- Context API (Global state)

// Authentication
- MSAL (Microsoft Authentication Library) 3.7.0
- MSAL React 2.2.0 (React integration)
- Google OAuth 2.0 (Custom implementation)

// Development Tools
- ESLint 9.15.0 (Code linting)
- TypeScript ESLint (TypeScript-specific linting)
- Vitest 2.1.8 (Unit testing)
- PostCSS 8.5.0 (CSS processing)
```

#### **Hospital Agent (WPF Desktop)**
```csharp
// Core Technologies
- .NET 8.0 (Latest LTS version)
- WPF (Windows Presentation Foundation)
- C# 12.0 (Latest language features)
- MVVM Pattern (Model-View-ViewModel)

// UI Framework
- Material Design in XAML Toolkit
- Custom Healthcare UI Components
- Responsive Layout System
- Accessibility Features

// Data & Communication
- Entity Framework Core 8.0
- HTTP Client (API communication)
- SignalR (Real-time communication)
- JSON.NET (Serialization)
```

### **Backend Technologies**

#### **API Services (.NET Core)**
```csharp
// Core Framework
- .NET 8.0 (Latest LTS version)
- ASP.NET Core 8.0 (Web API framework)
- C# 12.0 (Latest language features)

// Architecture Patterns
- Clean Architecture (Domain, Application, Infrastructure layers)
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Unit of Work Pattern
- Dependency Injection

// Data Access
- Entity Framework Core 8.0 (ORM)
- SQL Server (Primary database)
- Dapper (High-performance queries)
- AutoMapper (Object mapping)

// Authentication & Security
- JWT Bearer Authentication
- Microsoft Entra ID Integration
- Google OAuth 2.0
- Role-based Authorization
- HIPAA Compliance Features

// Additional Libraries
- FluentValidation (Input validation)
- Serilog (Structured logging)
- Swagger/OpenAPI (API documentation)
- MediatR (CQRS implementation)
```

### **Database Technologies**

#### **SQL Server Database**
```sql
-- Database Engine
- SQL Server 2022 (Latest version)
- SQL Server Management Studio (SSMS)
- Azure SQL Database (Cloud option)

-- Database Design
- Normalized Schema (3NF compliance)
- Stored Procedures (Business logic)
- Functions (Reusable logic)
- Views (Data abstraction)
- Indexes (Performance optimization)
- Triggers (Data integrity)

-- Security Features
- Row-Level Security (RLS)
- Dynamic Data Masking
- Always Encrypted
- Transparent Data Encryption (TDE)
- Audit Logging
```

---

## 🗄️ **Database Design**

### **Database Schema Overview**

```sql
-- Core Schema Structure
CREATE SCHEMA pt;  -- Patient data
CREATE SCHEMA ag;  -- Agent/Hospital data
CREATE SCHEMA sec; -- Security and authentication
CREATE SCHEMA meta; -- Metadata and configuration
```

### **Patient Schema (pt)**

#### **Core Patient Tables**
```sql
-- Primary patient information
CREATE TABLE pt.Patients (
    PatientId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100),
    LastName NVARCHAR(100) NOT NULL,
    Suffix NVARCHAR(20),
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(20) NOT NULL,
    GenderId INT,
    MaritalStatus NVARCHAR(50),
    MaritalStatusId INT,
    Race NVARCHAR(100),
    RaceId INT,
    Ethnicity NVARCHAR(100),
    PrimaryLanguageId INT,
    Email NVARCHAR(255),
    Phone NVARCHAR(20),
    AddressLine1 NVARCHAR(255),
    AddressLine2 NVARCHAR(255),
    City NVARCHAR(100),
    State NVARCHAR(50),
    ZipCode NVARCHAR(20),
    Country NVARCHAR(100),
    EmergencyContactName NVARCHAR(200),
    EmergencyContactPhone NVARCHAR(20),
    EmergencyContactRelationship NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    ModifiedBy NVARCHAR(100)
);

-- Patient medical history
CREATE TABLE pt.MedicalHistory (
    MedicalHistoryId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    ConditionName NVARCHAR(200) NOT NULL,
    DiagnosisDate DATE,
    Status NVARCHAR(50),
    Notes NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId)
);

-- Patient allergies
CREATE TABLE pt.Allergies (
    AllergyId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    AllergenName NVARCHAR(200) NOT NULL,
    AllergenType NVARCHAR(100),
    Severity NVARCHAR(50),
    Reaction NVARCHAR(500),
    OnsetDate DATE,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId)
);

-- Patient medications
CREATE TABLE pt.Medications (
    MedicationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    MedicationName NVARCHAR(200) NOT NULL,
    Dosage NVARCHAR(100),
    Frequency NVARCHAR(100),
    StartDate DATE,
    EndDate DATE,
    PrescribingPhysician NVARCHAR(200),
    Notes NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId)
);

-- Insurance information
CREATE TABLE pt.Insurance (
    InsuranceId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    InsuranceType NVARCHAR(50) NOT NULL,
    PayerName NVARCHAR(200) NOT NULL,
    PlanName NVARCHAR(200),
    MemberId NVARCHAR(100),
    GroupNumber NVARCHAR(100),
    EffectiveDate DATE,
    ExpirationDate DATE,
    IsPrimary BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId)
);

-- Family members
CREATE TABLE pt.FamilyMembers (
    FamilyMemberId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    Relationship NVARCHAR(100) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(20),
    MedicalHistory NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId)
);

-- Healthcare providers
CREATE TABLE pt.Providers (
    ProviderId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    ProviderType NVARCHAR(100) NOT NULL,
    ProviderName NVARCHAR(200) NOT NULL,
    Specialty NVARCHAR(200),
    Phone NVARCHAR(20),
    Email NVARCHAR(255),
    Address NVARCHAR(500),
    IsPrimary BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId)
);
```

### **Agent Schema (ag)**

#### **Hospital Agent Tables**
```sql
-- Hospital agents
CREATE TABLE ag.HospitalAgents (
    AgentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentName NVARCHAR(200) NOT NULL,
    HospitalName NVARCHAR(200) NOT NULL,
    ContactEmail NVARCHAR(255),
    ContactPhone NVARCHAR(20),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE()
);

-- Agent sessions
CREATE TABLE ag.AgentSessions (
    SessionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    SessionStart DATETIME2 DEFAULT GETUTCDATE(),
    SessionEnd DATETIME2,
    Status NVARCHAR(50) DEFAULT 'Active',
    PatientCount INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (AgentId) REFERENCES ag.HospitalAgents(AgentId)
);

-- Intake requests
CREATE TABLE ag.IntakeRequests (
    IntakeRequestId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SessionId UNIQUEIDENTIFIER NOT NULL,
    PatientId UNIQUEIDENTIFIER,
    RequestType NVARCHAR(100) NOT NULL,
    RequestData NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT 'Pending',
    ProcessedDate DATETIME2,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (SessionId) REFERENCES ag.AgentSessions(SessionId),
    FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId)
);

-- Transfer logs
CREATE TABLE ag.TransferLogs (
    TransferLogId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FromAgentId UNIQUEIDENTIFIER,
    ToAgentId UNIQUEIDENTIFIER,
    PatientId UNIQUEIDENTIFIER NOT NULL,
    TransferReason NVARCHAR(500),
    TransferDate DATETIME2 DEFAULT GETUTCDATE(),
    Status NVARCHAR(50) DEFAULT 'Completed',
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (FromAgentId) REFERENCES ag.HospitalAgents(AgentId),
    FOREIGN KEY (ToAgentId) REFERENCES ag.HospitalAgents(AgentId),
    FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId)
);
```

### **Security Schema (sec)**

#### **Authentication & Authorization**
```sql
-- User accounts
CREATE TABLE sec.Users (
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    UserType NVARCHAR(50) NOT NULL, -- 'Patient', 'Hospital', 'Admin'
    IsActive BIT DEFAULT 1,
    EmailVerified BIT DEFAULT 0,
    LastLoginDate DATETIME2,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE()
);

-- User roles
CREATE TABLE sec.Roles (
    RoleId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoleName NVARCHAR(100) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE()
);

-- User role assignments
CREATE TABLE sec.UserRoles (
    UserRoleId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    AssignedDate DATETIME2 DEFAULT GETUTCDATE(),
    AssignedBy NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES sec.Users(UserId),
    FOREIGN KEY (RoleId) REFERENCES sec.Roles(RoleId)
);

-- Audit logs
CREATE TABLE sec.AuditLogs (
    AuditLogId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER,
    Action NVARCHAR(100) NOT NULL,
    TableName NVARCHAR(100),
    RecordId UNIQUEIDENTIFIER,
    OldValues NVARCHAR(MAX),
    NewValues NVARCHAR(MAX),
    IpAddress NVARCHAR(45),
    UserAgent NVARCHAR(500),
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES sec.Users(UserId)
);
```

### **Metadata Schema (meta)**

#### **Configuration & Metadata**
```sql
-- System configuration
CREATE TABLE meta.SystemConfig (
    ConfigId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ConfigKey NVARCHAR(100) UNIQUE NOT NULL,
    ConfigValue NVARCHAR(MAX),
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE()
);

-- Lookup tables
CREATE TABLE meta.LookupTables (
    LookupTableId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TableName NVARCHAR(100) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE()
);

-- Lookup values
CREATE TABLE meta.LookupValues (
    LookupValueId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LookupTableId UNIQUEIDENTIFIER NOT NULL,
    Value NVARCHAR(200) NOT NULL,
    DisplayName NVARCHAR(200) NOT NULL,
    SortOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (LookupTableId) REFERENCES meta.LookupTables(LookupTableId)
);
```

### **Database Indexes & Performance**

```sql
-- Performance indexes
CREATE INDEX IX_Patients_Email ON pt.Patients(Email);
CREATE INDEX IX_Patients_LastName_FirstName ON pt.Patients(LastName, FirstName);
CREATE INDEX IX_Patients_DateOfBirth ON pt.Patients(DateOfBirth);
CREATE INDEX IX_MedicalHistory_PatientId ON pt.MedicalHistory(PatientId);
CREATE INDEX IX_Allergies_PatientId ON pt.Allergies(PatientId);
CREATE INDEX IX_Medications_PatientId ON pt.Medications(PatientId);
CREATE INDEX IX_Insurance_PatientId ON pt.Insurance(PatientId);
CREATE INDEX IX_FamilyMembers_PatientId ON pt.FamilyMembers(PatientId);
CREATE INDEX IX_Providers_PatientId ON pt.Providers(PatientId);

-- Audit and security indexes
CREATE INDEX IX_AuditLogs_UserId ON sec.AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_CreatedDate ON sec.AuditLogs(CreatedDate);
CREATE INDEX IX_Users_Email ON sec.Users(Email);
CREATE INDEX IX_UserRoles_UserId ON sec.UserRoles(UserId);
```

---

## 🔐 **Authentication & Security**

### **Authentication Architecture**

#### **Multi-Provider Authentication**
```typescript
// Authentication Providers
interface AuthProvider {
  name: 'microsoft' | 'google' | 'local';
  config: AuthConfig;
  login(): Promise<AuthResult>;
  logout(): Promise<void>;
  getUserInfo(): Promise<UserInfo>;
}

// Microsoft Entra ID Configuration
const microsoftAuthConfig = {
  clientId: process.env.VITE_AZURE_AD_CLIENT_ID,
  tenantId: process.env.VITE_AZURE_AD_TENANT_ID,
  authority: `https://login.microsoftonline.com/${process.env.VITE_AZURE_AD_TENANT_ID}`,
  scopes: ['User.Read', 'openid', 'profile', 'email'],
  redirectUri: process.env.VITE_AZURE_AD_REDIRECT_URI || 'http://localhost:5173'
};

// Google OAuth Configuration
const googleAuthConfig = {
  clientId: 'your-google-client-id',
  scopes: ['openid', 'profile', 'email'],
  redirectUri: 'http://localhost:5173'
};
```

#### **JWT Token Management**
```csharp
// JWT Configuration
public class JwtSettings
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

// Token Service
public class TokenService
{
    public string GenerateAccessToken(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim("userType", user.UserType)
        };

        roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### **Security Implementation**

#### **HIPAA Compliance Features**
```csharp
// Data Encryption
public class EncryptionService
{
    public string EncryptSensitiveData(string data)
    {
        // AES-256 encryption for sensitive data
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _initializationVector;
        
        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);
        
        swEncrypt.Write(data);
        return Convert.ToBase64String(msEncrypt.ToArray());
    }
}

// Audit Logging
public class AuditService
{
    public async Task LogAccess(string userId, string action, string resource)
    {
        var auditLog = new AuditLog
        {
            UserId = Guid.Parse(userId),
            Action = action,
            Resource = resource,
            IpAddress = GetClientIpAddress(),
            UserAgent = GetUserAgent(),
            Timestamp = DateTime.UtcNow
        };
        
        await _auditRepository.CreateAsync(auditLog);
    }
}
```

#### **Role-Based Access Control (RBAC)**
```csharp
// Authorization Policies
public class AuthorizationPolicies
{
    public static void Configure(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("PatientAccess", policy =>
                policy.RequireRole("Patient"));
                
            options.AddPolicy("HospitalAccess", policy =>
                policy.RequireRole("Hospital", "Admin"));
                
            options.AddPolicy("AdminAccess", policy =>
                policy.RequireRole("Admin"));
                
            options.AddPolicy("HIPAACompliant", policy =>
                policy.RequireClaim("hipaaCompliant", "true"));
        });
    }
}

// Custom Authorization Attributes
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class HIPAACompliantAttribute : AuthorizeAttribute
{
    public HIPAACompliantAttribute()
    {
        Policy = "HIPAACompliant";
    }
}
```

---

## 🌐 **API Architecture**

### **RESTful API Design**

#### **API Structure**
```
https://localhost:58069/api/
├── patients/
│   ├── GET /patients/{id}                    # Get patient details
│   ├── PUT /patients/{id}                    # Update patient
│   ├── GET /patients/{id}/medical-history    # Get medical history
│   ├── POST /patients/{id}/medical-history   # Add medical history
│   ├── GET /patients/{id}/allergies          # Get allergies
│   ├── POST /patients/{id}/allergies         # Add allergy
│   ├── GET /patients/{id}/medications        # Get medications
│   ├── POST /patients/{id}/medications       # Add medication
│   ├── GET /patients/{id}/insurance          # Get insurance
│   ├── POST /patients/{id}/insurance         # Add insurance
│   ├── GET /patients/{id}/family             # Get family members
│   ├── POST /patients/{id}/family            # Add family member
│   └── GET /patients/{id}/providers          # Get providers
├── agents/
│   ├── GET /agents                           # Get all agents
│   ├── POST /agents                          # Create agent
│   ├── GET /agents/{id}/sessions             # Get agent sessions
│   ├── POST /agents/{id}/sessions            # Start session
│   └── GET /agents/{id}/intake-requests      # Get intake requests
├── auth/
│   ├── POST /auth/login                      # User login
│   ├── POST /auth/register                   # User registration
│   ├── POST /auth/refresh                    # Refresh token
│   ├── POST /auth/logout                     # User logout
│   └── GET /auth/user                        # Get current user
└── metadata/
    ├── GET /metadata/patients                # Get patient metadata
    ├── GET /metadata/agents                  # Get agent metadata
    └── GET /metadata/lookup/{table}          # Get lookup values
```

#### **API Controllers**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IAuditService _auditService;

    [HttpGet("{id}")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        await _auditService.LogAccess(User.GetUserId(), "Read", $"Patient/{id}");
        return Ok(patient);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "PatientAccess")]
    [HIPAACompliant]
    public async Task<ActionResult<PatientDto>> UpdatePatient(Guid id, UpdatePatientDto dto)
    {
        var patient = await _patientService.UpdateAsync(id, dto);
        await _auditService.LogAccess(User.GetUserId(), "Update", $"Patient/{id}");
        return Ok(patient);
    }
}
```

### **API Response Standards**

#### **Standard Response Format**
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Success Response
{
    "success": true,
    "data": {
        "patientId": "3e35157e-ec48-4e9a-9456-23961eea2de0",
        "firstName": "Ashok",
        "lastName": "Thotakura",
        "email": "athotakura@err.com"
    },
    "message": "Patient retrieved successfully",
    "errors": null,
    "timestamp": "2025-01-18T10:30:00Z"
}

// Error Response
{
    "success": false,
    "data": null,
    "message": "Validation failed",
    "errors": [
        "First name is required",
        "Email format is invalid"
    ],
    "timestamp": "2025-01-18T10:30:00Z"
}
```

---

## ⚛️ **Frontend Architecture**

### **React Application Structure**

#### **Component Architecture**
```
src/
├── components/           # Reusable UI components
│   ├── Layout.tsx       # Main layout wrapper
│   ├── AuthModal.tsx    # Authentication modal
│   ├── SignupForm.tsx   # User registration form
│   ├── ProtectedRoute.tsx # Route protection
│   ├── ApiStatus.tsx    # API connection status
│   ├── TestModeToggle.tsx # Test mode toggle
│   └── DebugInfo.tsx    # Debug information
├── pages/               # Page components
│   ├── LandingPage.tsx  # Anonymous landing page
│   ├── About.tsx        # About page
│   ├── Contact.tsx      # Contact page
│   ├── Dashboard.tsx    # Patient dashboard
│   ├── Profile.tsx      # Patient profile
│   ├── MedicalHistory.tsx # Medical history
│   ├── Insurance.tsx    # Insurance information
│   ├── Providers.tsx    # Healthcare providers
│   ├── Family.tsx       # Family members
│   ├── SocialHistory.tsx # Social history
│   ├── ConsentAudit.tsx # Consent management
│   ├── Documents.tsx    # Document management
│   └── Transfers.tsx    # Transfer logs
├── config/              # Configuration files
│   ├── authConfig.ts    # Authentication configuration
│   ├── apiConfig.ts     # API configuration
│   ├── googleAuth.ts    # Google OAuth configuration
│   └── mockUser.ts      # Mock user configuration
├── hooks/               # Custom React hooks
│   ├── useAuth.ts       # Authentication hook
│   ├── useApi.ts        # API communication hook
│   └── useLocalStorage.ts # Local storage hook
├── constants/           # Application constants
│   ├── states.ts        # US states list
│   └── routes.ts        # Route definitions
├── types/               # TypeScript type definitions
│   ├── auth.ts          # Authentication types
│   ├── patient.ts       # Patient data types
│   └── api.ts           # API response types
└── utils/               # Utility functions
    ├── validation.ts    # Form validation
    ├── formatting.ts    # Data formatting
    └── helpers.ts       # General helpers
```

#### **State Management**
```typescript
// React Query Configuration
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 5 * 60 * 1000, // 5 minutes
      cacheTime: 10 * 60 * 1000, // 10 minutes
    },
  },
});

// Custom Hooks
export const usePatient = (patientId: string) => {
  return useQuery({
    queryKey: ['patient', patientId],
    queryFn: () => apiClient.get(`/patients/${patientId}`),
    enabled: !!patientId,
  });
};

export const useUpdatePatient = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: UpdatePatientDto) => 
      apiClient.put(`/patients/${data.id}`, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries(['patient', variables.id]);
    },
  });
};
```

### **UI/UX Design System**

#### **Design Tokens**
```typescript
// Color Palette
export const colors = {
  primary: {
    50: '#eff6ff',
    100: '#dbeafe',
    200: '#bfdbfe',
    300: '#93c5fd',
    400: '#60a5fa',
    500: '#3b82f6',
    600: '#2563eb',
    700: '#1d4ed8',
    800: '#1e40af',
    900: '#1e3a8a',
  },
  accent: {
    50: '#f0f9ff',
    100: '#e0f2fe',
    200: '#bae6fd',
    300: '#7dd3fc',
    400: '#38bdf8',
    500: '#0ea5e9',
    600: '#0284c7',
    700: '#0369a1',
    800: '#075985',
    900: '#0c4a6e',
  },
  success: {
    50: '#f0fdf4',
    100: '#dcfce7',
    200: '#bbf7d0',
    300: '#86efac',
    400: '#4ade80',
    500: '#22c55e',
    600: '#16a34a',
    700: '#15803d',
    800: '#166534',
    900: '#14532d',
  }
};

// Typography
export const typography = {
  fontFamily: {
    sans: ['Inter', 'system-ui', 'sans-serif'],
    mono: ['JetBrains Mono', 'monospace'],
  },
  fontSize: {
    xs: '0.75rem',
    sm: '0.875rem',
    base: '1rem',
    lg: '1.125rem',
    xl: '1.25rem',
    '2xl': '1.5rem',
    '3xl': '1.875rem',
    '4xl': '2.25rem',
    '5xl': '3rem',
  },
  fontWeight: {
    normal: '400',
    medium: '500',
    semibold: '600',
    bold: '700',
    extrabold: '800',
  }
};

// Spacing
export const spacing = {
  0: '0',
  1: '0.25rem',
  2: '0.5rem',
  3: '0.75rem',
  4: '1rem',
  5: '1.25rem',
  6: '1.5rem',
  8: '2rem',
  10: '2.5rem',
  12: '3rem',
  16: '4rem',
  20: '5rem',
  24: '6rem',
};
```

#### **Component Library**
```typescript
// Button Component
interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost';
  size?: 'sm' | 'md' | 'lg';
  disabled?: boolean;
  loading?: boolean;
  children: React.ReactNode;
  onClick?: () => void;
}

export const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  size = 'md',
  disabled = false,
  loading = false,
  children,
  onClick,
}) => {
  const baseClasses = 'font-semibold rounded-lg transition-all focus:outline-none focus:ring-2';
  const variantClasses = {
    primary: 'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500',
    secondary: 'bg-gray-600 text-white hover:bg-gray-700 focus:ring-gray-500',
    outline: 'border-2 border-blue-600 text-blue-600 hover:bg-blue-50 focus:ring-blue-500',
    ghost: 'text-blue-600 hover:bg-blue-50 focus:ring-blue-500',
  };
  const sizeClasses = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2 text-base',
    lg: 'px-6 py-3 text-lg',
  };

  return (
    <button
      className={`${baseClasses} ${variantClasses[variant]} ${sizeClasses[size]} ${
        disabled ? 'opacity-50 cursor-not-allowed' : ''
      }`}
      disabled={disabled || loading}
      onClick={onClick}
    >
      {loading ? (
        <div className="flex items-center gap-2">
          <div className="w-4 h-4 border-2 border-current border-t-transparent rounded-full animate-spin" />
          Loading...
        </div>
      ) : (
        children
      )}
    </button>
  );
};
```

---

## 🖥️ **Backend Services**

### **Clean Architecture Implementation**

#### **Project Structure**
```
PatientService/
├── PatientService.API/           # Web API layer
│   ├── Controllers/              # API controllers
│   ├── Models/                   # DTOs and request/response models
│   ├── Profiles/                 # AutoMapper profiles
│   ├── Middleware/               # Custom middleware
│   ├── Filters/                  # Action filters
│   └── Program.cs                # Application entry point
├── PatientService.Domain/        # Domain layer
│   ├── Entities/                 # Domain entities
│   ├── Interfaces/               # Repository interfaces
│   ├── Services/                 # Domain services
│   ├── ValueObjects/             # Value objects
│   └── Exceptions/               # Domain exceptions
├── PatientService.Infrastructure/ # Infrastructure layer
│   ├── Data/                     # Data context and configurations
│   ├── Repositories/             # Repository implementations
│   ├── Services/                 # Infrastructure services
│   └── Configurations/           # Entity configurations
└── PatientService.Tests/         # Unit and integration tests
    ├── Controllers/              # Controller tests
    ├── Services/                 # Service tests
    └── Repositories/             # Repository tests
```

#### **Domain Entities**
```csharp
// Patient Entity
public class Patient : BaseEntity
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Suffix { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public int? GenderId { get; set; }
    public string MaritalStatus { get; set; }
    public int? MaritalStatusId { get; set; }
    public string Race { get; set; }
    public int? RaceId { get; set; }
    public string Ethnicity { get; set; }
    public int? PrimaryLanguageId { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Address Address { get; set; }
    public EmergencyContact EmergencyContact { get; set; }
    
    // Navigation properties
    public ICollection<MedicalHistory> MedicalHistory { get; set; }
    public ICollection<Allergy> Allergies { get; set; }
    public ICollection<Medication> Medications { get; set; }
    public ICollection<Insurance> Insurance { get; set; }
    public ICollection<FamilyMember> FamilyMembers { get; set; }
    public ICollection<Provider> Providers { get; set; }
}

// Base Entity
public abstract class BaseEntity
{
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }
}
```

#### **Repository Pattern**
```csharp
// Generic Repository Interface
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

// Patient Repository Interface
public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
    Task<Patient> GetWithDetailsAsync(Guid id);
}

// Patient Repository Implementation
public class PatientRepository : IPatientRepository
{
    private readonly PatientDbContext _context;
    
    public PatientRepository(PatientDbContext context)
    {
        _context = context;
    }
    
    public async Task<Patient> GetByIdAsync(Guid id)
    {
        return await _context.Patients
            .Include(p => p.MedicalHistory)
            .Include(p => p.Allergies)
            .Include(p => p.Medications)
            .Include(p => p.Insurance)
            .Include(p => p.FamilyMembers)
            .Include(p => p.Providers)
            .FirstOrDefaultAsync(p => p.PatientId == id && p.IsActive);
    }
    
    public async Task<Patient> GetByEmailAsync(string email)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.Email == email && p.IsActive);
    }
}
```

### **Service Layer**

#### **Domain Services**
```csharp
// Patient Service Interface
public interface IPatientService
{
    Task<PatientDto> GetByIdAsync(Guid id);
    Task<IEnumerable<PatientDto>> GetAllAsync();
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
    Task<PatientDto> UpdateAsync(Guid id, UpdatePatientDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<PatientDto>> SearchAsync(string searchTerm);
}

// Patient Service Implementation
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    
    public PatientService(
        IPatientRepository patientRepository,
        IMapper mapper,
        IAuditService auditService)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
        _auditService = auditService;
    }
    
    public async Task<PatientDto> GetByIdAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            throw new NotFoundException($"Patient with ID {id} not found");
            
        return _mapper.Map<PatientDto>(patient);
    }
    
    public async Task<PatientDto> UpdateAsync(Guid id, UpdatePatientDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            throw new NotFoundException($"Patient with ID {id} not found");
            
        _mapper.Map(dto, patient);
        patient.ModifiedDate = DateTime.UtcNow;
        
        await _patientRepository.UpdateAsync(patient);
        await _auditService.LogAccessAsync("Update", $"Patient/{id}");
        
        return _mapper.Map<PatientDto>(patient);
    }
}
```

---

## 🚀 **Deployment Architecture**

### **Development Environment**

#### **Local Development Setup**
```yaml
# docker-compose.dev.yml
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "YourStrong@Passw0rd"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
      
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    
  patient-api:
    build:
      context: ./PatientService
      dockerfile: Dockerfile
    ports:
      - "58069:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=EMRR;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
      - JwtSettings__SecretKey=your-super-secret-key-here
    depends_on:
      - sqlserver
      - redis
      
  patient-portal:
    build:
      context: ./PatientPortal
      dockerfile: Dockerfile
    ports:
      - "5173:80"
    environment:
      - VITE_API_BASE_URL=http://localhost:58069/api
    depends_on:
      - patient-api

volumes:
  sqlserver_data:
```

#### **Development Scripts**
```json
{
  "scripts": {
    "dev": "concurrently \"npm run dev:api\" \"npm run dev:portal\"",
    "dev:api": "cd PatientService && dotnet watch run",
    "dev:portal": "cd PatientPortal && npm run dev",
    "build": "npm run build:api && npm run build:portal",
    "build:api": "cd PatientService && dotnet build",
    "build:portal": "cd PatientPortal && npm run build",
    "test": "npm run test:api && npm run test:portal",
    "test:api": "cd PatientService && dotnet test",
    "test:portal": "cd PatientPortal && npm run test"
  }
}
```

### **Production Deployment**

#### **Azure Deployment**
```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main
      - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  azureSubscription: 'EMRR-Azure-Subscription'
  resourceGroupName: 'emrr-rg'
  webAppName: 'emrr-patient-api'
  webAppNamePortal: 'emrr-patient-portal'

stages:
- stage: Build
  displayName: 'Build and Test'
  jobs:
  - job: BuildJob
    displayName: 'Build'
    steps:
    - task: DotNetCoreCLI@2
      displayName: 'Restore API packages'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
        
    - task: DotNetCoreCLI@2
      displayName: 'Build API'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration)'
        
    - task: DotNetCoreCLI@2
      displayName: 'Test API'
      inputs:
        command: 'test'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration) --collect "Code coverage"'
        
    - task: Npm@1
      displayName: 'Install Portal dependencies'
      inputs:
        command: 'install'
        workingDir: 'PatientPortal'
        
    - task: Npm@1
      displayName: 'Build Portal'
      inputs:
        command: 'custom'
        workingDir: 'PatientPortal'
        customCommand: 'run build'

- stage: Deploy
  displayName: 'Deploy to Azure'
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployJob
    displayName: 'Deploy'
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebApp@1
            displayName: 'Deploy API'
            inputs:
              azureSubscription: '$(azureSubscription)'
              appName: '$(webAppName)'
              package: '$(Pipeline.Workspace)/drop'
              
          - task: AzureWebApp@1
            displayName: 'Deploy Portal'
            inputs:
              azureSubscription: '$(azureSubscription)'
              appName: '$(webAppNamePortal)'
              package: '$(Pipeline.Workspace)/drop/PatientPortal/dist'
```

#### **Production Configuration**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:emrr-sql.database.windows.net,1433;Initial Catalog=EMRR;Persist Security Info=False;User ID=emrr-admin;Password=YourSecurePassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "JwtSettings": {
    "SecretKey": "your-production-secret-key",
    "Issuer": "https://emrr-patient-api.azurewebsites.net",
    "Audience": "https://emrr-patient-portal.azurewebsites.net",
    "ExpirationMinutes": 60
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "${AZURE_AD_TENANT_ID}",
    "ClientId": "${AZURE_AD_CLIENT_ID}",
    "ClientSecret": "${AZURE_AD_CLIENT_SECRET}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  },
  "ApplicationInsights": {
    "ConnectionString": "your-application-insights-connection-string"
  }
}
```

---

## ⚙️ **Configuration Management**

### **Environment Configuration**

#### **Frontend Environment Variables**
```bash
# .env.development
VITE_API_BASE_URL=https://localhost:58069/api
VITE_AZURE_AD_CLIENT_ID=your-azure-client-id
VITE_AZURE_AD_TENANT_ID=your-azure-tenant-id
VITE_AZURE_AD_REDIRECT_URI=http://localhost:5173
VITE_GOOGLE_CLIENT_ID=your-google-client-id
VITE_GOOGLE_REDIRECT_URI=http://localhost:5173

# .env.production
VITE_API_BASE_URL=https://emrr-patient-api.azurewebsites.net/api
VITE_AZURE_AD_CLIENT_ID=your-azure-client-id
VITE_AZURE_AD_TENANT_ID=your-azure-tenant-id
VITE_AZURE_AD_REDIRECT_URI=https://emrr-patient-portal.azurewebsites.net
VITE_GOOGLE_CLIENT_ID=your-production-google-client-id
VITE_GOOGLE_REDIRECT_URI=https://emrr-patient-portal.azurewebsites.net
```

#### **Backend Configuration**
```csharp
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EMRR_Dev;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "development-secret-key-not-for-production",
    "Issuer": "https://localhost:58069",
    "Audience": "https://localhost:5173",
    "ExpirationMinutes": 60
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "${AZURE_AD_TENANT_ID}",
    "ClientId": "${AZURE_AD_CLIENT_ID}",
    "ClientSecret": "${AZURE_AD_CLIENT_SECRET}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### **Feature Flags**
```csharp
// Feature Flags Configuration
public class FeatureFlags
{
    public bool EnableTestMode { get; set; } = true;
    public bool EnableGoogleAuth { get; set; } = false;
    public bool EnableAdvancedAnalytics { get; set; } = false;
    public bool EnableRealTimeNotifications { get; set; } = true;
    public bool EnableAuditLogging { get; set; } = true;
}

// Feature Flag Service
public interface IFeatureFlagService
{
    bool IsEnabled(string featureName);
    Task<bool> IsEnabledAsync(string featureName);
}

public class FeatureFlagService : IFeatureFlagService
{
    private readonly IConfiguration _configuration;
    
    public FeatureFlagService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public bool IsEnabled(string featureName)
    {
        return _configuration.GetValue<bool>($"FeatureFlags:{featureName}");
    }
}
```

---

## 🧪 **Testing Strategy**

### **Frontend Testing**

#### **Unit Testing with Vitest**
```typescript
// Button.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { Button } from './Button';

describe('Button Component', () => {
  it('renders with correct text', () => {
    render(<Button>Click me</Button>);
    expect(screen.getByText('Click me')).toBeInTheDocument();
  });

  it('calls onClick when clicked', () => {
    const handleClick = vi.fn();
    render(<Button onClick={handleClick}>Click me</Button>);
    
    fireEvent.click(screen.getByText('Click me'));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('shows loading state', () => {
    render(<Button loading>Click me</Button>);
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('is disabled when loading', () => {
    render(<Button loading>Click me</Button>);
    expect(screen.getByRole('button')).toBeDisabled();
  });
});

// AuthModal.test.tsx
import { render, screen } from '@testing-library/react';
import { AuthModal } from './AuthModal';

describe('AuthModal Component', () => {
  it('renders login form when authType is login', () => {
    render(
      <AuthModal
        isOpen={true}
        onClose={() => {}}
        authType="login"
        onAuthTypeChange={() => {}}
      />
    );
    
    expect(screen.getByText('Welcome Back')).toBeInTheDocument();
  });

  it('renders signup form when authType is signup', () => {
    render(
      <AuthModal
        isOpen={true}
        onClose={() => {}}
        authType="signup"
        onAuthTypeChange={() => {}}
      />
    );
    
    expect(screen.getByText('Join EMRR Platform')).toBeInTheDocument();
  });
});
```

#### **Integration Testing**
```typescript
// Dashboard.integration.test.tsx
import { render, screen, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Dashboard } from './Dashboard';
import { server } from '../mocks/server';

describe('Dashboard Integration', () => {
  let queryClient: QueryClient;

  beforeEach(() => {
    queryClient = new QueryClient({
      defaultOptions: {
        queries: { retry: false },
        mutations: { retry: false },
      },
    });
  });

  it('loads and displays patient data', async () => {
    render(
      <QueryClientProvider client={queryClient}>
        <Dashboard />
      </QueryClientProvider>
    );

    await waitFor(() => {
      expect(screen.getByText('Ashok Thotakura')).toBeInTheDocument();
    });
  });
});
```

### **Backend Testing**

#### **Unit Testing with xUnit**
```csharp
// PatientServiceTests.cs
public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IAuditService> _mockAuditService;
    private readonly PatientService _service;

    public PatientServiceTests()
    {
        _mockRepository = new Mock<IPatientRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockAuditService = new Mock<IAuditService>();
        _service = new PatientService(_mockRepository.Object, _mockMapper.Object, _mockAuditService.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsPatientDto()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient { PatientId = patientId, FirstName = "John", LastName = "Doe" };
        var patientDto = new PatientDto { PatientId = patientId, FirstName = "John", LastName = "Doe" };

        _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _mockMapper.Setup(m => m.Map<PatientDto>(patient)).Returns(patientDto);

        // Act
        var result = await _service.GetByIdAsync(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patientId, result.PatientId);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(patientId));
    }
}
```

#### **Integration Testing**
```csharp
// PatientControllerIntegrationTests.cs
public class PatientControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PatientControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPatient_ValidId_ReturnsOk()
    {
        // Arrange
        var patientId = "3e35157e-ec48-4e9a-9456-23961eea2de0";

        // Act
        var response = await _client.GetAsync($"/api/patients/{patientId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var patient = JsonSerializer.Deserialize<PatientDto>(content);
        Assert.NotNull(patient);
        Assert.Equal(patientId, patient.PatientId.ToString());
    }
}
```

---

## ⚡ **Performance Considerations**

### **Frontend Performance**

#### **Code Splitting & Lazy Loading**
```typescript
// Lazy loading components
const Dashboard = lazy(() => import('./pages/Dashboard'));
const Profile = lazy(() => import('./pages/Profile'));
const MedicalHistory = lazy(() => import('./pages/MedicalHistory'));

// Route-based code splitting
const App = () => {
  return (
    <BrowserRouter>
      <Suspense fallback={<LoadingSpinner />}>
        <Routes>
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/medical-history" element={<MedicalHistory />} />
        </Routes>
      </Suspense>
    </BrowserRouter>
  );
};

// Image optimization
const OptimizedImage = ({ src, alt, ...props }) => {
  const [imageSrc, setImageSrc] = useState('/placeholder.jpg');
  
  useEffect(() => {
    const img = new Image();
    img.onload = () => setImageSrc(src);
    img.src = src;
  }, [src]);

  return <img src={imageSrc} alt={alt} {...props} />;
};
```

#### **Caching Strategy**
```typescript
// React Query caching configuration
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      cacheTime: 10 * 60 * 1000, // 10 minutes
      refetchOnWindowFocus: false,
      refetchOnReconnect: true,
    },
  },
});

// Custom cache hooks
export const usePatientCache = (patientId: string) => {
  return useQuery({
    queryKey: ['patient', patientId],
    queryFn: () => fetchPatient(patientId),
    staleTime: 10 * 60 * 1000, // 10 minutes for patient data
    cacheTime: 30 * 60 * 1000, // 30 minutes cache
  });
};
```

### **Backend Performance**

#### **Database Optimization**
```csharp
// Efficient queries with proper includes
public async Task<Patient> GetPatientWithDetailsAsync(Guid id)
{
    return await _context.Patients
        .AsNoTracking() // Read-only query
        .Include(p => p.MedicalHistory.Where(mh => mh.IsActive))
        .Include(p => p.Allergies.Where(a => a.IsActive))
        .Include(p => p.Medications.Where(m => m.IsActive))
        .FirstOrDefaultAsync(p => p.PatientId == id && p.IsActive);
}

// Pagination for large datasets
public async Task<PagedResult<Patient>> GetPatientsAsync(int page, int pageSize)
{
    var query = _context.Patients.Where(p => p.IsActive);
    
    var totalCount = await query.CountAsync();
    var patients = await query
        .OrderBy(p => p.LastName)
        .ThenBy(p => p.FirstName)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<Patient>
    {
        Data = patients,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}

// Caching with Redis
public class CachedPatientService : IPatientService
{
    private readonly IPatientService _patientService;
    private readonly IDistributedCache _cache;
    
    public async Task<PatientDto> GetByIdAsync(Guid id)
    {
        var cacheKey = $"patient:{id}";
        var cachedPatient = await _cache.GetStringAsync(cacheKey);
        
        if (cachedPatient != null)
        {
            return JsonSerializer.Deserialize<PatientDto>(cachedPatient);
        }
        
        var patient = await _patientService.GetByIdAsync(id);
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(patient), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        
        return patient;
    }
}
```

---

## 🔒 **Security Implementation**

### **Data Protection**

#### **Encryption at Rest**
```csharp
// Sensitive data encryption
public class EncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IConfiguration configuration)
    {
        _key = Convert.FromBase64String(configuration["Encryption:Key"]);
        _iv = Convert.FromBase64String(configuration["Encryption:IV"]);
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);

        swEncrypt.Write(plainText);
        return Convert.ToBase64String(msEncrypt.ToArray());
    }
}

// Entity configuration for encrypted fields
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.Property(p => p.SSN)
            .HasConversion(
                v => _encryptionService.Encrypt(v),
                v => _encryptionService.Decrypt(v));
    }
}
```

#### **Input Validation**
```csharp
// FluentValidation rules
public class CreatePatientValidator : AbstractValidator<CreatePatientDto>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s-']+$").WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.Today.AddYears(-150)).WithMessage("Invalid date of birth");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");
    }
}
```

### **API Security**

#### **Rate Limiting**
```csharp
// Rate limiting configuration
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientId(context);
        var key = $"rate_limit:{clientId}";
        
        var requestCount = _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return 0;
        });

        if (requestCount >= 100) // 100 requests per minute
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }

        _cache.Set(key, requestCount + 1);
        await _next(context);
    }
}
```

#### **CORS Configuration**
```csharp
// CORS policy configuration
public void ConfigureServices(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("EMRRPolicy", builder =>
        {
            builder.WithOrigins("https://emrr-patient-portal.azurewebsites.net")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
    });
}
```

---

## 📊 **Monitoring & Logging**

### **Application Insights Integration**

#### **Frontend Monitoring**
```typescript
// Application Insights setup
import { ApplicationInsights } from '@microsoft/applicationinsights-web';

const appInsights = new ApplicationInsights({
  config: {
    connectionString: import.meta.env.VITE_APP_INSIGHTS_CONNECTION_STRING,
    enableAutoRouteTracking: true,
    enableCorsCorrelation: true,
    enableRequestHeaderTracking: true,
    enableResponseHeaderTracking: true,
  }
});

appInsights.loadAppInsights();
appInsights.trackPageView();

// Custom telemetry
export const trackEvent = (name: string, properties?: Record<string, any>) => {
  appInsights.trackEvent({ name }, properties);
};

export const trackException = (error: Error, properties?: Record<string, any>) => {
  appInsights.trackException({ exception: error }, properties);
};
```

#### **Backend Logging**
```csharp
// Structured logging with Serilog
public void ConfigureServices(IServiceCollection services)
{
    services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.ApplicationInsights(
            services.GetRequiredService<TelemetryConfiguration>(),
            TelemetryConverter.Traces)
        .WriteTo.File("logs/emrr-.txt", 
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30));
}

// Custom logging
public class PatientService
{
    private readonly ILogger<PatientService> _logger;

    public async Task<PatientDto> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving patient with ID: {PatientId}", id);
        
        try
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            _logger.LogInformation("Successfully retrieved patient: {PatientName}", 
                $"{patient.FirstName} {patient.LastName}");
            return _mapper.Map<PatientDto>(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID: {PatientId}", id);
            throw;
        }
    }
}
```

### **Health Checks**
```csharp
// Health check configuration
public void ConfigureServices(IServiceCollection services)
{
    services.AddHealthChecks()
        .AddSqlServer(connectionString, name: "database")
        .AddRedis(redisConnectionString, name: "redis")
        .AddUrlGroup(new Uri("https://login.microsoftonline.com"), name: "azure-ad");
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
}
```

---

## 🚀 **Future Enhancements**

### **Planned Features**

#### **AI/ML Integration**
- **Predictive Analytics**: Patient risk assessment and health predictions
- **Natural Language Processing**: Automated medical record analysis
- **Computer Vision**: Medical image analysis and diagnosis assistance
- **Chatbot Integration**: AI-powered patient support and triage

#### **Advanced Features**
- **Real-time Collaboration**: Multi-provider patient care coordination
- **Telemedicine Integration**: Video consultations and remote monitoring
- **IoT Device Integration**: Wearable device data collection
- **Blockchain Integration**: Secure medical record sharing and verification

#### **Scalability Improvements**
- **Microservices Architecture**: Further service decomposition
- **Event-Driven Architecture**: Asynchronous communication patterns
- **Multi-tenant Support**: Support for multiple healthcare organizations
- **Global Deployment**: Multi-region deployment and data residency

### **Technology Roadmap**

#### **Short-term (3-6 months)**
- Complete HIPAA compliance audit
- Performance optimization and caching
- Enhanced security features
- Mobile application development

#### **Medium-term (6-12 months)**
- AI/ML model integration
- Advanced analytics dashboard
- Third-party system integrations
- API versioning and backward compatibility

#### **Long-term (12+ months)**
- Blockchain integration
- Global deployment
- Advanced AI features
- Industry-specific customizations

---

## 📚 **Conclusion**

The EMRR Platform represents a comprehensive, modern healthcare management system built with cutting-edge technologies and best practices. The architecture is designed for scalability, security, and maintainability while providing an exceptional user experience for both patients and healthcare providers.

### **Key Strengths**
- **Modern Technology Stack**: Latest versions of React, .NET, and SQL Server
- **Security-First Design**: HIPAA compliance and enterprise-grade security
- **Scalable Architecture**: Microservices and cloud-ready design
- **User-Centric Design**: Intuitive interfaces and responsive design
- **Comprehensive Testing**: Unit, integration, and end-to-end testing
- **DevOps Ready**: CI/CD pipelines and containerization support

### **Technical Excellence**
- Clean architecture principles
- SOLID design patterns
- Comprehensive error handling
- Performance optimization
- Security best practices
- Monitoring and observability

This technical documentation serves as a comprehensive guide for developers, architects, and stakeholders involved in the EMRR Platform development and maintenance.

---

**Document Version**: 1.0  
**Last Updated**: January 18, 2025  
**Next Review**: April 18, 2025
