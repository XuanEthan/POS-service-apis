<script setup>
import { ref } from 'vue'

const orderItems = ref([
  { id: 1, name: 'Coca Cola', quantity: 3, price: 15000, total: 45000, user: 'admin' },
  { id: 2, name: 'Redbull Thái', quantity: 2, price: 18000, total: 36000, user: 'admin' },
  { id: 3, name: 'Dr Thanh', quantity: 10, price: 18000, total: 180000, user: 'admin' },
  { id: 4, name: 'Sting sữa', quantity: 2, price: 25000, total: 50000, user: 'admin' },
  { id: 5, name: 'Bạc Xỉu', quantity: 2, price: 22000, total: 44000, user: 'admin' },
  { id: 6, name: 'Cafe Sữa', quantity: 8, price: 20000, total: 160000, user: 'admin' },
  { id: 7, name: 'Cafe Nóng', quantity: 5, price: 15000, total: 75000, user: 'admin' }
])

const products = ref([
  { id: 1, name: 'Sữa Tươi nóng', unit: 'Ly', price: 15000, image: 'https://placehold.co/100x70/333/fff?text=Sữa+Tươi' },
  { id: 2, name: 'Sữa Tươi Đá', unit: 'Ly', price: 15000, image: 'https://placehold.co/100x70/555/fff?text=Sữa+Đá' },
  { id: 3, name: 'Sữa Tươi Không Đá', unit: 'Ly', price: 17000, image: 'https://placehold.co/100x70/777/fff?text=Không+Đá' },
  { id: 4, name: 'Sữa Tươi Cafe Đá', unit: 'Ly', price: 20000, image: 'https://placehold.co/100x70/999/fff?text=Cafe+Đá' },
  { id: 5, name: 'Sữa Tươi Cafe Nóng', unit: 'Ly', price: 20000, image: 'https://placehold.co/100x70/brown/fff?text=Cafe+Nóng' },
  { id: 6, name: 'Sữa Tươi Cacao Đá', unit: 'Ly', price: 25000, image: 'https://placehold.co/100x70/664422/fff?text=Cacao' },
  { id: 7, name: 'Sữa Đá Chanh', unit: 'Ly', price: 17000, image: 'https://placehold.co/100x70/green/fff?text=Sữa+Chanh' },
  { id: 8, name: 'Sữa Tắc', unit: 'Ly', price: 17000, image: 'https://placehold.co/100x70/lime/fff?text=Sữa+Tắc' },
  { id: 9, name: 'Sinh Tố Bơ', unit: 'Ly', price: 25000, image: 'https://placehold.co/100x70/4CAF50/fff?text=Bơ' },
  { id: 10, name: 'Sinh Tố Dâu', unit: 'Ly', price: 26000, image: 'https://placehold.co/100x70/E91E63/fff?text=Dâu' },
  { id: 11, name: 'Sinh Tố Mãng Cầu', unit: 'Ly', price: 27000, image: 'https://placehold.co/100x70/eee/333?text=Mãng+Cầu' },
  { id: 12, name: 'Sinh Tố Cà Chua', unit: 'Ly', price: 28000, image: 'https://placehold.co/100x70/f44336/fff?text=Cà+Chua' }
])

const grandTotal = ref('755,000')
const serviceFee = ref('0')
const discount = ref('75,500')
const tax = ref('0')
const mustPay = ref('679,500')
const customerPay = ref('0')
const selectedItemIndex = ref(0)

function formatPrice(price) {
  return price.toLocaleString('vi-VN')
}
</script>

<template>
  <div class="payment-container">
    <aside class="left-panel">
      <div class="lp-header">
        <div class="lp-info-row">
          <span style="font-size: 16px; font-weight: bold">SV02 - Sân vườn</span>
          <span>Giờ vào: 05/07/2019 12:18</span>
        </div>
      </div>

      <div class="order-table-container">
        <table>
          <thead>
            <tr>
              <th style="width: 20px">#</th>
              <th style="width: 30px">#</th>
              <th>Tên hàng</th>
              <th style="width: 40px">S.L</th>
              <th style="width: 60px">Giá bán</th>
              <th style="width: 70px">T.Tiền</th>
              <th style="width: 50px">User</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(item, index) in orderItems"
              :key="item.id"
              :class="{ selected: index === selectedItemIndex }"
              @click="selectedItemIndex = index"
            >
              <td class="col-center">●</td>
              <td class="col-center">{{ item.id }}</td>
              <td>{{ item.name }}</td>
              <td class="col-center">{{ item.quantity.toFixed(2) }}</td>
              <td class="col-right">{{ formatPrice(item.price) }}</td>
              <td class="col-right">{{ formatPrice(item.total) }}</td>
              <td class="col-center">{{ item.user }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="lp-footer">
        <div class="payment-summary">
          <div class="summary-row">
            <span>Tổng tiền</span>
            <input type="text" class="summary-input grand-total" v-model="grandTotal" />
          </div>
          <div class="summary-row">
            <span>Phí phục vụ</span>
            <input type="text" class="summary-input" v-model="serviceFee" />
          </div>
          <div class="summary-row">
            <span>Giảm giá</span>
            <input type="text" class="summary-input" v-model="discount" />
          </div>
          <div class="summary-row">
            <span>Thuế</span>
            <input type="text" class="summary-input" v-model="tax" />
          </div>

          <div class="pay-row">
            <span style="font-weight: bold">Phải trả:</span>
            <span class="lp-total-display">{{ mustPay }}</span>
          </div>
          <div class="pay-row">
            <span>Khách trả:</span>
            <input type="text" class="summary-input khach-tra-input" v-model="customerPay" />
          </div>
        </div>
        <div class="actions-row">
          <div class="col">
            <button class="action-btn btn-info">
              <span class="btn-icon">ℹ️</span><span>Mở rộng</span>
            </button>
            <button class="action-btn btn-green">
              <span class="btn-icon">👥</span><span>Ghép bàn</span>
            </button>
          </div>
          <div class="col">
            <button class="action-btn btn-purple">
              <span class="btn-icon">🔄</span><span>Chuyển bàn</span>
            </button>
            <button class="action-btn btn-blue">
              <span class="btn-icon">💾</span><span>F5 Lưu h.đơn</span>
            </button>
          </div>
          <div class="col">
            <button class="action-btn btn-pink">
              <span class="btn-icon">🖨️</span><span>F4 In h.đơn</span>
            </button>
            <button class="action-btn btn-orange">
              <span class="btn-icon">🍲</span><span>F3 In bếp/bar</span>
            </button>
          </div>
          <button class="action-btn btn-pay">Thanh toán</button>
        </div>
      </div>
    </aside>

    <main class="right-panel">
      <div class="rp-top-bar">
        <div class="tab-btn tab-room">BÀN / PHÒNG</div>
        <div class="tab-btn tab-menu">THỰC ĐƠN</div>
        <div class="search-box">
          <input type="text" placeholder="F9 Nhập mã/tên hàng để tìm kiếm" />
        </div>
      </div>

      <div class="product-grid-container">
        <div class="nav-arrow">‹</div>

        <div class="grid-wrapper">
          <div class="product-card" v-for="product in products" :key="product.id">
            <div class="product-img">
              <img :src="product.image" alt="sp" />
            </div>
            <div class="prod-name">{{ product.name }}</div>
            <div class="prod-unit">{{ product.unit }}</div>
            <div class="prod-price">$ {{ formatPrice(product.price) }}</div>
          </div>
        </div>

        <div class="nav-arrow">›</div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.payment-container {
  display: flex;
  width: 100%;
  height: 100vh;
  font-family: Arial, sans-serif;
  font-size: 13px;
}

/* Left Panel */
.left-panel {
  width: 45%;
  background: #f5f5f5;
  display: flex;
  flex-direction: column;
  border-right: 1px solid #ddd;
}

.lp-header {
  background: linear-gradient(90deg, #3498db 0%, #2980b9 100%);
  color: white;
  padding: 14px 15px;
}

.lp-info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.order-table-container {
  flex: 1;
  overflow-y: auto;
  background: white;
}

.order-table-container table {
  width: 100%;
  border-collapse: collapse;
}

.order-table-container thead th {
  background: #e0e0e0;
  padding: 8px 5px;
  text-align: left;
  font-weight: 600;
  border-bottom: 1px solid #ccc;
  position: sticky;
  top: 0;
}

.order-table-container tbody td {
  padding: 8px 5px;
  border-bottom: 1px solid #eee;
}

.order-table-container tbody tr:hover {
  background: #f0f0f0;
  cursor: pointer;
}

.order-table-container tbody tr.selected {
  background: #bbdefb;
}

.col-center {
  text-align: center;
}

.col-right {
  text-align: right;
}

/* Footer summary */
.lp-footer {
  background: #fafafa;
  border-top: 1px solid #ddd;
}

.payment-summary {
  padding: 10px 15px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.summary-input {
  width: 120px;
  text-align: right;
  padding: 5px 8px;
  border: 1px solid #ccc;
  border-radius: 3px;
}

.summary-input.grand-total {
  font-weight: bold;
  color: #d32f2f;
}

.pay-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
  padding: 5px 0;
}

.lp-total-display {
  font-size: 18px;
  font-weight: bold;
  color: #d32f2f;
}

.khach-tra-input {
  width: 120px;
}

/* Actions */
.actions-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px;
  padding: 10px 15px;
  background: #eee;
}

.actions-row .col {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
  padding: 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  font-weight: 600;
  color: white;
}

.btn-icon {
  font-size: 14px;
}

.btn-info { background: #17a2b8; }
.btn-green { background: #27ae60; }
.btn-purple { background: #9b59b6; }
.btn-blue { background: #3498db; }
.btn-pink { background: #e74c3c; }
.btn-orange { background: #f39c12; }

.btn-pay {
  grid-column: 1 / -1;
  background: #27ae60;
  font-size: 16px;
  padding: 15px;
}

.btn-pay:hover {
  background: #219a52;
}

/* Right Panel */
.right-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: #f9f9f9;
}

.rp-top-bar {
  display: flex;
  align-items: center;
  background: linear-gradient(90deg, #2c3e50 0%, #1a252f 100%);
  padding: 10px 15px;
  gap: 10px;
}

.tab-btn {
  padding: 10px 20px;
  background: #34495e;
  color: white;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
  transition: background 0.2s;
}

.tab-btn:hover {
  background: #3498db;
}

.tab-btn.tab-room {
  background: #f39c12;
}

.tab-btn.tab-menu {
  background: #27ae60;
}

.search-box {
  flex: 1;
  margin-left: 10px;
}

.search-box input {
  width: 100%;
  padding: 10px 15px;
  border: none;
  border-radius: 4px;
  font-size: 13px;
}

/* Product Grid */
.product-grid-container {
  flex: 1;
  display: flex;
  align-items: stretch;
  padding: 15px;
  gap: 10px;
  overflow: hidden;
}

.nav-arrow {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 30px;
  font-size: 24px;
  color: #757575;
  cursor: pointer;
  background: #e0e0e0;
  border-radius: 4px;
}

.nav-arrow:hover {
  background: #bdbdbd;
}

.grid-wrapper {
  flex: 1;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
  gap: 12px;
  overflow-y: auto;
  align-content: start;
}

.product-card {
  background: white;
  border-radius: 6px;
  padding: 10px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.product-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.product-img {
  width: 100%;
  height: 70px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
}

.product-img img {
  max-width: 100%;
  max-height: 100%;
  object-fit: cover;
  border-radius: 4px;
}

.prod-name {
  font-weight: 600;
  font-size: 12px;
  margin-bottom: 4px;
  color: #333;
}

.prod-unit {
  font-size: 11px;
  color: #757575;
  margin-bottom: 4px;
}

.prod-price {
  font-weight: bold;
  color: #d32f2f;
  font-size: 13px;
}

/* Responsive */
@media (max-width: 900px) {
  .payment-container {
    flex-direction: column;
  }
  
  .left-panel {
    width: 100%;
    height: 50vh;
  }
  
  .right-panel {
    height: 50vh;
  }
}
</style>
