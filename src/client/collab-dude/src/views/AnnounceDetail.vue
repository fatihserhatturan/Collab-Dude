<template>
  <div class="announce-detail-page">
    <div class="container">
      <!-- Loading -->
      <div v-if="loading" class="text-center py-5">
        <div class="loading-spinner mx-auto mb-3"></div>
        <p>Duyuru yükleniyor...</p>
      </div>

      <!-- Announce Detail -->
      <div v-else-if="announce" class="row">
        <div class="col-lg-8">
          <!-- Main Content -->
          <div class="card card-custom mb-4">
            <div class="card-header border-0 bg-transparent">
              <div class="d-flex justify-content-between align-items-start">
                <div class="d-flex align-items-center">
                  <div class="user-avatar me-3">
                    <i class="fas fa-user-circle fa-2x text-primary"></i>
                  </div>
                  <div>
                    <h6 class="fw-bold mb-0">{{ announce.username }}</h6>
                    <small class="text-muted">{{ formatDate(announce.createdAt) }}</small>
                  </div>
                </div>
                <div class="announce-actions" v-if="isOwner">
                  <router-link
                    :to="`/announces/${announce.id}/edit`"
                    class="btn btn-sm btn-outline-primary me-2"
                  >
                    <i class="fas fa-edit"></i>
                  </router-link>
                  <button
                    @click="deleteAnnounce"
                    class="btn btn-sm btn-outline-danger"
                    :disabled="deleting"
                  >
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </div>
            </div>

            <div class="card-body">
              <h1 class="fw-bold mb-3">{{ announce.title }}</h1>

              <!-- Status and Category -->
              <div class="mb-4">
                <span
                  class="badge me-2"
                  :class="getStatusBadgeClass(announce.status)"
                >
                  {{ getStatusText(announce.status) }}
                </span>
                <span class="badge bg-primary me-2">
                  <i class="fas fa-folder me-1"></i>
                  {{ announce.category?.name }}
                </span>
                <span class="badge bg-secondary">
                  <i class="fas fa-handshake me-1"></i>
                  {{ getCollaborationTypeText(announce.collaborationType) }}
                </span>
              </div>

              <!-- Description -->
              <div v-if="announce.description" class="mb-4">
                <h5>Kısa Açıklama</h5>
                <p class="text-muted">{{ announce.description }}</p>
              </div>

              <!-- Content -->
              <div class="mb-4">
                <h5>Detaylı İçerik</h5>
                <div class="content-text">{{ announce.content }}</div>
              </div>

              <!-- Required Skills -->
              <div v-if="announce.requiredSkills" class="mb-4">
                <h5>Gerekli Beceriler</h5>
                <p>{{ announce.requiredSkills }}</p>
              </div>

              <!-- Tags -->
              <div v-if="announce.tags && announce.tags.length > 0" class="mb-4">
                <h5>Etiketler</h5>
                <div>
                  <span
                    v-for="tag in announce.tags"
                    :key="tag.id"
                    class="badge me-2 mb-2"
                    :style="{ backgroundColor: tag.color, color: 'white' }"
                  >
                    {{ tag.name }}
                  </span>
                </div>
              </div>

              <!-- Contact Info -->
              <div v-if="announce.contactInfo" class="mb-4">
                <h5>İletişim Bilgileri</h5>
                <p>{{ announce.contactInfo }}</p>
              </div>
            </div>
          </div>

          <!-- Comments Section -->
          <div class="card card-custom">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="fas fa-comments me-2"></i>
                Yorumlar ({{ comments.length }})
              </h5>
            </div>
            <div class="card-body">
              <!-- Add Comment Form -->
              <div v-if="authStore.isLoggedIn" class="mb-4">
                <form @submit.prevent="submitComment">
                  <div class="mb-3">
                    <textarea
                      class="form-control"
                      v-model="newComment"
                      rows="3"
                      placeholder="Yorumunuzu yazın..."
                      maxlength="1000"
                      required
                    ></textarea>
                  </div>
                  <button
                    type="submit"
                    class="btn btn-primary"
                    :disabled="!newComment.trim() || submittingComment"
                  >
                    <span v-if="submittingComment" class="loading-spinner me-2"></span>
                    <i v-else class="fas fa-paper-plane me-2"></i>
                    Yorum Yap
                  </button>
                </form>
              </div>

              <!-- Comments List -->
              <div v-if="comments.length > 0">
                <div
                  v-for="comment in comments"
                  :key="comment.id"
                  class="comment-item mb-3"
                >
                  <div class="d-flex">
                    <div class="user-avatar me-3">
                      <i class="fas fa-user-circle text-primary"></i>
                    </div>
                    <div class="flex-grow-1">
                      <div class="comment-header d-flex justify-content-between">
                        <div>
                          <strong>{{ comment.username }}</strong>
                          <small class="text-muted ms-2">{{ formatDate(comment.createdAt) }}</small>
                          <span v-if="comment.isEdited" class="badge bg-light text-dark ms-2">
                            düzenlendi
                          </span>
                        </div>
                        <div v-if="comment.username === authStore.userName" class="comment-actions">
                          <button
                            @click="editComment(comment)"
                            class="btn btn-sm btn-outline-secondary me-1"
                          >
                            <i class="fas fa-edit"></i>
                          </button>
                          <button
                            @click="deleteComment(comment.id)"
                            class="btn btn-sm btn-outline-danger"
                          >
                            <i class="fas fa-trash"></i>
                          </button>
                        </div>
                      </div>
                      <div class="comment-content mt-2">
                        <div v-if="editingComment?.id === comment.id">
                          <form @submit.prevent="updateComment">
                            <textarea
                              class="form-control mb-2"
                              v-model="editingComment.content"
                              rows="3"
                              maxlength="1000"
                              required
                            ></textarea>
                            <button type="submit" class="btn btn-sm btn-primary me-2">
                              Güncelle
                            </button>
                            <button
                              type="button"
                              @click="cancelEdit"
                              class="btn btn-sm btn-secondary"
                            >
                              İptal
                            </button>
                          </form>
                        </div>
                        <p v-else class="mb-2">{{ comment.content }}</p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div v-else class="text-center text-muted py-4">
                <i class="fas fa-comment fa-3x mb-3"></i>
                <p>Henüz yorum yapılmamış. İlk yorumu siz yapın!</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="col-lg-4">
          <!-- Application Card -->
          <div class="card card-custom mb-4">
            <div class="card-header bg-primary text-white">
              <h5 class="mb-0">
                <i class="fas fa-paper-plane me-2"></i>
                Başvuru Bilgileri
              </h5>
            </div>
            <div class="card-body">
              <!-- Participant Info -->
              <div class="mb-3">
                <div class="d-flex justify-content-between align-items-center mb-2">
                  <span>Katılımcılar:</span>
                  <strong>{{ announce.currentParticipants }}/{{ announce.maxParticipants }}</strong>
                </div>
                <div class="progress">
                  <div
                    class="progress-bar"
                    :class="getProgressBarClass(announce.currentParticipants, announce.maxParticipants)"
                    :style="{ width: getProgressPercentage(announce.currentParticipants, announce.maxParticipants) + '%' }"
                  ></div>
                </div>
              </div>

              <!-- Expiry Date -->
              <div v-if="announce.expiryDate" class="mb-3">
                <div class="d-flex justify-content-between">
                  <span>Son Başvuru:</span>
                  <span
                    class="fw-bold"
                    :class="{ 'text-danger': isNearExpiry(announce.expiryDate) }"
                  >
                    {{ formatDate(announce.expiryDate) }}
                  </span>
                </div>
              </div>

              <!-- Location -->
              <div v-if="announce.location" class="mb-3">
                <div class="d-flex justify-content-between">
                  <span>Konum:</span>
                  <span class="fw-bold">{{ announce.location }}</span>
                </div>
              </div>

              <!-- Application Status -->
              <div v-if="authStore.isLoggedIn && !isOwner">
                <div v-if="userApplication" class="alert alert-info">
                  <i class="fas fa-info-circle me-2"></i>
                  <strong>Başvuru Durumu:</strong>
                  {{ getApplicationStatusText(userApplication.status) }}
                  <div v-if="userApplication.reviewNote" class="mt-2">
                    <small><strong>Not:</strong> {{ userApplication.reviewNote }}</small>
                  </div>
                </div>
                <button
                  v-else-if="canApply"
                  @click="showApplicationModal = true"
                  class="btn btn-primary w-100"
                >
                  <i class="fas fa-paper-plane me-2"></i>
                  Başvuru Yap
                </button>
                <div v-else class="alert alert-warning">
                  <i class="fas fa-exclamation-triangle me-2"></i>
                  Başvuru yapılamıyor
                  <div v-if="announce.isFull" class="mt-1">
                    <small>Katılımcı sayısı dolu</small>
                  </div>
                  <div v-if="announce.isExpired" class="mt-1">
                    <small>Başvuru süresi dolmuş</small>
                  </div>
                </div>
              </div>

              <div v-else-if="!authStore.isLoggedIn" class="alert alert-info">
                Başvuru yapmak için <router-link to="/login">giriş yapın</router-link>
              </div>
            </div>
          </div>

          <!-- Statistics Card -->
          <div class="card card-custom">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="fas fa-chart-bar me-2"></i>
                İstatistikler
              </h5>
            </div>
            <div class="card-body">
              <div class="stat-item d-flex justify-content-between mb-2">
                <span>Görüntülenme:</span>
                <span class="fw-bold">{{ Math.floor(Math.random() * 1000) + 100 }}</span>
              </div>
              <div class="stat-item d-flex justify-content-between mb-2">
                <span>Başvuru Sayısı:</span>
                <span class="fw-bold">{{ announce.applicationsCount || 0 }}</span>
              </div>
              <div class="stat-item d-flex justify-content-between">
                <span>Yorum Sayısı:</span>
                <span class="fw-bold">{{ announce.commentsCount || 0 }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Not Found -->
      <div v-else class="text-center py-5">
        <i class="fas fa-exclamation-triangle fa-4x text-warning mb-4"></i>
        <h3>Duyuru bulunamadı</h3>
        <p class="text-muted">Aradığınız duyuru mevcut değil veya silinmiş olabilir.</p>
        <router-link to="/announces" class="btn btn-primary">
          <i class="fas fa-arrow-left me-2"></i>Duyurulara Dön
        </router-link>
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
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import ApplicationModal from '@/components/ApplicationModal.vue'
import axios from 'axios'
import moment from 'moment'
import { useToast } from 'vue-toastification'

export default {
  name: 'AnnounceDetail',
  components: {
    ApplicationModal
  },
  props: {
    id: {
      type: String,
      required: true
    }
  },
  setup(props) {
    const router = useRouter()
    const authStore = useAuthStore()
    const toast = useToast()

    const announce = ref(null)
    const comments = ref([])
    const userApplication = ref(null)
    const loading = ref(false)
    const deleting = ref(false)
    const showApplicationModal = ref(false)

    // Comment form
    const newComment = ref('')
    const submittingComment = ref(false)
    const editingComment = ref(null)

    const isOwner = computed(() => {
      return authStore.isLoggedIn && announce.value?.username === authStore.userName
    })

    const canApply = computed(() => {
      return authStore.isLoggedIn &&
             !isOwner.value &&
             announce.value &&
             !announce.value.isFull &&
             !announce.value.isExpired &&
             announce.value.status === 1 &&
             !userApplication.value
    })

    const fetchAnnounceDetail = async () => {
      loading.value = true
      try {
        const response = await axios.get(`/announces/${props.id}/details`)
        if (response.data.success) {
          announce.value = response.data.data
          comments.value = response.data.data.comments || []
        } else {
          announce.value = null
        }
      } catch (error) {
        console.error('Error fetching announce detail:', error)
        announce.value = null
      } finally {
        loading.value = false
      }
    }

    const fetchUserApplication = async () => {
      if (!authStore.isLoggedIn || isOwner.value) return

      try {
        const response = await axios.get(`/applications/announce/${props.id}/has-applied`)
        if (response.data.success && response.data.data) {
          const applicationsResponse = await axios.get('/applications/my-applications')
          if (applicationsResponse.data.success) {
            userApplication.value = applicationsResponse.data.data.find(
              app => app.announceId === props.id
            )
          }
        }
      } catch (error) {
        console.error('Error fetching user application:', error)
      }
    }

    const submitComment = async () => {
      if (!newComment.value.trim()) return

      submittingComment.value = true
      try {
        const response = await axios.post('/comments', {
          announceId: props.id,
          content: newComment.value.trim()
        })

        if (response.data.success) {
          comments.value.push(response.data.data)
          newComment.value = ''
          toast.success('Yorum başarıyla eklendi!')
        }
      } catch (error) {
        toast.error('Yorum eklenirken hata oluştu')
      } finally {
        submittingComment.value = false
      }
    }

    const editComment = (comment) => {
      editingComment.value = { ...comment }
    }

    const cancelEdit = () => {
      editingComment.value = null
    }

    const updateComment = async () => {
      if (!editingComment.value.content.trim()) return

      try {
        const response = await axios.put(`/comments/${editingComment.value.id}`, {
          id: editingComment.value.id,
          content: editingComment.value.content.trim()
        })

        if (response.data.success) {
          const index = comments.value.findIndex(c => c.id === editingComment.value.id)
          if (index !== -1) {
            comments.value[index] = response.data.data
          }
          editingComment.value = null
          toast.success('Yorum başarıyla güncellendi!')
        }
      } catch (error) {
        toast.error('Yorum güncellenirken hata oluştu')
      }
    }

    const deleteComment = async (commentId) => {
      if (!confirm('Yorumu silmek istediğinizden emin misiniz?')) return

      try {
        const response = await axios.delete(`/comments/${commentId}`)
        if (response.data.success) {
          comments.value = comments.value.filter(c => c.id !== commentId)
          toast.success('Yorum başarıyla silindi!')
        }
      } catch (error) {
        toast.error('Yorum silinirken hata oluştu')
      }
    }

    const deleteAnnounce = async () => {
      if (!confirm('Duyuruyu silmek istediğinizden emin misiniz?')) return

      deleting.value = true
      try {
        const response = await axios.delete(`/announces/${props.id}`)
        if (response.data.success) {
          toast.success('Duyuru başarıyla silindi!')
          router.push('/my-announces')
        }
      } catch (error) {
        toast.error('Duyuru silinirken hata oluştu')
      } finally {
        deleting.value = false
      }
    }

    const handleApplicationSubmitted = () => {
      showApplicationModal.value = false
      fetchUserApplication()
      toast.success('Başvurunuz başarıyla gönderildi!')
    }

    // Utility functions
    const formatDate = (date) => moment(date).fromNow()
    const isNearExpiry = (expiryDate) => moment(expiryDate).diff(moment(), 'days') <= 3

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

    const getProgressPercentage = (current, max) => max > 0 ? (current / max) * 100 : 0

    const getProgressBarClass = (current, max) => {
      const percentage = getProgressPercentage(current, max)
      if (percentage >= 90) return 'bg-danger'
      if (percentage >= 70) return 'bg-warning'
      return 'bg-success'
    }

    const getApplicationStatusText = (status) => {
      const statusMap = {
        0: 'Beklemede', 1: 'Onaylandı', 2: 'Reddedildi', 3: 'Geri Çekildi'
      }
      return statusMap[status] || 'Bilinmiyor'
    }

    onMounted(async () => {
      await fetchAnnounceDetail()
      if (authStore.isLoggedIn) {
        await fetchUserApplication()
      }
    })

    return {
      announce,
      comments,
      userApplication,
      loading,
      deleting,
      showApplicationModal,
      newComment,
      submittingComment,
      editingComment,
      isOwner,
      canApply,
      authStore,
      submitComment,
      editComment,
      cancelEdit,
      updateComment,
      deleteComment,
      deleteAnnounce,
      handleApplicationSubmitted,
      formatDate,
      isNearExpiry,
      getStatusText,
      getStatusBadgeClass,
      getCollaborationTypeText,
      getProgressPercentage,
      getProgressBarClass,
      getApplicationStatusText
    }
  }
}
</script>

<style scoped>
.announce-detail-page {
  padding: 2rem 0;
}

.user-avatar {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.content-text {
  line-height: 1.6;
  white-space: pre-wrap;
}

.comment-item {
  padding: 1rem;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  background-color: #f8f9fa;
}

.comment-actions .btn {
  padding: 0.25rem 0.5rem;
}

.progress {
  height: 8px;
  border-radius: 10px;
}

.stat-item {
  padding: 0.5rem 0;
  border-bottom: 1px solid #e9ecef;
}

.stat-item:last-child {
  border-bottom: none;
}

@media (max-width: 768px) {
  .announce-detail-page {
    padding: 1rem 0;
  }

  .announce-actions {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>
