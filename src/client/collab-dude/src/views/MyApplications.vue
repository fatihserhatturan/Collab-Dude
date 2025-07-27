<template>
  <div class="my-applications-page">
    <div class="container">
      <!-- Page Header -->
      <div class="row mb-4">
        <div class="col-md-8">
          <h1 class="fw-bold">
            <i class="fas fa-file-alt me-2 text-primary"></i>
            Başvurularım
          </h1>
          <p class="text-muted">Yaptığınız başvuruları takip edin</p>
        </div>
      </div>

      <!-- Statistics Cards -->
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card card-custom bg-gradient-info text-white">
            <div class="card-body text-center">
              <i class="fas fa-file-alt fa-2x mb-2"></i>
              <h4>{{ stats.total }}</h4>
              <small>Toplam Başvuru</small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card card-custom bg-gradient-warning text-white">
            <div class="card-body text-center">
              <i class="fas fa-clock fa-2x mb-2"></i>
              <h4>{{ stats.pending }}</h4>
              <small>Beklemede</small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card card-custom bg-gradient-success text-white">
            <div class="card-body text-center">
              <i class="fas fa-check-circle fa-2x mb-2"></i>
              <h4>{{ stats.approved }}</h4>
              <small>Onaylanan</small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card card-custom bg-gradient-danger text-white">
            <div class="card-body text-center">
              <i class="fas fa-times-circle fa-2x mb-2"></i>
              <h4>{{ stats.rejected }}</h4>
              <small>Reddedilen</small>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="card card-custom mb-4">
        <div class="card-body">
          <div class="row g-3">
            <div class="col-md-4">
              <label class="form-label">Başvuru Durumu</label>
              <select class="form-select" v-model="statusFilter" @change="applyFilters">
                <option value="">Tüm Durumlar</option>
                <option value="0">Beklemede</option>
                <option value="1">Onaylandı</option>
                <option value="2">Reddedildi</option>
                <option value="3">Geri Çekildi</option>
              </select>
            </div>
            <div class="col-md-4">
              <label class="form-label">Sıralama</label>
              <select class="form-select" v-model="sortOrder" @change="applyFilters">
                <option value="newest">En Yeni</option>
                <option value="oldest">En Eski</option>
                <option value="status">Duruma Göre</option>
              </select>
            </div>
            <div class="col-md-4 d-flex align-items-end">
              <button @click="refreshApplications" class="btn btn-outline-primary">
                <i class="fas fa-refresh me-2"></i>Yenile
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="text-center py-5">
        <div class="loading-spinner mx-auto mb-3"></div>
        <p>Başvurularınız yükleniyor...</p>
      </div>

      <!-- Applications List -->
      <div v-else-if="filteredApplications.length > 0">
        <div class="row">
          <div
            v-for="application in filteredApplications"
            :key="application.id"
            class="col-lg-6 mb-4"
          >
            <div class="card card-custom h-100">
              <div class="card-header border-0 bg-transparent">
                <div class="d-flex justify-content-between align-items-start">
                  <span
                    class="badge"
                    :class="getStatusBadgeClass(application.status)"
                  >
                    {{ getStatusText(application.status) }}
                  </span>
                  <small class="text-muted">{{ formatDate(application.createdAt) }}</small>
                </div>
              </div>

              <div class="card-body">
                <h5 class="card-title fw-bold mb-2">
                  <router-link
                    :to="`/announces/${application.announceId}`"
                    class="text-decoration-none text-dark"
                  >
                    {{ application.announce?.title }}
                  </router-link>
                </h5>

                <div class="mb-3">
                  <span class="badge bg-primary me-2">
                    {{ application.announce?.category?.name }}
                  </span>
                  <span class="badge bg-secondary">
                    {{ getCollaborationTypeText(application.announce?.collaborationType) }}
                  </span>
                </div>

                <div class="application-message mb-3">
                  <strong>Başvuru Mesajınız:</strong>
                  <p class="text-muted mt-1">{{ truncateText(application.message, 150) }}</p>
                </div>

                <div v-if="application.reviewNote" class="review-note mb-3">
                  <strong>İnceleme Notu:</strong>
                  <p class="text-muted mt-1">{{ application.reviewNote }}</p>
                </div>

                <div v-if="application.reviewedAt" class="review-date mb-3">
                  <small class="text-muted">
                    <i class="fas fa-clock me-1"></i>
                    İncelendi: {{ formatDate(application.reviewedAt) }}
                  </small>
                </div>
              </div>

              <div class="card-footer border-0 bg-transparent">
                <div class="d-flex justify-content-between align-items-center">
                  <router-link
                    :to="`/announces/${application.announceId}`"
                    class="btn btn-sm btn-outline-primary"
                  >
                    <i class="fas fa-eye me-1"></i>Duyuruyu Görüntüle
                  </router-link>

                  <button
                    v-if="application.status === 0"
                    @click="withdrawApplication(application.id)"
                    class="btn btn-sm btn-outline-danger"
                  >
                    <i class="fas fa-times me-1"></i>Geri Çek
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- No Results -->
      <div v-else class="text-center py-5">
        <i class="fas fa-file-alt fa-4x text-muted mb-4"></i>
        <h3>Henüz başvuru yapmamışsınız</h3>
        <p class="text-muted">İlginizi çeken projelere başvuru yaparak ekiplere katılın</p>
        <router-link to="/announces" class="btn btn-primary btn-lg">
          <i class="fas fa-search me-2"></i>Duyuruları Keşfedin
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
  name: 'MyApplications',
  setup() {
    const authStore = useAuthStore()
    const toast = useToast()

    const applications = ref([])
    const loading = ref(false)

    // Filters
    const statusFilter = ref('')
    const sortOrder = ref('newest')

    const stats = computed(() => {
      const total = applications.value.length
      const pending = applications.value.filter(a => a.status === 0).length
      const approved = applications.value.filter(a => a.status === 1).length
      const rejected = applications.value.filter(a => a.status === 2).length

      return { total, pending, approved, rejected }
    })

    const filteredApplications = computed(() => {
      let filtered = [...applications.value]

      // Status filter
      if (statusFilter.value) {
        filtered = filtered.filter(a => a.status.toString() === statusFilter.value)
      }

      // Sort
      switch (sortOrder.value) {
        case 'oldest':
          filtered.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt))
          break
        case 'status':
          filtered.sort((a, b) => a.status - b.status)
          break
        default: // newest
          filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      }

      return filtered
    })

    const fetchMyApplications = async () => {
      loading.value = true
      try {
        const response = await axios.get('/applications/my-applications')
        if (response.data.success) {
          applications.value = response.data.data
        }
      } catch (error) {
        toast.error('Başvurular yüklenirken hata oluştu')
        console.error('Error fetching my applications:', error)
      } finally {
        loading.value = false
      }
    }

    const withdrawApplication = async (applicationId) => {
      if (!confirm('Başvurunuzu geri çekmek istediğinizden emin misiniz?')) return

      try {
        const response = await axios.delete(`/applications/${applicationId}`)
        if (response.data.success) {
          applications.value = applications.value.filter(a => a.id !== applicationId)
          toast.success('Başvuru başarıyla geri çekildi')
        }
      } catch (error) {
        toast.error('Başvuru geri çekilirken hata oluştu')
      }
    }

    const refreshApplications = () => {
      fetchMyApplications()
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
        0: 'Beklemede', 1: 'Onaylandı', 2: 'Reddedildi', 3: 'Geri Çekildi'
      }
      return statusMap[status] || 'Bilinmiyor'
    }

    const getStatusBadgeClass = (status) => {
      const classMap = {
        0: 'bg-warning', 1: 'bg-success', 2: 'bg-danger', 3: 'bg-secondary'
      }
      return classMap[status] || 'bg-secondary'
    }

    const getCollaborationTypeText = (type) => {
      const typeMap = {
        0: 'Proje', 1: 'Ortaklık', 2: 'Araştırma', 3: 'Etkinlik', 4: 'Gönüllülük', 5: 'Diğer'
      }
      return typeMap[type] || 'Proje'
    }

    onMounted(() => {
      fetchMyApplications()
    })

    return {
      applications,
      loading,
      statusFilter,
      sortOrder,
      stats,
      filteredApplications,
      authStore,
      withdrawApplication,
      refreshApplications,
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
.my-applications-page {
  padding: 2rem 0;
}

.application-message,
.review-note {
  background-color: #f8f9fa;
  border-left: 4px solid #007bff;
  padding: 0.75rem;
  border-radius: 0 8px 8px 0;
}

.review-note {
  border-left-color: #28a745;
}

.card-custom:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

@media (max-width: 768px) {
  .my-applications-page {
    padding: 1rem 0;
  }
}
</style>
