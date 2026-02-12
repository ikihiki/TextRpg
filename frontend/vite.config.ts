import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import process from 'node:process'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server:{
    port: Number.parseInt( process.env.PORT)
  }
})
