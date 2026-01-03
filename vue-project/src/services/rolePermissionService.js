import { get, post, put, del } from './api'

const ENDPOINT = '/RolePermission/rolepermissions'

export async function getRolePermissions(filters) {
  let qs = ''
  if(filters !=null){
    qs = new URLSearchParams(filters).toString()
  }
  return await get(`${ENDPOINT}?${qs}`)
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
