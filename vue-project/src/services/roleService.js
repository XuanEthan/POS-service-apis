import { get, post, put, del } from './api'

const ENDPOINT = '/Roles'

export async function getRoles() {
  return await get(ENDPOINT)
}

export async function getRoleById(id) {
  return await get(`${ENDPOINT}/${id}`)
}

export async function createRole(role) {
  return await post(ENDPOINT, role)
}

export async function updateRole(id, role) {
  return await put(`${ENDPOINT}/${id}`, role)
}

export async function deleteRole(id) {
  return await del(`${ENDPOINT}/${id}`)
}

export default {
  getRoles,
  getRoleById,
  createRole,
  updateRole,
  deleteRole
}
