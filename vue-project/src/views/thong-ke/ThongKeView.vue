<script setup>
import { ref, computed, onMounted } from 'vue'
import PermissionAlert from '@/components/PermissionAlert.vue'
import { hasPermission } from '@/utils/auth'
import { PERMISSIONS } from '@/constants/permissions'

// Kiểm tra quyền xem thống kê (có quyền THONGKE)
const canAccessModule_thongke = computed(() => hasPermission(PERMISSIONS.THONGKE))

// State
const loading = ref(false)
const selectedPeriod = ref('today')
const dateFrom = ref('')
const dateTo = ref('')

// Dữ liệu thống kê mẫu (thay bằng API thực tế)
const stats = ref({
  totalRevenue: 15850000,
  totalOrders: 45,
  totalProducts: 128,
  totalCustomers: 32,
  averageOrderValue: 352222,
  topProducts: [
    { name: 'Cà phê sữa đá', quantity: 85, revenue: 2125000 },
    { name: 'Trà đào cam sả', quantity: 62, revenue: 1860000 },
    { name: 'Bánh mì thịt', quantity: 48, revenue: 1200000 },
    { name: 'Phở bò tái', quantity: 35, revenue: 1575000 },
    { name: 'Bún chả Hà Nội', quantity: 28, revenue: 1400000 },
  ],
  revenueByHour: [
    { hour: '06:00', revenue: 450000 },
    { hour: '07:00', revenue: 1200000 },
    { hour: '08:00', revenue: 1850000 },
    { hour: '09:00', revenue: 1100000 },
    { hour: '10:00', revenue: 950000 },
    { hour: '11:00', revenue: 2100000 },
    { hour: '12:00', revenue: 2800000 },
    { hour: '13:00', revenue: 1500000 },
    { hour: '14:00', revenue: 800000 },
    { hour: '15:00', revenue: 650000 },
    { hour: '16:00', revenue: 750000 },
    { hour: '17:00', revenue: 1200000 },
    { hour: '18:00', revenue: 1850000 },
    { hour: '19:00', revenue: 2200000 },
    { hour: '20:00', revenue: 1500000 },
    { hour: '21:00', revenue: 800000 },
  ],
  recentOrders: [
    { id: 'HD001', time: '20:35', total: 185000, items: 3, status: 'completed' },
    { id: 'HD002', time: '20:22', total: 95000, items: 2, status: 'completed' },
    { id: 'HD003', time: '20:15', total: 320000, items: 5, status: 'completed' },
    { id: 'HD004', time: '19:58', total: 75000, items: 1, status: 'completed' },
    { id: 'HD005', time: '19:45', total: 245000, items: 4, status: 'completed' },
  ],
  paymentMethods: [
    { method: 'Tiền mặt', amount: 9500000, percent: 60 },
    { method: 'Chuyển khoản', amount: 4750000, percent: 30 },
    { method: 'Thẻ', amount: 1600000, percent: 10 },
  ]
})

// Tính max revenue để vẽ biểu đồ
const maxHourlyRevenue = computed(() => {
  return Math.max(...stats.value.revenueByHour.map(h => h.revenue))
})

// Format tiền VND
function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value)
}

// Format số
function formatNumber(value) {
  return new Intl.NumberFormat('vi-VN').format(value)
}

// Tính chiều cao cột biểu đồ (%)
function getBarHeight(revenue) {
  return (revenue / maxHourlyRevenue.value) * 100
}

// Xử lý thay đổi kỳ thống kê
function handlePeriodChange() {
  // TODO: Fetch data từ API theo period
  console.log('Period changed:', selectedPeriod.value)
}

// Xử lý tìm kiếm theo ngày
function handleDateSearch() {
  // TODO: Fetch data từ API theo date range
  console.log('Date range:', dateFrom.value, '-', dateTo.value)
}

// Refresh data
async function refreshData() {
  loading.value = true
  try {
    // TODO: Fetch từ API thực tế
    await new Promise(resolve => setTimeout(resolve, 500))
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  // Set default dates
  const today = new Date()
  dateTo.value = today.toISOString().split('T')[0]
  dateFrom.value = today.toISOString().split('T')[0]
})
</script>

<template>
  <!-- Kiểm tra quyền - nếu không có quyền thì hiển thị thông báo -->
  <PermissionAlert :hasPermission="canAccessModule_thongke" />

  <div v-if="canAccessModule_thongke" class="page-container thong-ke-page">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title"><i class="fas fa-chart-bar"></i> THỐNG KÊ BÁN HÀNG</h1>
    </div>

    <!-- Toolbar -->
    <div class="page-toolbar">
      <div class="toolbar-left">
        <select v-model="selectedPeriod" @change="handlePeriodChange" class="form-control">
          <option value="today">Hôm nay</option>
          <option value="yesterday">Hôm qua</option>
          <option value="week">7 ngày qua</option>
          <option value="month">Tháng này</option>
          <option value="lastMonth">Tháng trước</option>
          <option value="custom">Tùy chọn</option>
        </select>
        
        <template v-if="selectedPeriod === 'custom'">
          <input type="date" v-model="dateFrom" class="form-control" />
          <span>đến</span>
          <input type="date" v-model="dateTo" class="form-control" />
          <button class="btn btn-primary" @click="handleDateSearch">Xem</button>
        </template>
      </div>
      
      <div class="toolbar-right">
        <button class="btn btn-secondary" @click="refreshData" :disabled="loading">
          <i class="fas fa-sync-alt"></i> {{ loading ? 'Đang tải...' : 'Làm mới' }}
        </button>
        <button class="btn btn-success"><i class="fas fa-file-excel"></i> Xuất Excel</button>
        <button class="btn btn-info"><i class="fas fa-print"></i> In báo cáo</button>
      </div>
    </div>

    <!-- Loading Overlay -->
    <div v-if="loading" class="loading-overlay">
      <div class="loading-spinner"></div>
      <span>Đang tải dữ liệu...</span>
    </div>

    <!-- Stats Cards -->
    <div class="stats-cards">
      <div class="stat-card revenue">
        <div class="stat-icon"><i class="fas fa-coins"></i></div>
        <div class="stat-content">
          <div class="stat-value">{{ formatCurrency(stats.totalRevenue) }}</div>
          <div class="stat-label">Doanh thu</div>
        </div>
        <div class="stat-trend up">↑ 12%</div>
      </div>
      
      <div class="stat-card orders">
        <div class="stat-icon"><i class="fas fa-file-invoice"></i></div>
        <div class="stat-content">
          <div class="stat-value">{{ formatNumber(stats.totalOrders) }}</div>
          <div class="stat-label">Đơn hàng</div>
        </div>
        <div class="stat-trend up">↑ 8%</div>
      </div>
      
      <div class="stat-card products">
        <div class="stat-icon"><i class="fas fa-box"></i></div>
        <div class="stat-content">
          <div class="stat-value">{{ formatNumber(stats.totalProducts) }}</div>
          <div class="stat-label">Sản phẩm bán</div>
        </div>
        <div class="stat-trend up">↑ 15%</div>
      </div>
      
      <div class="stat-card average">
        <div class="stat-icon"><i class="fas fa-chart-line"></i></div>
        <div class="stat-content">
          <div class="stat-value">{{ formatCurrency(stats.averageOrderValue) }}</div>
          <div class="stat-label">TB/Đơn hàng</div>
        </div>
        <div class="stat-trend down">↓ 3%</div>
      </div>
    </div>

    <!-- Main Content Grid -->
    <div class="stats-grid">
      <!-- Biểu đồ doanh thu theo giờ -->
      <div class="stats-section chart-section">
        <div class="section-header">
          <h3><i class="fas fa-chart-bar"></i> Doanh thu theo giờ</h3>
        </div>
        <div class="chart-container">
          <div class="bar-chart">
            <div 
              v-for="item in stats.revenueByHour" 
              :key="item.hour" 
              class="bar-item"
            >
              <div class="bar-wrapper">
                <div 
                  class="bar" 
                  :style="{ height: getBarHeight(item.revenue) + '%' }"
                  :title="formatCurrency(item.revenue)"
                ></div>
              </div>
              <div class="bar-label">{{ item.hour.split(':')[0] }}h</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Top sản phẩm -->
      <div class="stats-section">
        <div class="section-header">
          <h3><i class="fas fa-trophy"></i> Top sản phẩm bán chạy</h3>
        </div>
        <div class="top-products">
          <div 
            v-for="(product, index) in stats.topProducts" 
            :key="product.name" 
            class="product-item"
          >
            <div class="product-rank" :class="'rank-' + (index + 1)">{{ index + 1 }}</div>
            <div class="product-info">
              <div class="product-name">{{ product.name }}</div>
              <div class="product-stats">
                <span>{{ formatNumber(product.quantity) }} đã bán</span>
              </div>
            </div>
            <div class="product-revenue">{{ formatCurrency(product.revenue) }}</div>
          </div>
        </div>
      </div>

      <!-- Phương thức thanh toán -->
      <div class="stats-section">
        <div class="section-header">
          <h3><i class="fas fa-credit-card"></i> Phương thức thanh toán</h3>
        </div>
        <div class="payment-methods">
          <div 
            v-for="payment in stats.paymentMethods" 
            :key="payment.method" 
            class="payment-item"
          >
            <div class="payment-info">
              <div class="payment-method">{{ payment.method }}</div>
              <div class="payment-amount">{{ formatCurrency(payment.amount) }}</div>
            </div>
            <div class="payment-bar-container">
              <div class="payment-bar" :style="{ width: payment.percent + '%' }"></div>
            </div>
            <div class="payment-percent">{{ payment.percent }}%</div>
          </div>
        </div>
      </div>

      <!-- Đơn hàng gần đây -->
      <div class="stats-section">
        <div class="section-header">
          <h3><i class="fas fa-clock"></i> Đơn hàng gần đây</h3>
          <a href="/hoa-don" class="view-all">Xem tất cả →</a>
        </div>
        <div class="recent-orders">
          <table class="mini-table">
            <thead>
              <tr>
                <th>Mã HĐ</th>
                <th>Giờ</th>
                <th>Số SP</th>
                <th>Tổng tiền</th>
                <th>TT</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="order in stats.recentOrders" :key="order.id">
                <td class="order-id">{{ order.id }}</td>
                <td>{{ order.time }}</td>
                <td class="text-center">{{ order.items }}</td>
                <td class="text-right">{{ formatCurrency(order.total) }}</td>
                <td>
                  <span class="status-badge completed">✓</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.thong-ke-page {
  background: #f5f7fa;
  min-height: 100%;
  padding-bottom: 16px;
  overflow: auto;
}

.page-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 16px;
  background: #fff;
  border-bottom: 1px solid #e0e0e0;
  flex-wrap: wrap;
  gap: 8px;
}

.toolbar-left, .toolbar-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

.toolbar-left span {
  color: #666;
  font-size: 11px;
}

.form-control {
  padding: 5px 8px;
  border: 1px solid #ddd;
  border-radius: 2px;
  font-size: 11px;
}

.btn {
  padding: 5px 10px;
  border: none;
  border-radius: 2px;
  cursor: pointer;
  font-size: 11px;
  display: flex;
  align-items: center;
  gap: 4px;
  transition: opacity 0.2s;
}

.btn i {
  font-size: 10px;
}

.btn-primary { background: #3498db; color: white; }
.btn-secondary { background: #95a5a6; color: white; }
.btn-success { background: #27ae60; color: white; }
.btn-info { background: #17a2b8; color: white; }

.btn:hover { opacity: 0.9; }
.btn:disabled { opacity: 0.6; cursor: not-allowed; }

/* Loading Overlay */
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255,255,255,0.8);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  gap: 10px;
}

.loading-spinner {
  width: 30px;
  height: 30px;
  border: 3px solid #e0e0e0;
  border-top-color: #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Stats Cards */
.stats-cards {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
  padding: 12px 16px;
}

.stat-card {
  background: white;
  border-radius: 2px;
  padding: 12px;
  display: flex;
  align-items: center;
  gap: 10px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
  position: relative;
  overflow: hidden;
}

.stat-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 3px;
  height: 100%;
}

.stat-card.revenue::before { background: #27ae60; }
.stat-card.orders::before { background: #3498db; }
.stat-card.products::before { background: #9b59b6; }
.stat-card.average::before { background: #f39c12; }

.stat-icon {
  font-size: 20px;
  opacity: 0.9;
}

.stat-card.revenue .stat-icon { color: #27ae60; }
.stat-card.orders .stat-icon { color: #3498db; }
.stat-card.products .stat-icon { color: #9b59b6; }
.stat-card.average .stat-icon { color: #f39c12; }

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 14px;
  font-weight: 600;
  color: #2c3e50;
}

.stat-label {
  font-size: 10px;
  color: #7f8c8d;
  margin-top: 2px;
}

.stat-trend {
  font-size: 10px;
  font-weight: 500;
  padding: 2px 6px;
  border-radius: 2px;
}

.stat-trend.up { background: #d4edda; color: #155724; }
.stat-trend.down { background: #f8d7da; color: #721c24; }

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
  padding: 0 16px;
}

.stats-section {
  background: white;
  border-radius: 2px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
  overflow: hidden;
}

.stats-section.chart-section {
  grid-column: span 2;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 12px;
  border-bottom: 1px solid #eee;
}

.section-header h3 {
  font-size: 12px;
  font-weight: 600;
  color: #2c3e50;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 6px;
}

.section-header h3 i {
  color: #7f8c8d;
}

.view-all {
  font-size: 11px;
  color: #3498db;
  text-decoration: none;
}

.view-all:hover { text-decoration: underline; }

/* Bar Chart */
.chart-container {
  padding: 12px;
  height: 180px;
}

.bar-chart {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  height: 100%;
  gap: 4px;
}

.bar-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100%;
}

.bar-wrapper {
  flex: 1;
  width: 100%;
  display: flex;
  align-items: flex-end;
  justify-content: center;
}

.bar {
  width: 70%;
  background: #3498db;
  border-radius: 2px 2px 0 0;
  min-height: 3px;
  transition: height 0.3s ease;
  cursor: pointer;
}

.bar:hover {
  background: #2980b9;
}

.bar-label {
  font-size: 9px;
  color: #7f8c8d;
  margin-top: 4px;
}

/* Top Products */
.top-products {
  padding: 6px 0;
}

.product-item {
  display: flex;
  align-items: center;
  padding: 8px 12px;
  gap: 10px;
  border-bottom: 1px solid #f0f0f0;
}

.product-item:last-child { border-bottom: none; }

.product-rank {
  width: 20px;
  height: 20px;
  border-radius: 2px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 10px;
  color: white;
}

.rank-1 { background: #f1c40f; }
.rank-2 { background: #95a5a6; }
.rank-3 { background: #e67e22; }
.rank-4, .rank-5 { background: #7f8c8d; }

.product-info { flex: 1; }

.product-name {
  font-weight: 500;
  font-size: 11px;
  color: #2c3e50;
  margin-bottom: 1px;
}

.product-stats {
  font-size: 10px;
  color: #7f8c8d;
}

.product-revenue {
  font-weight: 600;
  font-size: 11px;
  color: #27ae60;
}

/* Payment Methods */
.payment-methods {
  padding: 10px 12px;
}

.payment-item {
  margin-bottom: 10px;
}

.payment-item:last-child { margin-bottom: 0; }

.payment-info {
  display: flex;
  justify-content: space-between;
  margin-bottom: 4px;
}

.payment-method {
  font-weight: 500;
  font-size: 11px;
  color: #2c3e50;
}

.payment-amount {
  color: #7f8c8d;
  font-size: 10px;
}

.payment-bar-container {
  height: 6px;
  background: #ecf0f1;
  border-radius: 2px;
  overflow: hidden;
  margin-bottom: 3px;
}

.payment-bar {
  height: 100%;
  background: #3498db;
  border-radius: 2px;
  transition: width 0.5s ease;
}

.payment-percent {
  font-size: 10px;
  color: #7f8c8d;
  text-align: right;
}

/* Recent Orders */
.recent-orders {
  padding: 0;
}

.mini-table {
  width: 100%;
  border-collapse: collapse;
}

.mini-table th {
  background: #f8f9fa;
  padding: 6px 10px;
  font-size: 10px;
  font-weight: 600;
  color: #7f8c8d;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.mini-table td {
  padding: 8px 10px;
  font-size: 11px;
  border-bottom: 1px solid #f0f0f0;
}

.mini-table tr:last-child td { border-bottom: none; }

.order-id {
  font-weight: 600;
  color: #3498db;
}

.text-center { text-align: center; }
.text-right { text-align: right; }

.status-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 18px;
  height: 18px;
  border-radius: 2px;
  font-size: 10px;
}

.status-badge.completed {
  background: #d4edda;
  color: #155724;
}

/* Responsive */
@media (max-width: 1200px) {
  .stats-cards {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .stats-grid {
    grid-template-columns: 1fr;
  }
  
  .stats-section.chart-section {
    grid-column: span 1;
  }
}

@media (max-width: 768px) {
  .stats-cards {
    grid-template-columns: 1fr;
  }
  
  .page-toolbar {
    flex-direction: column;
    align-items: stretch;
  }
  
  .toolbar-left, .toolbar-right {
    flex-wrap: wrap;
    justify-content: center;
  }
}
</style>
