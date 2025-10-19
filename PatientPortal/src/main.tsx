// React import not needed in React 19 with new JSX transform
import { createRoot } from 'react-dom/client';
import { MsalProvider } from '@azure/msal-react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { msalInstance } from './config/authConfig';
import App from './App';
import './index.css';

// Create a React Query client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 5 * 60 * 1000, // 5 minutes
    },
  },
});

// React 19 optimized root creation
const container = document.getElementById('root');
if (!container) {
  throw new Error('Root element not found');
}

const root = createRoot(container);

root.render(
  // Note: React.StrictMode disabled to prevent duplicate API calls in development
  // StrictMode intentionally double-invokes effects to help detect side effects
  // Re-enable before production deployment for safety checks
  // <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <MsalProvider instance={msalInstance}>
        <App />
      </MsalProvider>
    </QueryClientProvider>
  // </React.StrictMode>
);

