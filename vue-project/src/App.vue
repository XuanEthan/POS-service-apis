<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { RouterLink, RouterView, useRoute } from 'vue-router'
import { logout } from '@/services/authService'
import { getUser, getUserPermissions, isAdmin, PERMISSIONS, ROUTE_PERMISSIONS , getUserName } from '@/utils/auth'

const route = useRoute()

// Kiểm tra có phải trang login không
const isLoginPage = computed(() => route.name === 'login')

// Reactive state cho permissions - cập nhật khi route thay đổi
const permissionsKey = ref(0)

// Force re-check permissions khi route thay đổi
watch(() => route.path, () => {
  permissionsKey.value++
})

// Lấy thông tin user (reactive)
const currentUser = computed(() => {
  permissionsKey.value // dependency để force re-compute
  return getUser()
})

// Kiểm tra quyền admin (bypass tất cả)
const userIsAdmin = computed(() => {
  permissionsKey.value // dependency để force re-compute
  return isAdmin()
})

// Lấy danh sách permissions của user (reactive)
const userPermissions = computed(() => {
  permissionsKey.value // dependency để force re-compute
  return getUserPermissions()
})

// Hàm kiểm tra quyền truy cập route (reactive version)
function checkRouteAccess(routePath) {
  if (userIsAdmin.value) return true
  
  const requiredPermissions = ROUTE_PERMISSIONS[routePath]
  if (!requiredPermissions) return true // Route không yêu cầu quyền
  
  return requiredPermissions.some(permId => userPermissions.value.includes(permId))
}

// Kiểm tra quyền cho từng menu item (reactive)
const menuPermissions = computed(() => ({
  // Bán hàng
  sanpham: checkRouteAccess('/san-pham'),
  hoadon: checkRouteAccess('/hoa-don'),
  orderThuchi: checkRouteAccess('/ban-hang'),
  thongke: checkRouteAccess('/reports'),
  // Quản trị
  nguoidung: checkRouteAccess('/nguoi-dung'),
  role: checkRouteAccess('/role'),
  permission: checkRouteAccess('/permission'),
  rolePermission: checkRouteAccess('/role-permission'),
}))

// Kiểm tra có hiển thị group không (ẩn group nếu không có menu item nào)
// Tổng quan luôn hiển thị nên group Bán hàng luôn hiện
const showSalesGroup = computed(() => true)

const showAdminGroup = computed(() => 
  menuPermissions.value.nguoidung || 
  menuPermissions.value.role || 
  menuPermissions.value.permission || 
  menuPermissions.value.rolePermission
)

const currentTime = ref('')
const sidebarCollapsed = ref(false)

function updateTime() {
  const now = new Date()
  const dateStr = now.toLocaleDateString('vi-VN')
  const timeStr = now.toLocaleTimeString('vi-VN', {
    hour: '2-digit',
    minute: '2-digit'
  })
  currentTime.value = `Cur: ${dateStr} - ${timeStr}`
}

function toggleSidebar() {
  sidebarCollapsed.value = !sidebarCollapsed.value
  localStorage.setItem('sidebarCollapsed', sidebarCollapsed.value ? '1' : '0')
}

function handleLogout() {
  logout()
  window.location.href = '/login'
}

let intervalId = null

onMounted(() => {
  updateTime()
  intervalId = setInterval(updateTime, 1000)
  
  // Load saved state
  const saved = localStorage.getItem('sidebarCollapsed') === '1'
  sidebarCollapsed.value = saved
})

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId)
  }
})
</script>

<template>
  <!-- Trang Login: không có layout -->
  <RouterView v-if="isLoginPage" />
  
  <!-- Các trang khác: có layout đầy đủ -->
  <div v-else class="app-wrapper">
    <!-- Top Navbar -->
    <nav class="top-navbar">
      <!-- Logo -->
      <div class="navbar-brand">
        <i class="fas fa-cash-register brand-icon"></i>
        <span class="brand-text">POS</span>
      </div>

      <!-- Menu nghiệp vụ chính (bên trái) -->
      <div class="navbar-menu-left" v-if="showSalesGroup">
        <RouterLink to="/" class="nav-item" title="Tổng quan">
          <i class="fas fa-tachometer-alt nav-icon"></i>
          <span class="nav-label">Tổng quan</span>
        </RouterLink>
        <RouterLink v-if="menuPermissions.sanpham" to="/san-pham" class="nav-item" title="Sản phẩm">
          <i class="fas fa-box nav-icon"></i>
          <span class="nav-label">Sản phẩm</span>
        </RouterLink>
        <RouterLink v-if="menuPermissions.hoadon" to="/hoa-don" class="nav-item" title="Hóa đơn">
          <i class="fas fa-file-invoice nav-icon"></i>
          <span class="nav-label">Hóa đơn</span>
        </RouterLink>
        <RouterLink v-if="menuPermissions.orderThuchi" to="/ban-hang" class="nav-item" title="Quản lý bán hàng">
          <i class="fas fa-credit-card nav-icon"></i>
          <span class="nav-label">Quản lý bán hàng</span>
        </RouterLink>
        <RouterLink v-if="menuPermissions.thongke" to="/reports" class="nav-item" title="Báo cáo">
          <i class="fas fa-chart-bar nav-icon"></i>
          <span class="nav-label">Báo cáo</span>
        </RouterLink>
      </div>

      <!-- Spacer -->
      <div class="navbar-spacer"></div>
      <!-- Menu quản trị (bên phải) - Dropdown -->
      <div class="navbar-menu-right" v-if="showAdminGroup">
        <div class="nav-dropdown">
          <button class="nav-dropdown-toggle">
            <i class="fas fa-cog nav-icon"></i>
          </button>
          <div class="nav-dropdown-menu">
            <RouterLink v-if="menuPermissions.nguoidung" to="/nguoi-dung" class="dropdown-item">
              <i class="fas fa-users dropdown-icon"></i>
              <span>Người dùng</span>
            </RouterLink>
            <RouterLink v-if="menuPermissions.role" to="/role" class="dropdown-item">
              <i class="fas fa-user-tag dropdown-icon"></i>
              <span>Vai trò</span>
            </RouterLink>
            <!-- <RouterLink v-if="menuPermissions.permission" to="/permission" class="dropdown-item">
              <i class="fas fa-shield-alt dropdown-icon"></i>
              <span>Quyền</span>
            </RouterLink>
            <RouterLink v-if="menuPermissions.rolePermission" to="/role-permission" class="dropdown-item">
              <i class="fas fa-key dropdown-icon"></i>
              <span>Phân quyền</span>
            </RouterLink> -->
            <div class="dropdown-divider"></div>
            <div class="dropdown-item">
              <i class="fas fa-sliders-h dropdown-icon"></i>
              <span>Cài đặt</span>
            </div>
          </div>
        </div>
      </div>

      <!-- User info & Logout -->
      <div class="navbar-user">
        <i class="fas fa-user-circle user-avatar"></i>
        <span class="user-name">{{ getUserName() || 'Guest' }}</span>
        <button class="logout-btn" title="Đăng xuất" @click="handleLogout">
          <i class="fas fa-sign-out-alt"></i>
        </button>
      </div>
    </nav>

    <!-- Main Container -->
    <div class="main-container">
      <!-- Main Content -->
      <div id="content" class="content-area">
        <RouterView />
      </div>
    </div>
  </div>
</template>