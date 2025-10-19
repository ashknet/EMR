import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../config/apiConfig';
import { API_BASE_URL } from '../config/apiConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';
import axios from 'axios';

interface DashboardData {
  patientId: string;
  patientName: string;
  primaryInsurance: {
    payerName: string;
    planName: string;
    memberId: string;
    deductible: number;
    deductibleMet: number;
    outOfPocketMax: number;
    outOfPocketMet: number;
  } | null;
  familyMemberCount: number;
  upcomingVisits: Array<{
    encounterId: string;
    encounterType: string;
    providerName: string;
    plannedStartDate: string;
    facilityName: string;
  }>;
  recentDocuments: Array<{
    documentId: string;
    fileName: string;
    documentType: string;
    createdAt: string;
  }>;
  unreadNotifications: number;
  quickActions: Array<{
    label: string;
    url: string;
    icon: string;
  }>;
}

export default function Dashboard() {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  // Use mock patient ID for development - in production, get from auth context
  const patientId = MOCK_PATIENT_ID;

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      setLoading(true);
      
      // Load data from multiple API endpoints
      const [patientResponse, insuranceResponse, familyResponse] = await Promise.allSettled([
        axios.get(`${API_BASE_URL}/patients/${patientId}`),
        axios.get(`${API_BASE_URL}/insurance/patient/${patientId}`),
        axios.get(`${API_BASE_URL}/family/patient/${patientId}`).catch(() => ({ data: [] })) // Fallback if endpoint doesn't exist
      ]);

      const patientData = patientResponse.status === 'fulfilled' ? patientResponse.value.data : null;
      const insuranceData = insuranceResponse.status === 'fulfilled' ? insuranceResponse.value.data : [];
      const familyData = familyResponse.status === 'fulfilled' ? familyResponse.value.data : [];

      // Create dashboard data from real API responses
      const dashboardData: DashboardData = {
        patientId: patientId,
        patientName: patientData ? `${patientData.firstName} ${patientData.lastName}` : 'Patient',
        primaryInsurance: insuranceData.length > 0 ? {
          payerName: insuranceData[0].payerName || 'Unknown',
          planName: insuranceData[0].planName || 'Unknown Plan',
          memberId: insuranceData[0].memberId || 'Unknown',
          deductible: insuranceData[0].deductible || 5000,
          deductibleMet: insuranceData[0].deductibleMet || 1500,
          outOfPocketMax: insuranceData[0].outOfPocketMax || 8000,
          outOfPocketMet: insuranceData[0].outOfPocketMet || 2000,
        } : null,
        familyMemberCount: familyData.length || 0,
        upcomingVisits: [], // TODO: Load from visits API
        recentDocuments: [], // TODO: Load from documents API
        unreadNotifications: 3, // Mock for now
        quickActions: [
          { label: 'View Profile', url: '/profile', icon: 'user' },
          { label: 'Medical History', url: '/medical-history', icon: 'medical' },
          { label: 'Schedule Appointment', url: '/appointments', icon: 'calendar' },
          { label: 'Upload Documents', url: '/documents', icon: 'upload' },
          { label: 'Manage Family', url: '/family', icon: 'users' },
          { label: 'Share Records', url: '/transfers', icon: 'share' },
        ]
      };

      setData(dashboardData);
    } catch (err: any) {
      console.error('Dashboard load error:', err);
      setError('Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  const handleQuickAction = (action: { label: string; url: string; icon: string }) => {
    if (action.url.startsWith('/')) {
      navigate(action.url);
    } else {
      // Handle external URLs or special actions
      switch (action.label) {
        case 'Medical History':
          navigate('/medical-history');
          break;
        case 'Schedule Appointment':
          alert('Appointment scheduling will be implemented soon!');
          break;
        case 'Upload Documents':
          navigate('/documents');
          break;
        case 'Manage Family':
          navigate('/family');
          break;
        default:
          navigate(action.url);
      }
    }
  };

  if (loading) {
    return (
      <div className="flex flex-col justify-center items-center h-96" role="status" aria-live="polite">
        <div className="relative">
          <div className="animate-spin rounded-full h-16 w-16 border-4 border-primary-200"></div>
          <div className="animate-spin rounded-full h-16 w-16 border-4 border-primary-600 border-t-transparent absolute top-0 left-0"></div>
        </div>
        <p className="mt-4 text-neutral-600 font-medium">Loading your dashboard...</p>
        <span className="sr-only">Loading dashboard...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-gradient-to-br from-red-50 to-red-100 border-2 border-red-200 rounded-2xl p-8 shadow-lg" role="alert">
        <div className="flex items-start gap-4">
          <div className="flex-shrink-0">
            <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div className="flex-1">
            <h2 className="text-red-900 font-bold text-xl">Error Loading Dashboard</h2>
            <p className="text-red-800 mt-2">{error}</p>
            <button
              onClick={loadDashboard}
              className="mt-4 bg-gradient-to-r from-red-600 to-red-700 text-white px-6 py-3 rounded-xl hover:from-red-700 hover:to-red-800 font-semibold shadow-lg hover:shadow-xl transform hover:-translate-y-0.5 transition-all"
              aria-label="Retry loading dashboard"
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (!data) {
    return <div>No data available</div>;
  }

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  // Icon mapping for quick actions
  const getIconForAction = (iconName: string) => {
    const icons: Record<string, JSX.Element> = {
      medical: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
      ),
      calendar: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      ),
      upload: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
        </svg>
      ),
      card: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
        </svg>
      ),
      users: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
        </svg>
      ),
      share: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
        </svg>
      ),
    };
    return icons[iconName] || (
      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
      </svg>
    );
  };

  return (
    <div className="space-y-6">
      {/* Welcome Header */}
      <div className="bg-white rounded-lg shadow-sm border border-neutral-200 p-6">
        <div className="flex items-center gap-3">
          <div style={{width: '40px', height: '40px', background: 'linear-gradient(135deg, #0A4FD5 0%, #0891B2 100%)', borderRadius: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
            <svg style={{width: '20px', height: '20px', color: 'white'}} fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
          </div>
          <div>
            <h1 className="text-xl font-bold text-neutral-900">
              Welcome back, {data.patientName.split(' ')[0]}!
            </h1>
            <p className="text-neutral-600 mt-0.5 text-sm">Here's an overview of your healthcare information</p>
          </div>
        </div>
      </div>

      {/* Notifications Alert */}
      {data.unreadNotifications > 0 && (
        <div className="bg-gradient-to-r from-accent-50 to-accent-100 border border-accent-200 rounded-xl p-4 shadow-sm" role="status">
          <div className="flex items-center gap-3">
            <div className="flex-shrink-0">
              <div className="w-9 h-9 bg-accent-500 rounded-lg flex items-center justify-center">
                <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                </svg>
              </div>
            </div>
            <p className="text-accent-900 font-semibold">
              You have {data.unreadNotifications} unread notification{data.unreadNotifications > 1 ? 's' : ''}
            </p>
          </div>
        </div>
      )}

      {/* Primary Insurance Card */}
      {data.primaryInsurance && (
        <section className="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow border border-neutral-200 p-6" aria-labelledby="insurance-heading">
          <div className="flex items-center gap-2.5 mb-5">
            <div className="w-9 h-9 bg-gradient-to-br from-success-500 to-success-600 rounded-lg flex items-center justify-center">
              <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
              </svg>
            </div>
            <h2 id="insurance-heading" className="text-xl font-bold text-neutral-900">
              Primary Insurance
            </h2>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
              <p className="text-xs text-neutral-500 font-semibold mb-1">Payer</p>
              <p className="font-bold text-neutral-900">{data.primaryInsurance.payerName}</p>
            </div>
            <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
              <p className="text-xs text-neutral-500 font-semibold mb-1">Plan</p>
              <p className="font-bold text-neutral-900">{data.primaryInsurance.planName}</p>
            </div>
            <div className="bg-neutral-50 rounded-lg p-4 border border-neutral-200">
              <p className="text-xs text-neutral-500 font-semibold mb-1">Member ID</p>
              <p className="font-bold text-neutral-900">{data.primaryInsurance.memberId}</p>
            </div>
            <div className="bg-success-50 rounded-lg p-4 border border-success-200">
              <p className="text-xs text-success-700 font-semibold mb-2">Deductible Progress</p>
              <div className="mt-2">
                <div className="bg-success-200 rounded-full h-2 overflow-hidden">
                  <div
                    className="bg-gradient-to-r from-success-500 to-success-600 h-2 rounded-full transition-all duration-500"
                    style={{
                      width: `${(data.primaryInsurance.deductibleMet / data.primaryInsurance.deductible) * 100}%`,
                    }}
                    role="progressbar"
                    aria-valuenow={(data.primaryInsurance.deductibleMet / data.primaryInsurance.deductible) * 100}
                    aria-valuemin={0}
                    aria-valuemax={100}
                    aria-label="Deductible progress"
                  ></div>
                </div>
                <p className="text-xs mt-2 font-bold text-success-700">
                  {formatCurrency(data.primaryInsurance.deductibleMet)} of{' '}
                  {formatCurrency(data.primaryInsurance.deductible)}
                </p>
              </div>
            </div>
          </div>
        </section>
      )}

      {/* Quick Actions */}
      <section className="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow border border-neutral-200 p-6" aria-labelledby="actions-heading">
        <div className="flex items-center gap-2.5 mb-5">
          <div className="w-9 h-9 bg-gradient-to-br from-accent-500 to-accent-600 rounded-lg flex items-center justify-center">
            <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
          </div>
          <h2 id="actions-heading" className="text-xl font-bold text-neutral-900">
            Quick Actions
          </h2>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-3">
          {data.quickActions.map((action, index) => (
            <button
              key={index}
              onClick={() => handleQuickAction(action)}
              className="group flex flex-col items-center p-4 bg-neutral-50 rounded-lg hover:bg-primary-50 border border-neutral-200 hover:border-primary-300 transition-all hover:shadow-md focus:outline-none focus:ring-2 focus:ring-primary-500 cursor-pointer"
              aria-label={action.label}
            >
              <div className="w-10 h-10 bg-white rounded-lg border border-neutral-200 group-hover:border-primary-300 flex items-center justify-center mb-2 transition-colors text-neutral-600 group-hover:text-primary-600">
                {getIconForAction(action.icon)}
              </div>
              <span className="text-xs font-semibold text-center text-neutral-700 group-hover:text-primary-600">{action.label}</span>
            </button>
          ))}
        </div>
      </section>

      {/* Upcoming Visits */}
      <section className="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow border border-neutral-200 p-6" aria-labelledby="visits-heading">
        <div className="flex items-center gap-2.5 mb-5">
          <div className="w-9 h-9 bg-gradient-to-br from-primary-500 to-primary-600 rounded-lg flex items-center justify-center">
            <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
          </div>
          <h2 id="visits-heading" className="text-xl font-bold text-neutral-900">
            Upcoming Visits
          </h2>
        </div>
        {data.upcomingVisits.length === 0 ? (
          <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
            <p className="text-neutral-500 text-sm">No upcoming appointments</p>
          </div>
        ) : (
          <ul className="space-y-3" role="list">
            {data.upcomingVisits.map((visit) => (
              <li key={visit.encounterId} className="bg-gradient-to-r from-primary-50 to-accent-50 border-l-3 border-primary-500 rounded-r-lg pl-4 pr-4 py-3 hover:shadow-sm transition-shadow">
                <p className="font-bold text-neutral-900">{visit.encounterType}</p>
                <p className="text-sm text-neutral-600 mt-1 flex items-center gap-1.5">
                  <svg className="w-4 h-4 text-primary-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  {visit.providerName} at {visit.facilityName}
                </p>
                <p className="text-xs text-neutral-500 mt-1.5 flex items-center gap-1.5 font-medium">
                  <svg className="w-4 h-4 text-accent-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  {formatDate(visit.plannedStartDate)}
                </p>
              </li>
            ))}
          </ul>
        )}
      </section>

      {/* Recent Documents */}
      <section className="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow border border-neutral-200 p-6" aria-labelledby="documents-heading">
        <div className="flex items-center gap-2.5 mb-5">
          <div className="w-9 h-9 bg-gradient-to-br from-accent-500 to-accent-600 rounded-lg flex items-center justify-center">
            <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </div>
          <h2 id="documents-heading" className="text-xl font-bold text-neutral-900">
            Recent Documents
          </h2>
        </div>
        {data.recentDocuments.length === 0 ? (
          <div className="text-center py-8 bg-neutral-50 rounded-lg border border-dashed border-neutral-300">
            <p className="text-neutral-500 text-sm">No recent documents</p>
          </div>
        ) : (
          <ul className="space-y-2" role="list">
            {data.recentDocuments.map((doc) => (
              <li key={doc.documentId} className="flex items-center justify-between p-3 bg-neutral-50 rounded-lg hover:bg-accent-50 border border-neutral-200 hover:border-accent-300 transition-all hover:shadow-sm">
                <div className="flex items-center gap-3">
                  <div className="w-9 h-9 bg-gradient-to-br from-accent-500 to-accent-600 rounded-lg flex items-center justify-center flex-shrink-0">
                    <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                    </svg>
                  </div>
                  <div>
                    <p className="font-bold text-neutral-900 text-sm">{doc.fileName}</p>
                    <p className="text-xs text-neutral-600">{doc.documentType}</p>
                  </div>
                </div>
                <p className="text-xs text-neutral-500 font-medium">{formatDate(doc.createdAt)}</p>
              </li>
            ))}
          </ul>
        )}
      </section>

      {/* Family Summary */}
      <section className="bg-gradient-to-br from-primary-500 via-primary-600 to-accent-600 rounded-xl shadow-lg p-6 text-white relative overflow-hidden" aria-labelledby="family-heading">
        <div className="absolute top-0 right-0 w-32 h-32 bg-white/10 rounded-full -mr-16 -mt-16"></div>
        <div className="absolute bottom-0 left-0 w-24 h-24 bg-white/10 rounded-full -ml-12 -mb-12"></div>
        <div className="relative z-10">
          <div className="flex items-center gap-2.5 mb-3">
            <div className="w-10 h-10 bg-white/20 backdrop-blur-sm rounded-lg flex items-center justify-center">
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <h2 id="family-heading" className="text-xl font-bold">
              Family Members
            </h2>
          </div>
          <p className="text-primary-100 mb-4">
            You have {data.familyMemberCount} family member{data.familyMemberCount !== 1 ? 's' : ''} in your care
          </p>
          <a
            href="/family"
            className="inline-flex items-center gap-2 bg-white text-primary-600 px-5 py-2.5 rounded-lg font-semibold hover:bg-primary-50 transition-all shadow-md hover:shadow-lg"
          >
            Manage Family
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M13 7l5 5m0 0l-5 5m5-5H6" />
            </svg>
          </a>
        </div>
      </section>
    </div>
  );
}

