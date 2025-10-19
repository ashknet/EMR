import { Outlet, NavLink } from 'react-router-dom';
import { useMsal } from '@azure/msal-react';
import { useQuery } from '@tanstack/react-query';
import axios from 'axios';
import { isDevelopmentMode } from '../config/authConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';
import { API_BASE_URL } from '../config/apiConfig';

export default function Layout() {
  const { instance, accounts } = useMsal();
  
  // Fetch patient data from database
  const { data: patientData } = useQuery({
    queryKey: ['currentUser', MOCK_PATIENT_ID],
    queryFn: async () => {
      const response = await axios.get(`${API_BASE_URL}/patients/${MOCK_PATIENT_ID}`);
      return response.data;
    },
    enabled: isDevelopmentMode, // Only fetch in development mode
  });

  const userName = isDevelopmentMode 
    ? (patientData ? `${patientData.firstName} ${patientData.lastName}` : 'Loading...') 
    : (accounts[0]?.name || 'User');

  const handleLogout = () => {
    instance.logoutRedirect();
  };

  const navItems = [
    { to: '/dashboard', label: 'Dashboard' },
    { to: '/profile', label: 'Profile' },
    { to: '/medical-history', label: 'Medical History' },
    { to: '/insurance', label: 'Insurance' },
    { to: '/providers', label: 'Providers' },
    { to: '/family', label: 'Family' },
    { to: '/social-history', label: 'Social History' },
    { to: '/consent', label: 'Consent' },
    { to: '/documents', label: 'Documents' },
    { to: '/transfers', label: 'Transfers' },
  ];

  return (
    <div className="min-h-screen bg-gradient-to-br from-neutral-50 via-primary-50 to-accent-50">
      {/* Skip to main content link for accessibility */}
      <a href="#main-content" className="skip-link">
        Skip to main content
      </a>

      {/* Header */}
      <header className="bg-white shadow-soft border-b border-neutral-100 sticky top-0 z-50 backdrop-blur-lg bg-opacity-95" role="banner">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-5">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-primary-500 to-accent-500 rounded-xl flex items-center justify-center shadow-lg">
                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                </svg>
              </div>
              <div>
                <h1 className="text-2xl font-bold bg-gradient-to-r from-primary-600 to-accent-600 bg-clip-text text-transparent">
                  Patient Portal
                </h1>
                <p className="text-xs text-neutral-500 font-medium">HIPAA-Compliant Healthcare</p>
              </div>
            </div>
            <div className="flex items-center gap-4">
              <div className="hidden sm:flex items-center gap-3 px-4 py-2 bg-gradient-to-r from-primary-50 to-accent-50 rounded-xl border border-primary-100">
                <div className="w-8 h-8 bg-gradient-to-br from-primary-500 to-accent-500 rounded-full flex items-center justify-center text-white font-semibold text-sm">
                  {userName.charAt(0).toUpperCase()}
                </div>
                <span className="text-sm font-medium text-neutral-700" aria-label={`Logged in as ${userName}`}>
                  {userName}
                </span>
              </div>
              {!isDevelopmentMode && (
                <button
                  onClick={handleLogout}
                  className="bg-gradient-to-r from-primary-500 to-primary-600 hover:from-primary-600 hover:to-primary-700 text-white px-5 py-2.5 rounded-xl text-sm font-semibold shadow-lg shadow-primary-500/30 hover:shadow-xl hover:shadow-primary-500/40 transform hover:-translate-y-0.5 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2"
                  aria-label="Sign out"
                >
                  Sign Out
                </button>
              )}
            </div>
          </div>
        </div>
      </header>

      {/* Navigation */}
      <nav className="bg-white border-b border-neutral-100 shadow-sm sticky top-[73px] z-40 backdrop-blur-lg bg-opacity-95" role="navigation" aria-label="Main navigation">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <ul className="flex space-x-1 overflow-x-auto" role="menubar">
            {navItems.map((item) => (
              <li key={item.to} role="none">
                <NavLink
                  to={item.to}
                  role="menuitem"
                  className={({ isActive }) =>
                    `flex items-center gap-2 px-6 py-3 text-sm font-semibold whitespace-nowrap transition-all relative ${
                      isActive
                        ? 'text-primary-600 bg-gradient-to-b from-primary-50 to-transparent'
                        : 'text-neutral-600 hover:text-primary-600 hover:bg-neutral-50'
                    }`
                  }
                  aria-label={item.label}
                >
                  {({ isActive }) => (
                    <>
                      {item.label}
                      {isActive && (
                        <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-gradient-to-r from-primary-500 to-accent-500" />
                      )}
                    </>
                  )}
                </NavLink>
              </li>
            ))}
          </ul>
        </div>
      </nav>

      {/* Main Content */}
      <main id="main-content" className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8" role="main">
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="bg-white border-t border-neutral-200 mt-12 shadow-soft" role="contentinfo">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="flex flex-col md:flex-row justify-between items-center gap-4">
            <div className="flex items-center gap-3">
              <div className="w-8 h-8 bg-gradient-to-br from-primary-500 to-accent-500 rounded-lg flex items-center justify-center">
                <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-semibold text-neutral-700">
                  © 2025 Healthcare Platform
                </p>
                <p className="text-xs text-neutral-500">HIPAA Compliant & Secure</p>
              </div>
            </div>
            <div className="flex items-center gap-6">
              <a href="mailto:support@healthcare-platform.com" className="text-sm text-neutral-600 hover:text-primary-600 font-medium">
                Support
              </a>
              <a href="#" className="text-sm text-neutral-600 hover:text-primary-600 font-medium">
                Privacy Policy
              </a>
              <a href="#" className="text-sm text-neutral-600 hover:text-primary-600 font-medium">
                Terms
              </a>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
}

