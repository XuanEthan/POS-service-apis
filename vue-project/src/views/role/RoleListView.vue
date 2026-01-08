<script setup>
import { ref, computed, onMounted } from 'vue'
import { getRoles, deleteRole, createRole, updateRole } from '@/services/roleService'
import { getUsers } from '@/services/userService'
import { getRolePermissions, createRolePermission } from '@/services/rolePermissionService'
import { getPermissions } from '@/services/permissionService'
import TreeView from '@/components/TreeView.vue'
import RoleModal from '@/components/RoleModal.vue'
import RolePermissionModal from '@/components/RolePermissionModal.vue'
import PermissionAlert from '@/components/PermissionAlert.vue'
import { canAccessModule, canDoAction } from '@/utils/auth'
import { MODULE_LABELS } from '@/constants/permissions'

const filterKeyword = ref('')
const loading = ref(false)
const error = ref('')

const roles = ref([])
const rolePermissionsFilter = {}
const rolePermissions = ref([])
const permissions = ref([])

const filteredRolePermissions = computed(() => {
  return rolePermissions.value
})
// Modal state
const showModal = ref(false)
const modalMode = ref('create') // 'create', 'edit', 'view'
const selectedRole = ref({})

// Modal phân quyền state
const showPermissionModal = ref(false)
const permissionModalMode = ref('edit')
const selectedRoleForPermission = ref({})

// Kiểm tra quyền truy cập module role (bất kỳ quyền nào)
const canAccessModule_role = computed(() => canAccessModule('role'))
const canAdd = computed(() => canDoAction('role', 'add'))
const canEdit = computed(() => canDoAction('role', 'edit'))
const canView = computed(() => canDoAction('role', 'view'))
const canDelete = computed(() => canDoAction('role', 'delete'))

// Fetch roles từ API
async function fetchRoles() {
  loading.value = true
  error.value = ''
  try {
    const response = await getRoles()
    if (response.isSuccess) {
      roles.value = response.object || []
    } else {
      if (response.code === 403) {
        error.value = `❌ Truy cập bị từ chối! Bạn không có quyền xem danh sách "${MODULE_LABELS.role || 'vai trò'}". Vui lòng liên hệ quản trị viên.`
        return
      }
      error.value = response.message || 'Không thể tải danh sách vai trò'
    }
  } catch (e) {
    error.value = 'Lỗi kết nối server'
  } finally {
    loading.value = false
  }
}

// Fetch permissions từ API
async function fetchPermissions() {
  try {
    const response = await getPermissions()
    if (response.isSuccess) {
      permissions.value = response.object || []
    }
  } catch (e) {
    console.error('Lỗi tải permissions:', e)
  }
}

// Mở modal thêm mới
function openCreateModal() {
  selectedRole.value = {}
  modalMode.value = 'create'
  showModal.value = true
}

// Mở modal sửa
function openEditModal(role) {
  selectedRole.value = { ...role }
  modalMode.value = 'edit'
  showModal.value = true
}

// Mở modal xem chi tiết
function openViewModal(role) {
  selectedRole.value = { ...role }
  modalMode.value = 'view'
  showModal.value = true
}

// Đóng modal
function closeModal() {
  showModal.value = false
  selectedRole.value = {}
}

// Lưu role (thêm mới hoặc cập nhật)
async function handleSaveRole(roleData) {
  try {
    let response
    if (modalMode.value === 'create') {
      response = await createRole(roleData)
    } else {
      response = await updateRole(roleData.roleId, roleData)
    }

    if (response.isSuccess) {
      alert(modalMode.value === 'create' ? 'Thêm mới thành công!' : 'Cập nhật thành công!')
      closeModal()
      await fetchRoles()
    } else {
      alert(response.message || 'Thao tác thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

// Xóa role - Kiểm tra ràng buộc trước khi xóa
async function handleDelete(node) {
  if (!confirm(`Bạn có chắc muốn xóa vai trò "${node.title}"?`)) return

  try {
    // Bước 1: Kiểm tra có User nào đang sử dụng Role này không
    const usersResponse = await getUsers()
    if (usersResponse.isSuccess) {
      const usersUsingRole = (usersResponse.object || []).filter(
        user => user.roleId === node.roleId
      )
      if (usersUsingRole.length > 0) {
        alert(`❌ Không thể xóa vai trò "${node.title}"!\n\n` +
          `Lý do: Có ${usersUsingRole.length} người dùng đang sử dụng vai trò này.\n\n` +
          `Danh sách: ${usersUsingRole.map(u => u.userName).slice(0, 5).join(', ')}` +
          (usersUsingRole.length > 5 ? '...' : '') +
          `\n\nVui lòng chuyển người dùng sang vai trò khác trước khi xóa.`)
        return
      }
    }

    // Bước 2: Kiểm tra có RolePermission nào đang sử dụng Role này không
    // const rolePermissionsResponse = await getRolePermissions()
    // if (rolePermissionsResponse.isSuccess) {
    //   const permissionsUsingRole = (rolePermissionsResponse.object || []).filter(
    //     rp => rp.roleId === node.roleId
    //   )
    //   if (permissionsUsingRole.length > 0) {
    //     alert(`❌ Không thể xóa vai trò "${node.title}"!\n\n` +
    //       `Lý do: Có ${permissionsUsingRole.length} phân quyền đang gắn với vai trò này.\n\n` +
    //       `Vui lòng xóa các phân quyền liên quan trước khi xóa vai trò.`)
    //     return
    //   }
    // }

    // Bước 3: Kiểm tra có Role con nào không (parentId = roleId hiện tại)
    const childRoles = roles.value.filter(r => r.parentId === node.roleId)
    if (childRoles.length > 0) {
      alert(`❌ Không thể xóa vai trò "${node.title}"!\n\n` +
        `Lý do: Có ${childRoles.length} vai trò con đang thuộc vai trò này.\n\n` +
        `Danh sách: ${childRoles.map(r => r.title).join(', ')}\n\n` +
        `Vui lòng xóa hoặc chuyển các vai trò con trước.`)
      return
    }

    // Bước 4: Không có ràng buộc, tiến hành xóa
    const response = await deleteRole(node.roleId)
    if (response.isSuccess) {
      alert('✅ Xóa thành công!')
      await fetchRoles()
    } else {
      alert(response.message || 'Xóa thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

// Sửa role
function handleEdit(node) {
  openEditModal(node)
}

// Xem chi tiết
function handleView(node) {
  openViewModal(node)
}

// Mở modal phân quyền
function openPermissionModal(role) {
  selectedRoleForPermission.value = { ...role }
  permissionModalMode.value = 'edit'
  showPermissionModal.value = true
}

// Đóng modal phân quyền
function closePermissionModal() {
  showPermissionModal.value = false
  selectedRoleForPermission.value = {}
}

// Lưu phân quyền
async function handleSaveRolePermission(data) {
  try {
    const response = await createRolePermission(data)
    if (response.isSuccess) {
      alert('Phân quyền thành công!')
      closePermissionModal()
    } else {
      alert(response.message || 'Phân quyền thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

// Load dữ liệu khi component mount
onMounted(() => {
  fetchRoles()
  fetchPermissions()
})

</script>

<template>
  <!-- Kiểm tra quyền - nếu không có quyền nào liên quan thì hiển thị thông báo -->
  <PermissionAlert :hasPermission="canAccessModule_role" />

  <div v-if="canAccessModule_role" class="page-container">
    <!-- Page Header & Toolbar -->
    <div class="page-header-toolbar">
      <div class="page-header">
        <h1 class="page-title">QUẢN LÝ VAI TRÒ</h1>
      </div>
      <div class="page-toolbar">
        <button v-if="canAdd" class="btn btn-primary" @click="openCreateModal"><span>+</span> Thêm mới</button>
        <!-- <button v-if="canAdd" class="btn btn-primary" @click="openCreateRolePermissionModal"><span>+</span> Phân quyền</button> -->
        <button class="btn btn-secondary" @click="fetchRoles">🔄 Tải lại</button>
      </div>
    </div>

    <div class="page-filters" style="display: flex; flex-wrap: nowrap; gap: 8px; align-items: center;">
      <div style="flex: 1;"></div>
      <div class="tree-stats" style="flex: 0 0 auto; font-size: 12px; color: #666;">
        Tổng: <strong>{{ roles.length }}</strong> vai trò
      </div>
    </div>

    <!-- Loading / Error -->
    <div v-if="loading" class="loading-indicator">
      <span>Đang tải dữ liệu...</span>
    </div>
    <div v-if="error" class="error-message">
      <span>{{ error }}</span>
      <button class="btn btn-sm btn-primary" @click="fetchRoles">Thử lại</button>
    </div>

    <!-- Tree View -->
    <div class="page-content" v-if="!loading && !error">
      <TreeView :items="roles" id-key="roleId" parent-key="parentId" label-key="title" code-key="code"
        :show-permission="canEdit"
        :show-edit="canEdit" :show-view="canView" :show-delete="canDelete" 
        @permission="openPermissionModal"
        @edit="handleEdit" @delete="handleDelete"
        @view="handleView"  />
    </div>

    <!-- Role Modal -->
    <RoleModal :visible="showModal" :mode="modalMode" :role="selectedRole" :roles="roles" @close="closeModal"
      @save="handleSaveRole" />

    <!-- Role Permission Modal -->
    <RolePermissionModal 
      :visible="showPermissionModal" 
      :mode="permissionModalMode" 
      :rolePermission="{ roleId: selectedRoleForPermission.roleId }"
      :roles="roles"
      :permissions="permissions"
      @close="closePermissionModal"
      @save="handleSaveRolePermission" 
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

.page-filters {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  background: #fff;
  border-bottom: 1px solid #e9ecef;
  gap: 20px;
}

.page-filters .input-group {
  flex: 1;
  max-width: 400px;
}

.tree-stats {
  font-size: 13px;
  color: #666;
}

.tree-stats strong {
  color: #333;
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

.page-content {
  flex: 1;
  overflow-y: scroll;
  /* height: 100%; */
}

/* Permissions Section */
.permissions-section {
  display: flex;
  flex-direction: column;
  background: #fff;
  border: 1px solid #dee2e6;
  height: 100%;
}

.permissions-header {
  padding: 12px 16px;
  background: #f8f9fa;
  border-bottom: 1px solid #dee2e6;
  min-height: 42px;
}

.permissions-title {
  margin: 0;
  font-size: 13px;
  font-weight: 600;
  color: #495057;
  display: flex;
  align-items: center;
  gap: 6px;
}

.permissions-title i {
  font-size: 12px;
  color: #0073ba;
}

.permissions-body {
  flex: 1;
  overflow-y: auto;
  padding: 12px 16px;
}

/* Empty State */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
  opacity: 0.5;
}

.empty-text {
  color: #6c757d;
  font-size: 13px;
  margin: 0;
}

/* Permissions wrapper */
.permissions-wrapper {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
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

/* Scrollbar Styling */
.permissions-body::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

.permissions-body::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.permissions-body::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 4px;
}

.permissions-body::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* Responsive Design */
@media (max-width: 1024px) {
  .page-content {
    grid-template-columns: 1fr;
    flex-direction: column;
  }

  .permissions-section {
    border-left: none;
    border-top: 1px solid #dee2e6;
    min-height: 300px;
  }
}
</style>
