// Production Azure AD Configuration
export const PRODUCTION_AZURE_CONFIG = {
  clientId: process.env.VITE_AZURE_AD_CLIENT_ID || '',
  tenantId: process.env.VITE_AZURE_AD_TENANT_ID || '',
  redirectUri: process.env.VITE_AZURE_AD_REDIRECT_URI || 'http://localhost:5173',
  // Note: Client secret should never be exposed in frontend code
  // Use server-side authentication for production
};

// For development, you can set these in your environment or use these values directly
export const getAzureConfig = () => {
  return {
    clientId: import.meta.env.VITE_AZURE_AD_CLIENT_ID || '',
    tenantId: import.meta.env.VITE_AZURE_AD_TENANT_ID || '',
    redirectUri: import.meta.env.VITE_AZURE_AD_REDIRECT_URI || 'http://localhost:5173',
  };
};
