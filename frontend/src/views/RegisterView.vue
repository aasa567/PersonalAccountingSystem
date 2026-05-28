<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import Swal from 'sweetalert2'
import chiikawaImg from '../assets/chiikawa.jpeg'

const username = ref('')
const password = ref('')
const confirmPassword = ref('') // 註冊通常會多一個確認密碼
const router = useRouter()

const API_URL = 'https://localhost:7283/api/Auth/register' // 記得檢查 Port 號碼

const handleRegister = async () => {
  // 基礎前端驗證
  if (password.value !== confirmPassword.value) {
    Swal.fire({
      title: '哎呀！',
      text: '兩次密碼輸入不一致喔！(๑•́ ₃ •̀๑)',
      icon: 'error',
      confirmButtonColor: '#ffdb4d' // 使用你登入按鈕的黃色
    })
    return
  }

  try {
    const response = await axios.post(API_URL, {
      username: username.value,
      password: password.value
    })

    if (response.status === 200) {
      // 成功的彈窗
      await Swal.fire({
        title: '註冊成功！',
        text: '快去登入跟吉伊卡哇一起記帳吧！(๑´ㅂ`๑)',
        icon: 'success',
        confirmButtonColor: '#ffdb4d',
        timer: 2000 // 兩秒後自動關閉
      })
      router.push('/login')
    }
  } catch (error) {
    const errorMsg = error.response?.data || '註冊失敗，請換個名稱試試'
    Swal.fire({
      title: '出錯了！',
      text: errorMsg,
      icon: 'warning',
      confirmButtonColor: '#ffdb4d'
    })
  }
}
</script>

<template>
  <div class="login-container">
    <div class="login-card">
      <img :src="chiikawaImg" alt="Chiikawa" class="login-img" />
      <h2>新夥伴註冊 🐣</h2>
      <div class="input-group">
        <input v-model="username" type="text" placeholder="設定使用者名稱" />
        <input v-model="password" type="password" placeholder="設定密碼" />
        <input v-model="confirmPassword" type="password" placeholder="再次確認密碼" @keyup.enter="handleRegister" />
      </div>
      <button @click="handleRegister" class="login-btn">註冊帳號</button>
      <p class="signup-link" @click="router.push('/login')">已經有帳號了？點此回登入</p>
    </div>
  </div>
</template>

<style scoped>
/* 直接複用你 LoginView 的美化樣式 */
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
</style>