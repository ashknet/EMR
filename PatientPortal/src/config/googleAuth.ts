// Google OAuth Configuration
export const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID || '';

// Google OAuth scopes
export const GOOGLE_SCOPES = [
  'openid',
  'profile',
  'email'
];

// Google OAuth configuration
export const googleAuthConfig = {
  clientId: GOOGLE_CLIENT_ID,
  scope: GOOGLE_SCOPES.join(' '),
  redirectUri: import.meta.env.VITE_GOOGLE_REDIRECT_URI || window.location.origin,
  responseType: 'code',
  accessType: 'offline',
  prompt: 'consent'
};

// Google OAuth URL generator
export const getGoogleAuthUrl = () => {
  const params = new URLSearchParams({
    client_id: googleAuthConfig.clientId,
    redirect_uri: googleAuthConfig.redirectUri,
    response_type: googleAuthConfig.responseType,
    scope: googleAuthConfig.scope,
    access_type: googleAuthConfig.accessType,
    prompt: googleAuthConfig.prompt,
    state: Math.random().toString(36).substring(7) // Random state for security
  });

  return `https://accounts.google.com/o/oauth2/v2/auth?${params.toString()}`;
};

// Handle Google OAuth callback
export const handleGoogleCallback = (code: string) => {
  // This would typically make a request to your backend
  // to exchange the authorization code for tokens
  console.log('Google OAuth code received:', code);
  
  // For now, we'll simulate a successful login
  return Promise.resolve({
    success: true,
    user: {
      id: 'google_user_' + Math.random().toString(36).substring(7),
      email: 'user@example.com',
      name: 'Google User',
      provider: 'google'
    }
  });
};
