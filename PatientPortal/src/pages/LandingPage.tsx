import { useState } from 'react';
import { Link } from 'react-router-dom';
import AuthModal from '../components/AuthModal';
import { ENABLE_HARDCODED_USER } from '../config/authConfig';

export default function LandingPage() {
  const [showAuthModal, setShowAuthModal] = useState(false);
  const [authType, setAuthType] = useState<'login' | 'signup'>('login');
  const [userType, setUserType] = useState<'patient' | 'hospital'>('patient');

  const features = [
    {
      icon: '🏥',
      title: 'Hospital Management',
      description: 'Comprehensive EMR system for healthcare providers with advanced patient management capabilities.'
    },
    {
      icon: '👤',
      title: 'Patient Portal',
      description: 'Secure access to medical records, appointment scheduling, and health information management.'
    },
    {
      icon: '🔒',
      title: 'HIPAA Compliant',
      description: 'Enterprise-grade security with end-to-end encryption and strict privacy controls.'
    },
    {
      icon: '📱',
      title: 'Mobile Ready',
      description: 'Access your healthcare data anywhere, anytime with our responsive design.'
    },
    {
      icon: '⚡',
      title: 'Real-time Updates',
      description: 'Instant notifications and real-time synchronization across all devices.'
    },
    {
      icon: '🤖',
      title: 'AI-Powered',
      description: 'Intelligent EMR agent for automated patient intake and healthcare assistance.'
    }
  ];

  const stats = [
    { number: '10K+', label: 'Active Patients' },
    { number: '500+', label: 'Healthcare Providers' },
    { number: '99.9%', label: 'Uptime' },
    { number: '24/7', label: 'Support' }
  ];

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-50">
      {/* Navigation */}
      <nav className="bg-white/80 backdrop-blur-xl border-b border-slate-200 sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-4">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-blue-600 to-indigo-600 rounded-xl flex items-center justify-center shadow-lg">
                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                </svg>
              </div>
              <div>
                <h1 className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                  EMRR Platform
                </h1>
                <p className="text-xs text-slate-500 font-medium">Healthcare Excellence</p>
              </div>
            </div>
            <div className="flex items-center gap-4">
              <Link to="/about" className="text-slate-600 hover:text-blue-600 font-medium transition-colors">
                About
              </Link>
              <Link to="/contact" className="text-slate-600 hover:text-blue-600 font-medium transition-colors">
                Contact
              </Link>
              {ENABLE_HARDCODED_USER && (
                <Link to="/dashboard" className="text-green-600 hover:text-green-700 font-medium transition-colors">
                  🧪 Test Mode
                </Link>
              )}
              <button
                onClick={() => setShowAuthModal(true)}
                className="bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white px-6 py-2.5 rounded-xl text-sm font-semibold shadow-lg shadow-blue-500/30 hover:shadow-xl hover:shadow-blue-500/40 transform hover:-translate-y-0.5 transition-all"
              >
                Get Started
              </button>
            </div>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <section className="relative overflow-hidden py-20 lg:py-32">
        <div className="absolute inset-0 bg-gradient-to-r from-blue-600/5 to-indigo-600/5"></div>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 relative">
          <div className="text-center">
            <h1 className="text-5xl lg:text-7xl font-extrabold text-slate-900 mb-6">
              Revolutionizing
              <span className="block bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                Healthcare
              </span>
            </h1>
            <p className="text-xl lg:text-2xl text-slate-600 mb-8 max-w-3xl mx-auto leading-relaxed">
              Experience the future of healthcare with our intelligent EMR system. 
              Seamlessly connect patients and providers through cutting-edge technology.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center items-center">
              <button
                onClick={() => { setAuthType('signup'); setShowAuthModal(true); }}
                className="bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white px-8 py-4 rounded-2xl text-lg font-bold shadow-2xl shadow-blue-500/30 hover:shadow-3xl hover:shadow-blue-500/40 transform hover:-translate-y-1 transition-all"
              >
                Start Your Journey
              </button>
              <button
                onClick={() => { setAuthType('login'); setShowAuthModal(true); }}
                className="bg-white hover:bg-slate-50 text-slate-700 px-8 py-4 rounded-2xl text-lg font-bold border-2 border-slate-200 hover:border-blue-300 transition-all"
              >
                Sign In
              </button>
              {ENABLE_HARDCODED_USER && (
                <Link
                  to="/dashboard"
                  className="bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800 text-white px-8 py-4 rounded-2xl text-lg font-bold shadow-2xl shadow-green-500/30 hover:shadow-3xl hover:shadow-green-500/40 transform hover:-translate-y-1 transition-all"
                >
                  🧪 Test Dashboard
                </Link>
              )}
            </div>
          </div>
        </div>
      </section>

      {/* Stats Section */}
      <section className="py-16 bg-white/50 backdrop-blur-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-2 lg:grid-cols-4 gap-8">
            {stats.map((stat, index) => (
              <div key={index} className="text-center">
                <div className="text-4xl lg:text-5xl font-extrabold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent mb-2">
                  {stat.number}
                </div>
                <div className="text-slate-600 font-semibold">{stat.label}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-4xl lg:text-5xl font-bold text-slate-900 mb-6">
              Why Choose EMRR?
            </h2>
            <p className="text-xl text-slate-600 max-w-3xl mx-auto">
              Our platform combines advanced technology with user-friendly design to deliver 
              exceptional healthcare experiences for both patients and providers.
            </p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {features.map((feature, index) => (
              <div key={index} className="bg-white/80 backdrop-blur-sm p-8 rounded-2xl shadow-lg hover:shadow-xl border border-slate-100 hover:border-blue-200 transition-all group">
                <div className="text-4xl mb-4 group-hover:scale-110 transition-transform">
                  {feature.icon}
                </div>
                <h3 className="text-xl font-bold text-slate-900 mb-3">{feature.title}</h3>
                <p className="text-slate-600 leading-relaxed">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 bg-gradient-to-r from-blue-600 to-indigo-600">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-4xl lg:text-5xl font-bold text-white mb-6">
            Ready to Transform Healthcare?
          </h2>
          <p className="text-xl text-blue-100 mb-8 max-w-3xl mx-auto">
            Join thousands of healthcare providers and patients who trust EMRR for their medical needs.
          </p>
          <button
            onClick={() => { setAuthType('signup'); setShowAuthModal(true); }}
            className="bg-white hover:bg-slate-50 text-blue-600 px-8 py-4 rounded-2xl text-lg font-bold shadow-2xl hover:shadow-3xl transform hover:-translate-y-1 transition-all"
          >
            Get Started Today
          </button>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-slate-900 text-white py-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div>
              <div className="flex items-center gap-3 mb-4">
                <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-indigo-500 rounded-lg flex items-center justify-center">
                  <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                  </svg>
                </div>
                <div>
                  <h3 className="text-lg font-bold">EMRR Platform</h3>
                  <p className="text-xs text-slate-400">Healthcare Excellence</p>
                </div>
              </div>
              <p className="text-slate-400 text-sm">
                Revolutionizing healthcare through intelligent EMR systems and patient-centered care.
              </p>
            </div>
            <div>
              <h4 className="font-semibold mb-4">For Patients</h4>
              <ul className="space-y-2 text-sm text-slate-400">
                <li><Link to="/patient-portal" className="hover:text-white transition-colors">Patient Portal</Link></li>
                <li><Link to="/medical-records" className="hover:text-white transition-colors">Medical Records</Link></li>
                <li><Link to="/appointments" className="hover:text-white transition-colors">Appointments</Link></li>
                <li><Link to="/prescriptions" className="hover:text-white transition-colors">Prescriptions</Link></li>
              </ul>
            </div>
            <div>
              <h4 className="font-semibold mb-4">For Providers</h4>
              <ul className="space-y-2 text-sm text-slate-400">
                <li><Link to="/hospital-portal" className="hover:text-white transition-colors">Hospital Portal</Link></li>
                <li><Link to="/patient-management" className="hover:text-white transition-colors">Patient Management</Link></li>
                <li><Link to="/analytics" className="hover:text-white transition-colors">Analytics</Link></li>
                <li><Link to="/integration" className="hover:text-white transition-colors">Integration</Link></li>
              </ul>
            </div>
            <div>
              <h4 className="font-semibold mb-4">Support</h4>
              <ul className="space-y-2 text-sm text-slate-400">
                <li><Link to="/help" className="hover:text-white transition-colors">Help Center</Link></li>
                <li><Link to="/contact" className="hover:text-white transition-colors">Contact Us</Link></li>
                <li><Link to="/privacy" className="hover:text-white transition-colors">Privacy Policy</Link></li>
                <li><Link to="/terms" className="hover:text-white transition-colors">Terms of Service</Link></li>
              </ul>
            </div>
          </div>
          <div className="border-t border-slate-800 mt-8 pt-8 text-center text-sm text-slate-400">
            <p>&copy; 2025 EMRR Platform. All rights reserved. HIPAA Compliant & Secure.</p>
          </div>
        </div>
      </footer>

      {/* Authentication Modal */}
      <AuthModal
        isOpen={showAuthModal}
        onClose={() => setShowAuthModal(false)}
        authType={authType}
        onAuthTypeChange={setAuthType}
      />
    </div>
  );
}
