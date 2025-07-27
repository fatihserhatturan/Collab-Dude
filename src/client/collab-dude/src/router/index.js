// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// Pages
import Login from '@/views/Login.vue'
import Register from '@/views/Register.vue'
import AnnounceDetail from '@/views/AnnounceDetail.vue'
import MyAnnounces from '@/views/MyAnnounces.vue'
import MyApplications from '@/views/MyApplications.vue'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { requiresGuest: true }
  },
  {
    path: '/register',
    name: 'Register',
    component: Register,
    meta: { requiresGuest: true }
  },
  {
    path: '/announces/:id',
    name: 'AnnounceDetail',
    component: AnnounceDetail,
    props: true
  },
  {
    path: '/my-announces',
    name: 'MyAnnounces',
    component: MyAnnounces,
    meta: { requiresAuth: true }
  },
  {
    path: '/my-applications',
    name: 'MyApplications',
    component: MyApplications,
    meta: { requiresAuth: true }
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guards
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  // Check if route requires authentication
  if (to.meta.requiresAuth && !authStore.isLoggedIn) {
    next('/login')
    return
  }

  // Check if route requires guest (not authenticated)
  if (to.meta.requiresGuest && authStore.isLoggedIn) {
    next('/')
    return
  }

  // Check if route requires admin
  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    next('/')
    return
  }

  next()
})

export default router
