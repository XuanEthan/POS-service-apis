import { ref, computed, onMounted, watch } from 'vue'
import { 
  hasPermission, 
  hasAnyPermission, 
  hasAllPermissions,
  canDoAction,
  canAccessRoute,
  getModulePermissions,
  getUserPermissions,
  getAccessibleRoutes,
  hasFeaturePermission,
  isAdmin,
  getTokenInfo,
  PERMISSIONS,
  FEATURE_PERMISSIONS,
  ROUTE_PERMISSIONS
} from '@/utils/auth'

/**
 * Composable để quản lý và kiểm tra quyền trong Vue components
 * Admin sẽ bypass tất cả các kiểm tra quyền
 * @param {string} moduleName - Tên module (optional) để lấy quyền của module đó
 * @returns {object} - Object chứa các hàm và reactive properties liên quan đến quyền
 */
export function usePermission(moduleName = null) {
  // Reactive state
  const userPermissions = ref([])
  const isLoading = ref(true)
  const userIsAdmin = ref(false)
  const tokenInfo = ref(null)
  
  // Load permissions khi mount
  onMounted(() => {
    loadPermissions()
  })
  
  /**
   * Load lại permissions từ localStorage
   */
  function loadPermissions() {
    isLoading.value = true
    userPermissions.value = getUserPermissions()
    userIsAdmin.value = isAdmin()
    tokenInfo.value = getTokenInfo()
    isLoading.value = false
  }
  
  /**
   * Refresh permissions (call sau khi login hoặc token refresh)
   */
  function refreshPermissions() {
    loadPermissions()
  }
  
  // ==================== COMPUTED PROPERTIES ====================
  
  /**
   * Kiểm tra user có phải admin không
   */
  const isUserAdmin = computed(() => userIsAdmin.value)
  
  /**
   * Quyền của module hiện tại (nếu có moduleName)
   */
  const modulePermissions = computed(() => {
    if (!moduleName) return null
    return getModulePermissions(moduleName)
  })
  
  /**
   * Kiểm tra có quyền xem danh sách không
   */
  const canList = computed(() => {
    if (!moduleName) return false
    return modulePermissions.value?.canList || false
  })
  
  /**
   * Kiểm tra có quyền thêm không
   */
  const canAdd = computed(() => {
    if (!moduleName) return false
    return modulePermissions.value?.canAdd || false
  })
  
  /**
   * Kiểm tra có quyền sửa không
   */
  const canEdit = computed(() => {
    if (!moduleName) return false
    return modulePermissions.value?.canEdit || false
  })
  
  /**
   * Kiểm tra có quyền xem chi tiết không
   */
  const canView = computed(() => {
    if (!moduleName) return false
    return modulePermissions.value?.canView || false
  })
  
  /**
   * Kiểm tra có quyền xóa không
   */
  const canDelete = computed(() => {
    if (!moduleName) return false
    return modulePermissions.value?.canDelete || false
  })
  
  /**
   * Danh sách routes có thể truy cập
   */
  const accessibleRoutes = computed(() => {
    return getAccessibleRoutes()
  })
  
  // ==================== METHODS ====================
  
  /**
   * Kiểm tra có quyền cụ thể không
   * @param {string} permissionId - ID quyền
   * @returns {boolean}
   */
  function checkPermission(permissionId) {
    return hasPermission(permissionId)
  }
  
  /**
   * Kiểm tra có ít nhất một trong các quyền không
   * @param {string[]} permissionIds - Danh sách ID quyền
   * @returns {boolean}
   */
  function checkAnyPermission(permissionIds) {
    return hasAnyPermission(permissionIds)
  }
  
  /**
   * Kiểm tra có tất cả các quyền không
   * @param {string[]} permissionIds - Danh sách ID quyền
   * @returns {boolean}
   */
  function checkAllPermissions(permissionIds) {
    return hasAllPermissions(permissionIds)
  }
  
  /**
   * Kiểm tra có quyền thực hiện action trên module không
   * @param {string} module - Tên module
   * @param {string} action - Tên action (list, add, edit, view, delete)
   * @returns {boolean}
   */
  function checkAction(module, action) {
    return canDoAction(module, action)
  }
  
  /**
   * Kiểm tra có quyền truy cập route không
   * @param {string} routePath - Đường dẫn route
   * @returns {boolean}
   */
  function checkRoute(routePath) {
    return canAccessRoute(routePath)
  }
  
  /**
   * Kiểm tra quyền parent và child
   * @param {string} parentId - ID quyền parent
   * @param {string} childId - ID quyền child
   * @returns {boolean}
   */
  function checkFeature(parentId, childId) {
    return hasFeaturePermission(parentId, childId)
  }
  
  /**
   * Lấy quyền của một module bất kỳ
   * @param {string} module - Tên module
   * @returns {object}
   */
  function getPermissionsForModule(module) {
    return getModulePermissions(module)
  }
  
  return {
    // State
    userPermissions,
    isLoading,
    userIsAdmin,
    tokenInfo,
    
    // Computed
    isUserAdmin,
    modulePermissions,
    canList,
    canAdd,
    canEdit,
    canView,
    canDelete,
    accessibleRoutes,
    
    // Methods
    loadPermissions,
    refreshPermissions,
    checkPermission,
    checkAnyPermission,
    checkAllPermissions,
    checkAction,
    checkRoute,
    checkFeature,
    getPermissionsForModule,
    
    // Constants
    PERMISSIONS,
    FEATURE_PERMISSIONS,
    ROUTE_PERMISSIONS
  }
}

export default usePermission