<template>
  <div class="my-announces-page">
    <div class="container">
      <!-- Page Header -->
      <div class="row mb-4">
        <div class="col-md-8">
          <h1 class="fw-bold">
            <i class="fas fa-bullhorn me-2 text-primary"></i>
            Duyurularım
          </h1>
          <p class="text-muted">Oluşturduğunuz duyuruları yönetin</p>
        </div>
        <div class="col-md-4 text-md-end">
          <router-link to="/announces/create" class="btn btn-primary btn-custom">
            <i class="fas fa-plus me-2"></i>Yeni Duyuru
          </router-link>
        </div>
      </div>

      <!-- Statistics Cards -->
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card card-custom bg-gradient-primary text-white">
            <div class="card-body text-center">
              <i class="fas fa-bullhorn fa-2x mb-2"></i>
              <h4>{{ stats.total }}</h4>
              <small>Toplam Duyuru</small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card card-custom bg-gradient-success text-white">
            <div class="card-body text-center">
              <i class="fas fa-check-circle fa-2x mb-2"></i>
              <h4>{{ stats.active }}</h4>
              <small>Aktif Duyuru</small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card card-custom bg-gradient-warning text-white">
            <div class="card-body text-center">
              <i class="fas fa-file-alt fa-2x mb-2"></i>
              <h4>{{ stats.applications }}</h4>
              <small>Toplam Başvuru</small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card card-custom bg-gradient-info text-white">
            <div class="card-body text-center">
              <i class="fas fa-eye fa-2x mb-2"></i>
              <h4>{{ stats.views }}</h4>
              <small>Toplam Görüntülenme</small>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="card card-custom mb-4">
        <div class="card-body">
          <div class="row g-3">
            <div class="col-md-4">
              <label class="form-label">Durum Filtresi</label>
              <select class="form-select" v-model="statusFilter" @change="applyFilters">
                <option value="">Tüm Durumlar</option>
                <option value="1">Aktif</option>
                <option value="2">Kapalı</option>
                <option value="3">Süresi Dolmuş</option>
                <option value="0">Taslak</option>
              </select>
            </div>
            <div class="col-md-4">
              <label class="form-label">Kategori</label>
              <select class="form-select" v-model="categoryFilter" @change="applyFilters">
                <option value="">Tüm Kategoriler</option>
                <option
                  v-for="category in categories"
                  :key="category.id"
                  :value="category.id"
                >
                  {{ category.name }}
                </option>
              </select>
            </div>
            <div class="col-md-4">
              <label class="form-label">Sıralama</label>
              <select class="form-select" v-model="sortOrder" @change="applyFilters">
                <option value="newest">En Yeni</option>
                <option value="oldest">En Eski</option>
                <option value="most-applications">En Çok Başvuru</option>
                <option value="expiring-soon">Süresi Yakın</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="text-center py-5">
        <div class="loading-spinner mx-auto mb-3"></div>
        <p>Duyurularınız yükleniyor...</p>
      </div>

      <!-- Announces List -->
      <div v-else-if="filteredAnnounces.length > 0">
        <div class="row">
          <div
            v-for="announce in filteredAnnounces"
            :key="announce.id"
            class="col-lg-6 col-xl-4 mb-4"
          >
            <div class="card card-custom h-100">
              <div class="card-header border-0 bg-transparent">
                <div class="d-flex justify-content-between align-items-center">
                  <span
                    class="badge"
                    :class="getStatusBadgeClass(announce.status)"
                  >
                    {{ getStatusText(announce.status) }}
                  </span>
                  <div class="dropdown">
                    <button
                      class="btn btn-sm btn-outline-secondary dropdown-toggle"
                      type="button"
                      data-bs-toggle="dropdown"
                    >
                      <i class="fas fa-ellipsis-v"></i>
                    </button>
                    <ul class="dropdown-menu">
                      <li>
                        <router-link
                          :to="`/announces/${announce.id}`"
                          class="dropdown-item"
                        >
                          <i class="fas fa-eye me-2"></i>Görüntüle
                        </router-link>
                      </li>
                      <li>
                        <router-link
                          :to="`/announces/${announce.id}/edit`"
                          class="dropdown-item"
                        >
                          <i class="fas fa-edit me-2"></i>Düzenle
                        </router-link>
                      </li>
                      <li><hr class="dropdown-divider"></li>
                      <li>
                        <button
                          @click="expireAnnounce(announce.id)"
                          class="dropdown-item text-warning"
                          v-if="announce.status === 1"
                        >
                          <i class="fas fa-clock me-2"></i>Süresi Bitir
                        </button>
                      </li>
                      <li>
                        <button
                          @click="deleteAnnounce(announce.id)"
                          class="dropdown-item text-danger"
                        >
                          <i class="fas fa-trash me-2"></i>Sil
                        </button>
                      </li>
                    </ul>
                  </div>
                </div>
              </div>

              <div class="card-body">
                <h5 class="card-title fw-bold mb-2">{{ announce.title }}</h5>
                <p class="card-text text-muted small mb-3">
                  {{ truncateText(announce.description, 80) }}
                </p>

                <div class="mb-3">
                  <span class="badge bg-primary me-2">
                    {{ announce.category?.name }}
                  </span>
                  <span class="badge bg-secondary">
                    {{ getCollaborationTypeText(announce.collaborationType) }}
                  </span>
                </div>

                <div class="stats-row">
                  <div class="stat-item">
                    <i class="fas fa-users text-primary"></i>
                    <span>{{ announce.currentParticipants }}/{{ announce.maxParticipants }}</span>
                  </div>
                  <div class="stat-item">
                    <i class="fas fa-file-alt text-info"></i>
                    <span>{{ announce.applicationsCount || 0 }}</span>
                  </div>
                  <div class="stat-item">
                    <i class="fas fa-comment text-success"></i>
                    <span>{{ announce.commentsCount || 0 }}</span>
                  </div>
                  <div class="stat-item">
                    <i class="fas fa-eye text-muted"></i>
                    <span>{{ Math.floor(Math.random() * 500) + 50 }}</span>
                  </div>
                </div>
              </div>

              <div class="card-footer border-0 bg-transparent">
                <div class="d-flex justify-content-between align-items-center">
                  <small class="text-muted">
                    {{ formatDate(announce.createdAt) }}
                  </small>
                  <router-link
                    :to="`/announces/${announce.id}`"
                    class="btn btn-sm btn-primary"
                  >
                    Detay
                  </router-link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- No Results -->
      <div v-else class="text-center py-5">
        <i class="fas fa-bullhorn fa-4x text-muted mb-4"></i>
        <h3>Henüz duyuru oluşturmamışsınız</h3>
        <p class="text-muted">İlk duyurunuzu oluşturarak projeleriniz için ekip arkadaşları bulun</p>
        <router-link to="/announces/create" class="btn btn-primary btn-lg">
          <i class="fas fa-plus me-2"></i>İlk Duyurunuzu Oluşturun
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import axios from 'axios'
import moment from 'moment'
import { useToast } from 'vue-toastification'

export default {
  name: 'MyAnnounces',
  setup() {
    const authStore = useAuthStore()
    const toast = useToast()

    const announces = ref([])
    const categories = ref([])
    const loading = ref(false)

    // Filters
    const statusFilter = ref('')
    const categoryFilter = ref('')
    const sortOrder = ref('newest')

    const stats = computed(() => {
      const total = announces.value.length
      const active = announces.value.filter(a => a.status === 1).length
      const applications = announces.value.reduce((sum, a) => sum + (a.applicationsCount || 0), 0)
      const views = announces.value.reduce((sum) => sum + Math.floor(Math.random() * 500) + 50, 0)

      return { total, active, applications, views }
    })

    const filteredAnnounces = computed(() => {
      let filtered = [...announces.value]

      // Status filter
      if (statusFilter.value) {
        filtered = filtered.filter(a => a.status.toString() === statusFilter.value)
      }

      // Category filter
      if (categoryFilter.value) {
        filtered = filtered.filter(a => a.categoryId === categoryFilter.value)
      }

      // Sort
      switch (sortOrder.value) {
        case 'oldest':
          filtered.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt))
          break
        case 'most-applications':
          filtered.sort((a, b) => (b.applicationsCount || 0) - (a.applicationsCount || 0))
          break
        case 'expiring-soon':
          filtered.sort((a, b) => {
            if (!a.expiryDate && !b.expiryDate) return 0
            if (!a.expiryDate) return 1
            if (!b.expiryDate) return -1
            return new Date(a.expiryDate) - new Date(b.expiryDate)
          })
          break
        default: // newest
          filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      }

      return filtered
    })

    const fetchMyAnnounces = async () => {
      loading.value = true
      try {
        const response = await axios.get('/announces/my-announces')
        if (response.data.success) {
          announces.value = response.data.data
        }
      } catch (error) {
        toast.error('Duyurular yüklenirken hata oluştu')
        console.error('Error fetching my announces:', error)
      } finally {
        loading.value = false
      }
    }

    const fetchCategories = async () => {
      try {
        const response = await axios.get('/categories/active')
        if (response.data.success) {
          categories.value = response.data.data
        }
      } catch (error) {
        console.error('Error fetching categories:', error)
      }
    }

    const expireAnnounce = async (announceId) => {
      if (!confirm('Duyurunun süresini bitirmek istediğinizden emin misiniz?')) return

      try {
        const response = await axios.post(`/announces/${announceId}/expire`)
        if (response.data.success) {
          const announce = announces.value.find(a => a.id === announceId)
          if (announce) {
            announce.status = 3 // Expired
          }
          toast.success('Duyuru süresi bitirildi')
        }
      } catch (error) {
        toast.error('Duyuru süresi bitirilirken hata oluştu')
      }
    }

    const deleteAnnounce = async (announceId) => {
      if (!confirm('Duyuruyu silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.')) return

      try {
        const response = await axios.delete(`/announces/${announceId}`)
        if (response.data.success) {
          announces.value = announces.value.filter(a => a.id !== announceId)
          toast.success('Duyuru başarıyla silindi')
        }
      } catch (error) {
        toast.error('Duyuru silinirken hata oluştu')
      }
    }

    const applyFilters = () => {
      // Filters are reactive, no need to do anything
    }

    // Utility functions
    const truncateText = (text, maxLength) => {
      if (!text) return ''
      return text.length > maxLength ? text.substring(0, maxLength) + '...' : text
    }

    const formatDate = (date) => moment(date).fromNow()

    const getStatusText = (status) => {
      const statusMap = {
        0: 'Taslak', 1: 'Aktif', 2: 'Kapalı', 3: 'Süresi Dolmuş', 4: 'Askıya Alınmış'
      }
      return statusMap[status] || 'Bilinmiyor'
    }

    const getStatusBadgeClass = (status) => {
      const classMap = {
        0: 'bg-secondary', 1: 'bg-success', 2: 'bg-warning', 3: 'bg-danger', 4: 'bg-dark'
      }
      return classMap[status] || 'bg-secondary'
    }

    const getCollaborationTypeText = (type) => {
      const typeMap = {
        0: 'Proje', 1: 'Ortaklık', 2: 'Araştırma', 3: 'Etkinlik', 4: 'Gönüllülük', 5: 'Diğer'
      }
      return typeMap[type] || 'Proje'
    }

    onMounted(async () => {
      await Promise.all([
        fetchMyAnnounces(),
        fetchCategories()
      ])
    })

    return {
      announces,
      categories,
      loading,
      statusFilter,
      categoryFilter,
      sortOrder,
      stats,
      filteredAnnounces,
      authStore,
      expireAnnounce,
      deleteAnnounce,
      applyFilters,
      truncateText,
      formatDate,
      getStatusText,
      getStatusBadgeClass,
      getCollaborationTypeText
    }
  }
}
</script>

<style scoped>
.my-announces-page {
  padding: 2rem 0;
}

.stats-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  border-top: 1px solid #e9ecef;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
}

.stat-item i {
  font-size: 0.8rem;
}

.card-custom:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

.dropdown-toggle::after {
  display: none;
}

@media (max-width: 768px) {
  .stats-row {
    flex-wrap: wrap;
    gap: 0.5rem;
  }

  .stat-item {
    font-size: 0.8rem;
  }
}
</style>
