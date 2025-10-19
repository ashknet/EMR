import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
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
        <Route element={<Layout />}>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
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
      </Routes>
    </BrowserRouter>
  );
}

export default App;

