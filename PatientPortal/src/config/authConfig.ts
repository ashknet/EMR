import { PublicClientApplication, Configuration, LogLevel } from '@azure/msal-browser';

const isDevelopment = import.meta.env.DEV;

// MSAL configuration
const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_AD_CLIENT_ID || 'development-client-id',
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AZURE_AD_TENANT_ID || 'common'}`,
    redirectUri: import.meta.env.VITE_AZURE_AD_REDIRECT_URI || 'http://localhost:5173',
  },
  cache: {
    cacheLocation: 'sessionStorage',
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return;
        
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
        }
      },
    },
  },
};

// Create MSAL instance
export const msalInstance = new PublicClientApplication(msalConfig);

// Login request
export const loginRequest = {
  scopes: ['User.Read', 'openid', 'profile'],
};

// Development mode mock - skip authentication
export const isDevelopmentMode = isDevelopment && !import.meta.env.VITE_AZURE_AD_CLIENT_ID;

if (isDevelopmentMode) {
  console.warn('Running in development mode without authentication');
}

