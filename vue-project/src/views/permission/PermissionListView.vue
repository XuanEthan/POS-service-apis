<script setup>
import { ref, computed, onMounted } from 'vue'
import { getPermissions, deletePermission } from '@/services/permissionService'
import TreeView from '@/components/TreeView.vue'
import PermissionAlert from '@/components/PermissionAlert.vue'
import { canAccessModule } from '@/utils/auth'
import { MODULE_LABELS } from '@/constants/permissions'

const filterKeyword = ref('')
const loading = ref(false)
const error = ref('')

const permissions = ref([])

// Kiểm tra quyền truy cập module permission (bất kỳ quyền nào)
const canAccessModule_permission = computed(() => canAccessModule('permission'))

// Fetch permissions từ API
async function fetchPermissions() {
  loading.value = true
  error.value = ''
  try {
    const response = await getPermissions()
    if (response.isSuccess) {
      permissions.value = response.object || []
    } else {
      if(response.code === 403){
        error.value = `❌ Truy cập bị từ chối! Bạn không có quyền xem danh sách "${MODULE_LABELS.permission || 'quyền'}". Vui lòng liên hệ quản trị viên.`
        return
      }
      error.value = response.message || 'Không thể tải danh sách quyền'
    }
  } catch (e) {
    error.value = 'Lỗi kết nối server'
  } finally {
    loading.value = false
  }
}

// Xóa permission
async function handleDelete(node) {
  if (!confirm(`Bạn có chắc muốn xóa quyền "${node.title}"?`)) return
  
  try {
    const response = await deletePermission(node.permissionId)
    if (response.isSuccess) {
      alert('Xóa thành công!')
      await fetchPermissions()
    } else {
      alert(response.message || 'Xóa thất bại!')
    }
  } catch (e) {
    alert('Lỗi kết nối server')
  }
}

// Sửa permission
function handleEdit(node) {
  alert(`Sửa quyền: ${node.title} (${node.code})`)
  // TODO: Open edit modal
}

// Xem chi tiết
function handleView(node) {
  alert(`Chi tiết quyền:\n- ID: ${node.permissionId}\n- Code: ${node.code}\n- Title: ${node.title}\n- Parent: ${node.parentId || 'N/A'}`)
}

// Handle selected nodes
function handleSelect(selectedIds) {
  console.log('Selected:', selectedIds)
}

// Load dữ liệu khi component mount
onMounted(() => {
  fetchPermissions()
})
</script>

<template>
  <!-- Kiểm tra quyền - nếu không có quyền nào liên quan thì hiển thị thông báo -->
  <PermissionAlert :hasPermission="canAccessModule_permission" />

  <div v-if="canAccessModule_permission" class="page-container">
    <!-- Page Header & Toolbar -->
    <div class="page-header-toolbar">
      <div class="page-header">
        <h1 class="page-title">QUẢN LÝ QUYỀN</h1>
      </div>
      <div class="tree-stats" style="flex: 0 0 auto; font-size: 12px; color: #666;">
        Tổng: <strong>{{ permissions.length }}</strong> quyền
      </div>
    </div>


    <!-- Loading / Error -->
    <div v-if="loading" class="loading-indicator">
      <span>Đang tải dữ liệu...</span>
    </div>
    <div v-if="error" class="error-message">
      <span>{{ error }}</span>
      <button class="btn btn-sm btn-primary" @click="fetchPermissions">Thử lại</button>
    </div>

    <!-- Tree View -->
    <div class="page-content" v-if="!loading && !error">
      <TreeView
        :items="permissions"
        id-key="permissionId"
        parent-key="parentId"
        label-key="title"
        code-key="code"
        :show-edit="false"
        :show-view="true"
        :show-delete="false"
        @edit="handleEdit"
        @delete="handleDelete"
        @view="handleView"
        @select="handleSelect"
      />
    </div>
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
  border-radius: 2px;
}

.page-content {
  padding: 20px;
}
</style>
