import { useState } from 'react';
import { useMsal } from '@azure/msal-react';
import { loginRequest, UserType } from '../config/authConfig';
import { getGoogleAuthUrl, handleGoogleCallback } from '../config/googleAuth';
import SignupForm from './SignupForm';

interface AuthModalProps {
  isOpen: boolean;
  onClose: () => void;
  authType: 'login' | 'signup';
  onAuthTypeChange: (type: 'login' | 'signup') => void;
}

export default function AuthModal({ isOpen, onClose, authType, onAuthTypeChange }: AuthModalProps) {
  const { instance } = useMsal();
  const [userType, setUserType] = useState<UserType>('patient');
  const [isLoading, setIsLoading] = useState(false);
  const [showSignupForm, setShowSignupForm] = useState(false);

  const handleMicrosoftLogin = async () => {
    setIsLoading(true);
    try {
      await instance.loginRedirect(loginRequest);
    } catch (error) {
      console.error('Login failed:', error);
      setIsLoading(false);
    }
  };

  const handleGoogleLogin = async () => {
    setIsLoading(true);
    try {
      // For now, show a message that Google OAuth is not configured
      alert('Google OAuth is not configured yet. Please use Microsoft login or direct signup.');
      setIsLoading(false);
    } catch (error) {
      console.error('Google login failed:', error);
      setIsLoading(false);
    }
  };

  const handleSignupSuccess = () => {
    // Close modal and redirect to dashboard
    onClose();
    window.location.href = '/dashboard';
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div className={`bg-white rounded-2xl shadow-2xl w-full p-8 animate-slide-up relative ${
        showSignupForm ? 'max-w-2xl max-h-[90vh] overflow-y-auto' : 'max-w-md'
      }`}>
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-slate-400 hover:text-slate-600 transition-colors"
        >
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>

        {!showSignupForm ? (
          <div className="text-center mb-6">
            <div className="w-16 h-16 bg-gradient-to-br from-blue-600 to-indigo-600 rounded-2xl flex items-center justify-center mx-auto mb-4">
              <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            </div>
            <h2 className="text-2xl font-bold text-slate-900 mb-2">
              {authType === 'login' ? 'Welcome Back' : 'Join EMRR Platform'}
            </h2>
            <p className="text-slate-600">
              {authType === 'login' 
                ? 'Sign in to access your healthcare portal' 
                : 'Create your account to get started'
              }
            </p>
          </div>
        ) : (
          <div className="text-center mb-6">
            <button
              onClick={() => setShowSignupForm(false)}
              className="flex items-center gap-2 text-slate-600 hover:text-slate-700 font-medium text-sm mb-4"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
              </svg>
              Back to signup options
            </button>
          </div>
        )}

        {showSignupForm ? (
          <SignupForm
            userType={userType}
            onSuccess={handleSignupSuccess}
            onBack={() => setShowSignupForm(false)}
          />
        ) : (
          <>
            {authType === 'signup' && (
              <div className="mb-6">
                <p className="text-sm font-semibold text-slate-700 mb-3">I want to register as:</p>
                <div className="grid grid-cols-2 gap-3">
                  <button
                    onClick={() => setUserType('patient')}
                    className={`p-4 rounded-xl border-2 transition-all ${
                      userType === 'patient'
                        ? 'border-blue-500 bg-blue-50 text-blue-700'
                        : 'border-slate-200 hover:border-slate-300 text-slate-600'
                    }`}
                  >
                    <div className="text-2xl mb-2">👤</div>
                    <div className="font-semibold">Patient</div>
                  </button>
                  <button
                    onClick={() => setUserType('hospital')}
                    className={`p-4 rounded-xl border-2 transition-all ${
                      userType === 'hospital'
                        ? 'border-blue-500 bg-blue-50 text-blue-700'
                        : 'border-slate-200 hover:border-slate-300 text-slate-600'
                    }`}
                  >
                    <div className="text-2xl mb-2">🏥</div>
                    <div className="font-semibold">Hospital</div>
                  </button>
                </div>
              </div>
            )}

            <div className="space-y-4">
              <button
                onClick={handleGoogleLogin}
                disabled={isLoading}
                className="w-full flex items-center justify-center gap-3 py-3 px-4 border-2 border-slate-200 rounded-xl hover:border-slate-300 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isLoading ? (
                  <div className="w-5 h-5 border-2 border-slate-400 border-t-transparent rounded-full animate-spin"></div>
                ) : (
                  <svg className="w-5 h-5" viewBox="0 0 24 24">
                    <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/>
                    <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
                    <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
                    <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
                  </svg>
                )}
                <span className="font-semibold text-slate-700">
                  Continue with Google
                </span>
              </button>

              <button
                onClick={handleMicrosoftLogin}
                disabled={isLoading}
                className="w-full flex items-center justify-center gap-3 py-3 px-4 border-2 border-slate-200 rounded-xl hover:border-slate-300 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isLoading ? (
                  <div className="w-5 h-5 border-2 border-slate-400 border-t-transparent rounded-full animate-spin"></div>
                ) : (
                  <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                    <path d="M11.4 24H0V12.6h11.4V24zM24 24H12.6V12.6H24V24zM11.4 11.4H0V0h11.4v11.4zm12.6 0H12.6V0H24v11.4z"/>
                  </svg>
                )}
                <span className="font-semibold text-slate-700">
                  Continue with Microsoft
                </span>
              </button>

              {authType === 'signup' && (
                <div className="relative">
                  <div className="absolute inset-0 flex items-center">
                    <div className="w-full border-t border-slate-200"></div>
                  </div>
                  <div className="relative flex justify-center text-sm">
                    <span className="px-2 bg-white text-slate-500">Or</span>
                  </div>
                </div>
              )}

              {authType === 'signup' && (
                <button
                  onClick={() => setShowSignupForm(true)}
                  className="w-full bg-white hover:bg-slate-50 text-slate-700 px-6 py-3 rounded-xl font-semibold border-2 border-slate-200 hover:border-slate-300 transition-all"
                >
                  Sign up with Email & Phone
                </button>
              )}
            </div>
          </>
        )}

        <div className="mt-6 pt-6 border-t border-slate-200">
          <p className="text-center text-sm text-slate-600">
            {authType === 'login' ? "Don't have an account? " : "Already have an account? "}
            <button
              onClick={() => onAuthTypeChange(authType === 'login' ? 'signup' : 'login')}
              className="text-blue-600 hover:text-blue-700 font-semibold"
            >
              {authType === 'login' ? 'Sign up' : 'Sign in'}
            </button>
          </p>
        </div>

        <div className="mt-6 pt-6 border-t border-slate-200">
          <div className="flex items-center justify-center gap-2 text-xs text-slate-500">
            <svg className="w-4 h-4 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
            </svg>
            <span>HIPAA Compliant & Secure</span>
          </div>
        </div>
      </div>
    </div>
  );
}
