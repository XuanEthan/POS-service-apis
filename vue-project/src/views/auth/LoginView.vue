<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { login } from '@/services/authService'

const router = useRouter()

const form = reactive({
  username: '',
  password: ''
})

const loading = ref(false)
const error = ref('')
const showPassword = ref(false)

async function handleSubmit() {
  // Validate
  if (!form.username.trim()) {
    error.value = 'Vui lòng nhập tên đăng nhập'
    return
  }
  if (!form.password) {
    error.value = 'Vui lòng nhập mật khẩu'
    return
  }
  
  loading.value = true
  error.value = ''
  
  try {
    const response = await login(form.username, form.password)
    
    if (response.isSuccess) {
      // Đăng nhập thành công, chuyển đến trang chủ
      router.push('/')
    console.log(response);
    } else {
      error.value = response.message || 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.'
    }
  } catch (e) {
    error.value = 'Lỗi kết nối server. Vui lòng thử lại sau.'
  } finally {
    loading.value = false
  }
}

function togglePassword() {
  showPassword.value = !showPassword.value
}
</script>

<template>
  <div class="login-container">
    <div class="login-box">
      <!-- Logo / Header -->
      <div class="login-header">
        <div class="logo">
          <i class="fas fa-cash-register logo-icon"></i>
          <span class="logo-text">POS System</span>
        </div>
        <h1>Đăng nhập</h1>
        <p>Chào mừng bạn quay trở lại!</p>
      </div>
      
      <!-- Error message -->
      <div v-if="error" class="error-alert">
        <i class="fas fa-exclamation-triangle error-icon"></i>
        <span>{{ error }}</span>
      </div>
      
      <!-- Login Form -->
      <form @submit.prevent="handleSubmit" class="login-form">
        <div class="form-group">
          <label for="username">Tên đăng nhập</label>
          <div class="input-wrapper">
            <i class="fas fa-user input-icon"></i>
            <input 
              type="text" 
              id="username" 
              v-model="form.username"
              placeholder="Nhập tên đăng nhập"
              :disabled="loading"
              autocomplete="username"
            />
          </div>
        </div>
        
        <div class="form-group">
          <label for="password">Mật khẩu</label>
          <div class="input-wrapper">
            <i class="fas fa-lock input-icon"></i>
            <input 
              :type="showPassword ? 'text' : 'password'" 
              id="password" 
              v-model="form.password"
              placeholder="Nhập mật khẩu"
              :disabled="loading"
              autocomplete="current-password"
            />
            <button 
              type="button" 
              class="toggle-password"
              @click="togglePassword"
              tabindex="-1"
            >
              <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
            </button>
          </div>
        </div>
        
        <div class="form-options">
          <label class="remember-me">
            <input type="checkbox" />
            <span>Ghi nhớ đăng nhập</span>
          </label>
          <a href="#" class="forgot-password">Quên mật khẩu?</a>
        </div>
        
        <button 
          type="submit" 
          class="btn-login"
          :disabled="loading"
        >
          <span v-if="loading" class="loading-spinner"></span>
          <template v-else>
            <i class="fas fa-sign-in-alt"></i>
            <span>Đăng nhập</span>
          </template>
        </button>
      </form>
      
      <!-- Footer -->
      <div class="login-footer">
        <p>© 2025 POS System</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #2c3e50;
  padding: 20px;
  position: relative;
  overflow: hidden;
}

/* Decorative background pattern */
.login-container::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: 
    linear-gradient(135deg, rgba(52, 73, 94, 0.8) 0%, rgba(44, 62, 80, 0.9) 100%);
  pointer-events: none;
}

.login-box {
  background: #fff;
  border-radius: 2px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  width: 100%;
  max-width: 360px;
  padding: 32px;
  position: relative;
  z-index: 1;
}

.login-header {
  text-align: center;
  margin-bottom: 24px;
}

.logo {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin-bottom: 16px;
}

.logo-icon {
  font-size: 24px;
  color: #3498db;
}

.logo-text {
  font-size: 18px;
  font-weight: 600;
  color: #2c3e50;
}

.login-header h1 {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0 0 6px 0;
}

.login-header p {
  color: #666;
  font-size: 12px;
  margin: 0;
}

.error-alert {
  background: #fef2f2;
  color: #b91c1c;
  padding: 8px 12px;
  border-radius: 2px;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  border-left: 3px solid #ef4444;
}

.error-icon {
  font-size: 12px;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  font-size: 12px;
  font-weight: 500;
  color: #333;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 10px;
  font-size: 12px;
  color: #6b7280;
  pointer-events: none;
}

.input-wrapper input {
  width: 100%;
  padding: 8px 10px 8px 32px;
  border: 1px solid #d1d5db;
  border-radius: 2px;
  font-size: 12px;
  transition: all 0.2s;
  background: #fff;
}

.input-wrapper input:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 0 1px rgba(52, 152, 219, 0.2);
}

.input-wrapper input:disabled {
  background: #f3f4f6;
  cursor: not-allowed;
}

.input-wrapper input::placeholder {
  color: #9ca3af;
}

.toggle-password {
  position: absolute;
  right: 10px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 12px;
  color: #6b7280;
  padding: 0;
  transition: color 0.2s;
}

.toggle-password:hover {
  color: #3498db;
}

.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.remember-me {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: #666;
  cursor: pointer;
}

.remember-me input {
  width: 14px;
  height: 14px;
  cursor: pointer;
  accent-color: #3498db;
}

.forgot-password {
  font-size: 12px;
  color: #3498db;
  text-decoration: none;
}

.forgot-password:hover {
  text-decoration: underline;
}

.btn-login {
  width: 100%;
  padding: 10px;
  background: #3498db;
  color: white;
  border: none;
  border-radius: 2px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.btn-login:hover:not(:disabled) {
  background: #2980b9;
}

.btn-login:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.loading-spinner {
  width: 14px;
  height: 14px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.login-footer {
  text-align: center;
  margin-top: 20px;
  padding-top: 16px;
  border-top: 1px solid #e5e7eb;
}

.login-footer p {
  color: #9ca3af;
  font-size: 11px;
  margin: 0;
}

/* Responsive */
@media (max-width: 480px) {
  .login-box {
    padding: 24px 20px;
  }
  
  .login-header h1 {
    font-size: 18px;
  }
}
</style>
