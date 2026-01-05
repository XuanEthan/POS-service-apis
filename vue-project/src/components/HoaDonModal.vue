<script setup>
import { ref, watch, computed } from 'vue'

const props = defineProps({
  show: {
    type: Boolean,
    required: true
  },
  mode: {
    type: String,
    required: true,
    validator: (value) => ['create', 'edit', 'view'].includes(value)
  },
  invoice: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['close', 'save'])

const localInvoice = ref({ ...props.invoice })

// Watch for invoice changes
watch(() => props.invoice, (newInvoice) => {
  localInvoice.value = { ...newInvoice }
}, { deep: true })

function handleClose() {
  emit('close')
}

function handleSave() {
  emit('save', localInvoice.value)
}

const modalTitle = computed(() => {
  const titles = {
    create: '🧾 Thêm hóa đơn mới',
    edit: '🧾 Sửa hóa đơn',
    view: '🧾 Chi tiết hóa đơn'
  }
  return titles[props.mode]
})

// Tính tổng tiền hàng
const subtotal = computed(() => {
  if (!localInvoice.value.items || !Array.isArray(localInvoice.value.items)) {
    return 0
  }
  return localInvoice.value.items.reduce((sum, item) => {
    return sum + (item.quantity * item.unitPrice)
  }, 0)
})

// Tính VAT
const taxAmount = computed(() => {
  return subtotal.value * (localInvoice.value.taxRate || 0) / 100
})

// Tính tổng cộng
const total = computed(() => {
  return subtotal.value + taxAmount.value - (localInvoice.value.discount || 0)
})

// Thêm item mới
function addItem() {
  if (!localInvoice.value.items) {
    localInvoice.value.items = []
  }
  localInvoice.value.items.push({
    productName: '',
    quantity: 1,
    unitPrice: 0
  })
}

// Xóa item
function removeItem(index) {
  localInvoice.value.items.splice(index, 1)
}

// Format tiền tệ
function formatCurrency(value) {
  return Number(value || 0).toLocaleString('vi-VN') + ' ₫'
}
</script>

<template>
  <div v-if="show" class="modal-overlay" @click.self="handleClose">
    <div class="modal-dialog">
      <div class="modal-header">
        <h3 class="modal-title">{{ modalTitle }}</h3>
        <button class="modal-close" @click="handleClose">&times;</button>
      </div>

      <div class="modal-body">
        <div class="form-section">
          <h4 class="section-title">Thông tin chung</h4>
          <div class="form-grid">
            <!-- Mã hóa đơn -->
            <div class="form-group">
              <label>Mã hóa đơn <span class="required">*</span></label>
              <input 
                type="text" 
                v-model="localInvoice.invoiceNumber" 
                class="form-control"
                :disabled="mode === 'view' || mode === 'edit'"
                placeholder="Tự động tạo nếu để trống"
              />
            </div>

            <!-- Khách hàng -->
            <div class="form-group">
              <label>Khách hàng <span class="required">*</span></label>
              <input 
                type="text" 
                v-model="localInvoice.customerName" 
                class="form-control"
                :disabled="mode === 'view'"
                placeholder="Tên khách hàng"
              />
            </div>

            <!-- Ngày lập -->
            <div class="form-group">
              <label>Ngày lập <span class="required">*</span></label>
              <input 
                type="date" 
                v-model="localInvoice.invoiceDate" 
                class="form-control"
                :disabled="mode === 'view'"
              />
            </div>

            <!-- Hạn thanh toán -->
            <div class="form-group">
              <label>Hạn thanh toán <span class="required">*</span></label>
              <input 
                type="date" 
                v-model="localInvoice.dueDate" 
                class="form-control"
                :disabled="mode === 'view'"
              />
            </div>

            <!-- Số điện thoại -->
            <div class="form-group">
              <label>Số điện thoại</label>
              <input 
                type="text" 
                v-model="localInvoice.customerPhone" 
                class="form-control"
                :disabled="mode === 'view'"
                placeholder="Số điện thoại"
              />
            </div>

            <!-- Trạng thái -->
            <div class="form-group">
              <label>Trạng thái</label>
              <select 
                v-model="localInvoice.status" 
                class="form-control"
                :disabled="mode === 'view'"
              >
                <option value="draft">Nháp</option>
                <option value="pending">Chưa thanh toán</option>
                <option value="paid">Đã thanh toán</option>
                <option value="overdue">Quá hạn</option>
                <option value="cancelled">Đã hủy</option>
              </select>
            </div>

            <!-- Địa chỉ -->
            <div class="form-group full-width">
              <label>Địa chỉ</label>
              <input 
                type="text" 
                v-model="localInvoice.customerAddress" 
                class="form-control"
                :disabled="mode === 'view'"
                placeholder="Địa chỉ khách hàng"
              />
            </div>
          </div>
        </div>

        <!-- Chi tiết sản phẩm -->
        <div class="form-section">
          <div class="section-header">
            <h4 class="section-title">Chi tiết sản phẩm</h4>
            <button 
              v-if="mode !== 'view'" 
              type="button" 
              class="btn btn-sm btn-primary" 
              @click="addItem"
            >
              + Thêm sản phẩm
            </button>
          </div>
          
          <div class="items-table">
            <table>
              <thead>
                <tr>
                  <th style="width: 40px;">STT</th>
                  <th>Tên sản phẩm</th>
                  <th style="width: 100px;">Số lượng</th>
                  <th style="width: 150px;">Đơn giá</th>
                  <th style="width: 150px;">Thành tiền</th>
                  <th v-if="mode !== 'view'" style="width: 60px;"></th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="!localInvoice.items || localInvoice.items.length === 0">
                  <td :colspan="mode === 'view' ? 5 : 6" class="text-center">Chưa có sản phẩm</td>
                </tr>
                <tr v-for="(item, index) in localInvoice.items" :key="index">
                  <td class="text-center">{{ index + 1 }}</td>
                  <td>
                    <input 
                      type="text" 
                      v-model="item.productName" 
                      class="form-control"
                      :disabled="mode === 'view'"
                      placeholder="Tên sản phẩm"
                    />
                  </td>
                  <td>
                    <input 
                      type="number" 
                      v-model.number="item.quantity" 
                      class="form-control text-right"
                      :disabled="mode === 'view'"
                      min="1"
                    />
                  </td>
                  <td>
                    <input 
                      type="number" 
                      v-model.number="item.unitPrice" 
                      class="form-control text-right"
                      :disabled="mode === 'view'"
                      min="0"
                    />
                  </td>
                  <td class="text-right">
                    {{ formatCurrency(item.quantity * item.unitPrice) }}
                  </td>
                  <td v-if="mode !== 'view'" class="text-center">
                    <button 
                      type="button" 
                      class="btn-remove" 
                      @click="removeItem(index)"
                      title="Xóa"
                    >
                      🗑️
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Tổng tiền -->
        <div class="form-section">
          <h4 class="section-title">Tổng tiền</h4>
          <div class="summary-grid">
            <div class="form-group">
              <label>Tổng tiền hàng</label>
              <input 
                type="text" 
                :value="formatCurrency(subtotal)" 
                class="form-control text-right"
                disabled
              />
            </div>

            <div class="form-group">
              <label>VAT (%)</label>
              <input 
                type="number" 
                v-model.number="localInvoice.taxRate" 
                class="form-control text-right"
                :disabled="mode === 'view'"
                min="0"
                max="100"
                step="1"
              />
            </div>

            <div class="form-group">
              <label>Tiền thuế</label>
              <input 
                type="text" 
                :value="formatCurrency(taxAmount)" 
                class="form-control text-right"
                disabled
              />
            </div>

            <div class="form-group">
              <label>Giảm giá</label>
              <input 
                type="number" 
                v-model.number="localInvoice.discount" 
                class="form-control text-right"
                :disabled="mode === 'view'"
                min="0"
              />
            </div>

            <div class="form-group total-group">
              <label>TỔNG CỘNG</label>
              <input 
                type="text" 
                :value="formatCurrency(total)" 
                class="form-control text-right total-input"
                disabled
              />
            </div>
          </div>

          <div class="form-group full-width">
            <label>Ghi chú</label>
            <textarea 
              v-model="localInvoice.notes" 
              class="form-control"
              :disabled="mode === 'view'"
              rows="3"
              placeholder="Ghi chú thêm về hóa đơn..."
            ></textarea>
          </div>
        </div>
      </div>

      <div class="modal-footer">
        <button class="btn btn-secondary" @click="handleClose">
          ✖ {{ mode === 'view' ? 'Đóng' : 'Hủy' }}
        </button>
        <button 
          v-if="mode !== 'view'" 
          class="btn btn-primary" 
          @click="handleSave"
        >
          💾 {{ mode === 'create' ? 'Thêm mới' : 'Cập nhật' }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  padding: 20px;
}

.modal-dialog {
  background: white;
  border-radius: 4px;
  width: 100%;
  max-width: 900px;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  position: relative;
  pointer-events: auto;
}

.modal-header {
  padding: 12px 16px;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #f8f9fa;
  flex-shrink: 0;
}

.modal-title {
  font-size: 14px;
  font-weight: 600;
  color: #2c3e50;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.modal-close {
  background: none;
  border: none;
  font-size: 24px;
  color: #6c757d;
  cursor: pointer;
  line-height: 1;
  padding: 0;
  width: 24px;
  height: 24px;
}

.modal-close:hover {
  color: #2c3e50;
}

.modal-body {
  padding: 16px;
  overflow-y: auto;
  flex: 1;
}

.form-section {
  margin-bottom: 20px;
  padding-bottom: 20px;
  border-bottom: 1px solid #e0e0e0;
}

.form-section:last-child {
  border-bottom: none;
  margin-bottom: 0;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.section-title {
  font-size: 13px;
  font-weight: 600;
  color: #2c3e50;
  margin: 0 0 12px 0;
  padding-bottom: 8px;
  border-bottom: 2px solid #3498db;
  display: inline-block;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
  margin-bottom: 12px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.form-group.full-width {
  grid-column: span 2;
}

.form-group.total-group {
  grid-column: span 2;
}

.form-group label {
  font-size: 11px;
  font-weight: 600;
  color: #495057;
}

.form-group .required {
  color: #e74c3c;
}

.form-group input[type="text"],
.form-group input[type="number"],
.form-group input[type="date"],
.form-group select,
.form-group textarea {
  padding: 6px 8px;
  border: 1px solid #ced4da;
  border-radius: 2px;
  font-size: 12px;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.1);
}

.form-group input:disabled,
.form-group select:disabled,
.form-group textarea:disabled {
  background: #e9ecef;
  cursor: not-allowed;
}

.total-input {
  font-weight: 700;
  font-size: 16px !important;
  color: #e74c3c;
  background: #fff3cd !important;
  border-color: #ffc107 !important;
}

.text-right {
  text-align: right;
}

.text-center {
  text-align: center;
}

/* Items Table */
.items-table {
  overflow-x: auto;
  margin-top: 8px;
}

.items-table table {
  width: 100%;
  border-collapse: collapse;
  font-size: 12px;
}

.items-table th {
  background: #f8f9fa;
  padding: 8px;
  text-align: left;
  font-weight: 600;
  border: 1px solid #dee2e6;
  font-size: 11px;
}

.items-table td {
  padding: 6px;
  border: 1px solid #dee2e6;
}

.items-table input {
  width: 100%;
  padding: 4px 6px;
  border: 1px solid #ced4da;
  border-radius: 2px;
  font-size: 12px;
}

.items-table input:focus {
  outline: none;
  border-color: #3498db;
}

.btn-remove {
  background: none;
  border: none;
  cursor: pointer;
  padding: 2px 4px;
  font-size: 14px;
  transition: transform 0.2s;
}

.btn-remove:hover {
  transform: scale(1.2);
}

.modal-footer {
  padding: 12px 16px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  background: #f8f9fa;
  flex-shrink: 0;
}

@media (max-width: 768px) {
  .form-grid,
  .summary-grid {
    grid-template-columns: 1fr;
  }
  
  .modal-dialog {
    max-width: 100%;
    margin: 0;
    max-height: 100vh;
  }
  
  .items-table {
    font-size: 10px;
  }
}
</style>
