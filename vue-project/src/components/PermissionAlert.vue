<template>
  <div v-if="!hasPermission" class="permission-denied-container">
    <div class="permission-denied-box">
      <div class="permission-denied-icon">
        <i class="fas fa-lock"></i>
      </div>
      <h2 class="permission-denied-title">Truy cập bị từ chối</h2>
      <p class="permission-denied-message">
        Bạn không có quyền truy cập chức năng này. 
        <br>
        Vui lòng liên hệ quản trị viên để được cấp quyền.
      </p>
      <button @click="goBack" class="btn btn-primary">
        <i class="fas fa-arrow-left"></i> Quay lại
      </button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const props = defineProps({
  hasPermission: {
    type: Boolean,
    required: true
  }
})

const router = useRouter()

function goBack() {
  router.back()
}
</script>

<style scoped>
.permission-denied-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 500px;
  padding: 20px;
}

.permission-denied-box {
  text-align: center;
  padding: 40px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  border-radius: 10px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  max-width: 400px;
}

.permission-denied-icon {
  font-size: 64px;
  color: #e74c3c;
  margin-bottom: 20px;
}

.permission-denied-title {
  color: #333;
  font-size: 24px;
  margin-bottom: 15px;
  font-weight: 600;
}

.permission-denied-message {
  color: #666;
  font-size: 16px;
  margin-bottom: 25px;
  line-height: 1.6;
}

.btn-primary {
  padding: 10px 25px;
  background-color: #3498db;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s ease;
}

.btn-primary:hover {
  background-color: #2980b9;
}

.btn-primary i {
  margin-right: 8px;
}
</style>
