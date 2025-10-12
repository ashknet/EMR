# =============================================
# Healthcare Platform - Run All Services Script
# PowerShell script to start all 10 microservices
# =============================================

Write-Host "🏥 Healthcare Platform - Starting All Services" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

# Function to start a service in a new window
function Start-Service {
    param(
        [string]$ServiceName,
        [string]$Path,
        [int]$Port
    )
    
    Write-Host "Starting $ServiceName on port $Port..." -ForegroundColor Green
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$Path'; dotnet run" -WindowStyle Normal
    
    Start-Sleep -Seconds 2
}

# Check if .NET is installed
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET SDK not found. Please install .NET 8 SDK" -ForegroundColor Red
    exit 1
}

Write-Host "✓ .NET SDK $dotnetVersion detected" -ForegroundColor Green
Write-Host ""

# Start all services
Write-Host "Starting 10 microservices..." -ForegroundColor Yellow
Write-Host ""

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path

Start-Service "Patient Service" "$scriptPath\src\Services\PatientService\PatientService.API" 5001
Start-Service "Family Service" "$scriptPath\src\Services\FamilyService\FamilyService.API" 5002
Start-Service "Health History Service" "$scriptPath\src\Services\HealthHistoryService\HealthHistoryService.API" 5003
Start-Service "Insurance Service" "$scriptPath\src\Services\InsuranceService\InsuranceService.API" 5004
Start-Service "Consent/Audit Service" "$scriptPath\src\Services\ConsentAuditService\ConsentAuditService.API" 5005
Start-Service "Data Integration Service" "$scriptPath\src\Services\DataIntegrationService\DataIntegrationService.API" 5006
Start-Service "Agent Management Service" "$scriptPath\src\Services\AgentManagementService\AgentManagementService.API" 5007
Start-Service "Transfer/Routing Service" "$scriptPath\src\Services\TransferRoutingService\TransferRoutingService.API" 5008
Start-Service "Notification Service" "$scriptPath\src\Services\NotificationService\NotificationService.API" 5009
Start-Service "Security/Compliance Service" "$scriptPath\src\Services\SecurityComplianceService\SecurityComplianceService.API" 5010

Write-Host ""
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "✅ All services are starting!" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Access Swagger UI at:" -ForegroundColor Yellow
Write-Host "  Patient Service:        http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  Family Service:         http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  Health History Service: http://localhost:5003/swagger" -ForegroundColor White
Write-Host "  Insurance Service:      http://localhost:5004/swagger" -ForegroundColor White
Write-Host "  Consent/Audit Service:  http://localhost:5005/swagger" -ForegroundColor White
Write-Host "  Data Integration:       http://localhost:5006/swagger" -ForegroundColor White
Write-Host "  Agent Management:       http://localhost:5007/swagger" -ForegroundColor White
Write-Host "  Transfer/Routing:       http://localhost:5008/swagger" -ForegroundColor White
Write-Host "  Notification:           http://localhost:5009/swagger" -ForegroundColor White
Write-Host "  Security/Compliance:    http://localhost:5010/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C in each window to stop services" -ForegroundColor Yellow
Write-Host ""
