// src/stores/auth.js
import { defineStore } from 'pinia'
import axios from 'axios'
import { useToast } from 'vue-toastification'

const toast = useToast()
const USER_SERVICE_URL = 'http://localhost:5001/api' // UserService

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('token') || null,
    loading: false
  }),

  getters: {
    isLoggedIn: (state) => !!state.token,
    isAdmin: (state) => state.user?.roleName === 'Admin',
    currentUser: (state) => state.user,
    userName: (state) => state.user?.userName || '',
    fullName: (state) => state.user?.fullName || ''
  },

  actions: {
    async login(credentials) {
      this.loading = true
      try {
        const response = await axios.post(`${USER_SERVICE_URL}/auth/login`, credentials)

        if (response.data.success) {
          this.token = response.data.data.token
          this.user = response.data.data.user

          localStorage.setItem('token', this.token)
          localStorage.setItem('user', JSON.stringify(this.user))

          // Set default authorization header
          axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`

          toast.success('Başarıyla giriş yapıldı!')
          return { success: true }
        }
      } catch (error) {
        const message = error.response?.data?.message || 'Giriş yapılırken hata oluştu'
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async register(userData) {
      this.loading = true
      try {
        const response = await axios.post(`${USER_SERVICE_URL}/users`, userData)

        if (response.data.success) {
          toast.success('Kayıt başarılı! Şimdi giriş yapabilirsiniz.')
          return { success: true }
        }
      } catch (error) {
        const message = error.response?.data?.message || 'Kayıt olurken hata oluştu'
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async logout() {
      try {
        if (this.token) {
          await axios.post(`${USER_SERVICE_URL}/auth/logout`)
        }
      } catch (error) {
        console.error('Logout error:', error)
      } finally {
        this.token = null
        this.user = null

        localStorage.removeItem('token')
        localStorage.removeItem('user')
        delete axios.defaults.headers.common['Authorization']

        toast.info('Başarıyla çıkış yapıldı')
      }
    },

    async validateToken() {
      if (!this.token) return false

      try {
        const response = await axios.post(`${USER_SERVICE_URL}/auth/validate`, this.token)
        return response.data.success && response.data.data
      } catch (error) {
        this.logout()
        return false
      }
    },

    async refreshToken() {
      if (!this.token) return false

      try {
        const response = await axios.post(`${USER_SERVICE_URL}/auth/refresh`, this.token)

        if (response.data.success) {
          this.token = response.data.data.token
          this.user = response.data.data.user

          localStorage.setItem('token', this.token)
          localStorage.setItem('user', JSON.stringify(this.user))

          return true
        }
      } catch (error) {
        this.logout()
        return false
      }
    },

    initializeAuth() {
      if (this.token) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
      }
    }
  }
})

