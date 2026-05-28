import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// 這份說明書會告訴 Vite：看到 .vue 檔案時，請用 vue 外掛處理
export default defineConfig({
  plugins: [vue()]
})