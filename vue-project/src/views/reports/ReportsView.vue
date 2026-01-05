<script setup>
import { ref, computed, onMounted } from 'vue'

// Fake data for static reports
const loading = ref(false)
const selectedPeriod = ref('today')
const dateFrom = ref('')
const dateTo = ref('')

const salesReport = ref([
  { id: 'R001', date: '2026-01-03', orders: 24, revenue: 1250000, profit: 375000 },
  { id: 'R002', date: '2026-01-02', orders: 31, revenue: 1825000, profit: 548000 },
  { id: 'R003', date: '2026-01-01', orders: 19, revenue: 920000, profit: 276000 },
  { id: 'R004', date: '2025-12-31', orders: 28, revenue: 1450000, profit: 435000 },
  { id: 'R005', date: '2025-12-30', orders: 22, revenue: 1125000, profit: 338000 },
])

const inventoryReport = ref([
  { sku: 'SP001', name: 'Cà phê sữa đá', stock: 12, threshold: 10, status: 'warning' },
  { sku: 'SP002', name: 'Trà đào cam sả', stock: 4, threshold: 10, status: 'critical' },
  { sku: 'SP003', name: 'Bánh mì thịt', stock: 28, threshold: 5, status: 'good' },
  { sku: 'SP004', name: 'Phở bò tái', stock: 8, threshold: 10, status: 'warning' },
  { sku: 'SP005', name: 'Bún chả Hà Nội', stock: 2, threshold: 5, status: 'critical' },
])

const paymentReport = ref([
  { method: 'Tiền mặt', count: 68, amount: 3850000, percent: 58 },
  { method: 'Chuyển khoản', count: 32, amount: 2120000, percent: 32 },
  { method: 'Thẻ', count: 12, amount: 680000, percent: 10 },
])

const taxSummary = ref({ 
  period: '01/2026', 
  taxableSales: 6650000, 
  vat: 665000,
  totalRevenue: 7315000,
  netRevenue: 5985000
})

const topProducts = ref([
  { rank: 1, name: 'Cà phê sữa đá', quantity: 85, revenue: 2125000 },
  { rank: 2, name: 'Trà đào cam sả', quantity: 62, revenue: 1860000 },
  { rank: 3, name: 'Bánh mì thịt', quantity: 48, revenue: 1200000 },
  { rank: 4, name: 'Phở bò tái', quantity: 35, revenue: 1575000 },
  { rank: 5, name: 'Bún chả Hà Nội', quantity: 28, revenue: 1400000 },
])

function formatCurrency(v) {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
}

function formatNumber(v) {
  return new Intl.NumberFormat('vi-VN').format(v)
}

function handlePeriodChange() {
  console.log('Period changed:', selectedPeriod.value)
}

function handleExportCSV() {
  alert('Chức năng xuất CSV (chưa triển khai)')
}

function handleExportPDF() {
  alert('Chức năng xuất PDF (chưa triển khai)')
}

function handlePrint() {
  window.print()
}

function refreshData() {
  loading.value = true
  setTimeout(() => {
    loading.value = false
    alert('Dữ liệu đã được làm mới!')
  }, 800)
}

onMounted(() => {
  const today = new Date()
  dateTo.value = today.toISOString().split('T')[0]
  const weekAgo = new Date(today)
  weekAgo.setDate(weekAgo.getDate() - 7)
  dateFrom.value = weekAgo.toISOString().split('T')[0]
})
</script>

<template>
  <div class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title"><i class="fas fa-file-alt"></i> BÁO CÁO</h1>
    </div>
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
          <span style="color: #666; font-size: 11px;">đến</span>
          <input type="date" v-model="dateTo" class="form-control" />
        </template>
      </div>
      
      <div class="toolbar-right">
        <button class="btn btn-secondary" @click="refreshData" :disabled="loading">
          <i class="fas fa-sync-alt"></i> {{ loading ? 'Đang tải...' : 'Làm mới' }}
        </button>
        <button class="btn btn-success" @click="handleExportCSV">
          <i class="fas fa-file-excel"></i> Xuất Excel
        </button>
        <button class="btn btn-info" @click="handleExportPDF">
          <i class="fas fa-file-pdf"></i> Xuất PDF
        </button>
        <button class="btn btn-primary" @click="handlePrint">
          <i class="fas fa-print"></i> In
        </button>
      </div>
    </div>

    <!-- Toolbar -->

    <!-- Stats Summary Cards -->
    <div class="stats-summary">
      <div class="summary-card revenue">
        <div class="card-icon"><i class="fas fa-coins"></i></div>
        <div class="card-content">
          <div class="card-value">{{ formatCurrency(taxSummary.totalRevenue) }}</div>
          <div class="card-label">Tổng doanh thu</div>
        </div>
      </div>
      <div class="summary-card orders">
        <div class="card-icon"><i class="fas fa-file-invoice"></i></div>
        <div class="card-content">
          <div class="card-value">{{ formatNumber(salesReport.reduce((sum, r) => sum + r.orders, 0)) }}</div>
          <div class="card-label">Tổng đơn hàng</div>
        </div>
      </div>
      <div class="summary-card profit">
        <div class="card-icon"><i class="fas fa-chart-line"></i></div>
        <div class="card-content">
          <div class="card-value">{{ formatCurrency(salesReport.reduce((sum, r) => sum + r.profit, 0)) }}</div>
          <div class="card-label">Lợi nhuận</div>
        </div>
      </div>
      <div class="summary-card vat">
        <div class="card-icon"><i class="fas fa-percentage"></i></div>
        <div class="card-content">
          <div class="card-value">{{ formatCurrency(taxSummary.vat) }}</div>
          <div class="card-label">VAT</div>
        </div>
      </div>
    </div>

    <!-- Reports Grid -->
    <div class="reports-grid">
      <!-- Báo cáo doanh thu -->
      <section class="report-section">
        <div class="section-header">
          <h3><i class="fas fa-chart-bar"></i> Báo cáo doanh thu</h3>
        </div>
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Mã</th>
                <th>Ngày</th>
                <th class="text-center">Số HĐ</th>
                <th class="text-right">Doanh thu</th>
                <th class="text-right">Lợi nhuận</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in salesReport" :key="row.id">
                <td class="row-id">{{ row.id }}</td>
                <td>{{ row.date }}</td>
                <td class="text-center">{{ row.orders }}</td>
                <td class="text-right">{{ formatCurrency(row.revenue) }}</td>
                <td class="text-right profit-col">{{ formatCurrency(row.profit) }}</td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="total-row">
                <td colspan="2"><strong>Tổng cộng</strong></td>
                <td class="text-center"><strong>{{ salesReport.reduce((sum, r) => sum + r.orders, 0) }}</strong></td>
                <td class="text-right"><strong>{{ formatCurrency(salesReport.reduce((sum, r) => sum + r.revenue, 0)) }}</strong></td>
                <td class="text-right"><strong>{{ formatCurrency(salesReport.reduce((sum, r) => sum + r.profit, 0)) }}</strong></td>
              </tr>
            </tfoot>
          </table>
        </div>
      </section>

      <!-- Top sản phẩm -->
      <section class="report-section">
        <div class="section-header">
          <h3><i class="fas fa-trophy"></i> Top sản phẩm bán chạy</h3>
        </div>
        <div class="top-products-list">
          <div v-for="product in topProducts" :key="product.rank" class="product-row">
            <div class="product-rank" :class="'rank-' + product.rank">{{ product.rank }}</div>
            <div class="product-info">
              <div class="product-name">{{ product.name }}</div>
              <div class="product-stats">{{ formatNumber(product.quantity) }} đã bán</div>
            </div>
            <div class="product-revenue">{{ formatCurrency(product.revenue) }}</div>
          </div>
        </div>
      </section>

      <!-- Báo cáo tồn kho -->
      <section class="report-section">
        <div class="section-header">
          <h3><i class="fas fa-box"></i> Báo cáo tồn kho</h3>
        </div>
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>SKU</th>
                <th>Tên sản phẩm</th>
                <th class="text-center">Tồn kho</th>
                <th class="text-center">Ngưỡng</th>
                <th class="text-center">Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in inventoryReport" :key="item.sku">
                <td class="row-id">{{ item.sku }}</td>
                <td>{{ item.name }}</td>
                <td class="text-center"><strong>{{ item.stock }}</strong></td>
                <td class="text-center">{{ item.threshold }}</td>
                <td class="text-center">
                  <span class="badge" :class="'badge-' + item.status">
                    {{ item.status === 'critical' ? 'Cần nhập gấp' : item.status === 'warning' ? 'Sắp hết' : 'Đủ' }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- Báo cáo thanh toán -->
      <section class="report-section">
        <div class="section-header">
          <h3><i class="fas fa-credit-card"></i> Phương thức thanh toán</h3>
        </div>
        <div class="payment-list">
          <div v-for="payment in paymentReport" :key="payment.method" class="payment-row">
            <div class="payment-method">
              <div class="method-name">{{ payment.method }}</div>
              <div class="method-count">{{ payment.count }} giao dịch</div>
            </div>
            <div class="payment-amount">{{ formatCurrency(payment.amount) }}</div>
            <div class="payment-bar-wrapper">
              <div class="payment-bar" :style="{ width: payment.percent + '%' }"></div>
            </div>
            <div class="payment-percent">{{ payment.percent }}%</div>
          </div>
        </div>
      </section>

      <!-- Tóm tắt thuế -->
      <section class="report-section full-width">
        <div class="section-header">
          <h3><i class="fas fa-calculator"></i> Tóm tắt thuế/VAT</h3>
        </div>
        <div class="tax-summary">
          <div class="tax-item">
            <span class="tax-label">Kỳ báo cáo:</span>
            <span class="tax-value">{{ taxSummary.period }}</span>
          </div>
          <div class="tax-item">
            <span class="tax-label">Tổng doanh thu:</span>
            <span class="tax-value">{{ formatCurrency(taxSummary.totalRevenue) }}</span>
          </div>
          <div class="tax-item">
            <span class="tax-label">Doanh thu chịu thuế:</span>
            <span class="tax-value">{{ formatCurrency(taxSummary.taxableSales) }}</span>
          </div>
          <div class="tax-item">
            <span class="tax-label">VAT (10%):</span>
            <span class="tax-value highlight">{{ formatCurrency(taxSummary.vat) }}</span>
          </div>
          <div class="tax-item">
            <span class="tax-label">Doanh thu thuần:</span>
            <span class="tax-value">{{ formatCurrency(taxSummary.netRevenue) }}</span>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
.page-container{
    overflow: scroll;
}
/* Toolbar */
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

.form-control {
  padding: 5px 8px;
  border: 1px solid #ddd;
  border-radius: 2px;
  font-size: 11px;
  background: #fff;
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
  font-weight: 500;
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

/* Stats Summary */
.stats-summary {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
  padding: 12px 16px;
  background: #f5f7fa;
}

.summary-card {
  background: white;
  border-radius: 2px;
  padding: 12px;
  display: flex;
  align-items: center;
  gap: 10px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
  border-left: 3px solid transparent;
}

.summary-card.revenue { border-left-color: #27ae60; }
.summary-card.orders { border-left-color: #3498db; }
.summary-card.profit { border-left-color: #f39c12; }
.summary-card.vat { border-left-color: #9b59b6; }

.card-icon {
  font-size: 20px;
  opacity: 0.9;
}

.summary-card.revenue .card-icon { color: #27ae60; }
.summary-card.orders .card-icon { color: #3498db; }
.summary-card.profit .card-icon { color: #f39c12; }
.summary-card.vat .card-icon { color: #9b59b6; }

.card-content {
  flex: 1;
}

.card-value {
  font-size: 14px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 2px;
}

.card-label {
  font-size: 10px;
  color: #7f8c8d;
}

/* Reports Grid */
.reports-grid {
  padding: 12px 16px;
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
  background: #f5f7fa;
}

.report-section {
  background: #fff;
  border-radius: 2px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
  overflow: hidden;
}

.report-section.full-width {
  grid-column: span 2;
}

.section-header {
  padding: 10px 12px;
  border-bottom: 1px solid #eee;
  background: #f8f9fa;
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
  font-size: 11px;
}

/* Tables */
.table-container {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 11px;
}

.data-table thead th {
  background: #f8f9fa;
  padding: 8px 10px;
  text-align: left;
  font-weight: 600;
  color: #2c3e50;
  border-bottom: 2px solid #e9ecef;
  font-size: 11px;
}

.data-table tbody td {
  padding: 8px 10px;
  border-bottom: 1px solid #f0f0f0;
  color: #2c3e50;
}

.data-table tbody tr:hover {
  background: #f8f9fa;
}

.data-table tfoot td {
  padding: 8px 10px;
  background: #f8f9fa;
  border-top: 2px solid #dee2e6;
  font-weight: 600;
  color: #2c3e50;
}

.row-id {
  color: #3498db;
  font-weight: 500;
}

.profit-col {
  color: #27ae60;
  font-weight: 500;
}

.text-center { text-align: center; }
.text-right { text-align: right; }

/* Top Products */
.top-products-list {
  padding: 8px;
}

.product-row {
  display: flex;
  align-items: center;
  padding: 8px;
  gap: 10px;
  border-bottom: 1px solid #f0f0f0;
}

.product-row:last-child {
  border-bottom: none;
}

.product-rank {
  width: 22px;
  height: 22px;
  border-radius: 2px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 11px;
  color: white;
  flex-shrink: 0;
}

.rank-1 { background: #f1c40f; }
.rank-2 { background: #95a5a6; }
.rank-3 { background: #e67e22; }
.rank-4, .rank-5 { background: #7f8c8d; }

.product-info {
  flex: 1;
}

.product-name {
  font-weight: 500;
  font-size: 11px;
  color: #2c3e50;
  margin-bottom: 2px;
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

/* Payment List */
.payment-list {
  padding: 10px 12px;
}

.payment-row {
  display: grid;
  grid-template-columns: 1fr auto 100px auto;
  gap: 10px;
  align-items: center;
  margin-bottom: 12px;
}

.payment-row:last-child {
  margin-bottom: 0;
}

.payment-method {
  display: flex;
  flex-direction: column;
}

.method-name {
  font-weight: 500;
  font-size: 11px;
  color: #2c3e50;
  margin-bottom: 2px;
}

.method-count {
  font-size: 10px;
  color: #7f8c8d;
}

.payment-amount {
  font-weight: 600;
  font-size: 11px;
  color: #2c3e50;
}

.payment-bar-wrapper {
  height: 6px;
  background: #ecf0f1;
  border-radius: 2px;
  overflow: hidden;
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
  width: 30px;
}

/* Tax Summary */
.tax-summary {
  padding: 12px;
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 12px;
}

.tax-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
  padding: 10px;
  background: #f8f9fa;
  border-radius: 2px;
}

.tax-label {
  font-size: 10px;
  color: #7f8c8d;
}

.tax-value {
  font-size: 12px;
  font-weight: 600;
  color: #2c3e50;
}

.tax-value.highlight {
  color: #e74c3c;
  font-size: 13px;
}

/* Badge */
.badge {
  display: inline-block;
  padding: 2px 6px;
  border-radius: 2px;
  font-size: 10px;
  font-weight: 500;
}

.badge-good { background: #d4edda; color: #155724; }
.badge-warning { background: #fff3cd; color: #856404; }
.badge-critical { background: #f8d7da; color: #721c24; }

/* Responsive */
@media (max-width: 1200px) {
  .stats-summary {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .reports-grid {
    grid-template-columns: 1fr;
  }
  
  .report-section.full-width {
    grid-column: span 1;
  }
  
  .tax-summary {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .stats-summary {
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
  
  .tax-summary {
    grid-template-columns: 1fr;
  }
}

@media print {
  .page-toolbar {
    display: none;
  }
}
</style>
