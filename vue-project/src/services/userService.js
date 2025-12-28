import { get, post, put, del } from './api'

const ENDPOINT = '/User/users'

export async function getUsers() {
  return await get(ENDPOINT)
}

export async function getUserById(id) {
  return await get(`${ENDPOINT}/${id}`)
}

export async function createUser(user) {
  return await post(ENDPOINT, user)
}

export async function updateUser(id, user) {
  return await put(`${ENDPOINT}/${id}`, user)
}

export async function deleteUser(id) {
  return await del(`${ENDPOINT}/${id}`)
}

export default {
  getUsers,
  getUserById,
  createUser,
  updateUser,
  deleteUser
}
