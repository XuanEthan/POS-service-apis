<script setup>
import { ref, watch, computed } from 'vue'
import { EMPTY_GUID } from '@/utils/uuid'
import { getRolePermissions } from '@/services/rolePermissionService'
import { hasPermission } from '@/utils/auth'

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

// Selected permissions (for multiple selection)
const selectedPermissions = ref([])
const loadingRolePermissions = ref(false)

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

// Fetch role permissions for edit mode
async function fetchRolePermissions(roleId) {
  if (!roleId) return
  loadingRolePermissions.value = true
  try {
    const response = await getRolePermissions({ RoleId: roleId })
    if (response.isSuccess) {
      selectedPermissions.value = (response.object || []).map(rp => rp.permissionId)
    }
  } catch (e) {
    console.error('Lỗi tải phân quyền:', e)
  } finally {
    loadingRolePermissions.value = false
  }
}

// Helper: Lấy tất cả children IDs của một permission (recursive)
function getAllChildrenIds(permissionId) {
  const permission = props.permissions.find(p => p.permissionId === permissionId)
  if (!permission) return []
  
  const childrenIds = []
  const children = props.permissions.filter(p => p.parentId === permissionId)
  
  children.forEach(child => {
    childrenIds.push(child.permissionId)
    // Recursive: lấy children của child
    childrenIds.push(...getAllChildrenIds(child.permissionId))
  })
  
  return childrenIds
}

// Toggle permission selection
function togglePermission(permissionId) {
  const index = selectedPermissions.value.indexOf(permissionId)
  
  if (index > -1) {
    // Bỏ chọn: Bỏ chọn cả node này và tất cả children
    selectedPermissions.value.splice(index, 1)
    const childrenIds = getAllChildrenIds(permissionId)
    childrenIds.forEach(childId => {
      const childIndex = selectedPermissions.value.indexOf(childId)
      if (childIndex > -1) {
        selectedPermissions.value.splice(childIndex, 1)
      }
    })
  } else {
    // Chọn: Chọn cả node này và tất cả children
    selectedPermissions.value.push(permissionId)
    const childrenIds = getAllChildrenIds(permissionId)
    childrenIds.forEach(childId => {
      if (!selectedPermissions.value.includes(childId)) {
        selectedPermissions.value.push(childId)
      }
    })
  }
}

// Check if permission is selected
function isPermissionSelected(permissionId) {
  return selectedPermissions.value.includes(permissionId)
}

// Watch for roleId change to fetch permissions
watch(() => formData.value.roleId, (newRoleId) => {
  if (newRoleId && (props.mode === 'edit' || props.mode === 'view')) {
    fetchRolePermissions(newRoleId)
  }
})

// Watch for visibility to reset form on open for create mode
watch(() => props.visible, (isVisible) => {
  if (isVisible) {
    if (props.mode === 'create') {
      resetForm()
    } else if ((props.mode === 'edit' || props.mode === 'view') && formData.value.roleId) {
      // Fetch existing permissions for this role
      fetchRolePermissions(formData.value.roleId)
    }
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
  selectedPermissions.value = []
  errors.value = {}
}

// Validate form
function validateForm() {
  errors.value = {}

  if (!formData.value.roleId) {
    errors.value.roleId = 'Vui lòng chọn vai trò'
  }

  if (selectedPermissions.value.length === 0) {
    errors.value.permissions = 'Vui lòng chọn ít nhất một quyền'
  }

  return Object.keys(errors.value).length === 0
}

// Handle save
function handleSave() {
  if (!validateForm()) return

  const data = {
    RoleId: formData.value.roleId || EMPTY_GUID,
    PermissionIds: selectedPermissions.value
  }
  console.log('Saving role permissions:', data)
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

// Build tree structure for permissions
const permissionsTree = computed(() => {
  const items = props.permissions || []
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
          <h3 class="modal-title">Phân quyền cho vai trò</h3>
          <button class="modal-close" @click="handleClose">&times;</button>
        </div>

        <!-- Modal Body -->
        <div class="modal-body">
          <div class="form-group">
            <label class="form-label">Vai trò <span class="required">*</span></label>
            <select v-model="formData.roleId" class="form-control" :class="{ 'is-invalid': errors.roleId }"
               disabled="true">
              <option value="">-- Chọn vai trò --</option>
              <option v-for="role in roles" :key="role.roleId" :value="role.roleId">
                {{ role.title }} ({{ role.code }})
              </option>
            </select>
            <span v-if="errors.roleId" class="error-text">{{ errors.roleId }}</span>
          </div>

          <div class="form-group">
            <label class="form-label">Phân quyền cho vai trò <span class="required">*</span></label>
            <div class="permissions-container">
              <div v-if="loadingRolePermissions" class="loading-text">Đang tải quyền...</div>
              <div v-else-if="permissions.length === 0" class="empty-text">Không tìm thấy quyền nào</div>
              <div v-else class="permissions-tree">
                <!-- Root level permissions -->
                <div v-for="permission in permissionsTree" :key="permission.permissionId" class="permission-node">
                  <div class="permission-item">
                    <input 
                      type="checkbox" 
                      :id="'perm-' + permission.permissionId"
                      :checked="isPermissionSelected(permission.permissionId)"
                      @change="togglePermission(permission.permissionId)"
                      :disabled="isReadonly"
                      class="permission-checkbox"
                    />
                    <label :for="'perm-' + permission.permissionId" class="permission-label">
                      <span class="permission-icon"></span>
                      <span class="permission-title">{{ permission.title }}</span>
                      <span class="permission-code">({{ permission.code }})</span>
                    </label>
                  </div>
                  
                  <!-- Children level 1 -->
                  <div v-if="permission.children && permission.children.length > 0" class="permission-children">
                    <div v-for="child in permission.children" :key="child.permissionId" class="permission-node">
                      <div class="permission-item">
                        <input 
                          type="checkbox" 
                          :id="'perm-' + child.permissionId"
                          :checked="isPermissionSelected(child.permissionId)"
                          @change="togglePermission(child.permissionId)"
                          :disabled="isReadonly"
                          class="permission-checkbox"
                        />
                        <label :for="'perm-' + child.permissionId" class="permission-label">
                          <span class="permission-icon"></span>
                          <span class="permission-title">{{ child.title }}</span>
                          <span class="permission-code">({{ child.code }})</span>
                        </label>
                      </div>
                      
                      <!-- Children level 2 -->
                      <div v-if="child.children && child.children.length > 0" class="permission-children">
                        <div v-for="grandChild in child.children" :key="grandChild.permissionId" class="permission-node">
                          <div class="permission-item">
                            <input 
                              type="checkbox" 
                              :id="'perm-' + grandChild.permissionId"
                              :checked="isPermissionSelected(grandChild.permissionId)"
                              @change="togglePermission(grandChild.permissionId)"
                              :disabled="isReadonly"
                              class="permission-checkbox"
                            />
                            <label :for="'perm-' + grandChild.permissionId" class="permission-label">
                              <span class="permission-icon">📄</span>
                              <span class="permission-title">{{ grandChild.title }}</span>
                              <span class="permission-code">({{ grandChild.code }})</span>
                            </label>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <span v-if="errors.permissions" class="error-text">{{ errors.permissions }}</span>
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
          <button v-if="!isReadonly" class="btn btn-primary" @click="handleSave">
            <!-- {{ mode === 'create' ? 'Thêm mới' : 'Cập nhật' }} -->
              Lưu
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

/* Permissions Tree */
.permissions-container {
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 12px;
  max-height: 400px;
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
