import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { API_BASE } from '../config/apiConfig';
import { MOCK_USER } from '../config/mockUser';

interface LegalConsent {
  hipaaAgreed: boolean;
  consentToTreat: boolean;
  advanceDirectives: boolean;
  advanceDirectivesPath?: string;
  assignmentOfBenefits: boolean;
  financialResponsibility: boolean;
  signedOnUtc?: string;
  signaturePath?: string;
}

export default function ConsentAudit() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [consents, setConsents] = useState<LegalConsent>({
    hipaaAgreed: false,
    consentToTreat: false,
    advanceDirectives: false,
    assignmentOfBenefits: false,
    financialResponsibility: false,
  });

  useEffect(() => {
    loadConsents();
  }, []);

  const loadConsents = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${API_BASE}/api/patients/${MOCK_USER.patientId}/step/legalconsents`);
      if (response.ok) {
        const data = await response.json();
        // Map the response data to match our LegalConsent interface
        setConsents({
          hipaaAgreed: data.hipaaAgreed || false,
          consentToTreat: data.consentToTreat || false,
          advanceDirectives: data.advanceDirectives || false,
          advanceDirectivesPath: data.advanceDirectivesPath,
          assignmentOfBenefits: data.assignmentOfBenefits || false,
          financialResponsibility: data.financialResponsibility || false,
          signedOnUtc: data.signedOnUtc,
          signaturePath: data.signaturePath
        });
      }
    } catch (error) {
      console.error('Error loading consents:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    try {
      setIsSaving(true);
      const response = await fetch(`${API_BASE}/api/patients/${MOCK_USER.patientId}/step/legalconsents`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          patientId: MOCK_USER.patientId,
          hipaaAgreed: consents.hipaaAgreed,
          consentToTreat: consents.consentToTreat,
          advanceDirectives: consents.advanceDirectives,
          advanceDirectivesPath: consents.advanceDirectivesPath,
          assignmentOfBenefits: consents.assignmentOfBenefits,
          financialResponsibility: consents.financialResponsibility,
          signedOnUtc: new Date().toISOString(),
          signaturePath: consents.signaturePath
        }),
      });

      if (response.ok) {
        setIsEditing(false);
        alert('Consents updated successfully!');
        loadConsents();
      }
    } catch (error) {
      console.error('Error saving consents:', error);
      alert('Error saving consents');
    } finally {
      setIsSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500 mx-auto mb-4"></div>
          <p className="text-neutral-600">Loading consents...</p>
        </div>
      </div>
    );
  }

  const hasConsents = consents.hipaaAgreed || consents.consentToTreat;

  return (
    <div className="max-w-6xl mx-auto p-6">
      {/* Header */}
      <div className="bg-white rounded-xl shadow-md p-6 mb-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">Legal Consents & Agreements</h1>
            <p className="text-gray-600">Review and manage your healthcare consents and legal agreements</p>
          </div>
          <div className="flex gap-2">
            <button
              onClick={() => navigate('/comprehensive-intake')}
              className="px-4 py-2 border-2 border-primary-300 rounded-lg hover:bg-primary-50 font-semibold text-primary-700 transition-all flex items-center gap-2"
            >
              <svg style={{width: '16px', height: '16px'}} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              Update All
            </button>
            {!isEditing && hasConsents && (
              <button
                onClick={() => setIsEditing(true)}
                className="bg-primary-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-primary-700 transition-all flex items-center gap-2"
              >
                <svg style={{width: '16px', height: '16px'}} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                </svg>
                Edit
              </button>
            )}
            {isEditing && (
              <>
                <button
                  onClick={handleSave}
                  disabled={isSaving}
                  className="bg-green-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-green-700 transition-all disabled:opacity-50"
                >
                  {isSaving ? 'Saving...' : 'Save'}
                </button>
                <button
                  onClick={() => {
                    setIsEditing(false);
                    loadConsents();
                  }}
                  className="bg-gray-300 text-gray-700 px-4 py-2 rounded-lg font-semibold hover:bg-gray-400 transition-all"
                >
                  Cancel
                </button>
              </>
            )}
          </div>
        </div>
      </div>

      {/* No Consents */}
      {!hasConsents && !isEditing && (
        <div className="bg-white rounded-xl shadow-md p-12 text-center">
          <div className="w-24 h-24 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-12 h-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </div>
          <h3 className="text-xl font-semibold text-gray-900 mb-2">No Consents on File</h3>
          <p className="text-gray-600 mb-6">Complete your legal consents and agreements to proceed with care</p>
          <div className="flex gap-3 justify-center">
            <button
              onClick={() => navigate('/comprehensive-intake')}
              className="bg-primary-600 text-white px-6 py-3 rounded-lg font-semibold hover:bg-primary-700"
            >
              Complete Intake Form
            </button>
            <button
              onClick={() => setIsEditing(true)}
              className="bg-gray-200 text-gray-700 px-6 py-3 rounded-lg font-semibold hover:bg-gray-300"
            >
              Quick Add
            </button>
          </div>
        </div>
      )}

      {/* Consent Information */}
      {(hasConsents || isEditing) && (
        <div className="space-y-6">
          {/* HIPAA Privacy Agreement */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <div className="flex items-start gap-4">
              <div className={`w-12 h-12 rounded-lg flex items-center justify-center flex-shrink-0 ${
                consents.hipaaAgreed ? 'bg-green-100' : 'bg-gray-100'
              }`}>
                {consents.hipaaAgreed ? (
                  <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                  </svg>
                ) : (
                  <svg className="w-6 h-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                )}
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-bold text-gray-900 mb-2">HIPAA Privacy Agreement</h3>
                <p className="text-gray-600 mb-3">
                  I acknowledge that I have received and reviewed the Notice of Privacy Practices and understand how my health information may be used and disclosed.
                </p>
                {isEditing ? (
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={consents.hipaaAgreed}
                      onChange={(e) => setConsents({...consents, hipaaAgreed: e.target.checked})}
                      className="w-5 h-5 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
                    />
                    <span className="text-sm font-medium text-gray-700">I agree to the HIPAA Privacy Agreement</span>
                  </label>
                ) : (
                  <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-semibold ${
                    consents.hipaaAgreed ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
                  }`}>
                    {consents.hipaaAgreed ? '✓ Agreed' : 'Not Agreed'}
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* Consent to Treat */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <div className="flex items-start gap-4">
              <div className={`w-12 h-12 rounded-lg flex items-center justify-center flex-shrink-0 ${
                consents.consentToTreat ? 'bg-green-100' : 'bg-gray-100'
              }`}>
                {consents.consentToTreat ? (
                  <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                  </svg>
                ) : (
                  <svg className="w-6 h-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                  </svg>
                )}
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-bold text-gray-900 mb-2">Consent to Treat</h3>
                <p className="text-gray-600 mb-3">
                  I voluntarily consent to medical treatment and authorize healthcare providers to examine, diagnose, and treat my medical condition.
                </p>
                {isEditing ? (
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={consents.consentToTreat}
                      onChange={(e) => setConsents({...consents, consentToTreat: e.target.checked})}
                      className="w-5 h-5 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
                    />
                    <span className="text-sm font-medium text-gray-700">I consent to medical treatment</span>
                  </label>
                ) : (
                  <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-semibold ${
                    consents.consentToTreat ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
                  }`}>
                    {consents.consentToTreat ? '✓ Consented' : 'Not Consented'}
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* Advance Directives */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <div className="flex items-start gap-4">
              <div className={`w-12 h-12 rounded-lg flex items-center justify-center flex-shrink-0 ${
                consents.advanceDirectives ? 'bg-blue-100' : 'bg-gray-100'
              }`}>
                <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-bold text-gray-900 mb-2">Advance Directives</h3>
                <p className="text-gray-600 mb-3">
                  Living will, healthcare proxy, or durable power of attorney for healthcare decisions.
                </p>
                {isEditing ? (
                  <div className="space-y-3">
                    <label className="flex items-center gap-2 cursor-pointer">
                      <input
                        type="checkbox"
                        checked={consents.advanceDirectives}
                        onChange={(e) => setConsents({...consents, advanceDirectives: e.target.checked})}
                        className="w-5 h-5 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
                      />
                      <span className="text-sm font-medium text-gray-700">I have advance directives on file</span>
                    </label>
                    {consents.advanceDirectives && (
                      <input
                        type="text"
                        value={consents.advanceDirectivesPath || ''}
                        onChange={(e) => setConsents({...consents, advanceDirectivesPath: e.target.value})}
                        placeholder="Document path or location"
                        className="w-full border border-gray-300 rounded-lg p-2 text-sm"
                      />
                    )}
                  </div>
                ) : (
                  <div>
                    <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-semibold ${
                      consents.advanceDirectives ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-700'
                    }`}>
                      {consents.advanceDirectives ? '✓ On File' : 'Not on File'}
                    </div>
                    {consents.advanceDirectivesPath && (
                      <p className="text-sm text-gray-600 mt-2">Document: {consents.advanceDirectivesPath}</p>
                    )}
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* Assignment of Benefits */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <div className="flex items-start gap-4">
              <div className={`w-12 h-12 rounded-lg flex items-center justify-center flex-shrink-0 ${
                consents.assignmentOfBenefits ? 'bg-purple-100' : 'bg-gray-100'
              }`}>
                <svg className="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                </svg>
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-bold text-gray-900 mb-2">Assignment of Benefits</h3>
                <p className="text-gray-600 mb-3">
                  I authorize payment of medical benefits directly to the healthcare provider for services rendered.
                </p>
                {isEditing ? (
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={consents.assignmentOfBenefits}
                      onChange={(e) => setConsents({...consents, assignmentOfBenefits: e.target.checked})}
                      className="w-5 h-5 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
                    />
                    <span className="text-sm font-medium text-gray-700">I authorize assignment of benefits</span>
                  </label>
                ) : (
                  <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-semibold ${
                    consents.assignmentOfBenefits ? 'bg-purple-100 text-purple-700' : 'bg-gray-100 text-gray-700'
                  }`}>
                    {consents.assignmentOfBenefits ? '✓ Authorized' : 'Not Authorized'}
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* Financial Responsibility */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <div className="flex items-start gap-4">
              <div className={`w-12 h-12 rounded-lg flex items-center justify-center flex-shrink-0 ${
                consents.financialResponsibility ? 'bg-orange-100' : 'bg-gray-100'
              }`}>
                <svg className="w-6 h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-bold text-gray-900 mb-2">Financial Responsibility Agreement</h3>
                <p className="text-gray-600 mb-3">
                  I understand that I am financially responsible for all charges not covered by insurance, including deductibles, co-payments, and non-covered services.
                </p>
                {isEditing ? (
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={consents.financialResponsibility}
                      onChange={(e) => setConsents({...consents, financialResponsibility: e.target.checked})}
                      className="w-5 h-5 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
                    />
                    <span className="text-sm font-medium text-gray-700">I acknowledge financial responsibility</span>
                  </label>
                ) : (
                  <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-semibold ${
                    consents.financialResponsibility ? 'bg-orange-100 text-orange-700' : 'bg-gray-100 text-gray-700'
                  }`}>
                    {consents.financialResponsibility ? '✓ Acknowledged' : 'Not Acknowledged'}
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* Signature Information */}
          {consents.signedOnUtc && (
            <div className="bg-gradient-to-r from-blue-50 to-purple-50 rounded-xl shadow-md p-6 border-2 border-blue-200">
              <div className="flex items-center gap-3 mb-3">
                <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <h3 className="text-lg font-bold text-gray-900">Electronic Signature</h3>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                <div>
                  <label className="block text-gray-600 mb-1">Signed Date</label>
                  <p className="font-semibold text-gray-900">
                    {new Date(consents.signedOnUtc).toLocaleDateString('en-US', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                      hour: '2-digit',
                      minute: '2-digit'
                    })}
                  </p>
                </div>
                {consents.signaturePath && (
                  <div>
                    <label className="block text-gray-600 mb-1">Signature</label>
                    <p className="font-semibold text-gray-900">{consents.signaturePath}</p>
                  </div>
                )}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
