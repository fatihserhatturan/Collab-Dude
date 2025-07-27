// src/stores/announces.js
import { defineStore } from 'pinia'
import axios from 'axios'
import { useToast } from 'vue-toastification'

const toast = useToast()

export const useAnnouncesStore = defineStore('announces', {
  state: () => ({
    announces: [],
    currentAnnounce: null,
    categories: [],
    tags: [],
    loading: false,
    pagination: {
      page: 1,
      pageSize: 10,
      totalCount: 0,
      totalPages: 0
    },
    filters: {
      searchTerm: '',
      categoryId: null,
      collaborationType: null,
      location: '',
      sortBy: 'CreatedAt',
      sortDescending: true
    }
  }),

  getters: {
    activeAnnounces: (state) => state.announces.filter(a => a.status === 1),
    expiredAnnounces: (state) => state.announces.filter(a => a.isExpired),
    myAnnounces: (state) => {
      const authStore = useAuthStore()
      return state.announces.filter(a => a.username === authStore.userName)
    }
  },

  actions: {
    async fetchAnnounces(filters = {}) {
      this.loading = true
      try {
        const params = { ...this.filters, ...filters }
        const response = await axios.get('/announces', { params })

        if (response.data.success) {
          this.announces = response.data.data.items
          this.pagination = {
            page: response.data.data.pageNumber,
            pageSize: response.data.data.pageSize,
            totalCount: response.data.data.totalCount,
            totalPages: response.data.data.totalPages
          }
        }
      } catch (error) {
        toast.error('Duyurular yüklenirken hata oluştu')
        console.error('Error fetching announces:', error)
      } finally {
        this.loading = false
      }
    },

    async fetchAnnounceDetails(id) {
      this.loading = true
      try {
        const response = await axios.get(`/announces/${id}/details`)

        if (response.data.success) {
          this.currentAnnounce = response.data.data
          return response.data.data
        }
      } catch (error) {
        toast.error('Duyuru detayları yüklenirken hata oluştu')
        console.error('Error fetching announce details:', error)
      } finally {
        this.loading = false
      }
    },

    async createAnnounce(announceData) {
      this.loading = true
      try {
        const response = await axios.post('/announces', announceData)

        if (response.data.success) {
          toast.success('Duyuru başarıyla oluşturuldu!')
          return { success: true, data: response.data.data }
        }
      } catch (error) {
        const message = error.response?.data?.message || 'Duyuru oluşturulurken hata oluştu'
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async updateAnnounce(id, announceData) {
      this.loading = true
      try {
        const response = await axios.put(`/announces/${id}`, announceData)

        if (response.data.success) {
          toast.success('Duyuru başarıyla güncellendi!')
          return { success: true, data: response.data.data }
        }
      } catch (error) {
        const message = error.response?.data?.message || 'Duyuru güncellenirken hata oluştu'
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async deleteAnnounce(id) {
      this.loading = true
      try {
        const response = await axios.delete(`/announces/${id}`)

        if (response.data.success) {
          this.announces = this.announces.filter(a => a.id !== id)
          toast.success('Duyuru başarıyla silindi!')
          return { success: true }
        }
      } catch (error) {
        const message = error.response?.data?.message || 'Duyuru silinirken hata oluştu'
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async fetchCategories() {
      try {
        const response = await axios.get('/categories/active')
        if (response.data.success) {
          this.categories = response.data.data
        }
      } catch (error) {
        console.error('Error fetching categories:', error)
      }
    },

    async fetchTags() {
      try {
        const response = await axios.get('/tags/active')
        if (response.data.success) {
          this.tags = response.data.data
        }
      } catch (error) {
        console.error('Error fetching tags:', error)
      }
    },

    setFilters(newFilters) {
      this.filters = { ...this.filters, ...newFilters }
    },

    clearFilters() {
      this.filters = {
        searchTerm: '',
        categoryId: null,
        collaborationType: null,
        location: '',
        sortBy: 'CreatedAt',
        sortDescending: true
      }
    }
  }
})
