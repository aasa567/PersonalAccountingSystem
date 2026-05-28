import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import HomeView from '../views/HomeView.vue'
import RegisterView from '../views/RegisterView.vue'
import AdminView from '../views/AdminView.vue'

const routes = [
  { path: '/login', component: LoginView },
  { path: '/register', component: RegisterView },
  { path: '/', component: HomeView },
  { path: '/Admin', component: AdminView }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// --- 這裡開始是保全邏輯 ---
router.beforeEach((to, from, next) => {
  // 檢查瀏覽器有沒有我們在 LoginView 存下的登入標記
  const isAuthenticated = localStorage.getItem('isLogin') === 'true'

  // 如果要去首頁但沒登入 -> 踢回登入
  if (to.path === '/' && !isAuthenticated) {
    next('/login')
  } 
  // 如果已經登入卻想去登入/註冊頁 -> 送回首頁
  else if ((to.path === '/login' || to.path === '/register') && isAuthenticated) {
    next('/')
  }
  else {
    next()
  }
})
// --- 這裡結束 ---

export default router