<template>
  <div class="page-container">
    <!-- Page Header -->
    <div class="page-header">
      <h1 class="page-title">403 - TRUY CẬP BỊ TỪ CHỐI</h1>
    </div>

    <!-- Content -->
    <div class="forbidden-content">
      <div class="forbidden-card">
        <div class="forbidden-icon">🚫</div>
        <h2 class="forbidden-message">
          Xin lỗi, bạn không có quyền truy cập trang này.
        </h2>
        <p class="forbidden-description">
          Vui lòng liên hệ quản trị viên nếu bạn cho rằng đây là lỗi.
        </p>
        <div class="forbidden-actions">
          <button @click="goBack" class="btn btn-secondary">
            ← Quay lại
          </button>
          <button @click="goHome" class="btn btn-primary">
            🏠 Về trang chủ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

function goBack() {
  router.go(-1)
}

function goHome() {
  router.push({ name: 'dashboard' })
}
</script>

<style scoped>
.page-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background: #f5f6fa;
}

.page-header {
  background: white;
  padding: 20px;
  border-bottom: 1px solid #e0e0e0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
}

.page-title {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #2c3e50;
}

.forbidden-content {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
}

.forbidden-card {
  background: white;
  border-radius: 4px;
  padding: 48px 40px;
  text-align: center;
  max-width: 500px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border-left: 4px solid #e74c3c;
}

.forbidden-icon {
  font-size: 80px;
  margin-bottom: 20px;
}

.forbidden-message {
  font-size: 18px;
  color: #2c3e50;
  margin: 0 0 16px 0;
  font-weight: 600;
}

.forbidden-description {
  color: #7f8c8d;
  font-size: 14px;
  line-height: 1.6;
  margin-bottom: 32px;
}

.forbidden-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
}

.btn {
  padding: 10px 20px;
  border-radius: 4px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  border: none;
  transition: all 0.2s ease;
  white-space: nowrap;
}

.btn-primary {
  background: #3498db;
  color: white;
}

.btn-primary:hover {
  background: #2980b9;
  box-shadow: 0 2px 4px rgba(52, 152, 219, 0.3);
}

.btn-secondary {
  background: #ecf0f1;
  color: #2c3e50;
  border: 1px solid #bdc3c7;
}

.btn-secondary:hover {
  background: #bdc3c7;
  border-color: #95a5a6;
}

@media (max-width: 768px) {
  .forbidden-card {
    padding: 32px 20px;
    max-width: 100%;
  }

  .forbidden-icon {
    font-size: 60px;
    margin-bottom: 16px;
  }

  .forbidden-message {
    font-size: 16px;
  }

  .forbidden-actions {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }
}
</style>
