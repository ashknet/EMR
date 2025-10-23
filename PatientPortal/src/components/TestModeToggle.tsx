import { useState, useEffect } from 'react';
import { ENABLE_HARDCODED_USER } from '../config/authConfig';

export default function TestModeToggle() {
  const [isTestMode, setIsTestMode] = useState(ENABLE_HARDCODED_USER);

  const toggleTestMode = () => {
    const newMode = !isTestMode;
    setIsTestMode(newMode);
    // Store in localStorage for persistence
    localStorage.setItem('testMode', newMode.toString());
    // Reload page to apply changes
    window.location.reload();
  };

  useEffect(() => {
    // Check localStorage for saved test mode preference
    const savedTestMode = localStorage.getItem('testMode');
    if (savedTestMode !== null) {
      setIsTestMode(savedTestMode === 'true');
    }
  }, []);

  if (!isTestMode && !ENABLE_HARDCODED_USER) {
    return null; // Don't show toggle if test mode is disabled
  }

  return (
    <div className="fixed bottom-4 right-4 z-50">
      <button
        onClick={toggleTestMode}
        className={`px-4 py-2 rounded-lg text-sm font-semibold shadow-lg transition-all transform hover:scale-105 ${
          isTestMode
            ? 'bg-green-500 hover:bg-green-600 text-white'
            : 'bg-slate-500 hover:bg-slate-600 text-white'
        }`}
        title={isTestMode ? 'Disable Test Mode' : 'Enable Test Mode'}
      >
        {isTestMode ? '🧪 Test Mode ON' : '🔒 Test Mode OFF'}
      </button>
    </div>
  );
}
