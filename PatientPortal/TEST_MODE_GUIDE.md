# EMRR Platform - Test Mode Guide

## 🧪 Test Mode Overview

Test Mode allows you to bypass authentication and access all dashboard pages with a hardcoded user for testing purposes. This is perfect for development, demos, and testing the full application functionality.

## 🚀 How to Enable Test Mode

### Method 1: Configuration File (Default)
Test mode is enabled by default in the configuration:
```typescript
// In src/config/authConfig.ts
export const ENABLE_HARDCODED_USER = true;
```

### Method 2: Toggle Button (Runtime)
- Look for the **"🧪 Test Mode ON/OFF"** button in the bottom-right corner of any page
- Click to toggle between test mode and normal authentication
- The setting persists in localStorage

## 👤 Hardcoded User Details

When test mode is enabled, you'll be logged in as:

```typescript
{
  id: 'test-user-123',
  email: 'test@emrr-platform.com',
  name: 'Test User',
  firstName: 'John',
  lastName: 'Doe',
  phone: '+1 (555) 123-4567',
  type: 'patient',
  provider: 'hardcoded',
  isAuthenticated: true
}
```

## 🎯 Quick Access Methods

### 1. Landing Page
- **"🧪 Test Dashboard"** button in the hero section
- **"🧪 Test Mode"** link in the navigation bar

### 2. Direct URL Access
You can directly access any protected page:
- Dashboard: `http://localhost:5173/dashboard`
- Profile: `http://localhost:5173/profile`
- Medical History: `http://localhost:5173/medical-history`
- Insurance: `http://localhost:5173/insurance`
- Providers: `http://localhost:5173/providers`
- Family: `http://localhost:5173/family`
- Social History: `http://localhost:5173/social-history`
- Consent: `http://localhost:5173/consent`
- Documents: `http://localhost:5173/documents`
- Transfers: `http://localhost:5173/transfers`
- Admin Metadata: `http://localhost:5173/admin/metadata`

### 3. Navigation
Once in test mode, use the normal navigation menu to access all pages.

## 🔍 Visual Indicators

### Test Mode Indicators:
- **Header**: Shows "🧪 Test Mode" badge instead of "Sign Out" button
- **User Info**: Displays "John Doe" as the logged-in user
- **Toggle Button**: Green "🧪 Test Mode ON" button in bottom-right corner
- **Landing Page**: Green "🧪 Test Dashboard" button and "🧪 Test Mode" link

### Normal Mode Indicators:
- **Header**: Shows "Sign Out" button
- **User Info**: Shows actual authenticated user or "Loading..."
- **Toggle Button**: Gray "🔒 Test Mode OFF" button in bottom-right corner

## 🛠️ Testing Scenarios

### 1. Full Application Flow
1. Start at landing page (`/`)
2. Click "🧪 Test Dashboard" to access dashboard
3. Navigate through all pages using the menu
4. Test all functionality without authentication

### 2. Authentication Testing
1. Toggle test mode OFF
2. Try accessing protected routes (should redirect to landing page)
3. Test Microsoft OAuth flow
4. Test direct signup form
5. Toggle test mode back ON for continued testing

### 3. User Experience Testing
1. Test responsive design on different screen sizes
2. Test navigation between pages
3. Test form interactions and validations
4. Test loading states and animations

## 🔧 Configuration Options

### Disable Test Mode
To disable test mode completely:
```typescript
// In src/config/authConfig.ts
export const ENABLE_HARDCODED_USER = false;
```

### Change Hardcoded User
To modify the test user details:
```typescript
// In src/config/authConfig.ts
export const HARDCODED_USER = {
  id: 'your-test-id',
  email: 'your-test@email.com',
  name: 'Your Test Name',
  firstName: 'Your',
  lastName: 'Name',
  phone: '+1 (555) 000-0000',
  type: 'patient' as UserType, // or 'hospital'
  provider: 'hardcoded' as 'microsoft' | 'google',
  isAuthenticated: true
};
```

## 🚨 Important Notes

### Security Considerations:
- **Never deploy with test mode enabled in production**
- Test mode bypasses all authentication and security checks
- The hardcoded user has full access to all features

### Development Best Practices:
- Use test mode for development and testing
- Test both authenticated and unauthenticated flows
- Verify that normal authentication still works when test mode is disabled

### Production Deployment:
- Set `ENABLE_HARDCODED_USER = false` before production deployment
- Remove or hide the test mode toggle in production builds
- Ensure proper authentication is working in production

## 🎉 Benefits of Test Mode

1. **Rapid Development**: Skip authentication setup during development
2. **Easy Testing**: Test all features without creating real accounts
3. **Demo Ready**: Perfect for demonstrations and presentations
4. **Debugging**: Isolate issues without authentication complications
5. **User Experience**: Test the full user journey seamlessly

## 🔄 Switching Between Modes

### From Test Mode to Normal Mode:
1. Click the "🧪 Test Mode ON" button
2. Page will reload with authentication required
3. Use normal login/signup flows

### From Normal Mode to Test Mode:
1. Click the "🔒 Test Mode OFF" button
2. Page will reload with test mode enabled
3. Access all pages without authentication

## 📱 Mobile Testing

Test mode works perfectly on mobile devices:
- Toggle button is accessible on mobile
- All pages are responsive
- Navigation works smoothly on touch devices

---

**Happy Testing! 🧪✨**

Use test mode to explore all the features of your EMRR Platform without any authentication barriers.
