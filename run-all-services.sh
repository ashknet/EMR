#!/bin/bash

# =============================================
# Healthcare Platform - Run All Services Script
# Bash script to start all 10 microservices (Linux/Mac)
# =============================================

echo "🏥 Healthcare Platform - Starting All Services"
echo "================================================="
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 8 SDK"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "✓ .NET SDK $DOTNET_VERSION detected"
echo ""

echo "Starting 10 microservices..."
echo ""

# Start services in background
cd src/Services/PatientService/PatientService.API && dotnet run &
sleep 2

cd ../../../FamilyService/FamilyService.API && dotnet run &
sleep 2

cd ../../../HealthHistoryService/HealthHistoryService.API && dotnet run &
sleep 2

cd ../../../InsuranceService/InsuranceService.API && dotnet run &
sleep 2

cd ../../../ConsentAuditService/ConsentAuditService.API && dotnet run &
sleep 2

cd ../../../DataIntegrationService/DataIntegrationService.API && dotnet run &
sleep 2

cd ../../../AgentManagementService/AgentManagementService.API && dotnet run &
sleep 2

cd ../../../TransferRoutingService/TransferRoutingService.API && dotnet run &
sleep 2

cd ../../../NotificationService/NotificationService.API && dotnet run &
sleep 2

cd ../../../SecurityComplianceService/SecurityComplianceService.API && dotnet run &

echo ""
echo "================================================="
echo "✅ All services are starting!"
echo "================================================="
echo ""
echo "Access Swagger UI at:"
echo "  Patient Service:        http://localhost:5001/swagger"
echo "  Family Service:         http://localhost:5002/swagger"
echo "  Health History Service: http://localhost:5003/swagger"
echo "  Insurance Service:      http://localhost:5004/swagger"
echo "  Consent/Audit Service:  http://localhost:5005/swagger"
echo "  Data Integration:       http://localhost:5006/swagger"
echo "  Agent Management:       http://localhost:5007/swagger"
echo "  Transfer/Routing:       http://localhost:5008/swagger"
echo "  Notification:           http://localhost:5009/swagger"
echo "  Security/Compliance:    http://localhost:5010/swagger"
echo ""
echo "Press Ctrl+C to stop all services"
echo ""

# Wait for user interrupt
wait
