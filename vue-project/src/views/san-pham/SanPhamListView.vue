<script setup>
import { ref, computed, onMounted } from 'vue'
import { getSanphams, getSanphamById, createSanpham, updateSanpham, deleteSanpham } from '@/services/sanphamService'
import SanPhamModal from '@/components/SanPhamModal.vue'
import PermissionAlert from '@/components/PermissionAlert.vue'
import { canAccessModule, canDoAction } from '@/utils/auth'
import { MODULE_LABELS } from '@/constants/permissions'

// State
const loading = ref(false)
const searchKeyword = ref('')
const filterActive = ref('')
const filterCategory = ref('')
const error = ref('')
const showModal = ref(false)
const modalMode = ref('create') // 'create', 'edit', 'view'
const selectedProduct = ref({})
const currentPage = ref(1)
const perPage = ref(10)

// Data from API
const products = ref([])

// Kiểm tra quyền truy cập module sản phẩm
const canAccessModule_sanpham = computed(() => canAccessModule('sanpham'))
const canAdd = computed(() => canDoAction('sanpham', 'add'))
const canEdit = computed(() => canDoAction('sanpham', 'edit'))
const canView = computed(() => canDoAction('sanpham', 'view'))
const canDelete = computed(() => canDoAction('sanpham', 'delete'))
const canList = computed(() => canDoAction('sanpham', 'list'))
const canSearch = computed(() => canDoAction('sanpham', 'search'))

// Fetch products from API
async function fetchSanphams() {
  loading.value = true
  error.value = ''
  try {
    const params = {
      Search: searchKeyword.value || ''
    }
    const response = await getSanphams(params)
    if (response.isSuccess) {
      products.value = response.object || []
    } else {
      if (response.code === 403) {
        error.value = `❌ Truy cập bị từ chối! Bạn không có quyền xem danh sách "${MODULE_LABELS.sanpham || 'sản phẩm'}". Vui lòng liên hệ quản trị viên.`
        return
      }
      error.value = response.message || 'Không thể tải danh sách sản phẩm'
    }
  } catch (e) {
    error.value = 'Lỗi kết nối server'
  } finally {
    loading.value = false
  }
}

// Computed
const filteredProducts = computed(() => {
  return products.value
})

const totalItems = computed(() => filteredProducts.value.length)
const totalPages = computed(() => Math.ceil(totalItems.value / perPage.value))
const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * perPage.value
  const end = start + perPage.value
  return filteredProducts.value.slice(start, end)
})

const pageStart = computed(() => {
  return totalItems.value === 0 ? 0 : (currentPage.value - 1) * perPage.value + 1
})

const pageEnd = computed(() => {
  return Math.min(currentPage.value * perPage.value, totalItems.value)
})

// Functions
function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value)
}

function formatNumber(value) {
  if (value === null || value === undefined) return 'N/A'
  return new Intl.NumberFormat('vi-VN').format(value)
}

function getStockBadgeClass(stock) {
  if (stock === null || stock === undefined) return 'badge-secondary'
  if (stock === 0) return 'badge-danger'
  if (stock < 10) return 'badge-warning'
  return 'badge-success'
}

function openCreateModal() {
  selectedProduct.value = {
    sanphamId: null,
    danhmucId: null,
    sku: '',
    name: '',
    barcode: '',
    price: 0,
    currency: 'VND',
    costPrice: 0,
    unitOfMeasure: 'pcs',
    stockOnHand: 0,
    active: null,
    createdAt: null,
    updatedAt: null,
  }
  modalMode.value = 'create'
  showModal.value = true
}

async function openEditModal(product) {
  if (!canEdit.value) {
    alert('Bạn không có quyền sửa sản phẩm')
    return
  }

  try {
    const response = await getSanphamById(product.sanphamId || product.SanphamId)
    if (response.isSuccess && response.object) {
      selectedProduct.value = { ...response.object }
      modalMode.value = 'edit'
      showModal.value = true
    } else {
      alert('Không thể tải thông tin sản phẩm')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

async function openViewModal(product) {
  if (!canView.value) {
    alert('Bạn không có quyền xem chi tiết sản phẩm')
    return
  }

  try {
    const response = await getSanphamById(product.sanphamId || product.SanphamId)
    if (response.isSuccess && response.object) {
      selectedProduct.value = { ...response.object }
      modalMode.value = 'view'
      showModal.value = true
    } else {
      alert('Không thể tải thông tin sản phẩm')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

function closeModal() {
  showModal.value = false
  selectedProduct.value = {}
}

async function handleSaveProduct(productData) {
  try {
    let response
    if (modalMode.value === 'create') {
      response = await createSanpham(productData)
    } else {
      response = await updateSanpham(productData.sanphamId, productData)
    }

    if (response.isSuccess) {
      alert(modalMode.value === 'create' ? 'Thêm mới thành công!' : 'Cập nhật thành công!')
      closeModal()
      await fetchSanphams()
    } else {
      alert(response.message || 'Thao tác thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

async function handleDelete(productId) {
  if (!confirm(`Bạn có chắc muốn xóa sản phẩm này?\n\nHành động này không thể hoàn tác!`)) return

  try {
    const response = await deleteSanpham(productId)
    if (response.isSuccess) {
      alert('✅ Xóa thành công!')
      await fetchSanphams()
    } else {
      alert(response.message || 'Xóa thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

function handleSearch() {
  currentPage.value = 1
  fetchSanphams()
}

function refreshData() {
  fetchSanphams()
}

function prevPage() {
  if (currentPage.value > 1) currentPage.value--
}

function nextPage() {
  if (currentPage.value < totalPages.value) currentPage.value++
}

function goToFirst() {
  currentPage.value = 1
}

function goToLast() {
  currentPage.value = totalPages.value
}

// Load data on mount
onMounted(() => {
  fetchSanphams()
})
</script>

<template>
  <div class="page-container">
    <!-- Permission Alert -->
    <PermissionAlert v-if="!canAccessModule_sanpham" :moduleName="MODULE_LABELS.sanpham" />

    <template v-else>
      <div class="page-header-toolbar">
        <div class="page-header">
          <h1 class="page-title"><i class="fas fa-box"></i> QUẢN LÝ SẢN PHẨM</h1>
        </div>
        <div class="page-toolbar">
          <div class="toolbar-left">
            <button v-if="canAdd" class="btn btn-primary" @click="openCreateModal"><span>+</span> Thêm mới</button>
            <button class="btn btn-secondary" @click="refreshData" :disabled="loading">🔄 {{ loading ? 'Đang tải...' :
              'Tải lại' }}</button>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div v-if="canSearch" class="page-filters">
        <div class="input-group">
          <input type="text" v-model="searchKeyword" placeholder="Nhập tên sản phẩm, SKU, Barcode ..."
            class="form-control" />
        </div>
        <div class="input-group" style="display: flex; align-items: flex-end;">
          <button class="btn btn-primary" @click="handleSearch" style="white-space: nowrap;"><i
              class="fas fa-search"></i> Tìm kiếm</button>
        </div>
      </div>

      <!-- Table -->
      <div class="table-wrapper">
        <!-- Loading -->
        <div v-if="loading" class="loading-indicator">
          <span>Đang tải dữ liệu...</span>
        </div>

        <!-- Error Message -->
        <div v-else-if="error" class="error-message">
          <span>{{ error }}</span>
          <button class="btn btn-sm btn-primary" @click="fetchSanphams">Thử lại</button>
        </div>

        <!-- Table Data -->
        <div v-else class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th style="width: 30px" class="col-stt">#</th>
                <th style="width: 100px">Mã sản phẩm</th>
                <th style="width: 180px">Danh mục</th>
                <th>Tên sản phẩm</th>
                <th style="width: 120px" class="text-right">Giá vốn</th>
                <th style="width: 100px" class="col-action">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="paginatedProducts.length === 0">
                <td colspan="7" class="text-center">Không có dữ liệu</td>
              </tr>
              <tr v-for="(product, index) in paginatedProducts" :key="product.sanphamId || product.Sku || index">
                <td class="col-stt">{{ (currentPage - 1) * perPage + index + 1 }}</td>
                <td class="sku-col">{{ product.Sku || product.sku }}</td>
                <td class="barcode-col">{{ '' }}</td>
                <td class="product-name">{{ product.Name || product.name }}</td>
                <td class="text-right price-col">{{ formatCurrency(product.CostPrice || product.costPrice || 0) }}
                </td>
                <td class="col-action">
                  <div class="dropdown" v-if="canEdit || canDelete || canView">
                    <button class="row-action-btn">⚙</button>
                    <div class="dropdown-menu">
                      <a v-if="canView" class="dropdown-item" @click="openViewModal(product)">👁️ Xem chi tiết</a>
                      <a v-if="canEdit" class="dropdown-item" @click="openEditModal(product)">✏️ Sửa</a>
                      <div v-if="canDelete && (canEdit || canView)" class="dropdown-divider"></div>
                      <a v-if="canDelete" class="dropdown-item text-danger" @click="handleDelete(product.sanphamId)">🗑️
                        Xóa</a>
                    </div>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Pagination -->
      <div class="table-footer" v-if="!error && !loading">
        <div class="perpage">
          <label>Hiển thị</label>
          <select v-model="perPage">
            <option :value="10">10</option>
            <option :value="25">25</option>
            <option :value="50">50</option>
          </select>
          <span>Hiển thị {{ pageStart }} đến {{ pageEnd }} / {{ totalItems }} bản ghi</span>
        </div>
        <div class="pagination">
          <button class="pg-btn" :disabled="currentPage <= 1" @click="goToFirst">|&lt;</button>
          <button class="pg-btn" :disabled="currentPage <= 1" @click="prevPage">&lt;</button>
          <button class="pg-btn active">{{ currentPage }} / {{ totalPages }}</button>
          <button class="pg-btn" :disabled="currentPage >= totalPages" @click="nextPage">&gt;</button>
          <button class="pg-btn" :disabled="currentPage >= totalPages" @click="goToLast">&gt;|</button>
        </div>
      </div>

      <!-- Modal Component -->
      <SanPhamModal :show="showModal" :mode="modalMode" :product="selectedProduct" @close="closeModal"
        @save="handleSaveProduct" />
    </template>
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

/* Page container with flex layout */
.page-container {
  display: flex;
  flex-direction: column;
  height: 100%;
}

/* Toolbar spacing */
.page-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 16px;
  background: #fff;
  gap: 12px;
  flex-shrink: 0;
}

.toolbar-left,
.toolbar-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

/* Table wrapper takes remaining space */
.table-wrapper {
  flex: 1;
  overflow-y: auto;
  min-height: 0;
}

.table-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-shrink: 0;
  background: #fff;
  border-top: 1px solid #e0e0e0;
  padding: 10px 16px;
}

.table-footer .perpage {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
}

.table-footer .perpage select {
  padding: 4px 6px;
  border-radius: 2px;
  border: 1px solid #ced4da;
}

.table-footer .pagination {
  display: flex;
  align-items: center;
  gap: 6px;
}

.pg-btn {
  padding: 6px 8px;
  border: 1px solid #dfe3e6;
  background: #fff;
  border-radius: 3px;
  cursor: pointer;
}

.pg-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-filters {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 12px;
  padding: 12px 16px;
  background: #fff;
  border-bottom: 1px solid #e0e0e0;
}

@media (max-width: 1200px) {
  .page-filters {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .page-filters {
    grid-template-columns: 1fr;
  }

  .page-filters .input-group {
    grid-column: span 1 !important;
  }

  .page-toolbar {
    flex-wrap: wrap;
  }
}

.sku-col {
  font-weight: 600;
  color: #3498db;
}

.product-name {
  font-weight: 500;
}

.barcode-col {
  font-family: monospace;
  font-size: 11px;
  color: #6c757d;
}

.price-col {
  font-weight: 600;
  color: #27ae60;
}

.text-danger {
  color: #e74c3c !important;
}

/* Fix dropdown actions */
.col-action {
  position: relative;
}

.col-action .dropdown {
  position: static;
}

.col-action .dropdown-menu {
  position: absolute;
  right: 10px;
  z-index: 9999;
}

.col-action .dropdown .dropdown-menu {
  display: none;
}

.col-action .dropdown:hover .dropdown-menu {
  display: block;
}
</style>
