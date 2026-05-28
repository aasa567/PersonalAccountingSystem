<template>
  <router-view></router-view>
</template>

 
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