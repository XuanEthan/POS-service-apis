import { hasPermission, hasAnyPermission, hasAllPermissions, canDoAction, isAdmin } from '@/utils/auth'

/**
 * Vue Directive để kiểm tra và ẩn/hiện element dựa trên quyền
 * Admin sẽ bypass tất cả các kiểm tra quyền
 * 
 * Cách sử dụng:
 * 
 * 1. Kiểm tra một quyền:
 *    <button v-permission="'permission-id'">Button</button>
 * 
 * 2. Kiểm tra nhiều quyền (có ít nhất 1):
 *    <button v-permission="['perm1', 'perm2']">Button</button>
 * 
 * 3. Kiểm tra với options:
 *    <button v-permission="{ 
 *      permissions: ['perm1', 'perm2'], 
 *      mode: 'all',      // 'any' (default) hoặc 'all'
 *      action: 'hide'    // 'hide' (default) hoặc 'disable'
 *    }">Button</button>
 * 
 * 4. Kiểm tra module + action:
 *    <button v-permission="{ module: 'user', action: 'add' }">Thêm user</button>
 * 
 * Modifiers:
 *    v-permission.disable="..." - Disable thay vì hide
 *    v-permission.all="[...]" - Yêu cầu tất cả quyền
 */

const permissionDirective = {
  mounted(el, binding) {
    checkPermission(el, binding)
  },
  updated(el, binding) {
    checkPermission(el, binding)
  }
}

function checkPermission(el, binding) {
  const { value, modifiers } = binding
  
  let hasAccess = false
  let actionOnDenied = modifiers.disable ? 'disable' : 'hide'
  
  // Admin bypass tất cả - luôn có quyền
  if (isAdmin()) {
    hasAccess = true
  }
  // Nếu không có value, cho phép truy cập
  else if (!value) {
    hasAccess = true
  }
  // Nếu là string (single permission)
  else if (typeof value === 'string') {
    hasAccess = hasPermission(value)
  }
  // Nếu là array (multiple permissions)
  else if (Array.isArray(value)) {
    hasAccess = modifiers.all 
      ? hasAllPermissions(value) 
      : hasAnyPermission(value)
  }
  // Nếu là object với options
  else if (typeof value === 'object') {
    // Kiểm tra theo module + action
    if (value.module && value.action) {
      hasAccess = canDoAction(value.module, value.action)
    }
    // Kiểm tra theo danh sách permissions
    else if (value.permissions) {
      const permissions = Array.isArray(value.permissions) 
        ? value.permissions 
        : [value.permissions]
      
      const mode = value.mode || 'any'
      hasAccess = mode === 'all' 
        ? hasAllPermissions(permissions) 
        : hasAnyPermission(permissions)
    }
    // Kiểm tra theo permission đơn
    else if (value.permission) {
      hasAccess = hasPermission(value.permission)
    }
    
    // Override action từ options
    if (value.actionOnDenied) {
      actionOnDenied = value.actionOnDenied
    }
  }
  
  // Xử lý khi không có quyền
  if (!hasAccess) {
    if (actionOnDenied === 'disable') {
      el.disabled = true
      el.classList.add('permission-disabled')
      el.style.pointerEvents = 'none'
      el.style.opacity = '0.5'
      el.title = 'Bạn không có quyền thực hiện chức năng này'
    } else {
      // Hide element
      el.style.display = 'none'
    }
  } else {
    // Restore element
    el.disabled = false
    el.classList.remove('permission-disabled')
    el.style.pointerEvents = ''
    el.style.opacity = ''
    el.style.display = ''
    el.title = ''
  }
}

/**
 * Plugin để đăng ký directive globally
 */
export const PermissionPlugin = {
  install(app) {
    app.directive('permission', permissionDirective)
  }
}

export default permissionDirective
