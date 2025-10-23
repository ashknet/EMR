import { useState } from 'react';
import { ENABLE_HARDCODED_USER, HARDCODED_USER } from '../config/authConfig';
import { MOCK_PATIENT_ID } from '../config/mockUser';
import { API_BASE_URL } from '../config/apiConfig';

export default function DebugInfo() {
  const [isExpanded, setIsExpanded] = useState(false);

  if (!ENABLE_HARDCODED_USER) {
    return null;
  }

  return (
    <div className="fixed bottom-4 left-4 z-50">
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className="bg-slate-800 text-white px-3 py-2 rounded-lg text-xs font-mono hover:bg-slate-700 transition-colors"
      >
        🔧 Debug
      </button>
      
      {isExpanded && (
        <div className="absolute bottom-12 left-0 bg-slate-900 text-white p-4 rounded-lg text-xs font-mono max-w-md">
          <div className="space-y-2">
            <div><strong>API URL:</strong> {API_BASE_URL}</div>
            <div><strong>Patient ID:</strong> {HARDCODED_USER.id}</div>
            <div><strong>Mock Patient ID:</strong> {MOCK_PATIENT_ID}</div>
            <div><strong>Expected Endpoint:</strong> {API_BASE_URL}/patients/{HARDCODED_USER.id}</div>
            <div><strong>Test Mode:</strong> {ENABLE_HARDCODED_USER ? 'ON' : 'OFF'}</div>
            <div className="pt-2 border-t border-slate-700">
              <div className="text-yellow-400">⚠️ Make sure your API server is running on https://localhost:58069</div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
