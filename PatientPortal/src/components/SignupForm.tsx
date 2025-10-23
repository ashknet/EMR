import { useState } from 'react';
import { UserType } from '../config/authConfig';

interface SignupFormProps {
  userType: UserType;
  onSuccess: () => void;
  onBack: () => void;
}

export default function SignupForm({ userType, onSuccess, onBack }: SignupFormProps) {
  const [step, setStep] = useState<'details' | 'otp' | 'success'>('details');
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    password: '',
    confirmPassword: '',
    agreeToTerms: false
  });
  const [otp, setOtp] = useState('');
  const [otpSent, setOtpSent] = useState(false);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const validateForm = () => {
    if (!formData.firstName || !formData.lastName || !formData.email || !formData.password) {
      alert('Please fill in all required fields');
      return false;
    }
    if (formData.password !== formData.confirmPassword) {
      alert('Passwords do not match');
      return false;
    }
    if (formData.password.length < 8) {
      alert('Password must be at least 8 characters long');
      return false;
    }
    if (!formData.agreeToTerms) {
      alert('Please agree to the terms and conditions');
      return false;
    }
    return true;
  };

  const handleSendOTP = async () => {
    if (!validateForm()) return;

    setIsLoading(true);
    try {
      // Simulate OTP sending
      await new Promise(resolve => setTimeout(resolve, 2000));
      setOtpSent(true);
      setStep('otp');
      alert('OTP sent to your email and phone number');
    } catch (error) {
      console.error('Error sending OTP:', error);
      alert('Failed to send OTP. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleVerifyOTP = async () => {
    if (!otp || otp.length !== 6) {
      alert('Please enter a valid 6-digit OTP');
      return;
    }

    setIsLoading(true);
    try {
      // Simulate OTP verification
      await new Promise(resolve => setTimeout(resolve, 2000));
      setStep('success');
      setTimeout(() => {
        onSuccess();
      }, 2000);
    } catch (error) {
      console.error('Error verifying OTP:', error);
      alert('Invalid OTP. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendOTP = async () => {
    setIsLoading(true);
    try {
      await new Promise(resolve => setTimeout(resolve, 1000));
      alert('OTP resent successfully');
    } catch (error) {
      console.error('Error resending OTP:', error);
      alert('Failed to resend OTP. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  if (step === 'success') {
    return (
      <div className="text-center py-8">
        <div className="text-6xl mb-4">✅</div>
        <h3 className="text-2xl font-bold text-slate-900 mb-4">Account Created Successfully!</h3>
        <p className="text-slate-600 mb-6">
          Welcome to EMRR Platform! Your {userType} account has been created.
        </p>
        <div className="flex items-center justify-center gap-2 text-sm text-slate-500">
          <div className="w-4 h-4 border-2 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
          <span>Redirecting to your dashboard...</span>
        </div>
      </div>
    );
  }

  if (step === 'otp') {
    return (
      <div className="space-y-6">
        <div className="text-center">
          <div className="text-4xl mb-4">📱</div>
          <h3 className="text-xl font-bold text-slate-900 mb-2">Verify Your Account</h3>
          <p className="text-slate-600">
            We've sent a 6-digit verification code to:
          </p>
          <p className="font-semibold text-slate-900 mt-2">
            {formData.email} • {formData.phone}
          </p>
        </div>

        <div>
          <label htmlFor="otp" className="block text-sm font-semibold text-slate-700 mb-2">
            Enter Verification Code
          </label>
          <input
            type="text"
            id="otp"
            value={otp}
            onChange={(e) => setOtp(e.target.value.replace(/\D/g, '').slice(0, 6))}
            placeholder="000000"
            className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all text-center text-2xl font-mono tracking-widest"
            maxLength={6}
          />
        </div>

        <div className="flex gap-3">
          <button
            onClick={handleVerifyOTP}
            disabled={isLoading || otp.length !== 6}
            className="flex-1 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 disabled:from-slate-400 disabled:to-slate-500 text-white px-6 py-3 rounded-xl font-semibold transition-all disabled:cursor-not-allowed"
          >
            {isLoading ? (
              <div className="flex items-center justify-center gap-2">
                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                Verifying...
              </div>
            ) : (
              'Verify & Create Account'
            )}
          </button>
        </div>

        <div className="text-center">
          <p className="text-sm text-slate-600 mb-2">Didn't receive the code?</p>
          <button
            onClick={handleResendOTP}
            disabled={isLoading}
            className="text-blue-600 hover:text-blue-700 font-semibold text-sm"
          >
            Resend OTP
          </button>
        </div>

        <button
          onClick={() => setStep('details')}
          className="w-full text-slate-600 hover:text-slate-700 font-medium text-sm"
        >
          ← Back to form
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="text-center">
        <div className="text-4xl mb-4">{userType === 'patient' ? '👤' : '🏥'}</div>
        <h3 className="text-xl font-bold text-slate-900 mb-2">
          Create {userType === 'patient' ? 'Patient' : 'Hospital'} Account
        </h3>
        <p className="text-slate-600">
          Fill in your details to get started
        </p>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label htmlFor="firstName" className="block text-sm font-semibold text-slate-700 mb-2">
            First Name *
          </label>
          <input
            type="text"
            id="firstName"
            name="firstName"
            value={formData.firstName}
            onChange={handleInputChange}
            required
            className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
            placeholder="Enter first name"
          />
        </div>
        <div>
          <label htmlFor="lastName" className="block text-sm font-semibold text-slate-700 mb-2">
            Last Name *
          </label>
          <input
            type="text"
            id="lastName"
            name="lastName"
            value={formData.lastName}
            onChange={handleInputChange}
            required
            className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
            placeholder="Enter last name"
          />
        </div>
      </div>

      <div>
        <label htmlFor="email" className="block text-sm font-semibold text-slate-700 mb-2">
          Email Address *
        </label>
        <input
          type="email"
          id="email"
          name="email"
          value={formData.email}
          onChange={handleInputChange}
          required
          className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
          placeholder="Enter email address"
        />
      </div>

      <div>
        <label htmlFor="phone" className="block text-sm font-semibold text-slate-700 mb-2">
          Phone Number *
        </label>
        <input
          type="tel"
          id="phone"
          name="phone"
          value={formData.phone}
          onChange={handleInputChange}
          required
          className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
          placeholder="Enter phone number"
        />
      </div>

      <div>
        <label htmlFor="password" className="block text-sm font-semibold text-slate-700 mb-2">
          Password *
        </label>
        <input
          type="password"
          id="password"
          name="password"
          value={formData.password}
          onChange={handleInputChange}
          required
          className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
          placeholder="Create a strong password"
        />
      </div>

      <div>
        <label htmlFor="confirmPassword" className="block text-sm font-semibold text-slate-700 mb-2">
          Confirm Password *
        </label>
        <input
          type="password"
          id="confirmPassword"
          name="confirmPassword"
          value={formData.confirmPassword}
          onChange={handleInputChange}
          required
          className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
          placeholder="Confirm your password"
        />
      </div>

      <div className="flex items-start gap-3">
        <input
          type="checkbox"
          id="agreeToTerms"
          name="agreeToTerms"
          checked={formData.agreeToTerms}
          onChange={handleInputChange}
          className="mt-1 w-4 h-4 text-blue-600 border-slate-300 rounded focus:ring-blue-500"
        />
        <label htmlFor="agreeToTerms" className="text-sm text-slate-600">
          I agree to the{' '}
          <a href="#" className="text-blue-600 hover:text-blue-700 font-semibold">
            Terms of Service
          </a>{' '}
          and{' '}
          <a href="#" className="text-blue-600 hover:text-blue-700 font-semibold">
            Privacy Policy
          </a>
        </label>
      </div>

      <button
        onClick={handleSendOTP}
        disabled={isLoading}
        className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 disabled:from-slate-400 disabled:to-slate-500 text-white px-6 py-3 rounded-xl font-semibold transition-all disabled:cursor-not-allowed"
      >
        {isLoading ? (
          <div className="flex items-center justify-center gap-2">
            <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
            Sending OTP...
          </div>
        ) : (
          'Send Verification Code'
        )}
      </button>

      <button
        onClick={onBack}
        className="w-full text-slate-600 hover:text-slate-700 font-medium text-sm"
      >
        ← Back to signup options
      </button>
    </div>
  );
}
