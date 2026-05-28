<template>
  <div class="main-wrapper">
    <div class="container">
      <div class="header">

       <div style="text-align: right; margin-bottom: 10px;">
         <span style="color: #a1887f; margin-right: 10px;">哈哇！{{ currentUserName }} 🐹</span>
         <button @click="handleLogout" style="padding: 5px 12px; font-size: 12px; background: #ffb6c1; color: white;">登出</button>
       </div>

        <h1>我的記帳本 🐹</h1>
        <p class="subtitle">一起努力存錢買想要的東西吧！</p>
      </div>

      <div class="total-section">
        <h2>目前總支出：<span class="total-price">${{ totalAmount }}</span></h2>
        <p class="status-msg">{{ statusMessage }}</p>
      </div>
      
      <div class="add-box">
        <input v-model="newItem.title" placeholder="項目（如：草莓聖代）" style="flex: 2;"/>
        <input v-model.number="newItem.amount" type="number" placeholder="金額" style="flex: 1;"/>
        
        <select v-model="newItem.category" class="cat-select">
          <option value="食">🍔 食</option>
          <option value="衣">🛍️ 衣</option>
          <option value="住">🏠 住</option>
          <option value="行">🚃 行</option>
          <option value="其他">❓ 其他</option>
        </select>

        <button @click="addItem">新增</button>
      </div>

      <hr />

      <div v-if="loading" class="loading-text">讀取中...</div>
      <ul v-else>
        <li v-for="item in items" :key="item.id">
          <div class="item-left">
            <span class="cat-tag">{{ item.category }}</span>
            <span class="item-title">{{ item.title }}</span>
          </div>
          <div>
            <span class="price">${{ item.amount }}</span>
            <button class="delete-btn" @click="deleteItem(item.id)">❌</button>
          </div>
        </li>
      </ul>
      
      <div class="footer-decoration">
        <span class="sticker">フワワ...</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router' // 1. 引入 Router
import Swal from 'sweetalert2'        // 2. 引入 SweetAlert2 (如果你想讓登出變漂亮)

const items = ref([])
const loading = ref(true)
const newItem = ref({ title: '', amount: 0, category: '食' }) // 預設食
const router = useRouter() // 3. 初始化 Router
const currentUserName = ref('') // 4. 用來存當前使用者名稱
// 記得這裡數字要改成你 Visual Studio 啟動後的 Port 號碼喔！
const API_URL = 'https://localhost:7283/api/transactions'

// 5. 新增登出函式
const handleLogout = () => {
  localStorage.removeItem('isLogin')
  localStorage.removeItem('userName')
  
  Swal.fire({
    title: '已成功登出！',
    text: '要記得回來記帳喔 (๑´ㅂ`๑)',
    icon: 'success',
    timer: 1500,
    showConfirmButton: false
  })
  
  router.push('/login')
}

//1. 自動計算總金額
const totalAmount = computed(() => {
  return items.value.reduce((sum, item) => sum + item.amount, 0)
})

// 2. 根據金額決定吉伊卡哇的表情
const statusMessage = computed(() => {
  if (totalAmount.value === 0) return "帳本空空的... ( ・ω・)"
  if (totalAmount.value > 2000) return "哇哇...花太多了啦 ( ᐡ ɞ̴̶̷ ❌ ɞ̴̶̷ ᐡ )"
  return "目前還很省喔！撒花！( ˶ˆ꒳ˆ˵ )"
})

const fetchItems = async () => {
  try {
    const res = await axios.get(API_URL)
    // 讓最新的資料排在最上面
    items.value = res.data.reverse()
  } catch (err) {
    console.error("抓取失敗，請確認後端有開：", err)
  } finally {
    loading.value = false
  }
}

const addItem = async () => {
  if (!newItem.value.title || newItem.value.amount <= 0) return
  try {
    await axios.post(API_URL, {
      title: newItem.value.title,
      amount: newItem.value.amount,
      date: new Date().toISOString(),
      type: "Expense"
    })
    newItem.value = { title: '', amount: 0, category: '食' }
    fetchItems()
  } catch (err) {
    alert("新增失敗，請檢查後端 API")
  }
}

const deleteItem = async (id) => {
  // 換成 SweetAlert2 的確認視窗
  const result = await Swal.fire({
    title: '確定要刪除嗎？',
    text: "刪掉就找不回補囉！(๑•́ ₃ •̀๑)",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#ffdb4d',
    cancelButtonColor: '#ffb6c1',
    confirmButtonText: '刪除！',
    cancelButtonText: '先不要'
  })

  if (result.isConfirmed) {
    try {
      await axios.delete(`${API_URL}/${id}`)
      fetchItems()
      Swal.fire('已刪除！', '', 'success')
    } catch (err) {
      Swal.fire('錯誤', '刪除失敗', 'error')
    }
  }
}

onMounted(fetchItems)
</script>

<style>
/* 1. 整個網頁的背景 - 淡淡的蜜桃粉色 */
body {
  background-color: #fff0f3; 
  margin: 0;
  font-family: 'Microsoft JhengHei', sans-serif;
  display: flex;
  justify-content: center;
  min-height: 100vh;
}

.main-wrapper {
  width: 100%;
  padding: 50px 0;
}

/* 2. 記帳框 - 像雲朵一樣浮起來 */
.container {
  max-width: 450px;
  width: 90%;
  margin: 0 auto;
  padding: 30px;
  background: rgba(255, 255, 255, 0.9); /* 半透明白 */
  border-radius: 30px; /* 超圓角 */
  box-shadow: 0 15px 30px rgba(255, 182, 193, 0.2); /* 粉色柔和陰影 */
}

.header {
  text-align: center;
  margin-bottom: 30px;
}

h1 {
  color: #5d5d5d;
  font-size: 28px;
  margin-bottom: 5px;
}

.subtitle {
  color: #a1887f;
  font-size: 14px;
  margin: 0;
}

.add-box {
  display: flex;
  gap: 10px;
  margin-bottom: 30px;
}

input {
  padding: 12px;
  border: 2px solid #ffecf0;
  border-radius: 12px;
  outline: none;
  transition: all 0.3s;
  flex: 1;
}

/* 輸入框焦點 - Chiikawa 黃色 */
input:focus {
  border-color: #ffd900;
  background-color: #fffdf5;
}

/* 按鈕 - 吉伊卡哇黃色配色 */
button {
  padding: 10px 20px;
  background: #ffdb4d;
  color: #5d4037;
  font-weight: bold;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  transition: transform 0.2s, background 0.3s;
}

button:hover {
  transform: scale(1.05);
  background: #fbc02d;
}

.delete-btn {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 16px;
  padding: 5px;
  margin-left: 10px;
  transition: transform 0.2s;
}

.delete-btn:hover {
  background: none;
  transform: scale(1.3); /* 碰到的時候稍微變大 */
}

hr {
  border: none;
  border-top: 2px dashed #ffecf0; /* 點點分割線 */
  margin: 25px 0;
}

.loading-text {
  text-align: center;
  color: #a1887f;
  margin: 20px 0;
}

ul {
  padding: 0;
}

/* 每一項列表 - 小八貓藍色側邊 */
li {
  background: white;
  margin-bottom: 12px;
  padding: 18px;
  border-radius: 15px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  list-style: none;
  border-left: 6px solid #a1d0f8; /* Hachiware 藍 */
  transition: transform 0.2s;
  box-shadow: 0 5px 10px rgba(0,0,0,0.02);
}

li:hover {
  transform: translateX(5px);
  background: #fdfdfd;
}

.item-title {
  color: #5d5d5d;
  font-weight: 500;
}

.price {
  font-size: 18px;
  font-weight: bold;
  color: #ef6c00;
}

/* 下方裝飾區 */
.footer-decoration {
  text-align: center;
  margin-top: 30px;
}

.sticker {
  font-family: cursive;
  font-size: 20px;
  color: #ffb6c1;
  background-color: white;
  padding: 5px 15px;
  border-radius: 20px;
  border: 2px solid #ffecf0;
}

/* 分類標籤樣式 */
.cat-tag {
  background: #e1f5fe;
  color: #0288d1;
  padding: 2px 8px;
  border-radius: 8px;
  font-size: 11px;
  margin-right: 8px;
  vertical-align: middle;
}

/* 分類下拉選單樣式 */
.cat-select {
  border: 2px solid #ffecf0;
  border-radius: 12px;
  padding: 5px;
  outline: none;
  cursor: pointer;
}

.item-left {
  display: flex;
  align-items: center;
}

.status-msg {
  font-weight: bold;
  color: #a1887f;
  margin-top: 5px;
  min-height: 1.2em; /* 防止文字跳動 */
}
</style>