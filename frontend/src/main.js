import { createApp } from 'vue'
import App from './App.vue'
import router from './router' //引入路由

// 這一行是關鍵，它會告訴 Vite：請把 App.vue 的內容渲染到 index.html 的 #app 裡面
createApp(App).use(router).mount('#app')