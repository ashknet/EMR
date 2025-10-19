/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#E6F0FF',
          100: '#CCE1FF',
          200: '#99C3FF',
          300: '#66A5FF',
          400: '#3387FF',
          500: '#0A4FD5',
          600: '#083FAD',
          700: '#062F85',
          800: '#04205C',
          900: '#021034',
        },
        accent: {
          50: '#E6F7F9',
          100: '#CCEFF4',
          200: '#99DFE9',
          300: '#66CFDD',
          400: '#33BFD2',
          500: '#0891B2',
          600: '#06748E',
          700: '#05576B',
          800: '#033A47',
          900: '#021D24',
        },
        success: {
          50: '#E6F7F1',
          100: '#CCEFE3',
          200: '#99DFC7',
          300: '#66CFAB',
          400: '#33BF8F',
          500: '#059669',
          600: '#047857',
          700: '#035A41',
          800: '#023C2B',
          900: '#011E16',
        },
        neutral: {
          50: '#F9FAFB',
          100: '#F3F4F6',
          200: '#E5E7EB',
          300: '#D1D5DB',
          400: '#9CA3AF',
          500: '#6B7280',
          600: '#4B5563',
          700: '#374151',
          800: '#1F2937',
          900: '#111827',
        }
      },
      boxShadow: {
        'soft': '0 2px 15px -3px rgba(10, 79, 213, 0.1), 0 10px 20px -2px rgba(10, 79, 213, 0.05)',
        'card': '0 4px 6px -1px rgba(0, 0, 0, 0.08), 0 2px 4px -1px rgba(0, 0, 0, 0.04)',
        'hover': '0 10px 25px -5px rgba(10, 79, 213, 0.15), 0 10px 10px -5px rgba(10, 79, 213, 0.04)',
      },
      animation: {
        'fade-in': 'fadeIn 0.5s ease-in-out',
        'slide-up': 'slideUp 0.4s ease-out',
        'pulse-slow': 'pulse 3s cubic-bezier(0.4, 0, 0.6, 1) infinite',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        slideUp: {
          '0%': { transform: 'translateY(10px)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
      },
    },
  },
  plugins: [],
}

