using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace Shared.Security.Authentication
{
    /// <summary>
    /// JWT Authentication configuration for Azure AD B2C and Azure AD
    /// Supports local development without authentication and production with authentication
    /// </summary>
    public static class JwtConfiguration
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, 
            IConfiguration configuration,
            bool isDevelopment = false)
        {
            if (isDevelopment)
            {
                // In development, skip authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Development";
                })
                .AddScheme<DevAuthenticationOptions, DevAuthenticationHandler>(
                    "Development", 
                    options => { });
            }
            else
            {
                // Production: Use Azure AD authentication
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

                services.AddAuthorization(options =>
                {
                    // HIPAA-compliant role-based access control policies
                    options.AddPolicy("PatientAccess", policy => 
                        policy.RequireRole("Patient", "Guardian", "ProxyUser"));
                    
                    options.AddPolicy("HospitalStaffAccess", policy => 
                        policy.RequireRole("Doctor", "Nurse", "Admin", "Technician"));
                    
                    options.AddPolicy("AdminAccess", policy => 
                        policy.RequireRole("SystemAdmin", "ComplianceOfficer"));
                    
                    options.AddPolicy("AuditAccess", policy => 
                        policy.RequireRole("ComplianceOfficer", "Auditor"));
                });
            }

            return services;
        }
    }
}
