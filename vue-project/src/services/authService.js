import { 
  saveTokens, 
  saveUser, 
  saveUserPermissions, 
  clearTokens,
  getPermissionsFromToken,
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
    const response = await fetch(`${BASE_URL}/Account/login`, {
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
      
      // Lấy và lưu permissions từ token
      if (data.accessToken) {
        const permissions = getPermissionsFromToken(data.accessToken)
        saveUserPermissions(permissions)
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
    const response = await fetch(`${BASE_URL}/Account/refreshtoken`, {
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
      
      // Cập nhật permissions từ token mới
      if (data.accessToken) {
        const permissions = getPermissionsFromToken(data.accessToken)
        saveUserPermissions(permissions)
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
