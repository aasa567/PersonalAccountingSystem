<template>
  <div class="admin-container">
    <div class="admin-card">
      <h2><span class="icon">🔐</span> 管理後台</h2>
      
      <div class="table-responsive">
        <table class="user-table">
          <thead>
            <tr>
              <th>使用者名稱</th>
              <th>安全操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in userList" :key="user.id">
              <td class="username-td">{{ user.username }}</td>
              <td>
                <button @click="resetPwd(user.username)" class="reset-btn">重設密碼</button>
                <button @click="removeUser(user.id)" class="delete-btn">刪除帳號</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <button @click="router.push('/')" class="back-btn">回記帳本首頁</button>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import Swal from 'sweetalert2'
import { useRouter } from 'vue-router'

const userList = ref([])
const router = useRouter()

const fetchUsers = async () => {
  try {
    // 換成你後端實際執行的 Port 號
    const res = await axios.get('https://localhost:7283/api/Auth/all-users') 
    userList.value = res.data // 把後端回傳的陣列給 userList
  } catch (error) {
    console.error('撈取使用者失敗：', error)
    Swal.fire('可惡！', '無法取得使用者清單(ಠ_ಠ)', 'error')
  }
}

const resetPwd = async (username) => {
  const result = await Swal.fire({
    title: `確定要重設 ${username} 的密碼嗎？`,
    text: "重設後密碼將變更為 0000",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#ffdb4d'
  })

  if (result.isConfirmed) {
    await axios.post('https://localhost:7283/api/Auth/reset-password', { username })
    Swal.fire('成功！', '密碼已更新', 'success')
  }
}

onMounted(fetchUsers)
</script>

<style scoped>
/* 背景滿版與置中 */
.admin-container {
  min-height: 100vh;
  background-color: #fcf6f0; /* 溫柔的淡淡粉膚色背景 */
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding: 40px 20px;
  font-family: 'PingFang TC', 'Microsoft JhengHei', sans-serif;
}

/* 主要卡片容器 */
.admin-card {
  background: #ffffff;
  padding: 30px;
  border-radius: 16px;
  box-shadow: 0 8px 24px rgba(161, 136, 127, 0.15); /* 柔和陰影 */
  width: 100%;
  max-width: 600px;
  border: 2px solid #fff3cd;
}

h2 {
  color: #5d4037;
  font-size: 24px;
  margin-bottom: 25px;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

/* 表格本體設計 */
.table-responsive {
  margin-bottom: 25px;
  border-radius: 12px;
  overflow: hidden; /* 讓表格圓角生效 */
  border: 1px solid #ffeeba;
}

.user-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.user-table th {
  background-color: #fff3cd; /* 吉伊卡哇招牌黃色 */
  color: #856404;
  padding: 14px 16px;
  font-weight: 600;
  font-size: 15px;
}

.user-table td {
  padding: 14px 16px;
  border-bottom: 1px solid #fdf5e6;
  color: #4e342e;
  vertical-align: middle;
}

.user-table tbody tr:hover {
  background-color: #fffdf5; /* 滑鼠游標滑過時微微變黃 */
}

.username-td {
  font-weight: bold;
  font-size: 16px;
}

/* 按鈕樣式群組 */
button {
  border: none;
  border-radius: 8px;
  padding: 8px 14px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.reset-btn {
  background-color: #ffdb4d;
  color: #5d4037;
  margin-right: 8px;
  box-shadow: 0 2px 4px rgba(255, 219, 77, 0.3);
}

.reset-btn:hover {
  background-color: #ffd000;
  transform: translateY(-1px);
}

.delete-btn {
  background-color: #fff0f0;
  color: #d9534f;
  border: 1px solid #f5c6cb;
}

.delete-btn:hover {
  background-color: #f8d7da;
  color: #a71d2a;
}

.back-btn {
  display: block;
  width: 100%;
  background-color: #a1887f;
  color: white;
  padding: 12px;
  font-size: 16px;
  margin-top: 10px;
}

.back-btn:hover {
  background-color: #8d6e63;
}
</style>