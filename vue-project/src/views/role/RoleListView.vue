<script setup>
import { ref, onMounted } from 'vue'
import { getRoles, deleteRole } from '@/services/roleService'
import TreeView from '@/components/TreeView.vue'

const filterKeyword = ref('')
const loading = ref(false)
const error = ref('')

const roles = ref([])

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

// Xóa role
async function handleDelete(node) {
  if (!confirm(`Bạn có chắc muốn xóa vai trò "${node.title}"?`)) return
  
  try {
    const response = await deleteRole(node.roleId)
    if (response.isSuccess) {
      alert('Xóa thành công!')
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
  alert(`Sửa vai trò: ${node.title} (${node.code})`)
  // TODO: Open edit modal
}

// Xem chi tiết
function handleView(node) {
  alert(`Chi tiết vai trò:\n- ID: ${node.roleId}\n- Code: ${node.code}\n- Title: ${node.title}\n- Parent: ${node.parentId || 'N/A'}`)
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
  <div class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">QUẢN LÝ VAI TRÒ</h1>
    </div>

    <!-- Toolbar -->
    <div class="page-toolbar">
      <button class="btn btn-primary"><span>+</span> Thêm mới</button>
      <button class="btn btn-danger">Xóa đã chọn</button>
      <button class="btn btn-warning">Xuất danh sách</button>
      <button class="btn btn-secondary" @click="fetchRoles">🔄 Tải lại</button>
    </div>

    <!-- Filters -->
    <div class="page-filters">
      <div class="input-group">
        <input v-model="filterKeyword" class="form-control" placeholder="Tìm kiếm theo mã hoặc tên vai trò..." />
        <button class="btn btn-primary">Tìm kiếm</button>
      </div>
      <div class="tree-stats">
        <span>Tổng: <strong>{{ roles.length }}</strong> vai trò</span>
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
        @edit="handleEdit"
        @delete="handleDelete"
        @view="handleView"
        @select="handleSelect"
      />
    </div>
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
