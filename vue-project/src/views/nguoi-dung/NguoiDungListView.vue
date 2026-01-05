<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { getUsers, deleteUser, createUser, updateUser } from '@/services/userService'
import { getRoles } from '@/services/roleService'
import UserModal from '@/components/UserModal.vue'
import PermissionAlert from '@/components/PermissionAlert.vue'
import { canAccessModule, canDoAction } from '@/utils/auth'
import { MODULE_LABELS, FEATURE_PERMISSIONS } from '@/constants/permissions'
import uuid from '@/utils/uuid'

const filterRole = ref('')
const filterStatus = ref('')
const filterKeyword = ref('')
const checkAll = ref(false)
const perPage = ref(10)
const currentPage = ref(1)
const loading = ref(false)
const error = ref('')

const users = ref([])
const roles = ref([])

// Server-side total count
const totalItemsServer = ref(0)
// Whether user has performed a search (only then fetch/paginate)
const hasSearched = ref(false)

// Modal state
const showModal = ref(false)
const modalMode = ref('create') // 'create', 'edit', 'view'
const selectedUser = ref({})

// Kiểm tra quyền truy cập module user (bất kỳ quyền nào)
const canAccessModule_user = computed(() => canAccessModule('user'))
const canAdd = computed(() => canDoAction('user', 'add'))
const canEdit = computed(() => canDoAction('user', 'edit'))
const canView = computed(() => canDoAction('user', 'view'))
const canDelete = computed(() => canDoAction('user', 'delete'))
const canSearch_user = computed(() => {
  const hasSearchAction = !!FEATURE_PERMISSIONS.user?.search
  return hasSearchAction ? canDoAction('user', 'search') : canDoAction('user', 'list')
})

// Fetch users từ API (server-side filtering & pagination)
async function fetchUsers() {
  loading.value = true
  error.value = ''
  try {
    const params = {
      Search: filterKeyword.value || '',
      RoleId: filterRole.value || uuid.EMPTY_GUID,
      StatusId: filterStatus.value || 0,
    }
    const response = await getUsers(params)
    // console.log('API response for getUsers:', params)
    if (response.isSuccess) {
      // Support both shapes: array or { items: [], totalCount }
      if (response.object && Array.isArray(response.object)) {
        users.value = response.object || []
        totalItemsServer.value = users.value.length
      } else if (response.object && response.object.items) {
        users.value = response.object.items || []
        totalItemsServer.value = response.object.totalCount || users.value.length
      } else {
        users.value = response.object || []
        totalItemsServer.value = users.value.length
      }
    } else {
      if (response.code === 403) {
        error.value = `❌ Truy cập bị từ chối! Bạn không có quyền xem danh sách "${MODULE_LABELS.user || 'người dùng'}". Vui lòng liên hệ quản trị viên.`
        return
      }
      error.value = response.message || 'Không thể tải danh sách người dùng'
    }
  } catch (e) {
    error.value = 'Lỗi kết nối server'
  } finally {
    loading.value = false
  }
}

// Fetch roles từ API
async function fetchRoles() {
  try {
    const response = await getRoles()
    if (response.isSuccess) {
      roles.value = response.object || []
    }
  } catch (e) {
    console.error('Lỗi tải danh sách vai trò:', e)
  }
}

// Mở modal thêm mới
function openCreateModal() {
  selectedUser.value = {}
  modalMode.value = 'create'
  showModal.value = true
}

// Mở modal sửa
function openEditModal(user) {
  selectedUser.value = { ...user }
  modalMode.value = 'edit'
  showModal.value = true
}

// Mở modal xem chi tiết
function openViewModal(user) {
  selectedUser.value = { ...user }
  modalMode.value = 'view'
  showModal.value = true
}

// Đóng modal
function closeModal() {
  showModal.value = false
  selectedUser.value = {}
}

// Lưu user (thêm mới hoặc cập nhật)
async function handleSaveUser(userData) {
  try {
    let response
    if (modalMode.value === 'create') {
      response = await createUser(userData)
    } else {
      response = await updateUser(userData.userId, userData)
    }

    if (response.isSuccess) {
      alert(modalMode.value === 'create' ? 'Thêm mới thành công!' : 'Cập nhật thành công!')
      closeModal()
      await fetchUsers()
    } else {
      alert(response.message || 'Thao tác thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

// Xóa user - User thường không có ràng buộc nên có thể xóa trực tiếp
async function handleDelete(userId) {
  // Tìm thông tin user để hiển thị
  const user = users.value.find(u => u.userId === userId)
  const userName = user ? user.userName : 'người dùng này'

  if (!confirm(`Bạn có chắc muốn xóa người dùng "${userName}"?\n\nHành động này không thể hoàn tác!`)) return

  try {
    const response = await deleteUser(userId)
    if (response.isSuccess) {
      alert('✅ Xóa thành công!')
      await fetchUsers()
    } else {
      alert(response.message || 'Xóa thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

// Helper: Lấy tên role từ roleId
function getRoleName(roleId) {
  const role = roles.value.find(r => r.roleId === roleId)
  return role ? role.title : 'N/A'
}

// Helper: Trạng thái người dùng
function getStatusText(statusId) {
  const s = statusId !== undefined && statusId !== null ? String(statusId) : ''
  switch (s) {
    case '1': return 'Đã kích hoạt'
    case '2': return 'Chưa kích hoạt'
    case '3': return 'Khóa'
    default: return 'Chưa kích hoạt'
  }
}

function statusBadgeClass(statusId) {
  const s = statusId !== undefined && statusId !== null ? String(statusId) : ''
  switch (s) {
    case '1': return 'badge-success'
    case '2': return 'badge-warning'
    case '3': return 'badge-danger'
    default: return 'badge-warning'
  }
}

// Server-side pagination computed values
const totalItems = computed(() => totalItemsServer.value)
const totalPages = computed(() => Math.max(1, Math.ceil(totalItems.value / Number(perPage.value || 1))))

// Ensure we refetch when page or pageSize change
watch([perPage, currentPage], () => {
  if (currentPage.value < 1) currentPage.value = 1
  if (currentPage.value > totalPages.value) currentPage.value = totalPages.value
  // Only fetch when user already initiated a search
  if (hasSearched.value) fetchUsers()
})

function prevPage() {
  if (currentPage.value > 1) currentPage.value--
}

function nextPage() {
  if (currentPage.value < totalPages.value) currentPage.value++
}

function goToFirst() { currentPage.value = 1 }
function goToLast() { currentPage.value = totalPages.value }

const pageStart = computed(() => {
  if (totalItems.value === 0) return 0
  return (Number(currentPage.value) - 1) * Number(perPage.value) + 1
})

const pageEnd = computed(() => Math.min(totalItems.value, Number(currentPage.value) * Number(perPage.value)))

function handleCheckAll() {
  // Handle check all logic
}

function handleSearch() {
  if (!canSearch_user.value) return
  currentPage.value = 1
  hasSearched.value = true
  fetchUsers()
}

// Load dữ liệu khi component mount
onMounted(() => {
  fetchUsers()
  fetchRoles()
})
</script>

<template>
  <!-- Kiểm tra quyền - nếu không có quyền nào liên quan thì hiển thị thông báo -->
  <PermissionAlert :hasPermission="canAccessModule_user" />

  <div v-if="canAccessModule_user" class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">QUẢN LÝ NGƯỜI DÙNG</h1>
    </div>

    <!-- Toolbar -->
    <div class="page-toolbar">
      <button v-if="canAdd" class="btn btn-primary" @click="openCreateModal"><span>+</span> Thêm mới</button>
      <button class="btn btn-secondary" @click="fetchUsers">🔄 Tải lại</button>
    </div>

    <!-- Filters -->
    <div v-if="canSearch_user" class="page-filters" style="display: flex; flex-wrap: nowrap; gap: 8px; align-items: center;">
      <select v-model="filterStatus" class="form-control" style="flex: 0 0 140px;">
        <option value="">-- Trạng thái --</option>
        <option value="1">Đã kích hoạt</option>
        <option value="2">Chưa kích hoạt</option>
        <option value="3">Khóa</option>
      </select>
      
      <select v-model="filterRole" class="form-control" style="flex: 0 0 140px;">
        <option value="">-- Tên vai trò --</option>
        <option v-for="role in roles" :key="role.roleId" :value="role.roleId">
          {{ role.title }}
        </option>
      </select>
      <input v-model="filterKeyword" class="form-control" style="flex: 0 0 250px;"
        placeholder="Tìm theo tên đăng nhập" @keyup.enter="handleSearch" />
      <button class="btn btn-primary" style="flex: 0 0 auto; white-space: nowrap;" @click="handleSearch">
        <i class="fas fa-search"></i> Tìm kiếm
      </button>
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
        <button class="btn btn-sm btn-primary" @click="fetchUsers">Thử lại</button>
      </div>

      <!-- Table Data -->
      <div v-else class="table-responsive">
        <table class="data-table">
          <thead>
            <tr>
              <th class="col-check"><input type="checkbox" v-model="checkAll" @change="handleCheckAll" /></th>
              <th class="col-stt">STT</th>
              <th class="col-action">Thao tác</th>
              <th>Tên đăng nhập</th>
              <th>Mật khẩu</th>
              <th>Vai trò</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="users.length === 0">
              <td colspan="7" class="text-center">Không có dữ liệu</td>
            </tr>
            <tr v-for="(user, index) in users" :key="user.userId">
              <td class="col-check"><input type="checkbox" /></td>
              <td class="col-stt">{{ (Number(currentPage) - 1) * Number(perPage) + index + 1 }}</td>
              <td class="col-action">
                <div class="dropdown" v-if="canEdit || canView || canDelete">
                  <button class="row-action-btn">⚙</button>
                  <div class="dropdown-menu">
                    <a v-if="canView" class="dropdown-item" @click="openViewModal(user)">👁️ Xem chi tiết</a>
                    <a v-if="canEdit" class="dropdown-item" @click="openEditModal(user)">✏️ Sửa</a>
                    <div v-if="canDelete && (canEdit || canView)" class="dropdown-divider"></div>
                    <a v-if="canDelete" class="dropdown-item" @click="handleDelete(user.userId)">🗑️ Xóa</a>
                  </div>
                </div>
              </td>
              <td>{{ user.userName }}</td>
              <td>********</td>
              <td>
                <span
                  :class="(user.roleTiTle === null || user.roleTiTle === '') ? 'badge badge-warning' : 'badge badge-info'">{{
                    user.roleTiTle || 'Chưa gán vai trò' }}</span>
              </td>
              <td>
                <span class="badge" :class="statusBadgeClass(user.statusId)">{{ getStatusText(user.statusId) }}</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Table Footer -->
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

    <!-- User Modal -->
    <UserModal :visible="showModal" :mode="modalMode" :user="selectedUser" :roles="roles" @close="closeModal"
      @save="handleSaveUser" />
  </div>
</template>

<style scoped>
.page-filters {
  grid-template-columns: repeat(4, 1fr);
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

.table-responsive {
  overflow-x: auto;
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
