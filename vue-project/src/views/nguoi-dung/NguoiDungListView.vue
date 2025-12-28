<script setup>
import { ref, computed, onMounted } from 'vue'
import { getUsers, deleteUser } from '@/services/userService'
import { getRoles } from '@/services/roleService'

const filterRole = ref('')
const filterKeyword = ref('')
const checkAll = ref(false)
const perPage = ref(10)
const loading = ref(false)
const error = ref('')

const users = ref([])
const roles = ref([])

// Fetch users từ API
async function fetchUsers() {
  loading.value = true
  error.value = ''
  try {
    const response = await getUsers()
    if (response.isSuccess) {
      users.value = response.object || []
    } else {
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

// Xóa user
async function handleDelete(userId) {
  if (!confirm('Bạn có chắc muốn xóa người dùng này?')) return
  
  try {
    const response = await deleteUser(userId)
    if (response.isSuccess) {
      alert('Xóa thành công!')
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
  return role ? role.title : (roleId || 'N/A')
}

// Lọc users theo model User: UserId, UserName, Password, RoleId, RoleCode
const filteredUsers = computed(() => {
  const q = filterKeyword.value.trim().toLowerCase()
  
  return users.value.filter(u => {
    const okQ = !q || 
      (u.userName && u.userName.toLowerCase().includes(q)) || 
      (u.roleCode && u.roleCode.toLowerCase().includes(q))
    const okRole = !filterRole.value || u.roleId === filterRole.value
    return okQ && okRole
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
  fetchUsers()
  fetchRoles()
})
</script>

<template>
  <div class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">QUẢN LÝ NGƯỜI DÙNG</h1>
    </div>

    <!-- Toolbar -->
    <div class="page-toolbar">
      <button class="btn btn-primary"><span>+</span> Thêm mới</button>
      <button class="btn btn-danger">Xóa vĩnh viễn</button>
      <button class="btn btn-warning">Xóa tạm</button>
      <button class="btn btn-info">Khôi phục</button>
      <button class="btn btn-warning">Xuất danh sách</button>
      <button class="btn btn-success">Import từ file Excel</button>
      <button class="btn btn-secondary" @click="fetchUsers">🔄 Tải lại</button>
    </div>

    <!-- Filters -->
    <div class="page-filters">
      <select v-model="filterRole" class="form-control">
        <option value="">-- Chọn Vai trò --</option>
        <option v-for="role in roles" :key="role.roleId" :value="role.roleId">
          {{ role.title }}
        </option>
      </select>
      <div class="input-group" style="grid-column: span 2;">
        <input v-model="filterKeyword" class="form-control" placeholder="Tìm kiếm theo tên đăng nhập hoặc mã vai trò..." @keyup.enter="handleSearch" />
        <button class="btn btn-primary" @click="handleSearch">Tìm kiếm</button>
      </div>
    </div>

    <!-- Loading / Error -->
    <div v-if="loading" class="loading-indicator">
      <span>Đang tải dữ liệu...</span>
    </div>
    <div v-if="error" class="error-message">
      <span>{{ error }}</span>
      <button class="btn btn-sm btn-primary" @click="fetchUsers">Thử lại</button>
    </div>

    <!-- Table -->
    <div class="page-content" v-if="!loading">
      <div class="table-responsive">
        <table class="data-table">
          <thead>
            <tr>
              <th class="col-check"><input type="checkbox" v-model="checkAll" @change="handleCheckAll" /></th>
              <th class="col-stt">STT</th>
              <th>Tên đăng nhập (UserName)</th>
              <th>Mật khẩu</th>
              <th>Vai trò (RoleCode)</th>
              <th class="col-action">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="filteredUsers.length === 0">
              <td colspan="6" class="text-center">Không có dữ liệu</td>
            </tr>
            <tr v-for="(user, index) in filteredUsers" :key="user.userId">
              <td class="col-check"><input type="checkbox" /></td>
              <td class="col-stt">{{ index + 1 }}</td>
              <td>{{ user.userName }}</td>
              <td>********</td>
              <td><span class="badge badge-info">{{ getRoleName(user.roleId) }}</span></td>
              <td class="col-action">
                <div class="dropdown">
                  <button class="row-action-btn">⚙</button>
                  <div class="dropdown-menu">
                    <a class="dropdown-item">✏️ Sửa</a>
                    <a class="dropdown-item">👁️ Xem chi tiết</a>
                    <div class="dropdown-divider"></div>
                    <a class="dropdown-item" @click="handleDelete(user.userId)">🗑️ Xóa</a>
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
        <span>Hiển thị 1 đến {{ filteredUsers.length }} / {{ filteredUsers.length }} bản ghi</span>
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
