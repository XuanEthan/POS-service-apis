<script setup>
import { RouterLink } from 'vue-router'
</script>

<template>
  <div class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">TỔNG QUAN</h1>
      <span class="page-subtitle">Chào mừng bạn đến với hệ thống POS - Dân Trí Soft</span>
    </div>

    <!-- Content -->
    <div class="dashboard-content">
      <!-- Stats Grid -->
      <div class="stats-grid">
        <div class="stat-card stat-blue">
          <div class="stat-icon">🧾</div>
          <div class="stat-info">
            <div class="stat-value">156</div>
            <div class="stat-label">Hóa đơn hôm nay</div>
          </div>
        </div>

        <div class="stat-card stat-green">
          <div class="stat-icon">💰</div>
          <div class="stat-info">
            <div class="stat-value">12,500,000 ₫</div>
            <div class="stat-label">Doanh thu hôm nay</div>
          </div>
        </div>

        <div class="stat-card stat-orange">
          <div class="stat-icon">👥</div>
          <div class="stat-info">
            <div class="stat-value">48</div>
            <div class="stat-label">Khách hàng</div>
          </div>
        </div>

        <div class="stat-card stat-purple">
          <div class="stat-icon">📦</div>
          <div class="stat-info">
            <div class="stat-value">234</div>
            <div class="stat-label">Sản phẩm đã bán</div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="quick-actions-section">
        <h2 class="section-title">Truy cập nhanh</h2>
        <div class="actions-grid">
          <RouterLink to="/thanh-toan" class="action-card action-primary">
            <span class="action-icon">💳</span>
            <span class="action-text">Thanh toán</span>
          </RouterLink>
          <RouterLink to="/hoa-don" class="action-card action-info">
            <span class="action-icon">🧾</span>
            <span class="action-text">Hóa đơn</span>
          </RouterLink>
          <RouterLink to="/nguoi-dung" class="action-card action-success">
            <span class="action-icon">👤</span>
            <span class="action-text">Người dùng</span>
          </RouterLink>
          <RouterLink to="/role" class="action-card action-warning">
            <span class="action-icon">🎭</span>
            <span class="action-text">Vai trò</span>
          </RouterLink>
          <RouterLink to="/permission" class="action-card action-purple">
            <span class="action-icon">🔑</span>
            <span class="action-text">Quyền</span>
          </RouterLink>
          <RouterLink to="/role-permission" class="action-card action-secondary">
            <span class="action-icon">🔐</span>
            <span class="action-text">Phân quyền</span>
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard-content {
  padding: 24px;
}

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 20px;
  margin-bottom: 32px;
}

.stat-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
  transition: transform 0.2s, box-shadow 0.2s;
  border-left: 4px solid transparent;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.stat-blue { border-left-color: #3498db; }
.stat-green { border-left-color: #27ae60; }
.stat-orange { border-left-color: #f39c12; }
.stat-purple { border-left-color: #9b59b6; }

.stat-icon {
  font-size: 32px;
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  background: #f8f9fa;
}

.stat-blue .stat-icon { background: #ebf5fb; }
.stat-green .stat-icon { background: #e8f6ef; }
.stat-orange .stat-icon { background: #fef5e7; }
.stat-purple .stat-icon { background: #f5eef8; }

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 22px;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 4px;
}

.stat-label {
  font-size: 13px;
  color: #7f8c8d;
}

/* Quick Actions */
.quick-actions-section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #e9ecef;
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 16px;
}

.action-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 24px 16px;
  border-radius: 8px;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  text-decoration: none;
  color: white;
}

.action-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.action-primary { background: linear-gradient(135deg, #3498db, #2980b9); }
.action-info { background: linear-gradient(135deg, #17a2b8, #138496); }
.action-success { background: linear-gradient(135deg, #27ae60, #219a52); }
.action-warning { background: linear-gradient(135deg, #f39c12, #d68910); }
.action-purple { background: linear-gradient(135deg, #9b59b6, #8e44ad); }
.action-secondary { background: linear-gradient(135deg, #6c757d, #5a6268); }

.action-icon {
  font-size: 32px;
  margin-bottom: 12px;
}

.action-text {
  font-size: 14px;
  font-weight: 600;
}

@media (max-width: 768px) {
  .dashboard-content {
    padding: 16px;
  }
  
  .stats-grid {
    grid-template-columns: 1fr;
  }
  
  .actions-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
