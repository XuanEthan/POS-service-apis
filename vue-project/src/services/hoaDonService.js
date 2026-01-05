import api from './api'

// ===== FAKE DATA SERVICE =====
// Dữ liệu giả để phát triển frontend
let fakeInvoices = [
  {
    id: 1,
    invoiceNumber: 'HD-2025-001',
    customerName: 'Cửa hàng ABC',
    customerPhone: '0901234567',
    customerAddress: '123 Nguyễn Văn Linh, Q.7, TP.HCM',
    invoiceDate: '2025-01-01',
    dueDate: '2025-01-31',
    status: 'paid',
    taxRate: 10,
    discount: 50000,
    notes: 'Giao hàng trong giờ hành chính',
    items: [
      { productName: 'Sản phẩm A', quantity: 10, unitPrice: 50000 },
      { productName: 'Sản phẩm B', quantity: 5, unitPrice: 80000 }
    ]
  },
  {
    id: 2,
    invoiceNumber: 'HD-2025-002',
    customerName: 'Cửa hàng XYZ',
    customerPhone: '0912345678',
    customerAddress: '456 Lê Văn Việt, Q.9, TP.HCM',
    invoiceDate: '2025-01-02',
    dueDate: '2025-02-02',
    status: 'pending',
    taxRate: 10,
    discount: 0,
    notes: '',
    items: [
      { productName: 'Sản phẩm C', quantity: 20, unitPrice: 45000 },
      { productName: 'Sản phẩm D', quantity: 15, unitPrice: 60000 }
    ]
  },
  {
    id: 3,
    invoiceNumber: 'HD-2025-003',
    customerName: 'Siêu thị MiniMart',
    customerPhone: '0923456789',
    customerAddress: '789 Võ Văn Ngân, Thủ Đức, TP.HCM',
    invoiceDate: '2024-12-15',
    dueDate: '2025-01-15',
    status: 'overdue',
    taxRate: 8,
    discount: 100000,
    notes: 'Khách hàng VIP',
    items: [
      { productName: 'Sản phẩm E', quantity: 30, unitPrice: 35000 },
      { productName: 'Sản phẩm F', quantity: 25, unitPrice: 42000 }
    ]
  },
  {
    id: 4,
    invoiceNumber: 'HD-2025-004',
    customerName: 'Cửa hàng Tiện Lợi 24/7',
    customerPhone: '0934567890',
    customerAddress: '321 Phan Văn Trị, Gò Vấp, TP.HCM',
    invoiceDate: '2025-01-03',
    dueDate: '2025-02-03',
    status: 'pending',
    taxRate: 10,
    discount: 0,
    notes: 'Giao hàng trước 9h sáng',
    items: [
      { productName: 'Sản phẩm G', quantity: 12, unitPrice: 55000 },
      { productName: 'Sản phẩm H', quantity: 8, unitPrice: 70000 }
    ]
  },
  {
    id: 5,
    invoiceNumber: 'HD-2025-005',
    customerName: 'Cửa hàng Thực Phẩm Sạch',
    customerPhone: '0945678901',
    customerAddress: '654 Hoàng Văn Thụ, Phú Nhuận, TP.HCM',
    invoiceDate: '2025-01-04',
    dueDate: '2025-01-19',
    status: 'draft',
    taxRate: 5,
    discount: 25000,
    notes: 'Đơn hàng thử nghiệm',
    items: [
      { productName: 'Sản phẩm I', quantity: 50, unitPrice: 25000 }
    ]
  }
]

let nextId = 6

// Delay để giả lập API call
const delay = (ms = 500) => new Promise(resolve => setTimeout(resolve, ms))

// Tính tổng tiền cho hóa đơn
function calculateInvoiceTotal(invoice) {
  if (!invoice.items || !Array.isArray(invoice.items)) {
    return 0
  }
  
  const subtotal = invoice.items.reduce((sum, item) => {
    return sum + (item.quantity * item.unitPrice)
  }, 0)
  
  const tax = subtotal * (invoice.taxRate || 0) / 100
  const total = subtotal + tax - (invoice.discount || 0)
  
  return Math.max(0, total)
}

// ===== API METHODS =====

// Lấy danh sách hóa đơn
export async function getInvoices(params = {}) {
  await delay()
  
  // Lọc theo từ khóa
  let filtered = [...fakeInvoices]
  
  if (params.keyword) {
    const keyword = params.keyword.toLowerCase()
    filtered = filtered.filter(inv => 
      inv.invoiceNumber.toLowerCase().includes(keyword) ||
      inv.customerName.toLowerCase().includes(keyword) ||
      inv.customerPhone.includes(keyword)
    )
  }
  
  // Lọc theo trạng thái
  if (params.status && params.status !== 'all') {
    filtered = filtered.filter(inv => inv.status === params.status)
  }
  
  // Thêm tổng tiền vào mỗi hóa đơn
  filtered = filtered.map(inv => ({
    ...inv,
    totalAmount: calculateInvoiceTotal(inv)
  }))
  
  return {
    success: true,
    data: filtered,
    total: filtered.length
  }
}

// Lấy chi tiết hóa đơn theo ID
export async function getInvoiceById(id) {
  await delay()
  
  const invoice = fakeInvoices.find(inv => inv.id === parseInt(id))
  
  if (!invoice) {
    return {
      success: false,
      message: 'Không tìm thấy hóa đơn'
    }
  }
  
  return {
    success: true,
    data: {
      ...invoice,
      totalAmount: calculateInvoiceTotal(invoice)
    }
  }
}

// Tạo hóa đơn mới
export async function createInvoice(invoiceData) {
  await delay()
  
  // Tạo mã hóa đơn tự động nếu chưa có
  if (!invoiceData.invoiceNumber) {
    const now = new Date()
    const year = now.getFullYear()
    const count = String(nextId).padStart(3, '0')
    invoiceData.invoiceNumber = `HD-${year}-${count}`
  }
  
  const newInvoice = {
    id: nextId++,
    ...invoiceData,
    items: invoiceData.items || []
  }
  
  fakeInvoices.unshift(newInvoice)
  
  return {
    success: true,
    message: 'Thêm hóa đơn thành công',
    data: {
      ...newInvoice,
      totalAmount: calculateInvoiceTotal(newInvoice)
    }
  }
}

// Cập nhật hóa đơn
export async function updateInvoice(id, invoiceData) {
  await delay()
  
  const index = fakeInvoices.findIndex(inv => inv.id === parseInt(id))
  
  if (index === -1) {
    return {
      success: false,
      message: 'Không tìm thấy hóa đơn'
    }
  }
  
  fakeInvoices[index] = {
    ...fakeInvoices[index],
    ...invoiceData,
    id: parseInt(id)
  }
  
  return {
    success: true,
    message: 'Cập nhật hóa đơn thành công',
    data: {
      ...fakeInvoices[index],
      totalAmount: calculateInvoiceTotal(fakeInvoices[index])
    }
  }
}

// Xóa hóa đơn
export async function deleteInvoice(id) {
  await delay()
  
  const index = fakeInvoices.findIndex(inv => inv.id === parseInt(id))
  
  if (index === -1) {
    return {
      success: false,
      message: 'Không tìm thấy hóa đơn'
    }
  }
  
  fakeInvoices.splice(index, 1)
  
  return {
    success: true,
    message: 'Xóa hóa đơn thành công'
  }
}

// Xóa nhiều hóa đơn
export async function deleteMultipleInvoices(ids) {
  await delay()
  
  fakeInvoices = fakeInvoices.filter(inv => !ids.includes(inv.id))
  
  return {
    success: true,
    message: `Đã xóa ${ids.length} hóa đơn`
  }
}

// Xuất hóa đơn (giả lập)
export async function exportInvoice(id) {
  await delay()
  
  const invoice = fakeInvoices.find(inv => inv.id === parseInt(id))
  
  if (!invoice) {
    return {
      success: false,
      message: 'Không tìm thấy hóa đơn'
    }
  }
  
  // Trong thực tế sẽ tạo file PDF hoặc Excel
  console.log('Xuất hóa đơn:', invoice)
  
  return {
    success: true,
    message: 'Xuất hóa đơn thành công',
    data: invoice
  }
}

// Gửi email hóa đơn (giả lập)
export async function sendInvoiceEmail(id, email) {
  await delay()
  
  const invoice = fakeInvoices.find(inv => inv.id === parseInt(id))
  
  if (!invoice) {
    return {
      success: false,
      message: 'Không tìm thấy hóa đơn'
    }
  }
  
  console.log(`Gửi hóa đơn ${invoice.invoiceNumber} đến email: ${email}`)
  
  return {
    success: true,
    message: 'Đã gửi hóa đơn qua email'
  }
}

export default {
  getInvoices,
  getInvoiceById,
  createInvoice,
  updateInvoice,
  deleteInvoice,
  deleteMultipleInvoices,
  exportInvoice,
  sendInvoiceEmail
}
