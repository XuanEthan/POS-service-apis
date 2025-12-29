<script setup>
import { ref, watch, computed } from 'vue'
import { generateUUID, EMPTY_GUID } from '@/utils/uuid'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  mode: {
    type: String,
    default: 'create' // 'create', 'edit', 'view'
  },
  user: {
    type: Object,
    default: () => ({})
  },
  roles: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['close', 'save'])

// Form data
const formData = ref({
  userId: '',
  userName: '',
  password: '',
  roleId: '',
  roleCode: ''
})

// Show password
const showPassword = ref(false)

// Validation errors
const errors = ref({})

// Title based on mode
const modalTitle = computed(() => {
  switch (props.mode) {
    case 'create': return 'THÊM MỚI NGƯỜI DÙNG'
    case 'edit': return 'SỬA NGƯỜI DÙNG'
    case 'view': return 'CHI TIẾT NGƯỜI DÙNG'
    default: return 'NGƯỜI DÙNG'
  }
})

// Is readonly mode
const isReadonly = computed(() => props.mode === 'view')

// Watch for user changes to populate form
watch(() => props.user, (newUser) => {
  if (newUser && Object.keys(newUser).length > 0) {
    formData.value = {
      userId: newUser.userId || '',
      userName: newUser.userName || '',
      password: '', // Don't populate password for security
      roleId: newUser.roleId || '',
      roleCode: newUser.roleCode || ''
    }
  } else {
    resetForm()
  }
}, { immediate: true, deep: true })

// Watch for visibility to reset form on open for create mode
watch(() => props.visible, (isVisible) => {
  if (isVisible && props.mode === 'create') {
    resetForm()
  }
  errors.value = {}
  showPassword.value = false
})

// Watch roleId to auto-update roleCode
watch(() => formData.value.roleId, (newRoleId) => {
  const role = props.roles.find(r => r.roleId === newRoleId)
  if (role) {
    formData.value.roleCode = role.code
  }
})

// Reset form
function resetForm() {
  formData.value = {
    userId: '',
    userName: '',
    password: '',
    roleId: '',
    roleCode: ''
  }
  errors.value = {}
}

// Validate form
function validateForm() {
  errors.value = {}
  
  if (!formData.value.userName || !formData.value.userName.trim()) {
    errors.value.userName = 'Tên đăng nhập không được để trống'
  }
  
  // Password is required only for create mode
  if (props.mode === 'create') {
    if (!formData.value.password || !formData.value.password.trim()) {
      errors.value.password = 'Mật khẩu không được để trống'
    } else if (formData.value.password.length < 6) {
      errors.value.password = 'Mật khẩu phải có ít nhất 6 ký tự'
    }
  }
  
  if (!formData.value.roleId) {
    errors.value.roleId = 'Vui lòng chọn vai trò'
  }
  
  return Object.keys(errors.value).length === 0
}

// Handle save
function handleSave() {
  if (!validateForm()) return
  
  const data = {
    ...formData.value,
    // Generate new UUID for create mode, keep existing for edit
    userId: props.mode === 'create' ? generateUUID() : formData.value.userId,
    // Use EMPTY_GUID if roleId is not selected
    roleId: formData.value.roleId || EMPTY_GUID
  }
  
  // Nếu đang edit và password trống, gửi null để backend biết không thay đổi
  if (props.mode === 'edit' && !data.password) {
    data.password = null
  }
  
  emit('save', data)
}

// Handle close
function handleClose() {
  emit('close')
}
</script>

<template>
  <Teleport to="body">
    <div class="modal-overlay" v-if="visible" @click.self="handleClose">
      <div class="modal-container">
        <!-- Modal Header -->
        <div class="modal-header">
          <h3 class="modal-title">{{ modalTitle }}</h3>
          <button class="modal-close" @click="handleClose">&times;</button>
        </div>

        <!-- Modal Body -->
        <div class="modal-body">
          <div class="form-group">
            <label class="form-label">Tên đăng nhập <span class="required">*</span></label>
            <input 
              v-model="formData.userName" 
              type="text" 
              class="form-control"
              :class="{ 'is-invalid': errors.userName }"
              :readonly="isReadonly"
              placeholder="Nhập tên đăng nhập"
            />
            <span v-if="errors.userName" class="error-text">{{ errors.userName }}</span>
          </div>

          <div class="form-group" v-if="!isReadonly">
            <label class="form-label">
              Mật khẩu 
              <span v-if="mode === 'create'" class="required">*</span>
              <span v-else class="hint">(để trống nếu không đổi)</span>
            </label>
            <div class="password-input">
              <input 
                v-model="formData.password" 
                :type="showPassword ? 'text' : 'password'" 
                class="form-control"
                :class="{ 'is-invalid': errors.password }"
                placeholder="Nhập mật khẩu"
              />
              <button 
                type="button"
                class="password-toggle"
                @click="showPassword = !showPassword"
              >
                {{ showPassword ? '🙈' : '👁️' }}
              </button>
            </div>
            <span v-if="errors.password" class="error-text">{{ errors.password }}</span>
          </div>

          <div class="form-group">
            <label class="form-label">Vai trò <span class="required">*</span></label>
            <select 
              v-model="formData.roleId" 
              class="form-control"
              :class="{ 'is-invalid': errors.roleId }"
              :disabled="isReadonly"
            >
              <option value="">-- Chọn vai trò --</option>
              <option 
                v-for="role in roles" 
                :key="role.roleId" 
                :value="role.roleId"
              >
                {{ role.title }} ({{ role.code }})
              </option>
            </select>
            <span v-if="errors.roleId" class="error-text">{{ errors.roleId }}</span>
          </div>

          <div class="form-group" v-if="isReadonly && formData.roleCode">
            <label class="form-label">Mã vai trò</label>
            <input 
              :value="formData.roleCode" 
              type="text" 
              class="form-control"
              readonly
            />
          </div>
        </div>

        <!-- Modal Footer -->
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="handleClose">
            {{ isReadonly ? 'Đóng' : 'Hủy' }}
          </button>
          <button 
            v-if="!isReadonly" 
            class="btn btn-primary" 
            @click="handleSave"
          >
            {{ mode === 'create' ? 'Thêm mới' : 'Cập nhật' }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-container {
  background: #fff;
  border-radius: 8px;
  width: 100%;
  max-width: 500px;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  background: #f8f9fa;
  border-bottom: 1px solid #e9ecef;
}

.modal-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.modal-close {
  background: none;
  border: none;
  font-size: 24px;
  color: #666;
  cursor: pointer;
  padding: 0;
  line-height: 1;
}

.modal-close:hover {
  color: #333;
}

.modal-body {
  padding: 20px;
  overflow-y: auto;
  flex: 1;
}

.modal-footer {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
  padding: 16px 20px;
  background: #f8f9fa;
  border-top: 1px solid #e9ecef;
}

.form-group {
  margin-bottom: 16px;
}

.form-label {
  display: block;
  margin-bottom: 6px;
  font-weight: 500;
  color: #333;
  font-size: 14px;
}

.required {
  color: #dc3545;
}

.hint {
  color: #6c757d;
  font-size: 12px;
  font-weight: normal;
}

.form-control {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  transition: border-color 0.2s;
  box-sizing: border-box;
}

.form-control:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.1);
}

.form-control:read-only,
.form-control:disabled {
  background: #f5f5f5;
  cursor: not-allowed;
}

.form-control.is-invalid {
  border-color: #dc3545;
}

.error-text {
  display: block;
  color: #dc3545;
  font-size: 12px;
  margin-top: 4px;
}

.password-input {
  position: relative;
}

.password-input .form-control {
  padding-right: 40px;
}

.password-toggle {
  position: absolute;
  right: 8px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  font-size: 16px;
  padding: 4px;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: #007bff;
  color: #fff;
}

.btn-primary:hover {
  background: #0056b3;
}

.btn-secondary {
  background: #6c757d;
  color: #fff;
}

.btn-secondary:hover {
  background: #545b62;
}
</style>
