import { 
  getAccessToken, 
  getRefreshToken, 
  saveTokens, 
  saveUser, 
  saveUserPermissions,
  clearTokens,
  getPermissionsFromToken
} from '@/utils/auth'

// API Configuration
const BASE_URL = 'https://localhost:7219/api'

// Flag để tránh gọi refresh token nhiều lần đồng thời
let isRefreshing = false
let refreshSubscribers = []

// Thêm subscriber để đợi refresh token hoàn thành
function subscribeTokenRefresh(callback) {
  refreshSubscribers.push(callback)
}

// Notify tất cả subscribers khi refresh xong
function onRefreshed(newToken) {
  refreshSubscribers.forEach(callback => callback(newToken))
  refreshSubscribers = []
}

/**
 * Refresh token - gọi API để lấy token mới
 */
async function refreshToken() {
  const accessToken = getAccessToken()
  const refreshTokenValue = getRefreshToken()
  
  if (!accessToken || !refreshTokenValue) {
    throw new Error('No tokens available')
  }
  
  const response = await fetch(`${BASE_URL}/Account/refreshtoken`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      accessToken: accessToken,
      refreshToken: refreshTokenValue
    })
  })
  
  const data = await response.json()
  
  // API trả về: { isSuccess, code, message, userId, object, accessToken, refreshToken }
  if (data.isSuccess && data.accessToken && data.refreshToken) {
    saveTokens(data.accessToken, data.refreshToken)
    
    // Cập nhật permissions từ token mới
    const permissions = getPermissionsFromToken(data.accessToken)
    saveUserPermissions(permissions)
    
    if (data.object) {
      saveUser(data.object)
    }
    
    return data.accessToken
  }
  
  throw new Error(data.message || 'Failed to refresh token')
}

// Generic API helper
async function request(endpoint, options = {}) {
  const url = `${BASE_URL}${endpoint}`
  
  const accessToken = getAccessToken()
  const refreshTokenValue = getRefreshToken()
  console.log(accessToken , refreshTokenValue);
  const config = {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers
    },
    ...options
  }
  
  // Thêm Authorization header nếu có token
  if (accessToken) {
    config.headers['Authorization'] = `Bearer ${accessToken}`
  }

  try {
    const response = await fetch(url, config)
    // Handle 401 Unauthorized - gọi refresh token
    if (response.status === 401) {
      if (!isRefreshing) {
        isRefreshing = true
        try {
          const newToken = await refreshToken()
          onRefreshed(newToken)
          
          // Retry request với token mới
          config.headers['Authorization'] = `Bearer ${newToken}`
          const retryResponse = await fetch(url, config)
          
          if (!retryResponse.ok) {
            // Nếu retry vẫn lỗi, redirect về login
            clearTokens()
            window.location.href = '/login'
            return {
              isSuccess: false,
              code: 401,
              message: 'Session expired. Please login again.',
              object: null
            }
          }
          
          const retryText = await retryResponse.text()
          if (!retryText || retryText.trim() === '') {
            return { isSuccess: true, code: 0, message: 'Success', object: [] }
          }
          return JSON.parse(retryText)
        } catch (error) {
          clearTokens()
          window.location.href = '/login'
          return {
            isSuccess: false,
            code: 401,
            message: 'Session expired. Please login again.',
            object: null
          }
        } finally {
          isRefreshing = false
        }
      } else {
        // Đang refresh, đợi hoàn thành rồi retry
        const newToken = await new Promise(resolve => {
          subscribeTokenRefresh(token => resolve(token))
        })
        config.headers['Authorization'] = `Bearer ${newToken}`
        const retryResponse = await fetch(url, config)
        const retryText = await retryResponse.text()
        if (!retryText || retryText.trim() === '') {
          return { isSuccess: true, code: 0, message: 'Success', object: [] }
        }
        return JSON.parse(retryText)
      }
    }
    
    // Check if response is ok
    if (!response.ok) {
      return {
        isSuccess: false,
        code: response.status,
        message: `HTTP Error: ${response.status} ${response.statusText}`,
        id: null,
        object: null
      }
    }
    
    // Check if response has content
    const text = await response.text()
    if (!text || text.trim() === '') {
      return {
        isSuccess: true,
        code: 0,
        message: 'Success (empty response)',
        id: null,
        object: []
      }
    }
    
    // Parse JSON
    try {
      const data = JSON.parse(text)
      // API trả về dạng: { isSuccess, code, message, id, object }
      return data
    } catch (parseError) {
      console.error('JSON Parse Error:', parseError, 'Response text:', text)
      return {
        isSuccess: false,
        code: -2,
        message: 'Invalid JSON response from server',
        id: null,
        object: null
      }
    }
  } catch (error) {
    console.error('API Error:', error)
    return {
      isSuccess: false,
      code: -1,
      message: error.message || 'Network error',
      id: null,
      object: null
    }
  }
}

// GET request
export async function get(endpoint) {
  return request(endpoint, { method: 'GET' })
}

// POST request
export async function post(endpoint, body) {
  return request(endpoint, {
    method: 'POST',
    body: JSON.stringify(body)
  })
}

// PUT request
export async function put(endpoint, body) {
  return request(endpoint, {
    method: 'PUT',
    body: JSON.stringify(body)
  })
}

// DELETE request
export async function del(endpoint) {
  return request(endpoint, { method: 'DELETE' })
}

export default { get, post, put, del, BASE_URL }
