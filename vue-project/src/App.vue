<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { RouterLink, RouterView, useRoute } from 'vue-router'
import { logout } from '@/services/authService'
import { getUser } from '@/utils/auth'

const route = useRoute()

// Kiểm tra có phải trang login không
const isLoginPage = computed(() => route.name === 'login')

// Lấy thông tin user
const currentUser = computed(() => getUser())

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
    <!-- Header Top -->
    <div class="header-top">
      <div>
        Thiết lập &nbsp;&nbsp; Server:
        <span style="color: orange"></span> Đã kết nối
      </div>
      <div id="clock">{{ currentTime }}</div>
      <div>
        CH: Dân Trí Soft &nbsp;&nbsp;&nbsp; Hello:
        <span>{{ currentUser?.userName || 'Guest' }}</span>
        <button class="logout-btn" title="Đăng xuất" @click="handleLogout">
          🚪 Đăng xuất
        </button>
      </div>
    </div>

    <!-- Main Container -->
    <div class="main-container">
      <!-- Sidebar -->
      <aside 
        class="app-sidebar" 
        :class="{ collapsed: sidebarCollapsed, expanded: !sidebarCollapsed }"
        aria-label="App functions"
      >
        <div class="sidebar-logo">POS</div>

        <div class="sidebar-group" data-group="sales">
          <div class="sidebar-group-title">Bán hàng</div>
          <RouterLink to="/" class="sidebar-item" title="Tổng quan">
            <span class="icon">🏠</span><span class="label">Tổng quan</span>
          </RouterLink>
          <RouterLink to="/hoa-don" class="sidebar-item" title="Hóa đơn">
            <span class="icon">🧾</span><span class="label">Hóa đơn</span>
          </RouterLink>
          <RouterLink to="/thanh-toan" class="sidebar-item" title="Thanh toán">
            <span class="icon">💳</span><span class="label">Thanh toán</span>
          </RouterLink>
          <div class="sidebar-item" title="Thống kê">
            <span class="icon">📊</span><span class="label">Thống kê</span>
          </div>
        </div>

        <div class="sidebar-group" data-group="admin">
          <div class="sidebar-group-title">Quản trị</div>
          <RouterLink to="/nguoi-dung" class="sidebar-item" title="Người dùng">
            <span class="icon">👤</span><span class="label">Người dùng</span>
          </RouterLink>
          <RouterLink to="/role" class="sidebar-item" title="Vai trò">
            <span class="icon">🧑‍💼</span><span class="label">Vai trò</span>
          </RouterLink>
          <RouterLink to="/permission" class="sidebar-item" title="Quyền">
            <span class="icon">🔐</span><span class="label">Quyền</span>
          </RouterLink>
          <RouterLink to="/role-permission" class="sidebar-item" title="Phân quyền">
            <span class="icon">🔑</span><span class="label">Phân quyền</span>
          </RouterLink>
          <div class="sidebar-item" title="Cài đặt">
            <span class="icon">⚙️</span><span class="label">Cài đặt</span>
          </div>
        </div>

        <button class="sidebar-toggle" @click="toggleSidebar" title="Thu/ Mở">
          {{ sidebarCollapsed ? '▶' : '◀' }}
        </button>
      </aside>

      <!-- Main Content -->
      <div id="content" class="content-area">
        <RouterView />
      </div>
    </div>
  </div>
</template>