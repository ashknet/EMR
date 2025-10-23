# EMRR Platform - Setup Instructions

## Azure AD Configuration Complete ✅

Your Microsoft Entra application has been configured with the following credentials:

- **Application ID**: `[REDACTED - Use environment variable VITE_AZURE_AD_CLIENT_ID]`
- **Tenant ID**: `[REDACTED - Use environment variable VITE_AZURE_AD_TENANT_ID]`
- **Client Secret**: `[REDACTED - Use environment variable VITE_AZURE_AD_CLIENT_SECRET]`

## Environment Variables Setup

Create a `.env` file in the PatientPortal directory with the following content:

```env
# Azure AD Configuration
VITE_AZURE_AD_CLIENT_ID=your-azure-client-id
VITE_AZURE_AD_TENANT_ID=your-azure-tenant-id
VITE_AZURE_AD_REDIRECT_URI=http://localhost:5173
VITE_AZURE_AD_CLIENT_SECRET=your-client-secret-here

# Google OAuth Configuration (Optional - Add when you have Google Client ID)
VITE_GOOGLE_CLIENT_ID=your-google-client-id
VITE_GOOGLE_REDIRECT_URI=http://localhost:5173

# API Configuration
VITE_API_BASE_URL=http://localhost:5000/api
```

## Azure AD App Registration Setup

To ensure your Azure AD application works correctly, verify these settings in the Azure Portal:

### 1. Authentication Settings
- **Redirect URIs**: Add `http://localhost:5173` for development
- **Logout URL**: Add `http://localhost:5173` (optional)
- **Supported account types**: Choose based on your needs (Single tenant, Multi-tenant, etc.)

### 2. API Permissions
Ensure your app has the following Microsoft Graph permissions:
- `User.Read` (Delegated)
- `openid` (Delegated)
- `profile` (Delegated)
- `email` (Delegated)

### 3. Token Configuration (Optional)
- **ID tokens**: Enable if you need user profile information
- **Access tokens**: Enable for API access

## Running the Application

1. **Install Dependencies** (if not already done):
   ```bash
   npm install
   ```

2. **Start Development Server**:
   ```bash
   npm run dev
   ```

3. **Access the Application**:
   - Open your browser to `http://localhost:5173`
   - You should see the anonymous landing page
   - Click "Get Started" to test the authentication modal

## Testing Authentication

1. **Microsoft Login**: Click "Continue with Microsoft" in the auth modal
2. **User Type Selection**: Choose between Patient or Hospital registration
3. **OAuth Flow**: You'll be redirected to Microsoft login
4. **Return to App**: After successful authentication, you'll return to the dashboard

## Production Deployment

For production deployment, update the following:

1. **Redirect URIs**: Change to your production domain
2. **Environment Variables**: Update with production URLs
3. **HTTPS**: Ensure your production site uses HTTPS
4. **Client Secret**: Move to server-side handling (never expose in frontend)

## Troubleshooting

### Common Issues:

1. **"AADSTS50011: The reply URL specified in the request does not match"**
   - Solution: Ensure the redirect URI in Azure AD matches your application URL

2. **"AADSTS65001: The user or administrator has not consented"**
   - Solution: Grant admin consent for the required permissions in Azure AD

3. **"Invalid client"**
   - Solution: Verify the Application ID is correct in your configuration

### Development Mode
The application includes a development mode that bypasses authentication when no Azure AD credentials are configured. This allows you to test the UI without setting up authentication.

## Security Notes

⚠️ **Important Security Considerations**:

1. **Client Secret**: Never expose the client secret in frontend code. In production, handle authentication server-side.

2. **Redirect URIs**: Only add trusted domains to your Azure AD app registration.

3. **HTTPS**: Always use HTTPS in production for secure OAuth flows.

4. **Token Storage**: The app uses sessionStorage for token storage, which is appropriate for SPAs.

## Next Steps

1. Test the authentication flow with your Azure AD credentials
2. Set up Google OAuth if needed (optional)
3. Customize the landing page content for your specific use case
4. Configure your backend API endpoints
5. Deploy to production with proper security measures

Your EMRR Platform is now ready with Microsoft Entra authentication! 🎉
