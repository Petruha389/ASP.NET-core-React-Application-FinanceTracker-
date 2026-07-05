import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'


export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'), // алиас для импортов
    },
  },
  server: {
    host: '127.0.0.1',
    port: 5173, // порт по умолчанию, можно изменить
    proxy: {
      '/api': {
        target: 'https://localhost:7249', // URL вашего бэкенда
        changeOrigin: true,
        secure: false, // если используете самоподписанный сертификат
      },
      '/auth': {
        target: 'https://localhost:7249',
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
