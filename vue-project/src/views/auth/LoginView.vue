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
          <span class="logo-icon">🏪</span>
          <span class="logo-text">POS System</span>
        </div>
        <h1>Đăng nhập</h1>
        <p>Chào mừng bạn quay trở lại!</p>
      </div>
      
      <!-- Error message -->
      <div v-if="error" class="error-alert">
        <span class="error-icon">⚠️</span>
        <span>{{ error }}</span>
      </div>
      
      <!-- Login Form -->
      <form @submit.prevent="handleSubmit" class="login-form">
        <div class="form-group">
          <label for="username">Tên đăng nhập</label>
          <div class="input-wrapper">
            <span class="input-icon">👤</span>
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
            <span class="input-icon">🔒</span>
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
              {{ showPassword ? '🙈' : '👁️' }}
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
          <span v-else>Đăng nhập</span>
        </button>
      </form>
      
      <!-- Footer -->
      <div class="login-footer">
        <p>© 2025 POS System - Dân Trí Soft</p>
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
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-box {
  background: white;
  border-radius: 16px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  width: 100%;
  max-width: 420px;
  padding: 40px;
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.logo {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  margin-bottom: 20px;
}

.logo-icon {
  font-size: 40px;
}

.logo-text {
  font-size: 24px;
  font-weight: 700;
  color: #667eea;
}

.login-header h1 {
  font-size: 28px;
  font-weight: 600;
  color: #333;
  margin: 0 0 8px 0;
}

.login-header p {
  color: #666;
  font-size: 14px;
  margin: 0;
}

.error-alert {
  background: #ffe6e6;
  color: #c00;
  padding: 12px 16px;
  border-radius: 8px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 14px;
}

.error-icon {
  font-size: 18px;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
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
  left: 14px;
  font-size: 16px;
  pointer-events: none;
}

.input-wrapper input {
  width: 100%;
  padding: 14px 14px 14px 44px;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  font-size: 15px;
  transition: all 0.2s;
  background: #fafafa;
}

.input-wrapper input:focus {
  outline: none;
  border-color: #667eea;
  background: white;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
}

.input-wrapper input:disabled {
  background: #f0f0f0;
  cursor: not-allowed;
}

.toggle-password {
  position: absolute;
  right: 14px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 16px;
  padding: 0;
}

.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.remember-me {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #666;
  cursor: pointer;
}

.remember-me input {
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.forgot-password {
  font-size: 14px;
  color: #667eea;
  text-decoration: none;
}

.forgot-password:hover {
  text-decoration: underline;
}

.btn-login {
  width: 100%;
  padding: 14px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.btn-login:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(102, 126, 234, 0.4);
}

.btn-login:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.login-footer {
  text-align: center;
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #eee;
}

.login-footer p {
  color: #999;
  font-size: 12px;
  margin: 0;
}

/* Responsive */
@media (max-width: 480px) {
  .login-box {
    padding: 30px 20px;
  }
  
  .login-header h1 {
    font-size: 24px;
  }
}
</style>
