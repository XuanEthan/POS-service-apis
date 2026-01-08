<script setup>
import { ref, watch, computed } from 'vue'
import { generateUUID, EMPTY_GUID } from '@/utils/uuid'
import { getPermissions } from '@/services/permissionService'
import { getRolePermissions } from '@/services/rolePermissionService'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  mode: {
    type: String,
    default: 'create' // 'create', 'edit', 'view'
  },
  role: {
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
  roleId: 0,
  code: '',
  title: '',
  parentId: 0
})

// Validation errors
const errors = ref({})

// Permissions data
const permissions = ref([])
const selectedPermissions = ref([])
const loadingPermissions = ref(false)

// Title based on mode
const modalTitle = computed(() => {
  switch (props.mode) {
    case 'create': return 'THÊM MỚI VAI TRÒ'
    case 'edit': return 'SỬA VAI TRÒ'
    case 'view': return 'CHI TIẾT VAI TRÒ'
    default: return 'VAI TRÒ'
  }
})

// Is readonly mode
const isReadonly = computed(() => props.mode === 'view')

// Fetch permissions list
async function fetchPermissions() {
  loadingPermissions.value = true
  try {
    const response = await getPermissions()
    if (response.isSuccess) {
      permissions.value = response.object || []
    }
  } catch (e) {
    console.error('Lỗi tải danh sách quyền:', e)
  } finally {
    loadingPermissions.value = false
  }
}

// Fetch role permissions for edit mode
async function fetchRolePermissions(roleId) {
  if (!roleId) return
  try {
    const response = await getRolePermissions({ RoleId: roleId })
    if (response.isSuccess) {
      selectedPermissions.value = (response.object || []).map(rp => rp.permissionId)
    }
  } catch (e) {
    console.error('Lỗi tải phân quyền:', e)
  }
}

// Watch for role changes to populate form
watch(() => props.role, (newRole) => {
  if (newRole && Object.keys(newRole).length > 0) {
    formData.value = {
      roleId: newRole.roleId || "",
      code: newRole.code || '',
      title: newRole.title || '',
      parentId: newRole.parentId || ""
    }
    // Fetch role permissions for edit mode
    if (props.mode === 'edit' && newRole.roleId) {
      fetchRolePermissions(newRole.roleId)
    }
  } else {
    resetForm()
  }
}, { immediate: true, deep: true })

// Watch for visibility to reset form on open for create mode
watch(() => props.visible, (isVisible) => {
  if (isVisible) {
    fetchPermissions()
    if (props.mode === 'create') {
      resetForm()
    }
  }
  errors.value = {}
})

// Reset form
function resetForm() {
  formData.value = {
    roleId: '',
    code: '',
    title: '',
    parentId: null
  }
  selectedPermissions.value = []
  errors.value = {}
}

// Validate form
function validateForm() {
  errors.value = {}
  
  if (!formData.value.code || !formData.value.code.trim()) {
    errors.value.code = 'Mã vai trò không được để trống'
  }
  
  if (!formData.value.title || !formData.value.title.trim()) {
    errors.value.title = 'Tên vai trò không được để trống'
  }
  
  return Object.keys(errors.value).length === 0
}

// Toggle permission selection
function togglePermission(permissionId) {
  const index = selectedPermissions.value.indexOf(permissionId)
  if (index > -1) {
    selectedPermissions.value.splice(index, 1)
  } else {
    selectedPermissions.value.push(permissionId)
  }
}

// Check if permission is selected
function isPermissionSelected(permissionId) {
  return selectedPermissions.value.includes(permissionId)
}

// Handle save
function handleSave() {
  if (!validateForm()) return
  
  const data = {
    ...formData.value,
    // Generate new UUID for create mode, keep existing for edit
    roleId: props.mode === 'create' ? generateUUID() : formData.value.roleId,
    // Use EMPTY_GUID if parentId is not selected
    parentId: formData.value.parentId || EMPTY_GUID,
    // Include selected permissions
    selectedPermissions: selectedPermissions.value
  }
  
  emit('save', data)
}

// Handle close
function handleClose() {
  emit('close')
}

// Filter out current role from parent options (avoid self-reference)
const parentOptions = computed(() => {
  if (props.mode === 'edit' && formData.value.roleId) {
    return props.roles.filter(r => r.roleId !== formData.value.roleId)
  }
  return props.roles
})

// Build tree structure for permissions
const permissionsTree = computed(() => {
  const items = permissions.value || []
  const map = new Map()
  const roots = []
  
  items.forEach(item => {
    map.set(item.permissionId, { ...item, children: [] })
  })
  
  items.forEach(item => {
    const node = map.get(item.permissionId)
    const parentId = item.parentId
    
    const isRoot = !parentId || parentId === '00000000-0000-0000-0000-000000000000'
    
    if (isRoot) {
      roots.push(node)
    } else {
      const parent = map.get(parentId)
      if (parent) {
        parent.children.push(node)
      } else {
        roots.push(node)
      }
    }
  })
  
  return roots
})
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
            <label class="form-label">Mã vai trò <span class="required">*</span></label>
            <input 
              v-model="formData.code" 
              type="text" 
              class="form-control"
              :class="{ 'is-invalid': errors.code }"
              :readonly="isReadonly"
              placeholder="Nhập mã vai trò (VD: ADMIN, USER...)"
            />
            <span v-if="errors.code" class="error-text">{{ errors.code }}</span>
          </div>

          <div class="form-group">
            <label class="form-label">Tên vai trò <span class="required">*</span></label>
            <input 
              v-model="formData.title" 
              type="text" 
              class="form-control"
              :class="{ 'is-invalid': errors.title }"
              :readonly="isReadonly"
              placeholder="Nhập tên vai trò"
            />
            <span v-if="errors.title" class="error-text">{{ errors.title }}</span>
          </div>

          <div class="form-group">
            <label class="form-label">Vai trò cha</label>
            <select 
              v-model="formData.parentId" 
              class="form-control"
              :disabled="isReadonly"
            >
              <option :value="null">-- Không có vai trò cha --</option>
              <option 
                v-for="r in parentOptions" 
                :key="r.roleId" 
                :value="r.roleId"
              >
                {{ r.title }} ({{ r.code }})
              </option>
            </select>
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

/* Permissions Tree */
.permissions-container {
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 12px;
  max-height: 300px;
  overflow-y: auto;
  background: #fafafa;
}

.loading-text,
.empty-text {
  text-align: center;
  color: #999;
  font-size: 13px;
  padding: 20px;
}

.permissions-tree {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.permission-node {
  display: flex;
  flex-direction: column;
}

.permission-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 8px;
  border-radius: 3px;
  transition: background 0.2s;
}

.permission-item:hover {
  background: #f0f0f0;
}

.permission-checkbox {
  width: 16px;
  height: 16px;
  cursor: pointer;
  flex-shrink: 0;
}

.permission-checkbox:disabled {
  cursor: not-allowed;
}

.permission-label {
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  flex: 1;
  font-size: 13px;
  user-select: none;
}

.permission-icon {
  font-size: 14px;
}

.permission-title {
  font-weight: 500;
  color: #333;
}

.permission-code {
  color: #666;
  font-size: 12px;
}

.permission-children {
  margin-left: 24px;
  display: flex;
  flex-direction: column;
  gap: 4px;
  border-left: 2px solid #e0e0e0;
  padding-left: 8px;
  margin-top: 4px;
}

.permissions-container::-webkit-scrollbar {
  width: 8px;
}

.permissions-container::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.permissions-container::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 4px;
}

.permissions-container::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}
</style>
