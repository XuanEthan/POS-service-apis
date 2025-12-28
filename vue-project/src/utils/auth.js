import { PERMISSIONS, ROUTE_PERMISSIONS, FEATURE_PERMISSIONS } from '@/constants/permissions'

// Token storage keys
const ACCESS_TOKEN_KEY = 'access_token'
const REFRESH_TOKEN_KEY = 'refresh_token'
const USER_KEY = 'user_info'
const USER_PERMISSIONS_KEY = 'user_permissions'

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

// ==================== PERMISSION CHECKING ====================

/**
 * Kiểm tra user có quyền cụ thể không
 * @param {string} permissionId - ID của quyền cần kiểm tra
 * @returns {boolean}
 */
export function hasPermission(permissionId) {
  const userPermissions = getUserPermissions()
  return userPermissions.includes(permissionId)
}

/**
 * Kiểm tra user có ít nhất một trong các quyền không
 * @param {string[]} permissionIds - Danh sách ID quyền
 * @returns {boolean}
 */
export function hasAnyPermission(permissionIds) {
  const userPermissions = getUserPermissions()
  return permissionIds.some(id => userPermissions.includes(id))
}

/**
 * Kiểm tra user có tất cả các quyền không
 * @param {string[]} permissionIds - Danh sách ID quyền
 * @returns {boolean}
 */
export function hasAllPermissions(permissionIds) {
  const userPermissions = getUserPermissions()
  return permissionIds.every(id => userPermissions.includes(id))
}

/**
 * Kiểm tra user có quyền truy cập route không
 * @param {string} routePath - Đường dẫn route
 * @returns {boolean}
 */
export function canAccessRoute(routePath) {
  const requiredPermissions = ROUTE_PERMISSIONS[routePath]
  if (!requiredPermissions) return true // Route không yêu cầu quyền
  return hasAnyPermission(requiredPermissions)
}

/**
 * Kiểm tra user có quyền thực hiện feature không
 * @param {string} module - Tên module (permission, role, rolePermission, ...)
 * @param {string} action - Hành động (list, add, edit, delete, ...)
 * @returns {boolean}
 */
export function canDoAction(module, action) {
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
    const base64Url = token.split('.')[1]
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
 * Lấy permissions từ JWT token
 * @param {string} token - JWT token
 * @returns {string[]}
 */
export function getPermissionsFromToken(token) {
  const payload = parseJwt(token)
  if (!payload) return []
  
  // Tùy thuộc vào cấu trúc JWT của backend
  // Có thể là payload.permissions, payload.Permissions, hoặc claim khác
  return payload.permissions || payload.Permissions || payload.permission || []
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
  hasPermission,
  hasAnyPermission,
  hasAllPermissions,
  canAccessRoute,
  canDoAction,
  parseJwt,
  isTokenExpired,
  getPermissionsFromToken,
  PERMISSIONS,
}
