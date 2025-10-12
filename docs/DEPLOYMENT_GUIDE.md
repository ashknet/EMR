# Deployment Guide - Healthcare Platform

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Local Development Setup](#local-development-setup)
3. [Azure Production Deployment](#azure-production-deployment)
4. [Database Migration](#database-migration)
5. [Security Configuration](#security-configuration)
6. [Monitoring & Logging](#monitoring--logging)
7. [Backup & Disaster Recovery](#backup--disaster-recovery)

## Prerequisites

### Local Development
- .NET 8 SDK
- Node.js 18+
- SQL Server 2019+ or SQL Server LocalDB
- Visual Studio 2022 or VS Code
- Azure CLI (for Azure deployment)
- Docker Desktop (optional)

### Azure Production
- Azure subscription
- Azure SQL Database (Business Critical tier recommended for HIPAA)
- Azure App Services or Azure Kubernetes Service
- Azure Key Vault
- Azure Storage Account (for documents/blobs)
- Azure Application Insights
- Azure API Management (optional, recommended for production)

## Local Development Setup

### 1. Database Setup

**Option A: SQL Server LocalDB**
```bash
# Create databases
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_Patient"
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_Family"
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_HealthHistory"
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_Insurance"
sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE HealthcarePlatform_Consent"

# Apply schemas
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/schemas/01_PatientService_Schema.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Family -i database/schemas/02_FamilyService_Schema.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_HealthHistory -i database/schemas/03_HealthHistoryService_Schema.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Insurance -i database/schemas/04_InsuranceService_Schema.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Consent -i database/schemas/05_ConsentAuditService_Schema.sql

# Apply stored procedures
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/stored-procedures/01_PatientService_Procedures.sql

# Load test data
sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/test-data/01_PatientService_TestData.sql
```

**Option B: Docker Compose**
```bash
# Start all services (databases + APIs + frontend)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

### 2. Run Microservices

**Run individually:**
```bash
# Terminal 1 - Patient Service
cd src/Services/PatientService/PatientService.API
dotnet restore
dotnet run

# Terminal 2 - Family Service
cd src/Services/FamilyService/FamilyService.API
dotnet restore
dotnet run

# Terminal 3 - Health History Service
cd src/Services/HealthHistoryService/HealthHistoryService.API
dotnet restore
dotnet run

# Continue for other services...
```

**Or use Visual Studio:**
1. Open `HealthcarePlatform.sln`
2. Set multiple startup projects
3. Select all API projects
4. Press F5 to debug

### 3. Run React Frontend

```bash
cd src/Frontend/PatientPortal
npm install
npm start
```

Frontend will be available at: http://localhost:3000

### 4. Run Azure Functions (Optional)

```bash
cd src/Services/PatientService/PatientService.Functions
func start
```

## Azure Production Deployment

### 1. Provision Azure Resources

```bash
# Login to Azure
az login

# Set variables
$RESOURCE_GROUP="healthcare-platform-rg"
$LOCATION="eastus"
$SQL_SERVER="healthcare-sql-server"
$SQL_ADMIN="sqladmin"
$SQL_PASSWORD="YourSecureP@ssw0rd!"
$KEY_VAULT="healthcare-keyvault"
$STORAGE_ACCOUNT="healthcarestorage"

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create Azure SQL Server
az sql server create `
  --name $SQL_SERVER `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION `
  --admin-user $SQL_ADMIN `
  --admin-password $SQL_PASSWORD

# Create databases
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER --name HealthcarePlatform_Patient --service-objective S1
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER --name HealthcarePlatform_Family --service-objective S1
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER --name HealthcarePlatform_HealthHistory --service-objective S1
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER --name HealthcarePlatform_Insurance --service-objective S1
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER --name HealthcarePlatform_Consent --service-objective S1

# Create Key Vault
az keyvault create `
  --name $KEY_VAULT `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION

# Create Storage Account
az storage account create `
  --name $STORAGE_ACCOUNT `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION `
  --sku Standard_LRS `
  --encryption-services blob `
  --https-only true

# Create App Service Plan
az appservice plan create `
  --name healthcare-plan `
  --resource-group $RESOURCE_GROUP `
  --sku P1V2 `
  --is-linux

# Create App Services
az webapp create --resource-group $RESOURCE_GROUP --plan healthcare-plan --name patient-api --runtime "DOTNETCORE:8.0"
az webapp create --resource-group $RESOURCE_GROUP --plan healthcare-plan --name family-api --runtime "DOTNETCORE:8.0"
az webapp create --resource-group $RESOURCE_GROUP --plan healthcare-plan --name healthhistory-api --runtime "DOTNETCORE:8.0"
```

### 2. Configure Azure SQL Firewall

```bash
# Allow Azure services
az sql server firewall-rule create `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER `
  --name AllowAzureServices `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 0.0.0.0

# Add your IP (for management)
$MY_IP = (Invoke-WebRequest -Uri "https://api.ipify.org").Content
az sql server firewall-rule create `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER `
  --name MyIP `
  --start-ip-address $MY_IP `
  --end-ip-address $MY_IP
```

### 3. Deploy Databases

```bash
# Connect to Azure SQL and run schema scripts
sqlcmd -S "$SQL_SERVER.database.windows.net" -d HealthcarePlatform_Patient -U $SQL_ADMIN -P $SQL_PASSWORD -i database/schemas/01_PatientService_Schema.sql

# Repeat for all databases...
```

### 4. Configure Managed Identity

```bash
# Enable Managed Identity for each App Service
az webapp identity assign --name patient-api --resource-group $RESOURCE_GROUP
az webapp identity assign --name family-api --resource-group $RESOURCE_GROUP

# Get the principal ID (object ID of the managed identity)
$PRINCIPAL_ID = az webapp identity show --name patient-api --resource-group $RESOURCE_GROUP --query principalId -o tsv

# Grant Key Vault access
az keyvault set-policy `
  --name $KEY_VAULT `
  --object-id $PRINCIPAL_ID `
  --secret-permissions get list
```

### 5. Store Secrets in Key Vault

```bash
# Database connection strings
$CONN_STRING = "Server=tcp:$SQL_SERVER.database.windows.net,1433;Database=HealthcarePlatform_Patient;User ID=$SQL_ADMIN;Password=$SQL_PASSWORD;Encrypt=True;Connection Timeout=30;"
az keyvault secret set --vault-name $KEY_VAULT --name "PatientServiceConnection" --value $CONN_STRING

# Encryption key
$ENCRYPTION_KEY = [System.Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
az keyvault secret set --vault-name $KEY_VAULT --name "EncryptionKey" --value $ENCRYPTION_KEY

# Storage connection string
$STORAGE_CONN = az storage account show-connection-string --name $STORAGE_ACCOUNT --resource-group $RESOURCE_GROUP --query connectionString -o tsv
az keyvault secret set --vault-name $KEY_VAULT --name "StorageConnectionString" --value $STORAGE_CONN
```

### 6. Deploy APIs

```bash
# Build and publish
cd src/Services/PatientService/PatientService.API
dotnet publish -c Release -o ./publish

# Create zip package
Compress-Archive -Path ./publish/* -DestinationPath ../patient-api.zip

# Deploy to Azure
az webapp deploy `
  --resource-group $RESOURCE_GROUP `
  --name patient-api `
  --src-path ../patient-api.zip `
  --type zip

# Repeat for other services...
```

### 7. Configure App Settings

```bash
# Patient API
az webapp config appsettings set `
  --resource-group $RESOURCE_GROUP `
  --name patient-api `
  --settings `
    "ConnectionStrings__ProductionConnection=@Microsoft.KeyVault(SecretUri=https://$KEY_VAULT.vault.azure.net/secrets/PatientServiceConnection/)" `
    "EncryptionKey=@Microsoft.KeyVault(SecretUri=https://$KEY_VAULT.vault.azure.net/secrets/EncryptionKey/)" `
    "ASPNETCORE_ENVIRONMENT=Production" `
    "WEBSITE_TIME_ZONE=Eastern Standard Time"
```

### 8. Deploy Azure Functions

```bash
# Create Function App
az functionapp create `
  --resource-group $RESOURCE_GROUP `
  --consumption-plan-location $LOCATION `
  --runtime dotnet-isolated `
  --runtime-version 8 `
  --functions-version 4 `
  --name patient-functions `
  --storage-account $STORAGE_ACCOUNT

# Deploy
cd src/Services/PatientService/PatientService.Functions
func azure functionapp publish patient-functions
```

### 9. Deploy React Frontend

**Option A: Azure Static Web Apps**
```bash
az staticwebapp create `
  --name patient-portal `
  --resource-group $RESOURCE_GROUP `
  --source https://github.com/your-org/healthcare-platform `
  --branch main `
  --app-location "/src/Frontend/PatientPortal" `
  --output-location "build" `
  --location $LOCATION
```

**Option B: Azure Storage Static Website**
```bash
# Enable static website hosting
az storage blob service-properties update `
  --account-name $STORAGE_ACCOUNT `
  --static-website `
  --index-document index.html `
  --404-document index.html

# Build React app
cd src/Frontend/PatientPortal
npm run build

# Upload to Azure Storage
az storage blob upload-batch `
  --account-name $STORAGE_ACCOUNT `
  --destination '$web' `
  --source ./build
```

## Security Configuration

### 1. Enable HTTPS Only

```bash
az webapp update --resource-group $RESOURCE_GROUP --name patient-api --set httpsOnly=true
```

### 2. Configure Azure AD Authentication

```bash
# Register application in Azure AD
$APP_ID = az ad app create --display-name "Healthcare Platform API" --query appId -o tsv

# Configure authentication
az webapp auth update `
  --resource-group $RESOURCE_GROUP `
  --name patient-api `
  --enabled true `
  --action RedirectToLoginPage `
  --aad-client-id $APP_ID `
  --aad-token-issuer-url "https://login.microsoftonline.com/{tenant-id}/v2.0"
```

### 3. Enable TLS 1.3

```bash
az webapp config set --resource-group $RESOURCE_GROUP --name patient-api --min-tls-version 1.2
```

## Monitoring & Logging

### 1. Enable Application Insights

```bash
# Create Application Insights
az monitor app-insights component create `
  --app healthcare-insights `
  --location $LOCATION `
  --resource-group $RESOURCE_GROUP

# Get instrumentation key
$INSTRUMENTATION_KEY = az monitor app-insights component show --app healthcare-insights --resource-group $RESOURCE_GROUP --query instrumentationKey -o tsv

# Configure App Services
az webapp config appsettings set `
  --resource-group $RESOURCE_GROUP `
  --name patient-api `
  --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$INSTRUMENTATION_KEY"
```

### 2. Configure Diagnostic Logging

```bash
az webapp log config `
  --resource-group $RESOURCE_GROUP `
  --name patient-api `
  --application-logging filesystem `
  --detailed-error-messages true `
  --failed-request-tracing true
```

## Backup & Disaster Recovery

### 1. Configure Automated Backups

```bash
# SQL Database automated backups (enabled by default)
az sql db show --resource-group $RESOURCE_GROUP --server $SQL_SERVER --name HealthcarePlatform_Patient --query "earliestRestoreDate"

# Configure long-term retention (7 years for HIPAA)
az sql db ltr-policy set `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER `
  --database HealthcarePlatform_Patient `
  --weekly-retention "P12W" `
  --monthly-retention "P12M" `
  --yearly-retention "P7Y" `
  --week-of-year 1
```

### 2. Enable Geo-Redundant Storage

```bash
az storage account update `
  --name $STORAGE_ACCOUNT `
  --resource-group $RESOURCE_GROUP `
  --sku Standard_GRS
```

### 3. Point-in-Time Restore

```bash
# Restore database to specific point in time
az sql db restore `
  --dest-name HealthcarePlatform_Patient_Restored `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER `
  --name HealthcarePlatform_Patient `
  --time "2025-10-12T10:30:00Z"
```

## Production Checklist

- [ ] All databases created and schemas applied
- [ ] Managed Identity configured for all services
- [ ] Secrets stored in Azure Key Vault
- [ ] Connection strings configured
- [ ] HTTPS enforced on all endpoints
- [ ] Azure AD authentication enabled
- [ ] Application Insights configured
- [ ] Automated backups enabled
- [ ] Geo-redundant storage configured
- [ ] HIPAA compliance settings verified
- [ ] BAA signed with Microsoft Azure
- [ ] Firewall rules configured
- [ ] Monitoring and alerts set up
- [ ] Disaster recovery plan documented
- [ ] Security scanning completed
- [ ] Penetration testing performed
- [ ] Load testing completed
- [ ] Documentation updated

## Post-Deployment

1. **Verify all services are running**
   ```bash
   az webapp browse --name patient-api --resource-group $RESOURCE_GROUP
   ```

2. **Test API endpoints**
   - Visit Swagger UI for each service
   - Test authentication
   - Verify database connectivity

3. **Monitor Application Insights**
   - Check for errors
   - Review performance metrics
   - Set up alerts

4. **Review audit logs**
   - Verify logging is working
   - Check access patterns
   - Confirm compliance

---

For additional support, refer to:
- [HIPAA Compliance Guide](HIPAA_COMPLIANCE.md)
- [API Documentation](API_CONTRACTS.md)
- [FHIR Implementation](FHIR_IMPLEMENTATION.md)
