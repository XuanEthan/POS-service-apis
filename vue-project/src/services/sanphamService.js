import { get, post, put, del } from './api'

const ENDPOINT = '/Sanpham/sanphams'

export async function getSanphams(params) {
  let queryString = ''
  if (params != null) {
    queryString = new URLSearchParams(params).toString()
  }
  return await get(`${ENDPOINT}?${queryString}`)
}

export async function getSanphamById(id) {
  return await get(`${ENDPOINT}/${id}`)
}

export async function createSanpham(sanpham) {
  return await post(ENDPOINT, sanpham)
}

export async function updateSanpham(id, sanpham) {
  return await put(`${ENDPOINT}/${id}`, sanpham)
}

export async function deleteSanpham(id) {
  return await del(`${ENDPOINT}/${id}`)
}

export default {
  getSanphams,
  getSanphamById,
  createSanpham,
  updateSanpham,
  deleteSanpham
}
