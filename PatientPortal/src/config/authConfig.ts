import { PublicClientApplication, Configuration, LogLevel } from '@azure/msal-browser';

const isDevelopment = import.meta.env.DEV;

// MSAL configuration
const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_AD_CLIENT_ID || '',
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AZURE_AD_TENANT_ID || ''}`,
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

// Hardcoded user for testing (bypass authentication)
// This uses the real patient ID from your database
export const HARDCODED_USER = {
  id: '3e35157e-ec48-4e9a-9456-23961eea2de0', // Real patient ID from your database
  email: 'athotakura@err.com',
  name: 'Ashok Thotakura',
  type: 'patient' as UserType,
  provider: 'hardcoded' as 'microsoft' | 'google',
  firstName: 'Ashok',
  lastName: 'Thotakura',
  phone: '+1 (555) 123-4567',
  isAuthenticated: true
};

// Enable hardcoded user mode (set to true for testing)
export const ENABLE_HARDCODED_USER = true;

// User type for registration
export type UserType = 'patient' | 'hospital';

// Authentication state management
export interface AuthUser {
  id: string;
  email: string;
  name: string;
  type: UserType;
  provider: 'microsoft' | 'google';
}

if (isDevelopmentMode) {
  console.warn('Running in development mode without authentication');
}

