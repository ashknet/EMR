import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { API_BASE } from '../config/apiConfig';
import { MOCK_USER } from '../config/mockUser';

interface SocialHistoryData {
  smokingStatus?: string;
  alcoholUse?: string;
  drugUse?: string;
  occupation?: string;
  livingSituation?: string;
  exerciseFrequency?: string;
  diet?: string;
  stressLevel?: string;
  sleepHours?: number;
}

export default function SocialHistory() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [socialHistory, setSocialHistory] = useState<SocialHistoryData>({});

  useEffect(() => {
    loadSocialHistory();
  }, []);

  const loadSocialHistory = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${API_BASE}/api/patients/${MOCK_USER.patientId}/social-history`);
      if (response.ok) {
        const data = await response.json();
        setSocialHistory(data);
      }
    } catch (error) {
      console.error('Error loading social history:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    try {
      setIsSaving(true);
      const response = await fetch(`${API_BASE}/api/patients/${MOCK_USER.patientId}/social-history`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(socialHistory),
      });

      if (response.ok) {
        setIsEditing(false);
        alert('Social history updated successfully!');
      }
    } catch (error) {
      console.error('Error saving social history:', error);
      alert('Error saving social history');
    } finally {
      setIsSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500 mx-auto mb-4"></div>
          <p className="text-neutral-600">Loading social history...</p>
        </div>
      </div>
    );
  }

  const hasSocialHistory = Object.values(socialHistory).some(value => value !== null && value !== undefined && value !== '');

  return (
    <div className="max-w-6xl mx-auto p-6">
      {/* Header */}
      <div className="bg-white rounded-xl shadow-md p-6 mb-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">Social History</h1>
            <p className="text-gray-600">Lifestyle and social factors that affect your health</p>
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
            {!isEditing && hasSocialHistory && (
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
                    loadSocialHistory();
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

      {/* No Social History */}
      {!hasSocialHistory && !isEditing && (
        <div className="bg-white rounded-xl shadow-md p-12 text-center">
          <div className="w-24 h-24 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-12 h-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
          </div>
          <h3 className="text-xl font-semibold text-gray-900 mb-2">No Social History</h3>
          <p className="text-gray-600 mb-6">Add your lifestyle and social information to help provide better care</p>
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

      {/* Social History Information */}
      {(hasSocialHistory || isEditing) && (
        <div className="space-y-6">
          {/* Substance Use */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
                <svg className="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
              </div>
              <div>
                <h2 className="text-xl font-bold text-gray-900">Substance Use</h2>
                <p className="text-sm text-gray-600">Tobacco, alcohol, and drug use</p>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Smoking Status</label>
                {isEditing ? (
                  <select
                    value={socialHistory.smokingStatus || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, smokingStatus: e.target.value})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                  >
                    <option value="">Select...</option>
                    <option value="Never">Never Smoked</option>
                    <option value="Current">Current Smoker</option>
                    <option value="Former">Former Smoker</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{socialHistory.smokingStatus || 'Not provided'}</p>
                )}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Alcohol Use</label>
                {isEditing ? (
                  <select
                    value={socialHistory.alcoholUse || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, alcoholUse: e.target.value})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                  >
                    <option value="">Select...</option>
                    <option value="None">None</option>
                    <option value="Occasional">Occasional (1-2 drinks/week)</option>
                    <option value="Moderate">Moderate (3-7 drinks/week)</option>
                    <option value="Heavy">Heavy (8+ drinks/week)</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{socialHistory.alcoholUse || 'Not provided'}</p>
                )}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Drug Use</label>
                {isEditing ? (
                  <select
                    value={socialHistory.drugUse || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, drugUse: e.target.value})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                  >
                    <option value="">Select...</option>
                    <option value="None">None</option>
                    <option value="Prescription">Prescription Only</option>
                    <option value="Recreational">Recreational</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{socialHistory.drugUse || 'Not provided'}</p>
                )}
              </div>
            </div>
          </div>

          {/* Lifestyle */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5" />
                </svg>
              </div>
              <div>
                <h2 className="text-xl font-bold text-gray-900">Lifestyle & Wellness</h2>
                <p className="text-sm text-gray-600">Exercise, diet, and daily habits</p>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Exercise Frequency</label>
                {isEditing ? (
                  <select
                    value={socialHistory.exerciseFrequency || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, exerciseFrequency: e.target.value})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                  >
                    <option value="">Select...</option>
                    <option value="None">None</option>
                    <option value="Rare">Rarely (&lt; 1x/week)</option>
                    <option value="Occasional">Occasional (1-2x/week)</option>
                    <option value="Regular">Regular (3-4x/week)</option>
                    <option value="Daily">Daily (5+ x/week)</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{socialHistory.exerciseFrequency || 'Not provided'}</p>
                )}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Diet</label>
                {isEditing ? (
                  <select
                    value={socialHistory.diet || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, diet: e.target.value})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                  >
                    <option value="">Select...</option>
                    <option value="Balanced">Balanced</option>
                    <option value="Vegetarian">Vegetarian</option>
                    <option value="Vegan">Vegan</option>
                    <option value="Low-Carb">Low Carb</option>
                    <option value="Mediterranean">Mediterranean</option>
                    <option value="Other">Other</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{socialHistory.diet || 'Not provided'}</p>
                )}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Stress Level</label>
                {isEditing ? (
                  <select
                    value={socialHistory.stressLevel || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, stressLevel: e.target.value})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                  >
                    <option value="">Select...</option>
                    <option value="Low">Low</option>
                    <option value="Moderate">Moderate</option>
                    <option value="High">High</option>
                    <option value="Very High">Very High</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{socialHistory.stressLevel || 'Not provided'}</p>
                )}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Sleep (hours/night)</label>
                {isEditing ? (
                  <input
                    type="number"
                    value={socialHistory.sleepHours || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, sleepHours: parseInt(e.target.value)})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                    min="0"
                    max="24"
                    placeholder="7-8"
                  />
                ) : (
                  <p className="text-gray-900">{socialHistory.sleepHours ? `${socialHistory.sleepHours} hours` : 'Not provided'}</p>
                )}
              </div>
            </div>
          </div>

          {/* Occupation & Living */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
                <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                </svg>
              </div>
              <div>
                <h2 className="text-xl font-bold text-gray-900">Occupation & Living Situation</h2>
                <p className="text-sm text-gray-600">Work and home environment</p>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Occupation</label>
                {isEditing ? (
                  <input
                    type="text"
                    value={socialHistory.occupation || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, occupation: e.target.value})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                    placeholder="Software Engineer"
                  />
                ) : (
                  <p className="text-gray-900">{socialHistory.occupation || 'Not provided'}</p>
                )}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Living Situation</label>
                {isEditing ? (
                  <select
                    value={socialHistory.livingSituation || ''}
                    onChange={(e) => setSocialHistory({...socialHistory, livingSituation: e.target.value})}
                    className="w-full border border-gray-300 rounded-lg p-2"
                  >
                    <option value="">Select...</option>
                    <option value="Alone">Lives Alone</option>
                    <option value="Family">Lives with Family</option>
                    <option value="Partner">Lives with Partner/Spouse</option>
                    <option value="Roommates">Lives with Roommates</option>
                    <option value="Assisted">Assisted Living</option>
                    <option value="Nursing">Nursing Home</option>
                    <option value="Other">Other</option>
                  </select>
                ) : (
                  <p className="text-gray-900">{socialHistory.livingSituation || 'Not provided'}</p>
                )}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

