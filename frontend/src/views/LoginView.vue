<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import Swal from 'sweetalert2' // 1. 引入 SweetAlert2
import chiikawaImg from '../assets/chiikawa.jpeg' // ✨ 加入這行，@ 代表 src 資料夾

const username = ref('')
const password = ref('')
const router = useRouter()

// 設定 SweetAlert2 的 Toast樣式 (右上角彈出)
const Toast = Swal.mixin({
  toast: true,
  position: 'top-end',
  showConfirmButton: false,
  timer: 3000,
  timerProgressBar: true,
  didOpen: (toast) => {
    toast.addEventListener('mouseenter', Swal.stopTimer)
    toast.addEventListener('mouseleave', Swal.resumeTimer)
  }
})

const handleForgot = () => {
  Swal.fire({
    title: '別擔心！哈哇！',
    text: '目前尚未開啟 Email 找回功能，請聯繫管理員「小八貓」為您重設密碼喔！',
    icon: 'info',
    // ✨ 將原本的網址換成你 import 的變數 chiikawaImg
    imageUrl: chiikawaImg, 
    imageWidth: 150,
    imageHeight: 150,
    imageAlt: '可愛的吉伊卡哇',
    confirmButtonColor: '#ffdb4d',
    confirmButtonText: '我知道了！'
  })
}

const handleLogin = async () => {
  // 前端初步檢查
  if (!username.value.trim() || !password.value.trim()) {
    Swal.fire({
      title: '哎呀！',
      text: '請輸入帳號密碼喔！(๑•́ ₃ •̀๑)',
      icon: 'warning',
      confirmButtonColor: '#ffdb4d'
    })
    return
  }

  try {
    // 💡 記得檢查你的 API Port 號碼
    const response = await axios.post('https://localhost:7283/api/Auth/login', {
      username: username.value,
      password: password.value
    })

    if (response.status === 200) {
      // 🌟 【重點修改】從後端回傳的資料中解構出 token, username, role
      const { token, username, role } = response.data

      // 🌟 將資訊存入瀏覽器的 localStorage
      localStorage.setItem('isLogin', 'true')
      localStorage.setItem('userToken', token)       // 儲存 JWT 通行證
      localStorage.setItem('userRole', role)         // 儲存角色權限 (Admin / User)
      localStorage.setItem('userName', username)     // 儲存使用者名稱

      // 2. 使用漂亮的 Toast 顯示成功
      await Toast.fire({
        icon: 'success',
        title: `哈哇！歡迎回來 ${username}！`
      })

      // 🌟 【重點修改】根據角色決定要把使用者送去哪裡
      if (role === 'Admin') {
        router.push('/Admin') // 管理員直接導向後台
      } else {
        router.push('/')      // 一般用戶導向首頁
      }
    }
  } catch (error) {
    // 3. 登入失敗的彈窗
    const errorMsg = error.response?.data || '帳號或密碼錯誤喔！'
    Swal.fire({
      title: '登入失敗',
      text: errorMsg,
      icon: 'error',
      confirmButtonColor: '#ffdb4d'
    })
  }
}
</script>

<template>
  <div class="login-container">
    <div class="login-card">
      <img :src="chiikawaImg" alt="Chiikawa" class="login-img" />
      <h2>哈哇！歡迎回來 🐹</h2>
      <div class="input-group">
        <input v-model="username" type="text" placeholder="使用者名稱" />
        <input v-model="password" type="password" placeholder="密碼" @keyup.enter="handleLogin" />
      </div>
      <button @click="handleLogin" class="login-btn">登入</button>

      <p class="forgot-link" @click="handleForgot">忘記密碼了嗎？ (๑•́ ₃ •̀๑)</p>
      
      <p class="signup-link" @click="router.push('/register')">還沒有帳號？按這裡註冊</p>
    </div>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #fff0f3;
}
.login-card {
  background: white;
  padding: 40px;
  border-radius: 30px;
  text-align: center;
  box-shadow: 0 10px 25px rgba(255, 182, 193, 0.3);
  width: 320px;
}
.login-img {
  width: 120px;
  margin-bottom: 20px;
}
.input-group input {
  width: 100%;
  padding: 12px;
  margin-bottom: 15px;
  border: 2px solid #ffecf0;
  border-radius: 12px;
  box-sizing: border-box;
}
.login-btn {
  width: 100%;
  padding: 12px;
  background-color: #ffdb4d;
  border: none;
  border-radius: 12px;
  color: #5d4037;
  font-weight: bold;
  cursor: pointer;
  transition: transform 0.2s;
}
.login-btn:hover {
  transform: scale(1.05);
}
h2 { color: #5d5d5d; margin-bottom: 20px; }
.signup-link {
  margin-top: 20px;
  font-size: 14px;
  color: #a1887f;
  cursor: pointer;
}
.forgot-link {
  margin-top: 15px;
  font-size: 13px;
  color: #a1887f;
  cursor: pointer;
  text-decoration: underline;
  transition: color 0.3s;
}

.forgot-link:hover {
  color: #ffb6c1; /* 碰到的時候變成粉紅色 */
}
</style>