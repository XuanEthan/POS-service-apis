import { get, post, put, del } from './api'

const ENDPOINT = '/Permissions'

export async function getPermissions() {
  return await get(ENDPOINT)
}

export async function getPermissionById(id) {
  return await get(`${ENDPOINT}/${id}`)
}

export async function createPermission(permission) {
  return await post(ENDPOINT, permission)
}

export async function updatePermission(id, permission) {
  return await put(`${ENDPOINT}/${id}`, permission)
}

export async function deletePermission(id) {
  return await del(`${ENDPOINT}/${id}`)
}

export default {
  getPermissions,
  getPermissionById,
  createPermission,
  updatePermission,
  deletePermission
}
