<script setup>
import { ref, watch } from 'vue'

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
  product: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['close', 'save'])

const localProduct = ref({ ...props.product })

// Watch for product changes
watch(() => props.product, (newProduct) => {
  localProduct.value = { ...newProduct }
}, { deep: true })

function handleClose() {
  emit('close')
}

function handleSave() {
  emit('save', localProduct.value)
}

const modalTitle = {
  create: '📦 Thêm sản phẩm mới',
  edit: '📦 Sửa sản phẩm',
  view: '📦 Chi tiết sản phẩm'
}
</script>

<template>
  <div v-if="show" class="modal-overlay" @click.self="handleClose">
    <div class="modal-dialog">
      <div class="modal-header">
        <h3 class="modal-title">{{ modalTitle[mode] }}</h3>
        <button class="modal-close" @click="handleClose">&times;</button>
      </div>
      <div class="modal-body">
        <div class="form-grid">
          <!-- Danh mục ID -->
          <!-- <div class="form-group" hidden>
            <label>Danh mục</label>
            <input 
              type="text" 
              v-model="localProduct.danhmucId" 
              class="form-control"
              :disabled="mode === 'view'"
              placeholder="ID danh mục (optional)"
            />
          </div> -->

          <!-- SKU -->
          <div class="form-group">
            <label>SKU <span class="required">*</span></label>
            <input 
              type="text" 
              v-model="localProduct.sku" 
              class="form-control"
              :disabled="mode === 'view'"
              placeholder="Mã SKU"
            />
          </div>

          <!-- Tên sản phẩm -->
          <div class="form-group full-width">
            <label>Tên sản phẩm <span class="required">*</span></label>
            <input 
              type="text" 
              v-model="localProduct.name" 
              class="form-control"
              :disabled="mode === 'view'"
              placeholder="Tên sản phẩm"
            />
          </div>

          <!-- Barcode -->
          <div class="form-group">
            <label>Barcode</label>
            <input 
              type="text" 
              v-model="localProduct.barcode" 
              class="form-control"
              :disabled="mode === 'view'"
              placeholder="Barcode/EAN/UPC"
            />
          </div>

          <!-- Đơn vị tính -->
          <div class="form-group">
            <label>Đơn vị tính</label>
            <input 
              type="text" 
              v-model="localProduct.unitOfMeasure" 
              class="form-control"
              :disabled="mode === 'view'"
              placeholder="pcs, kg, lít..."
            />
          </div>

          <!-- Giá bán -->
          <div class="form-group">
            <label>Giá bán <span class="required">*</span></label>
            <input 
              type="number" 
              v-model.number="localProduct.price" 
              class="form-control"
              :disabled="mode === 'view'"
              placeholder="0"
              min="0"
              step="0.01"
            />
          </div>

          <!-- Loại tiền -->
          <div class="form-group">
            <label>Loại tiền</label>
            <select 
              v-model="localProduct.currency" 
              class="form-control"
              :disabled="mode === 'view'"
            >
              <option value="VND">VND</option>
              <!-- <option value="USD">USD</option>
              <option value="EUR">EUR</option> -->
            </select>
          </div>

          <!-- Giá vốn -->
          <div class="form-group">
            <label>Giá vốn</label>
            <input 
              type="number" 
              v-model.number="localProduct.costPrice" 
              class="form-control"
              :disabled="mode === 'view'"
              placeholder="0"
              min="0"
              step="0.01"
            />
          </div>

          <!-- Tồn kho -->
          <div class="form-group">
            <label>Tồn kho</label>
            <input 
              type="number" 
              v-model.number="localProduct.stockOnHand" 
              class="form-control"
              :disabled="mode === 'view'"
              placeholder="0"
              min="0"
              step="0.01"
            />
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
  max-width: 700px;
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

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.form-group.full-width {
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
.form-group select {
  padding: 6px 8px;
  border: 1px solid #ced4da;
  border-radius: 2px;
  font-size: 12px;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.1);
}

.form-group input:disabled,
.form-group select:disabled {
  background: #e9ecef;
  cursor: not-allowed;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  margin-top: 8px;
}

.checkbox-label input[type="checkbox"] {
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.checkbox-label span {
  font-size: 12px;
  font-weight: 500;
}

.modal-footer {
  padding: 12px 16px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  background: #f8f9fa;
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
  
  .modal-dialog {
    max-width: 100%;
    margin: 0;
  }
}
</style>
