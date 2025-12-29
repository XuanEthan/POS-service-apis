import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import './assets/common.css'

// Import permission plugin
import { PermissionPlugin } from './directives/permission'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(PermissionPlugin) // Đăng ký directive v-permission globally

app.mount('#app')
