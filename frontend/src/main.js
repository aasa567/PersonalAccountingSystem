import { createApp } from 'vue'
import App from './App.vue'
import router from './router' //引入路由
import axios from 'axios'

//新增 Axios 請求攔截器
axios.interceptors.request.use(
  (config) => {
    // 從瀏覽器祕密基地拔出 Token
    const token = localStorage.getItem('userToken')
    
    // 如果有 Token，就自動在 Headers 戴上「Bearer 通行證手環」
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// 這一行是關鍵，它會告訴 Vite：請把 App.vue 的內容渲染到 index.html 的 #app 裡面
createApp(App).use(router).mount('#app')