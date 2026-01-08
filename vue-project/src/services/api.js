import {
  getAccessToken,
  getRefreshToken,
  saveTokens,
  saveUser,
  saveUserPermissions,
  clearTokens,
  getPermissionsFromToken,
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
  refreshSubscribers.forEach((callback) => callback(newToken))
  refreshSubscribers = []
}

/**
 * Refresh token - gọi API để lấy token mới
 */
async function refreshToken() {
  const accessToken = getAccessToken()
  const refreshTokenValue = getRefreshToken()

  if (!accessToken || !refreshTokenValue) {
    clearTokens()
    // window.location.href = '/login'
    throw new Error('Không tìm thấy token')
  }

  try {
    const response = await fetch(`${BASE_URL}/Accounts/refreshtoken`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        accessToken: accessToken,
        refreshToken: refreshTokenValue,
      }),
    })

    console.log('Refresh token response:', response)
    const data = await response.json()
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
    clearTokens()
    window.location.href = '/login'
    alert(data.message)
    throw new Error(data.message || 'Refresh token thất bại')
  } catch (error) {
    clearTokens()
    window.location.href = '/login'
    throw error
  }
}

// Helper: Parse response và xử lý lỗi
async function parseResponse(response, shouldRedirectOnError = true) {
  // Nếu vẫn 401 sau retry, redirect
  if (response.status === 401) {
    clearTokens()
    window.location.href = '/login'
    throw new Error('Phiên đăng nhập đã hết hạn')
  }

  // Check response ok
  if (!response.ok) {
    throw new Error(`Lỗi HTTP: ${response.status} ${response.statusText}`)
  }

  // Parse response text
  const text = await response.text()
  if (!text || text.trim() === '') {
    return null
  }

  const data = JSON.parse(text)

  // Kiểm tra isSuccess = false -> có thể redirect login
  if (data.isSuccess === false && data.message) {
    if (shouldRedirectOnError) {
      clearTokens()
      window.location.href = '/login'
    }
    alert(data.message)
    throw new Error(data.message)
  }

  return data
}

// Generic API helper
async function request(endpoint, options = {}) {
  const url = `${BASE_URL}${endpoint}`
  const accessToken = getAccessToken()
  
  const config = {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    ...options,
  }

  // Thêm Authorization header nếu có token
  if (accessToken) {
    config.headers['Authorization'] = `Bearer ${accessToken}`
  }

  try {
    const response = await fetch(url, config)

    // Handle 401 Unauthorized - gọi refresh token
    if (response.status === 401) {
      let newToken

      if (!isRefreshing) {
        // Bắt đầu refresh token
        isRefreshing = true
        try {
          newToken = await refreshToken()
          onRefreshed(newToken)
        } catch (error) {
          clearTokens()
          window.location.href = '/login'
          throw error
        } finally {
          isRefreshing = false
        }
      } else {
        // Đang refresh, đợi hoàn thành
        newToken = await new Promise((resolve) => {
          subscribeTokenRefresh((token) => resolve(token))
        })
      }

      // Retry request với token mới
      config.headers['Authorization'] = `Bearer ${newToken}`
      const retryResponse = await fetch(url, config)
      return parseResponse(retryResponse, false)
    }

    // Parse response bình thường
    return parseResponse(response, true)
  } catch (error) {
    console.log(error)
    throw error
  }
}

// GET request
export async function get(endpoint) {
  return request(endpoint, { method: 'GET' })
}

// POST request
export async function post(endpoint, body, headers = {}) {
  let bodyToSend = body
  const contentType = headers['Content-Type'] || headers['content-type'] || ''

  // If caller didn't specify content-type and body is an object, send JSON
  if (!contentType) {
    bodyToSend = JSON.stringify(body)
    headers['Content-Type'] = 'application/json'
  } else if (contentType.includes('application/json')) {
    bodyToSend = JSON.stringify(body)
  }

  return request(endpoint, {
    method: 'POST',
    headers,
    body: bodyToSend,
  })
}

// PUT request
export async function put(endpoint, body) {
  return request(endpoint, {
    method: 'PUT',
    body: JSON.stringify(body),
  })
}

// DELETE request
export async function del(endpoint) {
  return request(endpoint, { method: 'DELETE' })
}

export default { get, post, put, del, BASE_URL }
