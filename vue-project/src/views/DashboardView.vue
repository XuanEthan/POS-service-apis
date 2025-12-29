<script setup>
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import { getUserPermissions, isAdmin, ROUTE_PERMISSIONS } from '@/utils/auth'

// Kiểm tra quyền admin
const userIsAdmin = computed(() => isAdmin())

// Lấy permissions của user
const userPermissions = computed(() => getUserPermissions())

// Hàm kiểm tra quyền truy cập route
function checkRouteAccess(routePath) {
  if (userIsAdmin.value) return true
  
  const requiredPermissions = ROUTE_PERMISSIONS[routePath]
  if (!requiredPermissions) return true
  
  return requiredPermissions.some(permId => userPermissions.value.includes(permId))
}

// Kiểm tra quyền cho từng quick action
const canAccessThanhToan = computed(() => checkRouteAccess('/thanh-toan'))
const canAccessHoaDon = computed(() => checkRouteAccess('/hoa-don'))
const canAccessNguoiDung = computed(() => checkRouteAccess('/nguoi-dung'))
const canAccessRole = computed(() => checkRouteAccess('/role'))
const canAccessPermission = computed(() => checkRouteAccess('/permission'))
const canAccessRolePermission = computed(() => checkRouteAccess('/role-permission'))

// Kiểm tra có quick action nào để hiển thị không
const hasAnyQuickAction = computed(() => 
  canAccessThanhToan.value || 
  canAccessHoaDon.value || 
  canAccessNguoiDung.value || 
  canAccessRole.value || 
  canAccessPermission.value || 
  canAccessRolePermission.value
)
</script>

<template>
  <div class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">TỔNG QUAN</h1>
      <span class="page-subtitle">Chào mừng bạn đến với hệ thống POS</span>
    </div>

    <!-- Content -->
    <div class="dashboard-content">
      <!-- Stats Grid -->
      <div class="stats-grid">
        <div class="stat-card stat-blue">
          <div class="stat-icon"><i class="fas fa-file-invoice"></i></div>
          <div class="stat-info">
            <div class="stat-value">156</div>
            <div class="stat-label">Hóa đơn hôm nay</div>
          </div>
        </div>

        <div class="stat-card stat-green">
          <div class="stat-icon"><i class="fas fa-coins"></i></div>
          <div class="stat-info">
            <div class="stat-value">12,500,000 ₫</div>
            <div class="stat-label">Doanh thu hôm nay</div>
          </div>
        </div>

        <div class="stat-card stat-orange">
          <div class="stat-icon"><i class="fas fa-users"></i></div>
          <div class="stat-info">
            <div class="stat-value">48</div>
            <div class="stat-label">Khách hàng</div>
          </div>
        </div>

        <div class="stat-card stat-purple">
          <div class="stat-icon"><i class="fas fa-box"></i></div>
          <div class="stat-info">
            <div class="stat-value">234</div>
            <div class="stat-label">Sản phẩm đã bán</div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="quick-actions-section" v-if="hasAnyQuickAction">
        <h2 class="section-title">Truy cập nhanh</h2>
        <div class="actions-grid">
          <RouterLink v-if="canAccessThanhToan" to="/thanh-toan" class="action-card action-primary">
            <i class="fas fa-credit-card action-icon"></i>
            <span class="action-text">Order/Tính tiền</span>
          </RouterLink>
          <RouterLink v-if="canAccessHoaDon" to="/hoa-don" class="action-card action-info">
            <i class="fas fa-file-invoice action-icon"></i>
            <span class="action-text">Hóa đơn</span>
          </RouterLink>
          <RouterLink v-if="canAccessNguoiDung" to="/nguoi-dung" class="action-card action-success">
            <i class="fas fa-user action-icon"></i>
            <span class="action-text">Người dùng</span>
          </RouterLink>
          <RouterLink v-if="canAccessRole" to="/role" class="action-card action-warning">
            <i class="fas fa-user-tag action-icon"></i>
            <span class="action-text">Vai trò</span>
          </RouterLink>
          <RouterLink v-if="canAccessPermission" to="/permission" class="action-card action-purple">
            <i class="fas fa-key action-icon"></i>
            <span class="action-text">Quyền</span>
          </RouterLink>
          <RouterLink v-if="canAccessRolePermission" to="/role-permission" class="action-card action-secondary">
            <i class="fas fa-shield-alt action-icon"></i>
            <span class="action-text">Phân quyền</span>
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard-content {
  padding: 16px;
}

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 12px;
  margin-bottom: 20px;
}

.stat-card {
  background: white;
  border-radius: 2px;
  padding: 12px;
  display: flex;
  align-items: center;
  gap: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  transition: box-shadow 0.2s;
  border-left: 3px solid transparent;
}

.stat-card:hover {
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.12);
}

.stat-blue { border-left-color: #3498db; }
.stat-green { border-left-color: #27ae60; }
.stat-orange { border-left-color: #f39c12; }
.stat-purple { border-left-color: #9b59b6; }

.stat-icon {
  font-size: 20px;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 2px;
  background: #f8f9fa;
}

.stat-blue .stat-icon { background: #ebf5fb; color: #3498db; }
.stat-green .stat-icon { background: #e8f6ef; color: #27ae60; }
.stat-orange .stat-icon { background: #fef5e7; color: #f39c12; }
.stat-purple .stat-icon { background: #f5eef8; color: #9b59b6; }

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 16px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 2px;
}

.stat-label {
  font-size: 11px;
  color: #7f8c8d;
}

/* Quick Actions */
.quick-actions-section {
  background: white;
  border-radius: 2px;
  padding: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
}

.section-title {
  font-size: 13px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 1px solid #e9ecef;
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(100px, 1fr));
  gap: 10px;
}

.action-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 12px 8px;
  border-radius: 2px;
  cursor: pointer;
  transition: opacity 0.2s;
  text-decoration: none;
  color: white;
}

.action-card:hover {
  opacity: 0.9;
}

.action-primary { background: #3498db; }
.action-info { background: #17a2b8; }
.action-success { background: #27ae60; }
.action-warning { background: #f39c12; }
.action-purple { background: #9b59b6; }
.action-secondary { background: #6c757d; }

.action-icon {
  font-size: 18px;
  margin-bottom: 6px;
}

.action-text {
  font-size: 11px;
  font-weight: 500;
}

@media (max-width: 768px) {
  .dashboard-content {
    padding: 12px;
  }
  
  .stats-grid {
    grid-template-columns: 1fr 1fr;
  }
  
  .actions-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}
</style>
