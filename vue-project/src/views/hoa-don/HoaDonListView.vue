<script setup>
import { ref, computed } from 'vue'
import { canAccessModule } from '@/utils/auth'
import PermissionAlert from '@/components/PermissionAlert.vue'
import { MODULE_LABELS } from '@/constants/permissions'

// Kiểm tra quyền truy cập module hóa đơn (bất kỳ quyền nào: list, add, edit, delete)
const canAccessModule_hoadon = computed(() => canAccessModule('hoadon'))

const loading = ref(false)
const error = ref('')

const invoices = ref([
  { id: 'INV-001', customer: 'Cửa hàng A', date: '2025-11-05', due: '2025-12-05', amount: 980000, status: 'paid' },
  { id: 'INV-002', customer: 'Cửa hàng B', date: '2025-12-01', due: '2025-12-15', amount: 1250000, status: 'due' },
  { id: 'INV-003', customer: 'Cửa hàng C', date: '2025-10-25', due: '2025-11-25', amount: 450000, status: 'over' },
  { id: 'INV-004', customer: 'Cửa hàng D', date: '2025-09-02', due: '2025-09-30', amount: 230000, status: 'paid' },
  { id: 'INV-005', customer: 'Cửa hàng E', date: '2025-08-12', due: '2025-09-12', amount: 670000, status: 'due' }
])

const filterKeyword = ref('')
const filterStatus = ref('')
const filterProvince = ref('')
const filterDistrict = ref('')
const filterWard = ref('')
const checkAll = ref(false)
const perPage = ref(10)

const filteredInvoices = computed(() => {
  const q = filterKeyword.value.trim().toLowerCase()
  const st = filterStatus.value

  return invoices.value.filter(s => {
    const okQ = !q || s.id.toLowerCase().includes(q) || s.customer.toLowerCase().includes(q)
    const okS = (st === 'all' || st === '' || !st) ? true : s.status === st
    return okQ && okS
  })
})

function formatCurrency(n) {
  return n.toLocaleString('vi-VN') + ' ₫'
}

function getBadgeClass(status) {
  if (status === 'paid') return 'badge badge-success'
  if (status === 'due') return 'badge badge-warning'
  return 'badge badge-danger'
}

function getBadgeText(status) {
  if (status === 'paid') return 'Đã thanh toán'
  if (status === 'due') return 'Chưa thanh toán'
  return 'Quá hạn'
}

function handleCheckAll() {
  // Handle check all logic
}

// Tải lại danh sách hóa đơn
function fetchInvoices() {
  // Mô phỏng: trong thực tế sẽ gọi API
  // Ở đây chỉ reset error state
  error.value = ''
}
</script>

<template>
  <!-- Kiểm tra quyền - nếu không có quyền nào liên quan thì hiển thị thông báo -->
  <PermissionAlert :hasPermission="canAccessModule_hoadon" />

  <div v-if="canAccessModule_hoadon" class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">QUẢN LÝ HÓA ĐƠN</h1>
    </div>

    <!-- Toolbar -->
    <div class="page-toolbar">
      <button class="btn btn-primary"><span>+</span> Thêm mới</button>
      <button class="btn btn-secondary" @click="fetchInvoices">🔄 Tải lại</button>
      <!-- <button class="btn btn-danger">Xóa vĩnh viễn</button>
      <button class="btn btn-warning">Xóa tạm</button>
      <button class="btn btn-info">Khôi phục</button>
      <button class="btn btn-warning">Xuất danh sách</button>
      <button class="btn btn-success">Import từ file Excel</button> -->
    </div>

    <!-- Filters -->
    <div class="page-filters" style="display: flex; flex-wrap: nowrap; gap: 8px; align-items: center;">
      <select v-model="filterProvince" class="form-control" style="flex: 0 0 140px;">
        <option value="">-- Tỉnh/Thành phố --</option>
      </select>
      <select v-model="filterDistrict" class="form-control" style="flex: 0 0 140px;">
        <option value="">-- Quận/Huyện --</option>
      </select>
      <select v-model="filterWard" class="form-control" style="flex: 0 0 140px;">
        <option value="">-- Xã/Phường --</option>
      </select>
      <select v-model="filterStatus" class="form-control" style="flex: 0 0 140px;">
        <option value="">-- Chọn Trạng thái --</option>
        <option value="all">Tất cả</option>
        <option value="paid">Đã thanh toán</option>
        <option value="due">Chưa thanh toán</option>
        <option value="over">Quá hạn</option>
      </select>
      <input v-model="filterKeyword" class="form-control" style="flex: 0 0 250px;" placeholder="Nhập từ khóa tìm kiếm..." />
      <button class="btn btn-primary" style="flex: 0 0 auto; white-space: nowrap;">Tìm kiếm</button>
    </div>

    <!-- Table -->
    <div class="page-content">
      <!-- Loading -->
      <div v-if="loading" class="loading-indicator">
        <span>Đang tải dữ liệu...</span>
      </div>

      <!-- Error Message -->
      <div v-else-if="error" class="error-message">
        <span>{{ error }}</span>
        <button class="btn btn-sm btn-primary" @click="fetchInvoices">Thử lại</button>
      </div>

      <!-- Table Data -->
      <table v-else class="data-table">
        <thead>
          <tr>
            <th class="col-check"><input type="checkbox" v-model="checkAll" @change="handleCheckAll" /></th>
            <th class="col-stt">STT</th>
            <th>Mã hóa đơn</th>
            <th>Khách hàng</th>
            <th>Ngày lập</th>
            <th>Hạn TT</th>
            <th class="text-right">Số tiền</th>
            <th>Trạng thái</th>
            <th class="col-action">Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="filteredInvoices.length === 0">
            <td colspan="9" class="text-center">Không có dữ liệu</td>
          </tr>
          <tr v-for="(invoice, index) in filteredInvoices" :key="invoice.id">
            <td class="col-check"><input type="checkbox" /></td>
            <td class="col-stt">{{ index + 1 }}</td>
            <td>{{ invoice.id }}</td>
            <td>{{ invoice.customer }}</td>
            <td>{{ invoice.date }}</td>
            <td>{{ invoice.due }}</td>
            <td class="text-right">{{ formatCurrency(invoice.amount) }}</td>
            <td><span :class="getBadgeClass(invoice.status)">{{ getBadgeText(invoice.status) }}</span></td>
            <td class="col-action">
              <div class="dropdown">
                <button class="row-action-btn"><i class="fas fa-cog"></i></button>
                <div class="dropdown-menu">
                  <a class="dropdown-item"><i class="fas fa-edit"></i> Sửa</a>
                  <a class="dropdown-item"><i class="fas fa-eye"></i> Xem chi tiết</a>
                  <div class="dropdown-divider"></div>
                  <a class="dropdown-item"><i class="fas fa-trash"></i> Xóa</a>
                </div>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Table Footer -->
    <div class="table-footer">
      <div class="perpage">
        <label>Hiển thị</label>
        <select v-model="perPage">
          <option>10</option>
          <option>25</option>
          <option>50</option>
        </select>
        <span>Hiển thị 1 đến {{ filteredInvoices.length }} / {{ filteredInvoices.length }} bản ghi</span>
      </div>
      <div class="pagination">
        <button class="pg-btn">|&lt;</button>
        <button class="pg-btn">&lt;</button>
        <button class="pg-btn active">1</button>
        <button class="pg-btn">&gt;</button>
        <button class="pg-btn">&gt;|</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.loading-indicator {
  padding: 20px;
  text-align: center;
  color: #666;
}

.error-message {
  padding: 15px 20px;
  background: #ffe6e6;
  color: #c00;
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin: 10px 20px;
  border-radius: 4px;
}
</style>
