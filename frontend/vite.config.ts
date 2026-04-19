import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: true,
    allowedHosts: ['.coder.dev.sakuraya.cloud'],
    proxy: {
      '/api': {
        target: process.env.BFF_PROXY_TARGET ?? process.env.VITE_BFF_BASE_URL ?? 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
