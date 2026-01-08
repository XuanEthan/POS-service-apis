<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  items: {
    type: Array,
    default: () => []
  },
  idKey: {
    type: String,
    default: 'id'
  },
  parentKey: {
    type: String,
    default: 'parentId'
  },
  labelKey: {
    type: String,
    default: 'title'
  },
  codeKey: {
    type: String,
    default: 'code'
  },
  // Props để điều khiển hiển thị action buttons theo quyền
  showEdit: {
    type: Boolean,
    default: true
  },
  showView: {
    type: Boolean,
    default: true
  },
  showDelete: {
    type: Boolean,
    default: true
  },
  showPermission: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['edit', 'delete', 'view', 'select', 'permission'])

// Track expanded nodes
const expandedNodes = ref(new Set())

// Track selected nodes
const selectedNodes = ref(new Set())

// Convert flat array to tree structure
const treeData = computed(() => {
  const items = props.items || []
  const map = new Map()
  const roots = []
  
  // Create a map of all items
  items.forEach(item => {
    map.set(item[props.idKey], { ...item, children: [] })
  })
  
  // Build tree structure
  items.forEach(item => {
    const node = map.get(item[props.idKey])
    const parentId = item[props.parentKey]
    
    // Check if parentId is empty/null/undefined or equals "00000000-0000-0000-0000-000000000000"
    const isRoot = !parentId || 
      parentId === '00000000-0000-0000-0000-000000000000' ||
      parentId === null ||
      parentId === undefined
    
    if (isRoot) {
      roots.push(node)
    } else {
      const parent = map.get(parentId)
      if (parent) {
        parent.children.push(node)
      } else {
        // If parent not found, treat as root
        roots.push(node)
      }
    }
  })
  
  return roots
})


// Toggle expand/collapse
function toggleNode(nodeId) {
  if (expandedNodes.value.has(nodeId)) {
    expandedNodes.value.delete(nodeId)
  } else {
    expandedNodes.value.add(nodeId)
  }
  // Force reactivity
  expandedNodes.value = new Set(expandedNodes.value)
}

// Check if node is expanded
function isExpanded(nodeId) {
  return expandedNodes.value.has(nodeId)
}

// Toggle select
function toggleSelect(nodeId) {
  if (selectedNodes.value.has(nodeId)) {
    selectedNodes.value.delete(nodeId)
  } else {
    selectedNodes.value.add(nodeId)
  }
  selectedNodes.value = new Set(selectedNodes.value)
  emit('select', Array.from(selectedNodes.value))
}

// Check if node is selected
function isSelected(nodeId) {
  return selectedNodes.value.has(nodeId)
}

// Expand all
function expandAll() {
  props.items.forEach(item => {
    expandedNodes.value.add(item[props.idKey])
  })
  expandedNodes.value = new Set(expandedNodes.value)
}

// Collapse all
function collapseAll() {
  expandedNodes.value.clear()
  expandedNodes.value = new Set(expandedNodes.value)
}

function clickNode(id) {
  emit('select', id)
}

// Expose methods
defineExpose({ expandAll, collapseAll })
</script>

<template>
  <div class="tree-view">
    <div class="tree-toolbar">
      <button class="tree-btn" @click="expandAll" title="Mở rộng tất cả">
        <span>📂</span> Mở rộng
      </button>
      <button class="tree-btn" @click="collapseAll" title="Thu gọn tất cả">
        <span>📁</span> Thu gọn
      </button>
    </div>
    
    <div class="tree-container">
      <template v-if="treeData.length === 0">
        <div class="tree-empty">Không có dữ liệu</div>
      </template>
      
      <!-- Recursive tree node component -->
      <div class="tree-root">
        <template v-for="node in treeData" :key="node[idKey]">
          <div class="tree-node">
            <div 
              class="tree-node-content"
              :class="{ 'selected': isSelected(node[idKey]) }"
            >
              <!-- Expand/Collapse toggle -->
              <span 
                class="tree-toggle"
                @click="toggleNode(node[idKey])"
              >
                <template v-if="node.children && node.children.length > 0">
                  <span v-if="isExpanded(node[idKey])">▼</span>
                  <span v-else>▶</span>
                </template>
                <span v-else class="tree-toggle-empty">•</span>
              </span>
              
              <!-- Checkbox -->
              <!-- Icon -->
              <span class="tree-icon">
                <template v-if="node.children && node.children.length > 0">
                </template>
                <template v-else>
                </template>
              </span>
              
              <!-- Label -->
              <span class="tree-label">
                <span class="tree-code">{{ node[codeKey] }}</span>
                <span class="tree-title">{{ node[labelKey] }}</span>
              </span>
              
              <!-- Actions -->
              <div class="tree-actions" v-if="showEdit || showView || showDelete || showPermission">
                <!--  -->
                <button v-if="showPermission" class="action-btn permission" @click="$emit('permission', node)" title="Phân quyền">🔐</button>
                <button v-if="showEdit" class="action-btn edit" @click="$emit('edit', node)" title="Sửa">✏️</button>
                <button v-if="showView" class="action-btn view" @click="$emit('view', node)" title="Xem">👁️</button>
                <button v-if="showDelete" class="action-btn delete" @click="$emit('delete', node)" title="Xóa">🗑️</button>
              </div>
            </div>
            
            <!-- Children (recursive) -->
            <div 
              v-if="node.children && node.children.length > 0 && isExpanded(node[idKey])"
              class="tree-children"
            >
              <template v-for="child in node.children" :key="child[idKey]">
                <div class="tree-node">
                  <div 
                    class="tree-node-content"
                    :class="{ 'selected': isSelected(child[idKey]) }"
                  >
                    <span 
                      class="tree-toggle"
                      @click="toggleNode(child[idKey])"
                    >
                      <template v-if="child.children && child.children.length > 0">
                        <span v-if="isExpanded(child[idKey])">▼</span>
                        <span v-else>▶</span>
                      </template>
                      <span v-else class="tree-toggle-empty">•</span>
                    </span>
                    
                    <input 
                      type="checkbox" 
                      class="tree-checkbox"
                      :checked="isSelected(child[idKey])"
                      @change="toggleSelect(child[idKey])"
                    />
                    
                    <span class="tree-icon">
                      <template v-if="child.children && child.children.length > 0">📁</template>
                      <template v-else>📄</template>
                    </span>
                    
                    <span class="tree-label">
                      <span class="tree-code">{{ child[codeKey] }}</span>
                      <span class="tree-title">{{ child[labelKey] }}</span>
                    </span>
                    
                    <div class="tree-actions" v-if="showEdit || showView || showDelete || showPermission">
                      <button v-if="showPermission" class="action-btn permission" @click="$emit('permission', child)" title="Phân quyền">🔐</button>
                      <button v-if="showEdit" class="action-btn edit" @click="$emit('edit', child)" title="Sửa">✏️</button>
                      <button v-if="showView" class="action-btn view" @click="$emit('view', child)" title="Xem">👁️</button>
                      <button v-if="showDelete" class="action-btn delete" @click="$emit('delete', child)" title="Xóa">🗑️</button>
                    </div>
                  </div>
                  
                  <!-- Level 3 children -->
                  <div 
                    v-if="child.children && child.children.length > 0 && isExpanded(child[idKey])"
                    class="tree-children"
                  >
                    <template v-for="grandChild in child.children" :key="grandChild[idKey]">
                      <div class="tree-node">
                        <div 
                          class="tree-node-content"
                          :class="{ 'selected': isSelected(grandChild[idKey]) }"
                        >
                          <span class="tree-toggle">
                            <span class="tree-toggle-empty">•</span>
                          </span>
                          
                          <input 
                            type="checkbox" 
                            class="tree-checkbox"
                            :checked="isSelected(grandChild[idKey])"
                            @change="toggleSelect(grandChild[idKey])"
                          />
                          
                          <span class="tree-icon">📄</span>
                          
                          <span class="tree-label">
                            <span class="tree-code">{{ grandChild[codeKey] }}</span>
                            <span class="tree-title">{{ grandChild[labelKey] }}</span>
                          </span>
                          
                          <div class="tree-actions" v-if="showEdit || showView || showDelete || showPermission">
                            <button v-if="showPermission" class="action-btn permission" @click="$emit('permission', grandChild)" title="Phân quyền">🔐</button>
                            <button v-if="showEdit" class="action-btn edit" @click="$emit('edit', grandChild)" title="Sửa">✏️</button>
                            <button v-if="showView" class="action-btn view" @click="$emit('view', grandChild)" title="Xem">👁️</button>
                            <button v-if="showDelete" class="action-btn delete" @click="$emit('delete', grandChild)" title="Xóa">🗑️</button>
                          </div>
                        </div>
                      </div>
                    </template>
                  </div>
                </div>
              </template>
            </div>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<style scoped>
.tree-view {
  background: #fff;
  border: 1px solid #e9ecef;
  border-radius: 4px;
}

.tree-toolbar {
  padding: 10px 15px;
  border-bottom: 1px solid #e9ecef;
  background: #f8f9fa;
  display: flex;
  gap: 10px;
}

.tree-btn {
  padding: 6px 12px;
  border: 1px solid #ced4da;
  border-radius: 4px;
  background: #fff;
  cursor: pointer;
  font-size: 13px;
  display: flex;
  align-items: center;
  gap: 5px;
  transition: all 0.2s;
}

.tree-btn:hover {
  background: #e9ecef;
}

.tree-container {
  padding: 10px;
  max-height: 500px;
  overflow-y: auto;
}

.tree-empty {
  padding: 20px;
  text-align: center;
  color: #666;
}

.tree-root {
  padding-left: 0;
}

.tree-node {
  margin: 2px 0;
}

.tree-node-content {
  display: flex;
  align-items: center;
  padding: 8px 10px;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.15s;
  gap: 8px;
}

.tree-node-content:hover {
  background: #f0f7ff;
}

.tree-node-content.selected {
  background: #e3f2fd;
  border-left: 3px solid #2196f3;
}

.tree-toggle {
  width: 20px;
  text-align: center;
  cursor: pointer;
  color: #666;
  font-size: 10px;
  flex-shrink: 0;
}

.tree-toggle:hover {
  color: #333;
}

.tree-toggle-empty {
  color: #ccc;
}

.tree-checkbox {
  flex-shrink: 0;
  cursor: pointer;
  width: 16px;
  height: 16px;
}

.tree-icon {
  font-size: 16px;
  flex-shrink: 0;
}

.tree-label {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 10px;
  overflow: hidden;
}

.tree-code {
  background: #e9ecef;
  padding: 2px 8px;
  border-radius: 3px;
  font-size: 12px;
  font-family: monospace;
  color: #495057;
  flex-shrink: 0;
}

.tree-title {
  color: #333;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.tree-actions {
  display: none;
  gap: 5px;
  flex-shrink: 0;
}

.tree-node-content:hover .tree-actions {
  display: flex;
}

.action-btn {
  padding: 4px 8px;
  border: none;
  background: transparent;
  cursor: pointer;
  font-size: 14px;
  border-radius: 3px;
  transition: background 0.15s;
}

.action-btn:hover {
  background: #e9ecef;
}

.action-btn.permission:hover {
  background: #fff3cd;
}

.action-btn.delete:hover {
  background: #ffe6e6;
}

.tree-children {
  padding-left: 25px;
  border-left: 1px dashed #ddd;
  margin-left: 10px;
}
</style>
