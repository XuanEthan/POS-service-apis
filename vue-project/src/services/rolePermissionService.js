import { get, post, put, del } from './api'

const ENDPOINT = '/RolePermission/rolepermissions'

export async function getRolePermissions(roleId = null) {
  const params = roleId ? `?roleId=${roleId}` : ''
  return await get(`${ENDPOINT}${params}`)
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
