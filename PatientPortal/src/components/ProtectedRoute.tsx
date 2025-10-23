import { useMsal } from '@azure/msal-react';
import { Navigate } from 'react-router-dom';
import { isDevelopmentMode, ENABLE_HARDCODED_USER } from '../config/authConfig';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { accounts } = useMsal();

  // Allow access with hardcoded user for testing
  if (ENABLE_HARDCODED_USER) {
    return <>{children}</>;
  }

  // In development mode, allow access without authentication
  if (isDevelopmentMode) {
    return <>{children}</>;
  }

  // In production, check if user is authenticated
  if (accounts.length === 0) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
