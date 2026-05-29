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
        <input v-model="newItem.title" placeholder="項目（如：草莓聖代）" style="flex: 2; min-width: 0;"/>
        <input v-model="newItem.note" placeholder="備註（如：小八貓請客）" style="flex: 1.5; min-width: 0;"/>
        <input v-model.number="newItem.amount" type="number" placeholder="金額" style="flex: 1; min-width: 0;"/>
        
        <input v-model="newItem.date" type="date" class="date-input" style="flex: 1.5; min-width: 0;" />
        
        <select v-model="newItem.category" class="cat-select">
          <option value="食">🍔 食</option>
          <option value="衣">🛍️ 衣</option>
          <option value="住">🏠 住</option>
          <option value="行">🚃 行</option>
          <option value="其他">❓ 其他</option>
        </select>

        <button @click="addItem">新增</button>
      </div>

      <div class="filter-box">
        <div class="filter-group">
          <label class="filter-label">按分類篩選：</label>
          <select v-model="filterCategory" class="filter-select">
            <option value="全部">🌈 全部顯示</option>
            <option value="食">🍔 食</option>
            <option value="衣">🛍️ 衣</option>
            <option value="住">🏠 住</option>
            <option value="行">🚃 行</option>
            <option value="其他">❓ 其他</option>
          </select>
        </div>

        <div class="filter-group">
          <label class="filter-label">關鍵字搜尋：</label>
          <input v-model="searchKeyword" placeholder="搜尋項目或備註..." class="filter-input" />
        </div>
      </div>

      <hr />

      <div v-if="loading" class="loading-text">讀取中...</div>
      <ul v-else>
        <li v-for="item in filteredItems" :key="item.id">
          <div class="item-left">
            <span class="cat-tag">
              {{ item.category === '食' ? '🍔' : item.category === '衣' ? '🛍️' : item.category === '住' ? '🏠' : item.category === '行' ? '🚃' : '❓' }} {{ item.category }}
            </span>
            <div class="item-info">
              <span class="item-title">
                {{ item.title }}
                <span v-if="item.note" style="font-size: 12px; color: #bcaaa4; font-weight: normal; margin-left: 6px;">({{ item.note }})</span>
              </span>
              <span class="item-date">{{ item.date ? item.date.split('T')[0] : '' }}</span>
            </div>
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
import { useRouter } from 'vue-router'
import Swal from 'sweetalert2'

const items = ref([])
const loading = ref(true)

// 🔍 補上：用來記錄篩選條件的響應式變數
const filterCategory = ref('全部')
const searchKeyword = ref('')

// 📅 getTodayDate 預設帶入今天的日期 (格式：YYYY-MM-DD)
const getTodayDate = () => new Date().toISOString().split('T')[0]

// 📝 初始值完整包含 title, note, amount, category, date
const newItem = ref({ title: '', note: '', amount: 0, category: '食', date: getTodayDate() })

const router = useRouter()
const currentUserName = ref(localStorage.getItem('userName') || '') 

const API_URL = 'https://localhost:7283/api/transactions'

const handleLogout = () => {
  localStorage.clear() 
  
  Swal.fire({
    title: '已成功登出！',
    text: '要記得回來記帳喔 (๑´ㅂ`๑)',
    icon: 'success',
    timer: 1500,
    showConfirmButton: false
  })
  
  router.push('/login')
}

// 🌟 補上：計算篩選後的歷史紀錄邏輯
const filteredItems = computed(() => {
  return items.value.filter(item => {
    // 1. 檢查分類
    const matchCategory = filterCategory.value === '全部' || item.category === filterCategory.value
    
    // 2. 檢查關鍵字（同時搜尋項目 title 和備註 note，不分大小寫）
    const keyword = searchKeyword.value.trim().toLowerCase()
    const matchKeyword = !keyword || 
                         item.title.toLowerCase().includes(keyword) || 
                         (item.note && item.note.toLowerCase().includes(keyword))
    
    return matchCategory && matchKeyword
  })
})

// 自動計算總金額（改為連動篩選後的結果）
const totalAmount = computed(() => {
  return filteredItems.value.reduce((sum, item) => sum + item.amount, 0)
})

// 根據金額決定吉伊卡哇的表情
const statusMessage = computed(() => {
  if (totalAmount.value === 0) return "帳本空空的... ( ・ω・)"
  if (totalAmount.value > 2000) return "哇哇...花太多了啦 ( ᐡ ɞ̴̶̷ ❌ ɞ̴̶̷ ᐡ )"
  return "目前還很省喔！撒花！( ˶ˆ꒳ˆ˵ )"
})

const fetchItems = async () => {
  try {
    const res = await axios.get(API_URL)
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
    // 🌟 核心：一次把完整的欄位傳遞給後端 API
    await axios.post(API_URL, {
      title: newItem.value.title,
      note: newItem.value.note,                         // 📝 備註
      amount: newItem.value.amount,
      category: newItem.value.category,                 // 分類
      date: new Date(newItem.value.date).toISOString(), // 自訂日期
      type: "Expense"
    })
    
    // 重設輸入框狀態
    newItem.value = { title: '', note: '', amount: 0, category: '食', date: getTodayDate() }
    fetchItems()
  } catch (err) {
    Swal.fire('錯誤', '新增失敗，請檢查後端 API', 'error')
  }
}

const deleteItem = async (id) => {
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

.container {
  max-width: 600px; 
  width: 90%;
  margin: 0 auto;
  padding: 30px;
  background: rgba(255, 255, 255, 0.9); 
  border-radius: 30px; 
  box-shadow: 0 15px 30px rgba(255, 182, 193, 0.2); 
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
  gap: 8px;
  margin-bottom: 20px;
  align-items: center;
}

/* 🔍 新增的篩選區樣式 */
.filter-box {
  margin-bottom: 25px; 
  display: flex; 
  gap: 12px; 
  background: #fff8f9; 
  padding: 12px 18px; 
  border-radius: 16px; 
  border: 2px dashed #ffccd5;
}

.filter-group {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.filter-label {
  font-size: 12px; 
  color: #a1887f; 
  font-weight: bold;
  margin-bottom: 4px;
}

.filter-select, .filter-input {
  width: 100%; 
  padding: 10px; 
  border-radius: 10px; 
  border: 2px solid #ffecf0;
  outline: none;
  font-size: 14px;
  box-sizing: border-box;
  background-color: white;
}

.filter-select:focus, .filter-input:focus {
  border-color: #ffdb4d;
  background-color: #fffdf5;
}

input {
  padding: 12px;
  border: 2px solid #ffecf0;
  border-radius: 12px;
  outline: none;
  transition: all 0.3s;
}

input:focus {
  border-color: #ffd900;
  background-color: #fffdf5;
}

.date-input {
  cursor: pointer;
  padding: 11px 8px;
  color: #5d4037;
  font-family: sans-serif;
}

button {
  padding: 11px 16px;
  background: #ffdb4d;
  color: #5d4037;
  font-weight: bold;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  transition: transform 0.2s, background 0.3s;
  white-space: nowrap;
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
  transform: scale(1.3);
}

hr {
  border: none;
  border-top: 2px dashed #ffecf0; 
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

li {
  background: white;
  margin-bottom: 12px;
  padding: 15px 18px;
  border-radius: 15px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  list-style: none;
  border-left: 6px solid #a1d0f8; 
  transition: transform 0.2s;
  box-shadow: 0 5px 10px rgba(0,0,0,0.02);
}

li:hover {
  transform: translateX(5px);
  background: #fdfdfd;
}

.item-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.item-title {
  color: #5d5d5d;
  font-weight: 500;
}

.item-date {
  font-size: 11px;
  color: #bcaaa4;
}

.price {
  font-size: 18px;
  font-weight: bold;
  color: #ef6c00;
}

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

.cat-tag {
  background: #fff3cd; 
  color: #856404;
  padding: 4px 8px;
  border-radius: 8px;
  font-size: 12px;
  margin-right: 12px;
  white-space: nowrap;
}

.cat-select {
  border: 2px solid #ffecf0;
  border-radius: 12px;
  padding: 11px 6px;
  outline: none;
  cursor: pointer;
  color: #5d4037;
  background-color: white;
}

.item-left {
  display: flex;
  align-items: center;
}

.status-msg {
  font-weight: bold;
  color: #a1887f;
  margin-top: 5px;
  min-height: 1.2em; 
}
</style>