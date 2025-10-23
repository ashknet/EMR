import { useState, useEffect } from 'react';
import { API_BASE_URL } from '../config/apiConfig';

export default function ApiStatus() {
  const [apiStatus, setApiStatus] = useState<'checking' | 'connected' | 'error'>('checking');
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    const testApiConnection = async () => {
      try {
        // Test API connection with a simple endpoint that's more likely to exist
        const response = await fetch(`${API_BASE_URL}/patients/3e35157e-ec48-4e9a-9456-23961eea2de0`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
          },
        });
        
        if (response.ok) {
          setApiStatus('connected');
        } else {
          setApiStatus('error');
          setErrorMessage(`HTTP ${response.status}: ${response.statusText}`);
        }
      } catch (error) {
        setApiStatus('error');
        setErrorMessage(error instanceof Error ? error.message : 'Unknown error');
      }
    };

    testApiConnection();
  }, []);

  if (apiStatus === 'checking') {
    return (
      <div className="flex items-center gap-2 text-sm text-slate-600">
        <div className="w-3 h-3 border-2 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
        <span>Connecting to API...</span>
      </div>
    );
  }

  if (apiStatus === 'error') {
    return (
      <div className="flex items-center gap-2 text-sm text-red-600">
        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <span>API Error: {errorMessage}</span>
      </div>
    );
  }

  return (
    <div className="flex items-center gap-2 text-sm text-green-600">
      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
      </svg>
      <span>API Connected</span>
    </div>
  );
}
