#!/usr/bin/env node

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const envContent = `# Azure AD Configuration
VITE_AZURE_AD_CLIENT_ID=your-azure-client-id
VITE_AZURE_AD_TENANT_ID=your-azure-tenant-id
VITE_AZURE_AD_REDIRECT_URI=http://localhost:5173

# Google OAuth Configuration (Optional - Add when you have Google Client ID)
VITE_GOOGLE_CLIENT_ID=your-google-client-id
VITE_GOOGLE_REDIRECT_URI=http://localhost:5173

# API Configuration
VITE_API_BASE_URL=https://localhost:58069/api
`;

const envPath = path.join(__dirname, '.env');

try {
  if (fs.existsSync(envPath)) {
    console.log('⚠️  .env file already exists. Backing up to .env.backup');
    fs.copyFileSync(envPath, path.join(__dirname, '.env.backup'));
  }
  
  fs.writeFileSync(envPath, envContent);
  console.log('✅ .env file created successfully with your Azure AD credentials!');
  console.log('🚀 You can now run: npm run dev');
  console.log('🌐 Open http://localhost:5173 to see your application');
} catch (error) {
  console.error('❌ Error creating .env file:', error.message);
  console.log('📝 Please manually create a .env file with the following content:');
  console.log('\n' + envContent);
}
