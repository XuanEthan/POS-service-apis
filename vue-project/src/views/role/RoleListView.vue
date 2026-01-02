<script setup>
import { ref, computed, onMounted } from 'vue'
import { getRoles, deleteRole, createRole, updateRole } from '@/services/roleService'
import { getUsers } from '@/services/userService'
import { getRolePermissions } from '@/services/rolePermissionService'
import TreeView from '@/components/TreeView.vue'
import RoleModal from '@/components/RoleModal.vue'
import PermissionAlert from '@/components/PermissionAlert.vue'
import { canAccessModule, canDoAction } from '@/utils/auth'

const filterKeyword = ref('')
const loading = ref(false)
const error = ref('')

const roles = ref([])

// Modal state
const showModal = ref(false)
const modalMode = ref('create') // 'create', 'edit', 'view'
const selectedRole = ref({})

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
      error.value = response.message || 'Không thể tải danh sách vai trò'
    }
  } catch (e) {
    error.value = 'Lỗi kết nối server'
  } finally {
    loading.value = false
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
    const rolePermissionsResponse = await getRolePermissions()
    if (rolePermissionsResponse.isSuccess) {
      const permissionsUsingRole = (rolePermissionsResponse.object || []).filter(
        rp => rp.roleId === node.roleId
      )
      if (permissionsUsingRole.length > 0) {
        alert(`❌ Không thể xóa vai trò "${node.title}"!\n\n` +
              `Lý do: Có ${permissionsUsingRole.length} phân quyền đang gắn với vai trò này.\n\n` +
              `Vui lòng xóa các phân quyền liên quan trước khi xóa vai trò.`)
        return
      }
    }

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

// Handle selected nodes
function handleSelect(selectedIds) {
  console.log('Selected:', selectedIds)
}

// Load dữ liệu khi component mount
onMounted(() => {
  fetchRoles()
})
</script>

<template>
  <!-- Kiểm tra quyền - nếu không có quyền nào liên quan thì hiển thị thông báo -->
  <PermissionAlert :hasPermission="canAccessModule_role" />

  <div v-if="canAccessModule_role" class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">QUẢN LÝ VAI TRÒ</h1>
    </div>

    <!-- Toolbar -->
    <div class="page-toolbar">
      <button v-if="canAdd" class="btn btn-primary" @click="openCreateModal"><span>+</span> Thêm mới</button>
      <button class="btn btn-secondary" @click="fetchRoles">🔄 Tải lại</button>
    </div>

    <!-- Filters -->
    <div class="page-filters" style="display: flex; flex-wrap: nowrap; gap: 8px; align-items: center;">
      <input v-model="filterKeyword" class="form-control" style="flex: 1; max-width: 400px;" placeholder="Tìm theo mã hoặc tên vai trò..." />
      <button class="btn btn-primary" style="flex: 0 0 auto; white-space: nowrap;">
        <i class="fas fa-search"></i> Tìm kiếm
      </button>
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
    <div class="page-content" v-if="!loading">
      <TreeView
        :items="roles"
        id-key="roleId"
        parent-key="parentId"
        label-key="title"
        code-key="code"
        :show-edit="canEdit"
        :show-view="canView"
        :show-delete="canDelete"
        @edit="handleEdit"
        @delete="handleDelete"
        @view="handleView"
        @select="handleSelect"
      />
    </div>

    <!-- Role Modal -->
    <RoleModal
      :visible="showModal"
      :mode="modalMode"
      :role="selectedRole"
      :roles="roles"
      @close="closeModal"
      @save="handleSaveRole"
    />
  </div>
</template>

<style scoped>
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
  padding: 20px;
}
</style>
