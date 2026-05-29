import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import HomeView from '../views/HomeView.vue'
import RegisterView from '../views/RegisterView.vue'
import AdminView from '../views/AdminView.vue'

const routes = [
  { path: '/login', component: LoginView },
  { path: '/register', component: RegisterView },
  { path: '/', component: HomeView },
  // ✨ 可以在路由這裡加上 meta 屬性，標記這個頁面需要 Admin 權限，這樣未來的擴充性更好
  { path: '/Admin', component: AdminView, meta: { requiresAdmin: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// --- 升級後的進階保全邏輯 ---
router.beforeEach((to, from, next) => {
  // 1. 拿取登入狀態與「最重要的角色通行證」
  const isAuthenticated = localStorage.getItem('isLogin') === 'true'
  const userRole = localStorage.getItem('userRole') // 🌟 新增：拿取角色（Admin / User）

  // 🔒 關卡 A：如果要去需要「管理員權限」的頁面（例如 /Admin）
  if (to.meta.requiresAdmin) {
    if (isAuthenticated && userRole === 'Admin') {
      next() // 👑 既登入又是管理員，完美放行！
    } else {
      next('/') // ❌ 不是管理員，直接無情彈回首頁（或者彈到 /login）
    }
  }
  // 🔒 關卡 B：如果是去首頁但沒登入 -> 踢回登入
  else if (to.path === '/' && !isAuthenticated) {
    next('/login')
  } 
  // 🔒 關卡 C：如果已經登入卻想去登入/註冊頁 -> 送回首頁
  else if ((to.path === '/login' || to.path === '/register') && isAuthenticated) {
    next('/')
  }
  // 其他普通頁面，直接放行
  else {
    next()
  }
})
// --- 這裡結束 ---

export default router