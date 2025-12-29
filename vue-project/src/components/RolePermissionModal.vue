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
  rolePermission: {
    type: Object,
    default: () => ({})
  },
  roles: {
    type: Array,
    default: () => []
  },
  permissions: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['close', 'save'])

// Form data
const formData = ref({
  rolePermissionId: '',
  roleId: '',
  permissionId: '',
  statusId: 1
})

// Validation errors
const errors = ref({})

// Title based on mode
const modalTitle = computed(() => {
  switch (props.mode) {
    case 'create': return 'THÊM MỚI PHÂN QUYỀN'
    case 'edit': return 'SỬA PHÂN QUYỀN'
    case 'view': return 'CHI TIẾT PHÂN QUYỀN'
    default: return 'PHÂN QUYỀN'
  }
})

// Is readonly mode
const isReadonly = computed(() => props.mode === 'view')

// Watch for rolePermission changes to populate form
watch(() => props.rolePermission, (newRolePermission) => {
  if (newRolePermission && Object.keys(newRolePermission).length > 0) {
    formData.value = {
      rolePermissionId: newRolePermission.rolePermissionId || '',
      roleId: newRolePermission.roleId || '',
      permissionId: newRolePermission.permissionId || '',
      statusId: newRolePermission.statusId !== undefined ? newRolePermission.statusId : 1
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
})

// Reset form
function resetForm() {
  formData.value = {
    rolePermissionId: '',
    roleId: '',
    permissionId: '',
    statusId: 1
  }
  errors.value = {}
}

// Validate form
function validateForm() {
  errors.value = {}
  
  if (!formData.value.roleId) {
    errors.value.roleId = 'Vui lòng chọn vai trò'
  }
  
  if (!formData.value.permissionId) {
    errors.value.permissionId = 'Vui lòng chọn quyền'
  }
  
  return Object.keys(errors.value).length === 0
}

// Handle save
function handleSave() {
  if (!validateForm()) return
  
  const data = {
    ...formData.value,
    // Generate new UUID for create mode, keep existing for edit
    rolePermissionId: props.mode === 'create' ? generateUUID() : formData.value.rolePermissionId,
    // Use EMPTY_GUID if IDs are not selected
    roleId: formData.value.roleId || EMPTY_GUID,
    permissionId: formData.value.permissionId || EMPTY_GUID
  }
  emit('save', data)
}

// Handle close
function handleClose() {
  emit('close')
}

// Helper to get role name
function getRoleName(roleId) {
  const role = props.roles.find(r => r.roleId === roleId)
  return role ? role.title : roleId
}

// Helper to get permission name
function getPermissionName(permissionId) {
  const permission = props.permissions.find(p => p.permissionId === permissionId)
  return permission ? permission.title : permissionId
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

          <div class="form-group">
            <label class="form-label">Quyền <span class="required">*</span></label>
            <select 
              v-model="formData.permissionId" 
              class="form-control"
              :class="{ 'is-invalid': errors.permissionId }"
              :disabled="isReadonly"
            >
              <option value="">-- Chọn quyền --</option>
              <option 
                v-for="permission in permissions" 
                :key="permission.permissionId" 
                :value="permission.permissionId"
              >
                {{ permission.title }} ({{ permission.code }})
              </option>
            </select>
            <span v-if="errors.permissionId" class="error-text">{{ errors.permissionId }}</span>
          </div>

          <div class="form-group">
            <label class="form-label">Trạng thái</label>
            <select 
              v-model="formData.statusId" 
              class="form-control"
              :disabled="isReadonly"
            >
              <option :value="1">Hoạt động</option>
              <option :value="0">Không hoạt động</option>
            </select>
          </div>

          <!-- View mode: Show names -->
          <div v-if="isReadonly" class="info-section">
            <div class="info-row">
              <span class="info-label">Vai trò:</span>
              <span class="info-value badge badge-info">{{ getRoleName(formData.roleId) }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Quyền:</span>
              <span class="info-value badge badge-success">{{ getPermissionName(formData.permissionId) }}</span>
            </div>
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

.info-section {
  margin-top: 20px;
  padding-top: 16px;
  border-top: 1px solid #e9ecef;
}

.info-row {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}

.info-label {
  min-width: 80px;
  font-weight: 500;
  color: #666;
}

.info-value {
  flex: 1;
}

.badge {
  display: inline-block;
  padding: 4px 10px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.badge-info {
  background: #17a2b8;
  color: #fff;
}

.badge-success {
  background: #28a745;
  color: #fff;
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
