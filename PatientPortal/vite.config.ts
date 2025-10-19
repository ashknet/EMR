import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [
    react({
      jsxImportSource: 'react',
      babel: {
        plugins: [
          // React 19 optimizations
        ],
      },
    })
  ],
  server: {
    port: 5174,
    host: true
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    target: 'es2022'
  },
  css: {
    postcss: './postcss.config.js',
  },
  optimizeDeps: {
    include: ['react', 'react-dom', 'react-router-dom']
  }
});

