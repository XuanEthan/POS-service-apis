<script setup>
import { ref, computed, onMounted } from 'vue'
import { canAccessModule, canDoAction } from '@/utils/auth'
import PermissionAlert from '@/components/PermissionAlert.vue'
import HoaDonModal from '@/components/HoaDonModal.vue'
import { MODULE_LABELS, FEATURE_PERMISSIONS } from '@/constants/permissions'
import * as hoaDonService from '@/services/hoaDonService'

// Kiểm tra quyền truy cập module hóa đơn (bất kỳ quyền nào: list, add, edit, delete)
const canAccessModule_hoadon = computed(() => canAccessModule('hoadon'))
const canSearch_hoadon = computed(() => {
  const hasSearchAction = !!FEATURE_PERMISSIONS.hoadon?.search
  return hasSearchAction ? canDoAction('hoadon', 'search') : canDoAction('hoadon', 'list')
})
const canAdd_hoadon = computed(() => canDoAction('hoadon', 'add'))
const canEdit_hoadon = computed(() => canDoAction('hoadon', 'edit'))
const canView_hoadon = computed(() => canDoAction('hoadon', 'view'))
const canDelete_hoadon = computed(() => canDoAction('hoadon', 'delete'))

const loading = ref(false)
const error = ref('')
const invoices = ref([])

const filterKeyword = ref('')
const filterStatus = ref('')
const filterProvince = ref('')
const filterDistrict = ref('')
const filterWard = ref('')
const checkAll = ref(false)
const perPage = ref(10)

// Modal state
const showModal = ref(false)
const modalMode = ref('create') // 'create', 'edit', 'view'
const currentInvoice = ref({
  invoiceNumber: '',
  customerName: '',
  customerPhone: '',
  customerAddress: '',
  invoiceDate: new Date().toISOString().split('T')[0],
  dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
  status: 'draft',
  taxRate: 10,
  discount: 0,
  notes: '',
  items: []
})

const filteredInvoices = computed(() => {
  const q = filterKeyword.value.trim().toLowerCase()
  const st = filterStatus.value

  return invoices.value.filter(s => {
    const okQ = !q || 
      s.invoiceNumber.toLowerCase().includes(q) || 
      s.customerName.toLowerCase().includes(q) ||
      (s.customerPhone && s.customerPhone.includes(q))
    const okS = (st === 'all' || st === '' || !st) ? true : s.status === st
    return okQ && okS
  })
})

function formatCurrency(n) {
  return n.toLocaleString('vi-VN') + ' ₫'
}

function getBadgeClass(status) {
  if (status === 'paid') return 'badge badge-success'
  if (status === 'pending') return 'badge badge-warning'
  if (status === 'overdue') return 'badge badge-danger'
  if (status === 'cancelled') return 'badge badge-secondary'
  return 'badge badge-info'
}

function getBadgeText(status) {
  if (status === 'paid') return 'Đã thanh toán'
  if (status === 'pending') return 'Chưa thanh toán'
  if (status === 'overdue') return 'Quá hạn'
  if (status === 'cancelled') return 'Đã hủy'
  if (status === 'draft') return 'Nháp'
  return status
}

function handleCheckAll() {
  // Handle check all logic
}

// Tải lại danh sách hóa đơn
async function fetchInvoices() {
  try {
    loading.value = true
    error.value = ''
    
    const result = await hoaDonService.getInvoices({
      keyword: filterKeyword.value,
      status: filterStatus.value
    })
    
    if (result.success) {
      invoices.value = result.data
    } else {
      error.value = result.message || 'Không thể tải danh sách hóa đơn'
    }
  } catch (err) {
    error.value = 'Đã xảy ra lỗi khi tải dữ liệu'
    console.error(err)
  } finally {
    loading.value = false
  }
}

// Mở modal thêm mới
function openCreateModal() {
  if (!canAdd_hoadon.value) {
    alert('Bạn không có quyền thêm hóa đơn')
    return
  }
  
  modalMode.value = 'create'
  currentInvoice.value = {
    invoiceNumber: '',
    customerName: '',
    customerPhone: '',
    customerAddress: '',
    invoiceDate: new Date().toISOString().split('T')[0],
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
    status: 'draft',
    taxRate: 10,
    discount: 0,
    notes: '',
    items: []
  }
  showModal.value = true
}

// Mở modal chỉnh sửa
function openEditModal(invoice) {
  if (!canEdit_hoadon.value) {
    alert('Bạn không có quyền sửa hóa đơn')
    return
  }
  
  modalMode.value = 'edit'
  currentInvoice.value = { ...invoice }
  showModal.value = true
}

// Mở modal xem chi tiết
function openViewModal(invoice) {
  if (!canView_hoadon.value) {
    alert('Bạn không có quyền xem chi tiết hóa đơn')
    return
  }
  
  modalMode.value = 'view'
  currentInvoice.value = { ...invoice }
  showModal.value = true
}

// Đóng modal
function closeModal() {
  showModal.value = false
}

// Lưu hóa đơn
async function saveInvoice(invoiceData) {
  try {
    loading.value = true
    
    let result
    if (modalMode.value === 'create') {
      result = await hoaDonService.createInvoice(invoiceData)
    } else {
      result = await hoaDonService.updateInvoice(currentInvoice.value.id, invoiceData)
    }
    
    if (result.success) {
      alert(result.message)
      showModal.value = false
      await fetchInvoices()
    } else {
      alert(result.message || 'Đã xảy ra lỗi')
    }
  } catch (err) {
    alert('Đã xảy ra lỗi: ' + err.message)
    console.error(err)
  } finally {
    loading.value = false
  }
}

// Xóa hóa đơn
async function deleteInvoice(invoice) {
  if (!canDelete_hoadon.value) {
    alert('Bạn không có quyền xóa hóa đơn')
    return
  }
  
  if (!confirm(`Bạn có chắc chắn muốn xóa hóa đơn ${invoice.invoiceNumber}?`)) {
    return
  }
  
  try {
    loading.value = true
    const result = await hoaDonService.deleteInvoice(invoice.id)
    
    if (result.success) {
      alert(result.message)
      await fetchInvoices()
    } else {
      alert(result.message || 'Đã xảy ra lỗi')
    }
  } catch (err) {
    alert('Đã xảy ra lỗi: ' + err.message)
    console.error(err)
  } finally {
    loading.value = false
  }
}

// Tải dữ liệu khi component mounted
onMounted(() => {
  fetchInvoices()
})
</script>

<template>
  <!-- Kiểm tra quyền - nếu không có quyền nào liên quan thì hiển thị thông báo -->
  <PermissionAlert :hasPermission="canAccessModule_hoadon" />

  <div v-if="canAccessModule_hoadon" class="page-container">
    <!-- Page Header & Toolbar -->
    <div class="page-header-toolbar">
      <div class="page-header">
        <h1 class="page-title">QUẢN LÝ HÓA ĐƠN</h1>
      </div>
      <div class="page-toolbar">
        <button v-if="canAdd_hoadon" class="btn btn-primary" @click="openCreateModal">
          <span>+</span> Thêm mới
        </button>
        <button class="btn btn-secondary" @click="fetchInvoices">🔄 Tải lại</button>
      </div>
    </div>

    <!-- Filters -->
    <div v-if="canSearch_hoadon" class="page-filters" style="display: flex; flex-wrap: nowrap; gap: 8px; align-items: center;">
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
        <option value="draft">Nháp</option>
        <option value="pending">Chưa thanh toán</option>
        <option value="paid">Đã thanh toán</option>
        <option value="overdue">Quá hạn</option>
        <option value="cancelled">Đã hủy</option>
      </select>
      <input v-model="filterKeyword" class="form-control" style="flex: 0 0 250px;" placeholder="Nhập từ khóa tìm kiếm..." />
      <button class="btn btn-primary" style="flex: 0 0 auto; white-space: nowrap;" @click="fetchInvoices">Tìm kiếm</button>
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
            <th class="col-action">Thao tác</th>
            <th>Mã hóa đơn</th>
            <th>Khách hàng</th>
            <th>Số điện thoại</th>
            <th>Ngày lập</th>
            <th>Hạn TT</th>
            <th class="text-right">Số tiền</th>
            <th>Trạng thái</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="filteredInvoices.length === 0">
            <td colspan="10" class="text-center">Không có dữ liệu</td>
          </tr>
          <tr v-for="(invoice, index) in filteredInvoices" :key="invoice.id">
            <td class="col-check"><input type="checkbox" /></td>
            <td class="col-stt">{{ index + 1 }}</td>
            <td class="col-action">
              <div class="dropdown">
                <button class="row-action-btn"><i class="fas fa-cog"></i></button>
                <div class="dropdown-menu">
                  <a v-if="canView_hoadon" class="dropdown-item" @click="openViewModal(invoice)"><i class="fas fa-eye"></i> Xem chi tiết</a>
                  <a v-if="canEdit_hoadon" class="dropdown-item" @click="openEditModal(invoice)"><i class="fas fa-edit"></i> Sửa</a>
                  <div v-if="canDelete_hoadon && (canEdit_hoadon || canView_hoadon)" class="dropdown-divider"></div>
                  <a v-if="canDelete_hoadon" class="dropdown-item" @click="deleteInvoice(invoice)"><i class="fas fa-trash"></i> Xóa</a>
                </div>
              </div>
            </td>
            <td>{{ invoice.invoiceNumber }}</td>
            <td>{{ invoice.customerName }}</td>
            <td>{{ invoice.customerPhone || '-' }}</td>
            <td>{{ invoice.invoiceDate }}</td>
            <td>{{ invoice.dueDate }}</td>
            <td class="text-right">{{ formatCurrency(invoice.totalAmount || 0) }}</td>
            <td><span :class="getBadgeClass(invoice.status)">{{ getBadgeText(invoice.status) }}</span></td>
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

    <!-- Modal Component -->
    <HoaDonModal
      :show="showModal"
      :mode="modalMode"
      :invoice="currentInvoice"
      @close="closeModal"
      @save="saveInvoice"
    />
  </div>
</template>

<style scoped>
/* Page Header & Toolbar on same line */
.page-header-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.page-header-toolbar .page-header {
  margin: 0;
}

.page-header-toolbar .page-toolbar {
  display: flex;
  gap: 10px;
}

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
