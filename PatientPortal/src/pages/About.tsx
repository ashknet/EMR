import { Link } from 'react-router-dom';

export default function About() {
  const team = [
    {
      name: 'Bala Ramudu',
      role: 'Chief Medical Officer',
      image: '👩‍⚕️',
      bio: 'Leading healthcare innovation with 15+ years of experience in medical technology.'
    },
    {
      name: 'Bheeshmudu',
      role: 'Chief Technology Officer',
      image: '👨‍💻',
      bio: 'Expert in healthcare IT systems and patient data security protocols.'
    },
    {
      name: 'Dhronacharyudu',
      role: 'Head of Patient Experience',
      image: '👩‍⚕️',
      bio: 'Dedicated to improving patient outcomes through technology-enabled care.'
    },
    {
      name: 'Sanku Theerdhulu',
      role: 'Security & Compliance Lead',
      image: '👨‍🔒',
      bio: 'Ensuring HIPAA compliance and maintaining the highest security standards.'
    }
  ];

  const values = [
    {
      icon: '🔒',
      title: 'Security First',
      description: 'We prioritize patient data security with enterprise-grade encryption and compliance.'
    },
    {
      icon: '🤝',
      title: 'Patient-Centered',
      description: 'Every feature is designed with patient needs and healthcare outcomes in mind.'
    },
    {
      icon: '⚡',
      title: 'Innovation',
      description: 'Leveraging cutting-edge technology to transform healthcare delivery.'
    },
    {
      icon: '🌍',
      title: 'Accessibility',
      description: 'Making quality healthcare accessible to everyone, everywhere.'
    }
  ];

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-50">
      {/* Navigation */}
      <nav className="bg-white/80 backdrop-blur-xl border-b border-slate-200 sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-4">
            <Link to="/" className="flex items-center gap-3">
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
            </Link>
            <div className="flex items-center gap-4">
              <Link to="/" className="text-slate-600 hover:text-blue-600 font-medium transition-colors">
                Home
              </Link>
              <Link to="/contact" className="text-slate-600 hover:text-blue-600 font-medium transition-colors">
                Contact
              </Link>
              <Link to="/" className="bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white px-6 py-2.5 rounded-xl text-sm font-semibold shadow-lg shadow-blue-500/30 hover:shadow-xl hover:shadow-blue-500/40 transform hover:-translate-y-0.5 transition-all">
                Get Started
              </Link>
            </div>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <section className="py-20 lg:py-32">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <h1 className="text-5xl lg:text-6xl font-extrabold text-slate-900 mb-6">
              About
              <span className="block bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                EMRR Platform
              </span>
            </h1>
            <p className="text-xl lg:text-2xl text-slate-600 mb-8 max-w-4xl mx-auto leading-relaxed">
              We're revolutionizing healthcare through intelligent Electronic Medical Record systems 
              that connect patients and providers seamlessly.
            </p>
          </div>
        </div>
      </section>

      {/* Mission Section */}
      <section className="py-20 bg-white/50 backdrop-blur-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
            <div>
              <h2 className="text-4xl font-bold text-slate-900 mb-6">Our Mission</h2>
              <p className="text-lg text-slate-600 mb-6 leading-relaxed">
                To transform healthcare delivery by providing intelligent, secure, and user-friendly 
                Electronic Medical Record systems that enhance patient care and streamline provider workflows.
              </p>
              <p className="text-lg text-slate-600 leading-relaxed">
                We believe that technology should empower healthcare professionals to focus on what matters most: 
                delivering exceptional patient care.
              </p>
            </div>
            <div className="bg-gradient-to-br from-blue-600 to-indigo-600 rounded-2xl p-8 text-white">
              <h3 className="text-2xl font-bold mb-4">Our Vision</h3>
              <p className="text-blue-100 leading-relaxed">
                A world where healthcare is accessible, efficient, and patient-centered, 
                powered by intelligent technology that connects every aspect of the healthcare ecosystem.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Values Section */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-4xl font-bold text-slate-900 mb-6">Our Values</h2>
            <p className="text-xl text-slate-600 max-w-3xl mx-auto">
              These core principles guide everything we do and shape our commitment to healthcare excellence.
            </p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
            {values.map((value, index) => (
              <div key={index} className="bg-white/80 backdrop-blur-sm p-8 rounded-2xl shadow-lg hover:shadow-xl border border-slate-100 hover:border-blue-200 transition-all text-center">
                <div className="text-4xl mb-4">{value.icon}</div>
                <h3 className="text-xl font-bold text-slate-900 mb-3">{value.title}</h3>
                <p className="text-slate-600 leading-relaxed">{value.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Team Section */}
      <section className="py-20 bg-white/50 backdrop-blur-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-4xl font-bold text-slate-900 mb-6">Meet Our Team</h2>
            <p className="text-xl text-slate-600 max-w-3xl mx-auto">
              Healthcare and technology experts working together to transform patient care.
            </p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
            {team.map((member, index) => (
              <div key={index} className="bg-white/80 backdrop-blur-sm p-6 rounded-2xl shadow-lg hover:shadow-xl border border-slate-100 hover:border-blue-200 transition-all text-center">
                <div className="text-6xl mb-4">{member.image}</div>
                <h3 className="text-xl font-bold text-slate-900 mb-2">{member.name}</h3>
                <p className="text-blue-600 font-semibold mb-3">{member.role}</p>
                <p className="text-slate-600 text-sm leading-relaxed">{member.bio}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Technology Section */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
            <div>
              <h2 className="text-4xl font-bold text-slate-900 mb-6">Cutting-Edge Technology</h2>
              <p className="text-lg text-slate-600 mb-6 leading-relaxed">
                Our platform leverages the latest in healthcare technology, including AI-powered 
                patient intake, real-time data synchronization, and advanced security protocols.
              </p>
              <ul className="space-y-4">
                <li className="flex items-center gap-3">
                  <div className="w-2 h-2 bg-blue-600 rounded-full"></div>
                  <span className="text-slate-700">AI-Powered EMR Agent for automated patient intake</span>
                </li>
                <li className="flex items-center gap-3">
                  <div className="w-2 h-2 bg-blue-600 rounded-full"></div>
                  <span className="text-slate-700">Real-time data synchronization across all devices</span>
                </li>
                <li className="flex items-center gap-3">
                  <div className="w-2 h-2 bg-blue-600 rounded-full"></div>
                  <span className="text-slate-700">Enterprise-grade security with end-to-end encryption</span>
                </li>
                <li className="flex items-center gap-3">
                  <div className="w-2 h-2 bg-blue-600 rounded-full"></div>
                  <span className="text-slate-700">HIPAA-compliant infrastructure and processes</span>
                </li>
              </ul>
            </div>
            <div className="bg-gradient-to-br from-slate-900 to-slate-800 rounded-2xl p-8 text-white">
              <h3 className="text-2xl font-bold mb-6">Our Impact</h3>
              <div className="space-y-6">
                <div>
                  <div className="text-3xl font-bold text-blue-400 mb-2">99.9%</div>
                  <div className="text-slate-300">System Uptime</div>
                </div>
                <div>
                  <div className="text-3xl font-bold text-blue-400 mb-2">10K+</div>
                  <div className="text-slate-300">Active Patients</div>
                </div>
                <div>
                  <div className="text-3xl font-bold text-blue-400 mb-2">500+</div>
                  <div className="text-slate-300">Healthcare Providers</div>
                </div>
                <div>
                  <div className="text-3xl font-bold text-blue-400 mb-2">24/7</div>
                  <div className="text-slate-300">Support Available</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 bg-gradient-to-r from-blue-600 to-indigo-600">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-4xl font-bold text-white mb-6">
            Ready to Join Our Mission?
          </h2>
          <p className="text-xl text-blue-100 mb-8 max-w-3xl mx-auto">
            Be part of the healthcare revolution. Whether you're a patient or a healthcare provider, 
            we're here to support your journey.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link to="/" className="bg-white hover:bg-slate-50 text-blue-600 px-8 py-4 rounded-2xl text-lg font-bold shadow-2xl hover:shadow-3xl transform hover:-translate-y-1 transition-all">
              Get Started
            </Link>
            <Link to="/contact" className="bg-transparent hover:bg-white/10 text-white border-2 border-white px-8 py-4 rounded-2xl text-lg font-bold transition-all">
              Contact Us
            </Link>
          </div>
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
    </div>
  );
}
