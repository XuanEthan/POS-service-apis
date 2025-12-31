<script setup>
import { ref, computed, onMounted } from 'vue'
import { getRolePermissions, deleteRolePermission, createRolePermission, updateRolePermission } from '@/services/rolePermissionService'
import { getRoles } from '@/services/roleService'
import { getPermissions } from '@/services/permissionService'
import RolePermissionModal from '@/components/RolePermissionModal.vue'
import { canDoAction } from '@/utils/auth'

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

// Modal state
const showModal = ref(false)
const modalMode = ref('create') // 'create', 'edit', 'view'
const selectedRolePermission = ref({})

// Kiểm tra quyền cho các action
const canAdd = computed(() => canDoAction('rolePermission', 'add'))
const canEdit = computed(() => canDoAction('rolePermission', 'edit'))
const canView = computed(() => canDoAction('rolePermission', 'view'))
const canDelete = computed(() => canDoAction('rolePermission', 'delete'))

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

// Mở modal thêm mới
function openCreateModal() {
  selectedRolePermission.value = {}
  modalMode.value = 'create'
  showModal.value = true
}

// Mở modal thêm mới cho Role cụ thể
function openCreateModalForRole(roleId) {
  selectedRolePermission.value = { roleId }
  modalMode.value = 'create'
  showModal.value = true
}

// Mở modal sửa
function openEditModal(rp) {
  selectedRolePermission.value = { ...rp }
  modalMode.value = 'edit'
  showModal.value = true
}

// Mở modal xem chi tiết
function openViewModal(rp) {
  selectedRolePermission.value = { ...rp }
  modalMode.value = 'view'
  showModal.value = true
}

// Đóng modal
function closeModal() {
  showModal.value = false
  selectedRolePermission.value = {}
}

// Lưu role permission (thêm mới hoặc cập nhật)
async function handleSaveRolePermission(data) {
  try {
    let response
    if (modalMode.value === 'create') {
      response = await createRolePermission(data)
    } else {
      response = await updateRolePermission(data.rolePermissionId, data)
    }

    if (response.isSuccess) {
      alert(modalMode.value === 'create' ? 'Thêm mới thành công!' : 'Cập nhật thành công!')
      closeModal()
      await fetchRolePermissions()
    } else {
      alert(response.message || 'Thao tác thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
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

// Gom permissions theo từng Role
const groupedByRole = computed(() => {
  const groups = {}

  filteredRolePermissions.value.forEach(rp => {
    if (!groups[rp.roleId]) {
      groups[rp.roleId] = {
        roleId: rp.roleId,
        roleName: getRoleName(rp.roleId),
        permissions: []
      }
    }
    groups[rp.roleId].permissions.push({
      ...rp,
      permissionName: getPermissionName(rp.permissionId)
    })
  })

  return Object.values(groups)
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
      <h1 class="page-title">PHÂN QUYỀN</h1>
    </div>

    <!-- Toolbar -->
    <div class="page-toolbar">
      <button v-if="canAdd" class="btn btn-primary" @click="openCreateModal"><span>+</span> Thêm phân quyền</button>
      <button class="btn btn-secondary" @click="fetchRolePermissions">🔄 Tải lại</button>
    </div>

    <!-- Filters -->
    <div class="page-filters" style="display: flex; flex-wrap: nowrap; gap: 8px; align-items: center;">
      <select v-model="filterRole" class="form-control" style="flex: 1; min-width: 150px;">
        <option value="">-- Chọn Vai trò --</option>
        <option v-for="role in roles" :key="role.roleId" :value="role.roleId">
          {{ role.title }}
        </option>
      </select>
      <select v-model="filterPermission" class="form-control" style="flex: 1; min-width: 150px;">
        <option value="">-- Chọn Quyền --</option>
        <option v-for="permission in permissions" :key="permission.permissionId" :value="permission.permissionId">
          {{ permission.title }}
        </option>
      </select>
      <select v-model="filterStatus" class="form-control" style="flex: 0 0 140px;">
        <option value="">-- Trạng thái --</option>
        <option value="1">Hoạt động</option>
        <option value="0">Không hoạt động</option>
      </select>
      <button class="btn btn-primary" style="flex: 0 0 auto; white-space: nowrap;" @click="handleSearch">
        <i class="fas fa-search"></i> Tìm kiếm
      </button>
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
              <th class="col-stt">STT</th>
              <th style="width: 180px;">Vai trò</th>
              <th>Danh sách quyền</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="groupedByRole.length === 0">
              <td colspan="4" class="text-center">Không có dữ liệu</td>
            </tr>
            <tr v-for="(group, index) in groupedByRole" :key="group.roleId">
              <td class="col-stt">{{ index + 1 }}</td>
              <td>
                <span class="badge badge-info role-badge">{{ group.roleName }}</span>
                <div class="permission-count">{{ group.permissions.length }} quyền</div>
              </td>
              <td class="permissions-cell">
                <div class="permissions-wrapper">
                  <span v-for="perm in group.permissions" :key="perm.rolePermissionId" class="permission-tag">
                    {{ perm.permissionName }}
                    <button v-if="canDelete" class="remove-perm-btn" @click.stop="handleDelete(perm.rolePermissionId)"
                      title="Xóa quyền này">×</button>
                  </span>
                </div>
              </td>
              <!-- <td class="col-action">
                <button 
                  v-if="canAdd" 
                  class="btn btn-sm btn-primary" 
                  @click="openCreateModalForRole(group.roleId)"
                  title="Thêm quyền cho vai trò này"
                >
                  + Thêm
                </button>
              </td> -->
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Table Footer -->
    <div class="table-footer">
      <div class="perpage">
        <span>Tổng: {{ groupedByRole.length }} vai trò với {{ filteredRolePermissions.length }} phân quyền</span>
      </div>
    </div>

    <!-- Role Permission Modal -->
    <RolePermissionModal :visible="showModal" :mode="modalMode" :rolePermission="selectedRolePermission" :roles="roles"
      :permissions="permissions" @close="closeModal" @save="handleSaveRolePermission" />
  </div>
</template>

<style scoped>
.page-filters {
  display: flex;
  gap: 10px;
  padding: 15px 20px;
  background: #fff;
  border-bottom: 1px solid #e0e0e0;
  flex-wrap: wrap;
}

.page-filters .form-control {
  min-width: 180px;
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

/* Role badge styling */
.role-badge {
  font-size: 13px;
  padding: 6px 12px;
}

.permission-count {
  font-size: 11px;
  color: #7f8c8d;
  margin-top: 4px;
}

/* Permissions cell */
.permissions-cell {
  padding: 10px 15px !important;
}

.permissions-wrapper {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.permission-tag {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  background: linear-gradient(135deg, #27ae60, #2ecc71);
  color: white;
  padding: 4px 10px;
  border-radius: 15px;
  font-size: 12px;
  font-weight: 500;
  transition: all 0.2s;
}

.permission-tag:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 6px rgba(39, 174, 96, 0.3);
}

.permission-tag.inactive {
  background: linear-gradient(135deg, #95a5a6, #7f8c8d);
  opacity: 0.7;
}

.remove-perm-btn {
  background: rgba(255, 255, 255, 0.3);
  border: none;
  color: white;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  font-size: 14px;
  line-height: 1;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: 2px;
  padding: 0;
  transition: all 0.2s;
}

.remove-perm-btn:hover {
  background: rgba(255, 255, 255, 0.5);
  transform: scale(1.1);
}

.btn-sm {
  padding: 5px 10px;
  font-size: 12px;
}

.table-footer {
  padding: 15px 20px;
  background: #f8f9fa;
  border-top: 1px solid #e0e0e0;
}

.table-footer .perpage {
  color: #666;
  font-size: 13px;
}

@media (max-width: 768px) {
  .page-filters {
    flex-direction: column;
  }

  .page-filters .form-control {
    width: 100%;
  }
}
</style>
