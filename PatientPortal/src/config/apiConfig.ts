import axios from 'axios';
import { msalInstance } from './authConfig';

// Use Azure deployed API in production, localhost in development
const defaultApiUrl = import.meta.env.PROD 
  ? 'https://emrrapi.azurewebsites.net/api'
  : 'https://localhost:58069/api';

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || defaultApiUrl;

// Create axios instance
export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  async (config) => {
    try {
      // In development without Azure AD, skip auth
      if (import.meta.env.DEV && !import.meta.env.VITE_AZURE_AD_CLIENT_ID) {
        return config;
      }

      const accounts = msalInstance.getAllAccounts();
      if (accounts.length > 0) {
        const response = await msalInstance.acquireTokenSilent({
          scopes: ['User.Read'],
          account: accounts[0],
        });
        
        config.headers.Authorization = `Bearer ${response.accessToken}`;
      }
    } catch (error) {
      console.error('Error acquiring token:', error);
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      // Server responded with error
      const { status, data } = error.response;
      
      if (status === 401) {
        // Unauthorized - redirect to login
        msalInstance.loginRedirect();
      } else if (status === 403) {
        console.error('Forbidden - insufficient permissions');
      } else if (status === 404) {
        console.error('Resource not found');
      } else if (status >= 500) {
        console.error('Server error:', data.message || 'Unknown error');
      }
    } else if (error.request) {
      // Request made but no response
      console.error('No response from server');
    } else {
      console.error('Error:', error.message);
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;

// Use Azure deployed API in production, localhost in development
const defaultApiBase = import.meta.env.PROD
  ? 'https://emrrapi.azurewebsites.net'
  : 'https://localhost:58069';

export const API_BASE = import.meta.env.VITE_API_URL || defaultApiBase;

export const getMetadata = async (table: string) => {
  const res = await fetch(`${API_BASE}/api/metadata/${table}`);
  if (!res.ok) throw new Error('Failed to fetch metadata');
  return res.json();
};

