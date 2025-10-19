interface LoginProps {
  onLogin: () => void;
}

export default function Login({ onLogin }: LoginProps) {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 via-white to-accent-50 py-12 px-4 sm:px-6 lg:px-8 relative overflow-hidden">
      {/* Decorative background elements */}
      <div className="absolute top-0 left-0 w-96 h-96 bg-primary-200 rounded-full mix-blend-multiply filter blur-3xl opacity-30 animate-pulse-slow"></div>
      <div className="absolute bottom-0 right-0 w-96 h-96 bg-accent-200 rounded-full mix-blend-multiply filter blur-3xl opacity-30 animate-pulse-slow" style={{ animationDelay: '1s' }}></div>
      
      <div className="max-w-md w-full space-y-8 relative z-10">
        {/* Logo and Title */}
        <div className="text-center animate-fade-in">
          <div className="flex justify-center mb-6">
            <div className="w-20 h-20 bg-gradient-to-br from-primary-500 to-accent-500 rounded-2xl flex items-center justify-center shadow-2xl shadow-primary-500/40 transform hover:scale-110 transition-transform">
              <svg className="w-12 h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            </div>
          </div>
          <h1 className="text-4xl font-extrabold bg-gradient-to-r from-primary-600 via-primary-500 to-accent-600 bg-clip-text text-transparent mb-2">
            Patient Portal
          </h1>
          <p className="text-neutral-600 text-base font-medium">
            HIPAA-compliant healthcare management
          </p>
          <div className="mt-4 flex items-center justify-center gap-4">
            <div className="flex items-center gap-2 text-sm text-neutral-500">
              <svg className="w-5 h-5 text-success-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
              </svg>
              <span className="font-semibold">Secure</span>
            </div>
            <div className="flex items-center gap-2 text-sm text-neutral-500">
              <svg className="w-5 h-5 text-success-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M5 13l4 4L19 7" />
              </svg>
              <span className="font-semibold">Private</span>
            </div>
          </div>
        </div>

        {/* Login Card */}
        <div className="bg-white/80 backdrop-blur-xl py-10 px-8 shadow-2xl rounded-2xl border border-neutral-100 animate-slide-up">
          <button
            onClick={onLogin}
            className="group relative w-full flex items-center justify-center gap-3 py-4 px-6 border-2 border-transparent rounded-xl text-base font-bold text-white bg-gradient-to-r from-primary-500 via-primary-600 to-accent-600 hover:from-primary-600 hover:via-primary-700 hover:to-accent-700 focus:outline-none focus:ring-4 focus:ring-primary-300 shadow-xl shadow-primary-500/30 hover:shadow-2xl hover:shadow-primary-500/40 transform hover:-translate-y-1 transition-all duration-200"
            aria-label="Sign in with Azure Active Directory"
          >
            <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
              <path d="M11.4 24H0V12.6h11.4V24zM24 24H12.6V12.6H24V24zM11.4 11.4H0V0h11.4v11.4zm12.6 0H12.6V0H24v11.4z"/>
            </svg>
            <span>Sign in with Microsoft</span>
            <svg className="w-5 h-5 transform group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M13 7l5 5m0 0l-5 5m5-5H6" />
            </svg>
          </button>
          
          <div className="mt-6 flex items-center justify-center gap-2 text-sm text-neutral-500">
            <svg className="w-5 h-5 text-accent-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
            </svg>
            <p className="font-medium">
              Secure authentication powered by Azure AD
            </p>
          </div>
        </div>

        {/* Features */}
        <div className="grid grid-cols-3 gap-4 pt-4">
          <div className="bg-white/60 backdrop-blur-sm rounded-xl p-4 text-center border border-neutral-100 hover:border-primary-200 transition-colors">
            <div className="text-2xl mb-2">🔒</div>
            <p className="text-xs font-semibold text-neutral-700">End-to-End Encrypted</p>
          </div>
          <div className="bg-white/60 backdrop-blur-sm rounded-xl p-4 text-center border border-neutral-100 hover:border-primary-200 transition-colors">
            <div className="text-2xl mb-2">✅</div>
            <p className="text-xs font-semibold text-neutral-700">HIPAA Compliant</p>
          </div>
          <div className="bg-white/60 backdrop-blur-sm rounded-xl p-4 text-center border border-neutral-100 hover:border-primary-200 transition-colors">
            <div className="text-2xl mb-2">⚡</div>
            <p className="text-xs font-semibold text-neutral-700">Fast & Reliable</p>
          </div>
        </div>

        {/* Footer */}
        <p className="text-center text-xs text-neutral-500">
          By signing in, you agree to our{' '}
          <a href="#" className="text-primary-600 hover:text-primary-700 font-semibold">Terms of Service</a>
          {' '}and{' '}
          <a href="#" className="text-primary-600 hover:text-primary-700 font-semibold">Privacy Policy</a>
        </p>
      </div>
    </div>
  );
}

