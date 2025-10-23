import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import TestModeToggle from './components/TestModeToggle';
import DebugInfo from './components/DebugInfo';
import LandingPage from './pages/LandingPage';
import About from './pages/About';
import Contact from './pages/Contact';
import PatientIntakeForm from './pages/PatientIntakeForm';
import ComprehensiveIntake from './pages/ComprehensiveIntake';
import AdminMetadata from './pages/AdminMetadata';
import Dashboard from './pages/Dashboard';
import Profile from './pages/Profile';
import MedicalHistory from './pages/MedicalHistory';
import Insurance from './pages/Insurance';
import Providers from './pages/Providers';
import Family from './pages/Family';
import SocialHistory from './pages/SocialHistory';
import ConsentAudit from './pages/ConsentAudit';
import Documents from './pages/Documents';
import Transfers from './pages/Transfers';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public Routes */}
        <Route path="/" element={<LandingPage />} />
        <Route path="/about" element={<About />} />
        <Route path="/contact" element={<Contact />} />
        
        {/* Protected Routes */}
        <Route element={<ProtectedRoute><Layout /></ProtectedRoute>}>
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/profile/intake" element={<PatientIntakeForm />} />
          <Route path="/comprehensive-intake" element={<ComprehensiveIntake />} />
          <Route path="/medical-history" element={<MedicalHistory />} />
          <Route path="/insurance" element={<Insurance />} />
          <Route path="/providers" element={<Providers />} />
          <Route path="/family" element={<Family />} />
          <Route path="/social-history" element={<SocialHistory />} />
          <Route path="/consent" element={<ConsentAudit />} />
          <Route path="/documents" element={<Documents />} />
          <Route path="/transfers" element={<Transfers />} />
          <Route path="/admin/metadata" element={<AdminMetadata />} />
        </Route>
        
        {/* Catch all route */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
      
      {/* Test Mode Toggle - Available on all pages */}
      <TestModeToggle />
      
      {/* Debug Info - Available in test mode */}
      <DebugInfo />
    </BrowserRouter>
  );
}

export default App;

