<template>
  <div class="announce-card">
    <div class="card card-custom h-100">
      <div class="card-header border-0 bg-transparent">
        <div class="d-flex justify-content-between align-items-start">
          <div class="d-flex align-items-center">
            <div class="user-avatar me-2">
              <i class="fas fa-user-circle fa-lg text-primary"></i>
            </div>
            <div>
              <small class="fw-bold">{{ announce.username }}</small>
              <br>
              <small class="text-muted">{{ formatDate(announce.createdAt) }}</small>
            </div>
          </div>
          <span
            class="badge"
            :class="getStatusBadgeClass(announce.status)"
          >
            {{ getStatusText(announce.status) }}
          </span>
        </div>
      </div>

      <div class="card-body pb-2">
        <h5 class="card-title fw-bold mb-2">
          <router-link
            :to="`/announces/${announce.id}`"
            class="text-decoration-none text-dark"
          >
            {{ announce.title }}
          </router-link>
        </h5>

        <p class="card-text text-muted small mb-3" v-if="announce.description">
          {{ truncateText(announce.description, 100) }}
        </p>

        <!-- Category and Collaboration Type -->
        <div class="mb-3">
          <span class="badge bg-primary me-2">
            <i class="fas fa-folder me-1"></i>
            {{ announce.category?.name }}
          </span>
          <span class="badge bg-secondary">
            <i class="fas fa-handshake me-1"></i>
            {{ getCollaborationTypeText(announce.collaborationType) }}
          </span>
        </div>

        <!-- Tags -->
        <div class="mb-3" v-if="announce.tags && announce.tags.length > 0">
          <span
            v-for="tag in announce.tags.slice(0, 3)"
            :key="tag.id"
            class="badge badge-custom me-1 mb-1"
            :style="{ backgroundColor: tag.color }"
          >
            {{ tag.name }}
          </span>
          <span v-if="announce.tags.length > 3" class="badge bg-light text-dark">
            +{{ announce.tags.length - 3 }} more
          </span>
        </div>

        <!-- Location and Remote -->
        <div class="mb-3" v-if="announce.location">
          <small class="text-muted">
            <i class="fas fa-map-marker-alt me-1"></i>
            {{ announce.location }}
          </small>
        </div>

        <!-- Participants Info -->
        <div class="mb-3">
          <div class="d-flex justify-content-between align-items-center">
            <small class="text-muted">
              <i class="fas fa-users me-1"></i>
              {{ announce.currentParticipants }}/{{ announce.maxParticipants }} katılımcı
            </small>
            <div class="progress" style="width: 60px; height: 6px;">
              <div
                class="progress-bar"
                :class="getProgressBarClass(announce.currentParticipants, announce.maxParticipants)"
                :style="{ width: getProgressPercentage(announce.currentParticipants, announce.maxParticipants) + '%' }"
              ></div>
            </div>
          </div>
        </div>

        <!-- Expiry Date -->
        <div class="mb-3" v-if="announce.expiryDate">
          <small
            class="text-muted"
            :class="{ 'text-danger': isNearExpiry(announce.expiryDate) }"
          >
            <i class="fas fa-clock me-1"></i>
            Son başvuru: {{ formatDate(announce.expiryDate) }}
          </small>
        </div>
      </div>

      <div class="card-footer border-0 bg-transparent">
        <div class="d-flex justify-content-between align-items-center">
          <div class="social-stats">
            <small class="text-muted me-3">
              <i class="fas fa-comment me-1"></i>
              {{ announce.commentsCount || 0 }}
            </small>
            <small class="text-muted">
              <i class="fas fa-file-alt me-1"></i>
              {{ announce.applicationsCount || 0 }}
            </small>
          </div>

          <div class="action-buttons">
            <router-link
              :to="`/announces/${announce.id}`"
              class="btn btn-sm btn-outline-primary me-2"
            >
              <i class="fas fa-eye me-1"></i>
              Detay
            </router-link>

            <button
              v-if="canApply"
              @click="showApplicationModal = true"
              class="btn btn-sm btn-primary"
              :disabled="!authStore.isLoggedIn || announce.isFull || announce.isExpired"
            >
              <i class="fas fa-paper-plane me-1"></i>
              Başvur
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Application Modal -->
    <ApplicationModal
      v-if="showApplicationModal"
      :announce="announce"
      @close="showApplicationModal = false"
      @submitted="handleApplicationSubmitted"
    />
  </div>
</template>

<script>
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import ApplicationModal from '@/components/ApplicationModal.vue'
import moment from 'moment'

export default {
  name: 'AnnounceCard',
  components: {
    ApplicationModal
  },
  props: {
    announce: {
      type: Object,
      required: true
    }
  },
  emits: ['application-submitted'],
  setup(props, { emit }) {
    const authStore = useAuthStore()
    const showApplicationModal = ref(false)

    const canApply = computed(() => {
      return authStore.isLoggedIn &&
             props.announce.username !== authStore.userName &&
             !props.announce.isFull &&
             !props.announce.isExpired &&
             props.announce.status === 1
    })

    const truncateText = (text, maxLength) => {
      if (!text) return ''
      return text.length > maxLength ? text.substring(0, maxLength) + '...' : text
    }

    const formatDate = (date) => {
      return moment(date).fromNow()
    }

    const isNearExpiry = (expiryDate) => {
      const now = moment()
      const expiry = moment(expiryDate)
      return expiry.diff(now, 'days') <= 3
    }

    const getStatusText = (status) => {
      const statusMap = {
        0: 'Taslak',
        1: 'Aktif',
        2: 'Kapalı',
        3: 'Süresi Dolmuş',
        4: 'Askıya Alınmış'
      }
      return statusMap[status] || 'Bilinmiyor'
    }

    const getStatusBadgeClass = (status) => {
      const classMap = {
        0: 'bg-secondary',
        1: 'bg-success',
        2: 'bg-warning',
        3: 'bg-danger',
        4: 'bg-dark'
      }
      return classMap[status] || 'bg-secondary'
    }

    const getCollaborationTypeText = (type) => {
      const typeMap = {
        0: 'Proje',
        1: 'Ortaklık',
        2: 'Araştırma',
        3: 'Etkinlik',
        4: 'Gönüllülük',
        5: 'Diğer'
      }
      return typeMap[type] || 'Proje'
    }

    const getProgressPercentage = (current, max) => {
      return max > 0 ? (current / max) * 100 : 0
    }

    const getProgressBarClass = (current, max) => {
      const percentage = getProgressPercentage(current, max)
      if (percentage >= 90) return 'bg-danger'
      if (percentage >= 70) return 'bg-warning'
      return 'bg-success'
    }

    const handleApplicationSubmitted = () => {
      showApplicationModal.value = false
      emit('application-submitted')
    }

    return {
      authStore,
      showApplicationModal,
      canApply,
      truncateText,
      formatDate,
      isNearExpiry,
      getStatusText,
      getStatusBadgeClass,
      getCollaborationTypeText,
      getProgressPercentage,
      getProgressBarClass,
      handleApplicationSubmitted
    }
  }
}
</script>

<style scoped>
.announce-card {
  height: 100%;
}

.card-custom {
  transition: all 0.3s ease;
  border: 1px solid #e9ecef;
}

.card-custom:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.1);
  border-color: #007bff;
}

.user-avatar {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.card-title a:hover {
  color: #007bff !important;
}

.badge-custom {
  font-size: 0.7em;
  color: white !important;
}

.progress {
  border-radius: 10px;
  background-color: #e9ecef;
}

.progress-bar {
  border-radius: 10px;
}

.social-stats {
  display: flex;
  align-items: center;
}

.action-buttons .btn {
  font-size: 0.8rem;
  padding: 0.25rem 0.75rem;
}

@media (max-width: 768px) {
  .action-buttons {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .action-buttons .btn {
    font-size: 0.75rem;
  }
}
</style>
