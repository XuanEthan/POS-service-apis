import { 
  saveTokens, 
  saveUser, 
  saveUserPermissions,
  saveIsAdmin,
  saveTokenInfo,
  saveUserName,
  saveUserId,
  saveUserRole,
  saveRoleId,
  clearTokens,
  getPermissionsFromToken,
  getIsAdminFromToken,
  extractTokenInfo,
  getUserNameFromToken,
  getUserIdFromToken,
  getRoleFromToken,
  getRoleIdFromToken,
  getAccessToken,
  getRefreshToken
} from '@/utils/auth'

const BASE_URL = 'https://localhost:7219/api'

/**
 * Đăng nhập
 * @param {string} username 
 * @param {string} password 
 * @returns {Promise<object>}
 */
export async function login(username, password) {
  try {
    const response = await fetch(`${BASE_URL}/Accounts/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ userName: username, pwd : password , rememberMe: true })
    })
    
    const data = await response.json()
    
    if (data.isSuccess) {
      // Lưu tokens
      if (data.accessToken && data.refreshToken) {
        saveTokens(data.accessToken, data.refreshToken)
      }
      
      // Lưu user info
      if (data.object) {
        saveUser(data.object)
      }
      
      // Lấy và lưu thông tin từ token
      if (data.accessToken) {
        // Lưu permissions
        const permissions = getPermissionsFromToken(data.accessToken)
        saveUserPermissions(permissions)
        
        // Lưu trạng thái admin
        const isAdminValue = getIsAdminFromToken(data.accessToken)
        saveIsAdmin(isAdminValue)
        
        // Lưu username (unique_name)
        const userName = getUserNameFromToken(data.accessToken)
        if (userName) saveUserName(userName)
        
        // Lưu user ID (nameid)
        const userId = getUserIdFromToken(data.accessToken)
        if (userId) saveUserId(userId)
        
        // Lưu role name
        const roleName = getRoleFromToken(data.accessToken)
        if (roleName) saveUserRole(roleName)
        
        // Lưu role ID
        const roleId = getRoleIdFromToken(data.accessToken)
        if (roleId) saveRoleId(roleId)
        
        // Lưu thông tin đầy đủ từ token
        const tokenInfo = extractTokenInfo(data.accessToken)
        saveTokenInfo(tokenInfo)
      }
    }
    
    return data
  } catch (error) {
    console.error('Login error:', error)
    return {
      isSuccess: false,
      code: -1,
      message: error.message || 'Lỗi kết nối server',
      object: null
    }
  }
}

/**
 * Refresh token
 * @returns {Promise<object>}
 */
export async function refreshToken() {
  const accessToken = getAccessToken()
  const refreshTokenValue = getRefreshToken()
  
  if (!accessToken || !refreshTokenValue) {
    return {
      isSuccess: false,
      code: -1,
      message: 'No tokens available',
      object: null
    }
  }
  
  try {
    const response = await fetch(`${BASE_URL}/Accounts/refreshtoken`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        accessToken,
        refreshToken: refreshTokenValue
      })
    })
    
    const data = await response.json()
    
    if (data.isSuccess) {
      // Lưu tokens mới
      if (data.accessToken && data.refreshToken) {
        saveTokens(data.accessToken, data.refreshToken)
      }
      
      // Cập nhật user info
      if (data.object) {
        saveUser(data.object)
      }
      
      // Cập nhật thông tin từ token mới
      if (data.accessToken) {
        // Lưu permissions
        const permissions = getPermissionsFromToken(data.accessToken)
        saveUserPermissions(permissions)
        
        // Lưu trạng thái admin
        const isAdminValue = getIsAdminFromToken(data.accessToken)
        saveIsAdmin(isAdminValue)
        
        // Lưu username (unique_name)
        const userName = getUserNameFromToken(data.accessToken)
        if (userName) saveUserName(userName)
        
        // Lưu user ID (nameid)
        const userId = getUserIdFromToken(data.accessToken)
        if (userId) saveUserId(userId)
        
        // Lưu role name
        const roleName = getRoleFromToken(data.accessToken)
        if (roleName) saveUserRole(roleName)
        
        // Lưu role ID
        const roleId = getRoleIdFromToken(data.accessToken)
        if (roleId) saveRoleId(roleId)
        
        // Lưu thông tin đầy đủ từ token
        const tokenInfo = extractTokenInfo(data.accessToken)
        saveTokenInfo(tokenInfo)
      }
    }
    
    return data
  } catch (error) {
    console.error('Refresh token error:', error)
    return {
      isSuccess: false,
      code: -1,
      message: error.message || 'Lỗi kết nối server',
      object: null
    }
  }
}

/**
 * Đăng xuất
 */
export function logout() {
  clearTokens()
}

export default {
  login,
  refreshToken,
  logout
}
