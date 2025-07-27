<template>
  <div class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">
            <i class="fas fa-paper-plane me-2"></i>
            Başvuru Yap
          </h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>

        <div class="modal-body">
          <div class="mb-3">
            <h6 class="fw-bold">{{ announce.title }}</h6>
            <p class="text-muted small">{{ announce.description }}</p>
          </div>

          <form @submit.prevent="submitApplication">
            <div class="mb-3">
              <label for="applicationMessage" class="form-label">
                <i class="fas fa-message me-1"></i>
                Başvuru Mesajı *
              </label>
              <textarea
                id="applicationMessage"
                class="form-control"
                v-model="form.message"
                :class="{ 'is-invalid': errors.message }"
                rows="5"
                placeholder="Neden bu projeye katılmak istiyorsunuz? Deneyimleriniz ve katkılarınız neler olacak?"
                required
              ></textarea>
              <div v-if="errors.message" class="invalid-feedback">
                {{ errors.message }}
              </div>
              <div class="form-text">
                <small>{{ form.message.length }}/1000 karakter</small>
              </div>
            </div>

            <div class="alert alert-info">
              <i class="fas fa-info-circle me-2"></i>
              <strong>Başvuru Bilgileri:</strong>
              <ul class="mb-0 mt-2">
                <li>Başvurunuz proje sahibi tarafından incelenecektir</li>
                <li>Sonuç bildirimi email ve sistem bildirimleri ile yapılacaktır</li>
                <li>Başvurunuzu daha sonra geri çekebilirsiniz</li>
              </ul>
            </div>
          </form>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="$emit('close')">
            <i class="fas fa-times me-2"></i>İptal
          </button>
          <button
            type="button"
            class="btn btn-primary"
            @click="submitApplication"
            :disabled="loading || !form.message.trim()"
          >
            <span v-if="loading" class="loading-spinner me-2"></span>
            <i v-else class="fas fa-paper-plane me-2"></i>
            {{ loading ? 'Gönderiliyor...' : 'Başvuru Gönder' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toastification'

export default {
  name: 'ApplicationModal',
  props: {
    announce: {
      type: Object,
      required: true
    }
  },
  emits: ['close', 'submitted'],
  setup(props, { emit }) {
    const toast = useToast()

    const form = ref({
      message: ''
    })

    const errors = ref({})
    const loading = ref(false)

    const validateForm = () => {
      errors.value = {}

      if (!form.value.message.trim()) {
        errors.value.message = 'Başvuru mesajı gerekli'
      } else if (form.value.message.length > 1000) {
        errors.value.message = 'Mesaj 1000 karakterden uzun olamaz'
      } else if (form.value.message.length < 10) {
        errors.value.message = 'Mesaj en az 10 karakter olmalı'
      }

      return Object.keys(errors.value).length === 0
    }

    const submitApplication = async () => {
      if (!validateForm()) return

      loading.value = true
      try {
        const applicationData = {
          announceId: props.announce.id,
          message: form.value.message.trim()
        }

        const response = await axios.post('/applications', applicationData)

        if (response.data.success) {
          toast.success('Başvurunuz başarıyla gönderildi!')
          emit('submitted')
        }
      } catch (error) {
        const message = error.response?.data?.message || 'Başvuru gönderilirken hata oluştu'
        toast.error(message)
      } finally {
        loading.value = false
      }
    }

    return {
      form,
      errors,
      loading,
      submitApplication
    }
  }
}
</script>

<style scoped>
.modal {
  animation: fadeIn 0.3s ease;
}

.modal-content {
  animation: slideIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideIn {
  from { transform: translateY(-50px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

.form-control {
  border-radius: 8px;
}

.form-control:focus {
  border-color: #007bff;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}
</style>
