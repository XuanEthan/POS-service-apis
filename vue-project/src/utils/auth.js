import { PERMISSIONS, ROUTE_PERMISSIONS, FEATURE_PERMISSIONS } from '@/constants/permissions'

// Token storage keys
const ACCESS_TOKEN_KEY = 'access_token'
const REFRESH_TOKEN_KEY = 'refresh_token'
const USER_KEY = 'user_info'
const USER_PERMISSIONS_KEY = 'user_permissions'
const IS_ADMIN_KEY = 'is_admin'
const USER_TOKEN_INFO_KEY = 'user_token_info'
const USER_NAME_KEY = 'user_name'
const USER_ROLE_KEY = 'user_role'
const USER_ID_KEY = 'user_id'
const ROLE_ID_KEY = 'role_id'

// ==================== TOKEN MANAGEMENT ====================

/**
 * Lưu tokens vào localStorage
 */
export function saveTokens(accessToken, refreshToken) {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken)
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken)
}

/**
 * Lấy access token
 */
export function getAccessToken() {
  return localStorage.getItem(ACCESS_TOKEN_KEY)
}

/**
 * Lấy refresh token
 */
export function getRefreshToken() {
  return localStorage.getItem(REFRESH_TOKEN_KEY)
}

/**
 * Xóa tokens
 */
export function clearTokens() {
  localStorage.removeItem(ACCESS_TOKEN_KEY)
  localStorage.removeItem(REFRESH_TOKEN_KEY)
  localStorage.removeItem(USER_KEY)
  localStorage.removeItem(USER_PERMISSIONS_KEY)
  localStorage.removeItem(IS_ADMIN_KEY)
  localStorage.removeItem(USER_TOKEN_INFO_KEY)
  localStorage.removeItem(USER_NAME_KEY)
  localStorage.removeItem(USER_ROLE_KEY)
  localStorage.removeItem(USER_ID_KEY)
  localStorage.removeItem(ROLE_ID_KEY)
}

/**
 * Kiểm tra đã đăng nhập chưa
 */
export function isAuthenticated() {
  return !!getAccessToken()
}

// ==================== USER MANAGEMENT ====================

/**
 * Lưu thông tin user
 */
export function saveUser(user) {
  localStorage.setItem(USER_KEY, JSON.stringify(user))
}

/**
 * Lấy thông tin user
 */
export function getUser() {
  const user = localStorage.getItem(USER_KEY)
  return user ? JSON.parse(user) : null
}

/**
 * Lưu danh sách permissions của user (từ JWT hoặc API)
 */
export function saveUserPermissions(permissions) {
  localStorage.setItem(USER_PERMISSIONS_KEY, JSON.stringify(permissions))
}

/**
 * Lấy danh sách permissions của user
 */
export function getUserPermissions() {
  const permissions = localStorage.getItem(USER_PERMISSIONS_KEY)
  return permissions ? JSON.parse(permissions) : []
}

/**
 * Lưu trạng thái admin
 * @param {boolean} isAdmin
 */
export function saveIsAdmin(isAdmin) {
  localStorage.setItem(IS_ADMIN_KEY, JSON.stringify(isAdmin))
}

/**
 * Kiểm tra user có phải admin không
 * @returns {boolean}
 */
export function isAdmin() {
  const isAdminValue = localStorage.getItem(IS_ADMIN_KEY)
  return isAdminValue ? JSON.parse(isAdminValue) : false
}

/**
 * Lưu thông tin từ token
 * @param {object} tokenInfo - Object chứa thông tin từ token
 */
export function saveTokenInfo(tokenInfo) {
  localStorage.setItem(USER_TOKEN_INFO_KEY, JSON.stringify(tokenInfo))
}

/**
 * Lấy thông tin từ token
 * @returns {object|null}
 */
export function getTokenInfo() {
  const info = localStorage.getItem(USER_TOKEN_INFO_KEY)
  return info ? JSON.parse(info) : null
}

// ==================== USERNAME & ROLE MANAGEMENT ====================

/**
 * Lưu username (unique_name từ JWT)
 * @param {string} userName
 */
export function saveUserName(userName) {
  localStorage.setItem(USER_NAME_KEY, userName)
}

/**
 * Lấy username
 * @returns {string|null}
 */
export function getUserName() {
  return localStorage.getItem(USER_NAME_KEY)
}

/**
 * Lưu user ID (nameid từ JWT)
 * @param {string} userId
 */
export function saveUserId(userId) {
  localStorage.setItem(USER_ID_KEY, userId)
}

/**
 * Lấy user ID
 * @returns {string|null}
 */
export function getUserId() {
  return localStorage.getItem(USER_ID_KEY)
}

/**
 * Lưu tên vai trò (role từ JWT)
 * @param {string} roleName
 */
export function saveUserRole(roleName) {
  localStorage.setItem(USER_ROLE_KEY, roleName)
}

/**
 * Lấy tên vai trò
 * @returns {string|null}
 */
export function getUserRole() {
  return localStorage.getItem(USER_ROLE_KEY)
}

/**
 * Lưu role ID (roleId từ JWT)
 * @param {string} roleId
 */
export function saveRoleId(roleId) {
  localStorage.setItem(ROLE_ID_KEY, roleId)
}

/**
 * Lấy role ID
 * @returns {string|null}
 */
export function getRoleId() {
  return localStorage.getItem(ROLE_ID_KEY)
}

/**
 * Lấy username từ JWT token
 * @param {string} token - JWT token
 * @returns {string|null}
 */
export function getUserNameFromToken(token) {
  const payload = parseJwt(token)
  if (!payload) return null
  return payload.unique_name || null
}

/**
 * Lấy user ID từ JWT token
 * @param {string} token - JWT token
 * @returns {string|null}
 */
export function getUserIdFromToken(token) {
  const payload = parseJwt(token)
  if (!payload) return null
  return payload.nameid || null
}

/**
 * Lấy role name từ JWT token
 * @param {string} token - JWT token
 * @returns {string|null}
 */
export function getRoleFromToken(token) {
  const payload = parseJwt(token)
  if (!payload) return null
  return payload.role || null
}

/**
 * Lấy role ID từ JWT token
 * @param {string} token - JWT token
 * @returns {string|null}
 */
export function getRoleIdFromToken(token) {
  const payload = parseJwt(token)
  if (!payload) return null
  return payload.roleId || null
}

// ==================== PERMISSION CHECKING ====================

/**
 * Kiểm tra user có quyền cụ thể không
 * Admin sẽ bypass tất cả các kiểm tra quyền
 * @param {string} permissionId - ID của quyền cần kiểm tra
 * @returns {boolean}
 */
export function hasPermission(permissionId) {
  // Admin bypass tất cả
  if (isAdmin()) return true
  
  const userPermissions = getUserPermissions()
  return userPermissions.includes(permissionId)
}

/**
 * Kiểm tra user có ít nhất một trong các quyền không
 * Admin sẽ bypass tất cả các kiểm tra quyền
 * @param {string[]} permissionIds - Danh sách ID quyền
 * @returns {boolean}
 */
export function hasAnyPermission(permissionIds) {
  // Admin bypass tất cả
  if (isAdmin()) return true
  
  const userPermissions = getUserPermissions()
  return permissionIds.some(id => userPermissions.includes(id))
}

/**
 * Kiểm tra user có tất cả các quyền không
 * Admin sẽ bypass tất cả các kiểm tra quyền
 * @param {string[]} permissionIds - Danh sách ID quyền
 * @returns {boolean}
 */
export function hasAllPermissions(permissionIds) {
  // Admin bypass tất cả
  if (isAdmin()) return true
  
  const userPermissions = getUserPermissions()
  return permissionIds.every(id => userPermissions.includes(id))
}

/**
 * Kiểm tra user có quyền truy cập route không
 * Admin sẽ bypass tất cả các kiểm tra quyền
 * @param {string} routePath - Đường dẫn route
 * @returns {boolean}
 */
export function canAccessRoute(routePath) {
  // Admin bypass tất cả
  if (isAdmin()) return true
  
  const requiredPermissions = ROUTE_PERMISSIONS[routePath]
  if (!requiredPermissions) return true // Route không yêu cầu quyền
  return hasAnyPermission(requiredPermissions)
}

/**
 * Kiểm tra user có quyền thực hiện feature không
 * Admin sẽ bypass tất cả các kiểm tra quyền
 * @param {string} module - Tên module (permission, role, rolePermission, ...)
 * @param {string} action - Hành động (list, add, edit, delete, ...)
 * @returns {boolean}
 */
export function canDoAction(module, action) {
  // Admin bypass tất cả
  if (isAdmin()) return true
  
  const modulePermissions = FEATURE_PERMISSIONS[module]
  if (!modulePermissions) return false
  
  const requiredPermission = modulePermissions[action]
  if (!requiredPermission) return false
  
  return hasPermission(requiredPermission)
}

// ==================== JWT PARSING ====================

/**
 * Parse JWT token để lấy payload
 * @param {string} token - JWT token
 * @returns {object|null}
 */
export function parseJwt(token) {
  try {
    if (!token || typeof token !== 'string') return null
    
    const parts = token.split('.')
    if (parts.length !== 3) return null
    
    const base64Url = parts[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )
    return JSON.parse(jsonPayload)
  } catch (e) {
    console.error('Error parsing JWT:', e)
    return null
  }
}

/**
 * Kiểm tra token đã hết hạn chưa
 * @param {string} token - JWT token
 * @returns {boolean}
 */
export function isTokenExpired(token) {
  const payload = parseJwt(token)
  if (!payload || !payload.exp) return true
  
  const expTime = payload.exp * 1000 // Convert to milliseconds
  return Date.now() >= expTime
}

/**
 * Lấy permissions từ JWT token (key: "permission")
 * @param {string} token - JWT token
 * @returns {string[]}
 */
export function getPermissionsFromToken(token) {
  const payload = parseJwt(token)
  if (!payload) return []
  
  // Key từ backend là "permission"
  let permissions = payload.permission || payload.permissions || payload.Permissions || []
  
  // Nếu là string (single permission), convert thành array
  if (typeof permissions === 'string') {
    // Loại bỏ GUID rỗng (00000000-0000-0000-0000-000000000000)
    if (permissions === '00000000-0000-0000-0000-000000000000') {
      permissions = []
    } else {
      permissions = [permissions]
    }
  }
  
  // Lọc bỏ các GUID rỗng trong array
  if (Array.isArray(permissions)) {
    permissions = permissions.filter(p => p && p !== '00000000-0000-0000-0000-000000000000')
  }
  
  return Array.isArray(permissions) ? permissions : []
}

/**
 * Kiểm tra isAdmin từ JWT token
 * @param {string} token - JWT token
 * @returns {boolean}
 */
export function getIsAdminFromToken(token) {
  const payload = parseJwt(token)
  if (!payload) return false
  
  // isAdmin có thể là string "true"/"false" hoặc boolean
  const isAdminValue = payload.isAdmin
  if (typeof isAdminValue === 'boolean') return isAdminValue
  if (typeof isAdminValue === 'string') return isAdminValue.toLowerCase() === 'true'
  return false
}

/**
 * Lấy thông tin đầy đủ từ JWT token theo cấu trúc mới
 * @param {string} token - JWT token
 * @returns {object}
 */
export function extractTokenInfo(token) {
  const payload = parseJwt(token)
  if (!payload) return null
  
  return {
    userId: payload.nameid || null,
    username: payload.unique_name || null,
    roleName: payload.role || null,
    roleId: payload.roleId || null,
    isAdmin: getIsAdminFromToken(token),
    permissions: getPermissionsFromToken(token),
    jti: payload.jti || null,
    issuedAt: payload.iat ? new Date(payload.iat * 1000) : null,
    expiresAt: payload.exp ? new Date(payload.exp * 1000) : null,
    notBefore: payload.nbf ? new Date(payload.nbf * 1000) : null
  }
}

/**
 * Lấy toàn bộ payload từ access token hiện tại
 * @returns {object|null}
 */
export function getCurrentTokenPayload() {
  const token = getAccessToken()
  return parseJwt(token)
}

// ==================== ADVANCED PERMISSION CHECKING ====================

/**
 * Kiểm tra user có quyền parent (module) không
 * @param {string} parentPermissionId - ID của quyền parent
 * @returns {boolean}
 */
export function hasParentPermission(parentPermissionId) {
  return hasPermission(parentPermissionId)
}

/**
 * Kiểm tra user có quyền cho một chức năng cụ thể
 * Kiểm tra cả quyền parent và quyền con
 * @param {string} parentPermissionId - ID quyền parent (module)
 * @param {string} childPermissionId - ID quyền con (action)
 * @returns {boolean}
 */
export function hasFeaturePermission(parentPermissionId, childPermissionId) {
  // Phải có quyền parent VÀ quyền con
  return hasPermission(parentPermissionId) && hasPermission(childPermissionId)
}

/**
 * Kiểm tra và trả về object chứa các quyền của một module
 * @param {string} moduleName - Tên module (hoadon, user, role, ...)
 * @returns {object} Object chứa các boolean cho từng action
 */
export function getModulePermissions(moduleName) {
  const modulePerms = FEATURE_PERMISSIONS[moduleName]
  if (!modulePerms) {
    return {
      canList: false,
      canAdd: false,
      canEdit: false,
      canView: false,
      canDelete: false
    }
  }
  
  return {
    canList: modulePerms.list ? hasPermission(modulePerms.list) : false,
    canAdd: modulePerms.add ? hasPermission(modulePerms.add) : false,
    canEdit: modulePerms.edit ? hasPermission(modulePerms.edit) : false,
    canView: modulePerms.view ? hasPermission(modulePerms.view) : false,
    canDelete: modulePerms.delete ? hasPermission(modulePerms.delete) : false
  }
}

/**
 * Lấy danh sách các route mà user có quyền truy cập
 * @returns {string[]}
 */
export function getAccessibleRoutes() {
  const accessibleRoutes = []
  
  for (const [route, permissions] of Object.entries(ROUTE_PERMISSIONS)) {
    if (hasAnyPermission(permissions)) {
      accessibleRoutes.push(route)
    }
  }
  
  return accessibleRoutes
}

/**
 * Kiểm tra và redirect nếu không có quyền
 * @param {string} permissionId - ID quyền cần kiểm tra
 * @param {Function} redirectCallback - Callback để redirect
 * @returns {boolean} - true nếu có quyền, false nếu không
 */
export function checkPermissionOrRedirect(permissionId, redirectCallback) {
  if (!hasPermission(permissionId)) {
    if (typeof redirectCallback === 'function') {
      redirectCallback()
    }
    return false
  }
  return true
}

// Export constants
export { PERMISSIONS, ROUTE_PERMISSIONS, FEATURE_PERMISSIONS }

export default {
  saveTokens,
  getAccessToken,
  getRefreshToken,
  clearTokens,
  isAuthenticated,
  saveUser,
  getUser,
  saveUserPermissions,
  getUserPermissions,
  saveIsAdmin,
  isAdmin,
  saveTokenInfo,
  getTokenInfo,
  hasPermission,
  hasAnyPermission,
  hasAllPermissions,
  canAccessRoute,
  canDoAction,
  parseJwt,
  isTokenExpired,
  getPermissionsFromToken,
  getIsAdminFromToken,
  extractTokenInfo,
  getCurrentTokenPayload,
  hasParentPermission,
  hasFeaturePermission,
  getModulePermissions,
  getAccessibleRoutes,
  checkPermissionOrRedirect,
  saveUserName,
  getUserName,
  saveUserId,
  getUserId,
  saveUserRole,
  getUserRole,
  saveRoleId,
  getRoleId,
  getUserNameFromToken,
  getUserIdFromToken,
  getRoleFromToken,
  getRoleIdFromToken,
  PERMISSIONS,
}
