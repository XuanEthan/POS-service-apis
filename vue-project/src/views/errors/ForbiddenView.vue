<template>
  <div class="forbidden-container">
    <div class="forbidden-content">
      <div class="forbidden-icon">🚫</div>
      <h1 class="forbidden-title">403</h1>
      <h2 class="forbidden-subtitle">Không có quyền truy cập</h2>
      <p class="forbidden-message">
        Xin lỗi, bạn không có quyền truy cập trang này. 
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
.forbidden-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.forbidden-content {
  background: white;
  border-radius: 16px;
  padding: 48px;
  text-align: center;
  max-width: 500px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.forbidden-icon {
  font-size: 64px;
  margin-bottom: 16px;
}

.forbidden-title {
  font-size: 72px;
  font-weight: 700;
  color: #e74c3c;
  margin: 0;
  line-height: 1;
}

.forbidden-subtitle {
  font-size: 24px;
  color: #2c3e50;
  margin: 16px 0;
}

.forbidden-message {
  color: #7f8c8d;
  font-size: 16px;
  line-height: 1.6;
  margin-bottom: 32px;
}

.forbidden-actions {
  display: flex;
  gap: 16px;
  justify-content: center;
}

.btn {
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  border: none;
  transition: all 0.3s ease;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.btn-secondary {
  background: #ecf0f1;
  color: #2c3e50;
}

.btn-secondary:hover {
  background: #bdc3c7;
}
</style>
