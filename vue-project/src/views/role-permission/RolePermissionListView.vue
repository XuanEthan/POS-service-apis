<script setup>
import { ref, computed, onMounted } from 'vue'
import { getRolePermissions, deleteRolePermission } from '@/services/rolePermissionService'
import { getRoles } from '@/services/roleService'
import { getPermissions } from '@/services/permissionService'

const filterRole = ref('')
const filterPermission = ref('')
const filterStatus = ref('')
const checkAll = ref(false)
const perPage = ref(10)
const loading = ref(false)
const error = ref('')

const rolePermissions = ref([])
const roles = ref([])
const permissions = ref([])

// Fetch role permissions từ API
async function fetchRolePermissions() {
  loading.value = true
  error.value = ''
  try {
    const response = await getRolePermissions()
    if (response.isSuccess) {
      rolePermissions.value = response.object || []
    } else {
      error.value = response.message || 'Không thể tải danh sách phân quyền'
    }
  } catch (e) {
    error.value = 'Lỗi kết nối server'
  } finally {
    loading.value = false
  }
}

// Fetch roles
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

// Fetch permissions
async function fetchPermissions() {
  try {
    const response = await getPermissions()
    if (response.isSuccess) {
      permissions.value = response.object || []
    }
  } catch (e) {
    console.error('Lỗi tải danh sách quyền:', e)
  }
}

// Xóa role permission
async function handleDelete(rolePermissionId) {
  if (!confirm('Bạn có chắc muốn xóa phân quyền này?')) return
  
  try {
    const response = await deleteRolePermission(rolePermissionId)
    if (response.isSuccess) {
      alert('Xóa thành công!')
      await fetchRolePermissions()
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
  return role ? role.title : roleId
}

// Helper: Lấy tên permission từ permissionId
function getPermissionName(permissionId) {
  const permission = permissions.value.find(p => p.permissionId === permissionId)
  return permission ? permission.title : permissionId
}

// Helper: Lấy trạng thái
function getStatusText(statusId) {
  if (statusId === 1) return 'Hoạt động'
  if (statusId === 0) return 'Không hoạt động'
  return 'N/A'
}

function getStatusClass(statusId) {
  if (statusId === 1) return 'badge badge-success'
  if (statusId === 0) return 'badge badge-danger'
  return 'badge badge-secondary'
}

// Lọc role permissions
const filteredRolePermissions = computed(() => {
  return rolePermissions.value.filter(rp => {
    const okRole = !filterRole.value || rp.roleId === filterRole.value
    const okPermission = !filterPermission.value || rp.permissionId === filterPermission.value
    const okStatus = !filterStatus.value || String(rp.statusId) === filterStatus.value
    return okRole && okPermission && okStatus
  })
})

function handleCheckAll() {
  // Handle check all logic
}

function handleSearch() {
  // Trigger filter - computed sẽ tự động cập nhật
}

// Load dữ liệu khi component mount
onMounted(() => {
  fetchRolePermissions()
  fetchRoles()
  fetchPermissions()
})
</script>

<template>
  <div class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">QUẢN LÝ PHÂN QUYỀN</h1>
    </div>

    <!-- Toolbar -->
    <div class="page-toolbar">
      <button class="btn btn-primary"><span>+</span> Thêm phân quyền</button>
      <button class="btn btn-danger">Xóa vĩnh viễn</button>
      <button class="btn btn-warning">Xuất danh sách</button>
      <button class="btn btn-secondary" @click="fetchRolePermissions">🔄 Tải lại</button>
    </div>

    <!-- Filters -->
    <div class="page-filters">
      <select v-model="filterRole" class="form-control">
        <option value="">-- Chọn Vai trò --</option>
        <option v-for="role in roles" :key="role.roleId" :value="role.roleId">
          {{ role.title }}
        </option>
      </select>
      <select v-model="filterPermission" class="form-control">
        <option value="">-- Chọn Quyền --</option>
        <option v-for="permission in permissions" :key="permission.permissionId" :value="permission.permissionId">
          {{ permission.title }}
        </option>
      </select>
      <select v-model="filterStatus" class="form-control">
        <option value="">-- Chọn Trạng thái --</option>
        <option value="1">Hoạt động</option>
        <option value="0">Không hoạt động</option>
      </select>
      <button class="btn btn-primary" @click="handleSearch">Tìm kiếm</button>
    </div>

    <!-- Loading / Error -->
    <div v-if="loading" class="loading-indicator">
      <span>Đang tải dữ liệu...</span>
    </div>
    <div v-if="error" class="error-message">
      <span>{{ error }}</span>
      <button class="btn btn-sm btn-primary" @click="fetchRolePermissions">Thử lại</button>
    </div>

    <!-- Table -->
    <div class="page-content" v-if="!loading">
      <div class="table-responsive">
        <table class="data-table">
          <thead>
            <tr>
              <th class="col-check"><input type="checkbox" v-model="checkAll" @change="handleCheckAll" /></th>
              <th class="col-stt">STT</th>
              <th>Vai trò</th>
              <th>Quyền</th>
              <th>Trạng thái</th>
              <th>Đã xóa</th>
              <th class="col-action">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="filteredRolePermissions.length === 0">
              <td colspan="7" class="text-center">Không có dữ liệu</td>
            </tr>
            <tr v-for="(rp, index) in filteredRolePermissions" :key="rp.rolePermissionId">
              <td class="col-check"><input type="checkbox" /></td>
              <td class="col-stt">{{ index + 1 }}</td>
              <td><span class="badge badge-info">{{ getRoleName(rp.roleId) }}</span></td>
              <td><span class="badge badge-success">{{ getPermissionName(rp.permissionId) }}</span></td>
              <td><span :class="getStatusClass(rp.statusId)">{{ getStatusText(rp.statusId) }}</span></td>
              <td>
                <span v-if="rp.isDelete === 1" class="badge badge-danger">Đã xóa</span>
                <span v-else class="badge badge-success">Chưa xóa</span>
              </td>
              <td class="col-action">
                <div class="dropdown">
                  <button class="row-action-btn">⚙</button>
                  <div class="dropdown-menu">
                    <a class="dropdown-item">✏️ Sửa</a>
                    <a class="dropdown-item">👁️ Xem chi tiết</a>
                    <div class="dropdown-divider"></div>
                    <a class="dropdown-item" @click="handleDelete(rp.rolePermissionId)">🗑️ Xóa</a>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
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
        <span>Hiển thị 1 đến {{ filteredRolePermissions.length }} / {{ filteredRolePermissions.length }} bản ghi</span>
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
