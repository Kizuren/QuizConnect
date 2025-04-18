/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        background: '#1a1a1a',
        card: '#2d2d2d',
        primary: '#007bff',
        success: '#28a745',
        error: '#dc3545',
      },
    },
  },
  plugins: [],
};