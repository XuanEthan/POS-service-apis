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
    window.location.href = '/login'
    throw new Error('Không tìm thấy token')
  }

  try {
    const response = await fetch(`${BASE_URL}/Account/refreshtoken`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        accessToken: accessToken,
        refreshToken: refreshTokenValue,
      }),
    })

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

// Generic API helper
async function request(endpoint, options = {}) {
  const url = `${BASE_URL}${endpoint}`

  const accessToken = getAccessToken()
  const refreshTokenValue = getRefreshToken()
  console.log(accessToken, refreshTokenValue)
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
    console.log(response)
    if (response.status === 401) {
      if (!isRefreshing) {
        isRefreshing = true
        try {
          const newToken = await refreshToken()
          onRefreshed(newToken)

          // Retry request với token mới
          config.headers['Authorization'] = `Bearer ${newToken}`
          const retryResponse = await fetch(url, config)

          // Nếu retry vẫn 401, redirect về login
          if (retryResponse.status === 401) {
            clearTokens()
            // window.location.href = '/login'
            throw new Error('Phiên đăng nhập đã hết hạn')
          }

          // Parse response từ retry
          const retryText = await retryResponse.text()
          if (!retryText || retryText.trim() === '') {
            return null
          }

          const retryData = JSON.parse(retryText)

          // Kiểm tra isSuccess = false với message
          if (retryData.isSuccess === false && retryData.message) {
            clearTokens()
            // window.location.href = '/login'
            alert(retryData.message)
            throw new Error(retryData.message)
          }

          return retryData
        } catch (error) {
          clearTokens()
          // window.location.href = '/login'
          console.log(error);
          throw error
        } finally {
          isRefreshing = false
        }
      } else {
        // Đang refresh, đợi hoàn thành rồi retry
        const newToken = await new Promise((resolve) => {
          subscribeTokenRefresh((token) => resolve(token))
        })
        config.headers['Authorization'] = `Bearer ${newToken}`
        const retryResponse = await fetch(url, config)
        
        if (retryResponse.status === 401) {
          clearTokens()
          window.location.href = '/login'
          throw new Error('Phiên đăng nhập đã hết hạn')
        }

        const retryText = await retryResponse.text()
        if (!retryText || retryText.trim() === '') {
          return null
        }

        const retryData = JSON.parse(retryText)

        if (retryData.isSuccess === false && retryData.message) {
          clearTokens()
          window.location.href = '/login'
          alert(retryData.message)
          throw new Error(retryData.message)
        }

        return retryData
      }
    }

    // Check if response is ok
    if (!response.ok) {
      throw new Error(`Lỗi HTTP: ${response.status} ${response.statusText}`)
    }

    // Check if response has content
    const text = await response.text()
    if (!text || text.trim() === '') {
      return null
    }

    // Parse JSON
    const data = JSON.parse(text)

    // Kiểm tra isSuccess = false với message -> redirect login
    if (data.isSuccess === false && data.message) {
      clearTokens()
      window.location.href = '/login'
      alert(data.message)
      throw new Error(data.message)
    }

    return data
  } catch (error) {
    // Throw error để caller xử lý hiển thị message cho user
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
