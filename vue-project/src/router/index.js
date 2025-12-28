import { createRouter, createWebHistory } from 'vue-router'
import { isAuthenticated, canAccessRoute } from '@/utils/auth'

// Import views
import DashboardView from '@/views/DashboardView.vue'
import HoaDonListView from '@/views/hoa-don/HoaDonListView.vue'
import NguoiDungListView from '@/views/nguoi-dung/NguoiDungListView.vue'
import ThanhToanView from '@/views/thanh-toan/ThanhToanView.vue'
import RoleListView from '@/views/role/RoleListView.vue'
import PermissionListView from '@/views/permission/PermissionListView.vue'
import RolePermissionListView from '@/views/role-permission/RolePermissionListView.vue'
import LoginView from '@/views/auth/LoginView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { title: 'Đăng nhập', requiresAuth: false }
    },
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { title: 'Tổng quan', requiresAuth: true }
    },
    {
      path: '/hoa-don',
      name: 'hoa-don',
      component: HoaDonListView,
      meta: { title: 'Quản lý hóa đơn', requiresAuth: true }
    },
    {
      path: '/nguoi-dung',
      name: 'nguoi-dung',
      component: NguoiDungListView,
      meta: { title: 'Quản lý người dùng', requiresAuth: true }
    },
    {
      path: '/thanh-toan',
      name: 'thanh-toan',
      component: ThanhToanView,
      meta: { title: 'Thanh toán', requiresAuth: true }
    },
    {
      path: '/role',
      name: 'role',
      component: RoleListView,
      meta: { title: 'Quản lý vai trò', requiresAuth: true }
    },
    {
      path: '/permission',
      name: 'permission',
      component: PermissionListView,
      meta: { title: 'Quản lý quyền', requiresAuth: true }
    },
    {
      path: '/role-permission',
      name: 'role-permission',
      component: RolePermissionListView,
      meta: { title: 'Quản lý phân quyền', requiresAuth: true }
    }
  ],
})

// Navigation Guard - Kiểm tra đăng nhập và quyền truy cập
router.beforeEach((to, from, next) => {
  // Cập nhật title trang
  document.title = to.meta.title ? `${to.meta.title} - POS System` : 'POS System'
  
  // Kiểm tra route yêu cầu đăng nhập
  if (to.meta.requiresAuth !== false) {
    if (!isAuthenticated()) {
      // Chưa đăng nhập -> chuyển đến trang login
      next({ name: 'login', query: { redirect: to.fullPath } })
      return
    }
    
    // Kiểm tra quyền truy cập route (tùy chọn)
    // if (!canAccessRoute(to.path)) {
    //   next({ name: 'dashboard' })
    //   return
    // }
  }
  
  // Nếu đã đăng nhập mà vào trang login -> chuyển về dashboard
  if (to.name === 'login' && isAuthenticated()) {
    next({ name: 'dashboard' })
    return
  }
  
  next()
})

export default router
