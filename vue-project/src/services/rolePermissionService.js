import { get, post, put, del } from './api'

const ENDPOINT = '/RolePermission/role_permissions'

export async function getRolePermissions() {
  return await get(ENDPOINT)
}

export async function getRolePermissionById(id) {
  return await get(`${ENDPOINT}/${id}`)
}

export async function createRolePermission(rolePermission) {
  return await post(ENDPOINT, rolePermission)
}

export async function updateRolePermission(id, rolePermission) {
  return await put(`${ENDPOINT}/${id}`, rolePermission)
}

export async function deleteRolePermission(id) {
  return await del(`${ENDPOINT}/${id}`)
}

export default {
  getRolePermissions,
  getRolePermissionById,
  createRolePermission,
  updateRolePermission,
  deleteRolePermission
}
