<template>
  <div class="create-announce-page">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-lg-8">
          <div class="card card-custom shadow">
            <div class="card-header bg-primary text-white">
              <h3 class="mb-0">
                <i class="fas fa-plus me-2"></i>
                Yeni Duyuru Oluştur
              </h3>
            </div>

            <div class="card-body p-4">
              <form @submit.prevent="handleSubmit">
                <!-- Title -->
                <div class="mb-4">
                  <label for="title" class="form-label fw-bold">
                    <i class="fas fa-heading me-1"></i>
                    Başlık *
                  </label>
                  <input
                    id="title"
                    type="text"
                    class="form-control"
                    v-model="form.title"
                    :class="{ 'is-invalid': errors.title }"
                    placeholder="Projenizin çekici bir başlığını yazın"
                    maxlength="200"
                    required
                  >
                  <div v-if="errors.title" class="invalid-feedback">
                    {{ errors.title }}
                  </div>
                  <div class="form-text">
                    {{ form.title.length }}/200 karakter
                  </div>
                </div>

                <!-- Category and Collaboration Type -->
                <div class="row mb-4">
                  <div class="col-md-6">
                    <label for="categoryId" class="form-label fw-bold">
                      <i class="fas fa-folder me-1"></i>
                      Kategori *
                    </label>
                    <select
                      id="categoryId"
                      class="form-select"
                      v-model="form.categoryId"
                      :class="{ 'is-invalid': errors.categoryId }"
                      required
                    >
                      <option value="">Kategori Seçin</option>
                      <option
                        v-for="category in categories"
                        :key="category.id"
                        :value="category.id"
                      >
                        {{ category.name }}
                      </option>
                    </select>
                    <div v-if="errors.categoryId" class="invalid-feedback">
                      {{ errors.categoryId }}
                    </div>
                  </div>

                  <div class="col-md-6">
                    <label for="collaborationType" class="form-label fw-bold">
                      <i class="fas fa-handshake me-1"></i>
                      İşbirliği Türü *
                    </label>
                    <select
                      id="collaborationType"
                      class="form-select"
                      v-model="form.collaborationType"
                      :class="{ 'is-invalid': errors.collaborationType }"
                      required
                    >
                      <option value="">Tür Seçin</option>
                      <option value="0">Proje</option>
                      <option value="1">Ortaklık</option>
                      <option value="2">Araştırma</option>
                      <option value="3">Etkinlik</option>
                      <option value="4">Gönüllülük</option>
                      <option value="5">Diğer</option>
                    </select>
                    <div v-if="errors.collaborationType" class="invalid-feedback">
                      {{ errors.collaborationType }}
                    </div>
                  </div>
                </div>

                <!-- Description -->
                <div class="mb-4">
                  <label for="description" class="form-label fw-bold">
                    <i class="fas fa-align-left me-1"></i>
                    Kısa Açıklama
                  </label>
                  <textarea
                    id="description"
                    class="form-control"
                    v-model="form.description"
                    :class="{ 'is-invalid': errors.description }"
                    rows="3"
                    placeholder="Projenizin kısa açıklaması (duyuru kartında görünecek)"
                    maxlength="500"
                  ></textarea>
                  <div v-if="errors.description" class="invalid-feedback">
                    {{ errors.description }}
                  </div>
                  <div class="form-text">
                    {{ (form.description || '').length }}/500 karakter
                  </div>
                </div>

                <!-- Content -->
                <div class="mb-4">
                  <label for="content" class="form-label fw-bold">
                    <i class="fas fa-file-text me-1"></i>
                    Detaylı İçerik *
                  </label>
                  <textarea
                    id="content"
                    class="form-control"
                    v-model="form.content"
                    :class="{ 'is-invalid': errors.content }"
                    rows="8"
                    placeholder="Projenizin detaylı açıklaması, hedefler, beklentiler, süreç vb."
                    maxlength="5000"
                    required
                  ></textarea>
                  <div v-if="errors.content" class="invalid-feedback">
                    {{ errors.content }}
                  </div>
                  <div class="form-text">
                    {{ form.content.length }}/5000 karakter
                  </div>
                </div>

                <!-- Participants and Expiry -->
                <div class="row mb-4">
                  <div class="col-md-6">
                    <label for="maxParticipants" class="form-label fw-bold">
                      <i class="fas fa-users me-1"></i>
                      Maksimum Katılımcı *
                    </label>
                    <input
                      id="maxParticipants"
                      type="number"
                      class="form-control"
                      v-model.number="form.maxParticipants"
                      :class="{ 'is-invalid': errors.maxParticipants }"
                      min="1"
                      max="100"
                      required
                    >
                    <div v-if="errors.maxParticipants" class="invalid-feedback">
                      {{ errors.maxParticipants }}
                    </div>
                  </div>

                  <div class="col-md-6">
                    <label for="expiryDate" class="form-label fw-bold">
                      <i class="fas fa-calendar me-1"></i>
                      Son Başvuru Tarihi
                    </label>
                    <input
                      id="expiryDate"
                      type="datetime-local"
                      class="form-control"
                      v-model="form.expiryDate"
                      :class="{ 'is-invalid': errors.expiryDate }"
                      :min="minDate"
                    >
                    <div v-if="errors.expiryDate" class="invalid-feedback">
                      {{ errors.expiryDate }}
                    </div>
                  </div>
                </div>

                <!-- Location and Contact -->
                <div class="row mb-4">
                  <div class="col-md-6">
                    <label for="location" class="form-label fw-bold">
                      <i class="fas fa-map-marker-alt me-1"></i>
                      Konum
                    </label>
                    <input
                      id="location"
                      type="text"
                      class="form-control"
                      v-model="form.location"
                      :class="{ 'is-invalid': errors.location }"
                      placeholder="Şehir, ülke veya 'Remote'"
                      maxlength="200"
                    >
                    <div v-if="errors.location" class="invalid-feedback">
                      {{ errors.location }}
                    </div>
                  </div>

                  <div class="col-md-6">
                    <label for="contactInfo" class="form-label fw-bold">
                      <i class="fas fa-envelope me-1"></i>
                      İletişim Bilgisi
                    </label>
                    <input
                      id="contactInfo"
                      type="text"
                      class="form-control"
                      v-model="form.contactInfo"
                      :class="{ 'is-invalid': errors.contactInfo }"
                      placeholder="Email, telefon veya diğer iletişim"
                      maxlength="500"
                    >
                    <div v-if="errors.contactInfo" class="invalid-feedback">
                      {{ errors.contactInfo }}
                    </div>
                  </div>
                </div>

                <!-- Required Skills -->
                <div class="mb-4">
                  <label for="requiredSkills" class="form-label fw-bold">
                    <i class="fas fa-cogs me-1"></i>
                    Gerekli Beceriler
                  </label>
                  <textarea
                    id="requiredSkills"
                    class="form-control"
                    v-model="form.requiredSkills"
                    :class="{ 'is-invalid': errors.requiredSkills }"
                    rows="3"
                    placeholder="Hangi beceri ve deneyimlere ihtiyaç var?"
                    maxlength="1000"
                  ></textarea>
                  <div v-if="errors.requiredSkills" class="invalid-feedback">
                    {{ errors.requiredSkills }}
                  </div>
                  <div class="form-text">
                    {{ (form.requiredSkills || '').length }}/1000 karakter
                  </div>
                </div>

                <!-- Tags -->
                <div class="mb-4">
                  <label class="form-label fw-bold">
                    <i class="fas fa-tags me-1"></i>
                    Etiketler
                  </label>
                  <div class="tag-input-container">
                    <input
                      type="text"
                      class="form-control"
                      v-model="newTag"
                      @keydown.enter.prevent="addTag"
                      @keydown.comma.prevent="addTag"
                      placeholder="Etiket yazın ve Enter'a basın"
                      maxlength="50"
                    >
                    <div class="tag-list mt-2" v-if="form.tags.length > 0">
                      <span
                        v-for="(tag, index) in form.tags"
                        :key="index"
                        class="badge bg-primary me-2 mb-2 tag-badge"
                      >
                        {{ tag }}
                        <button
                          type="button"
                          class="btn-close btn-close-white ms-2"
                          @click="removeTag(index)"
                          style="font-size: 0.6em;"
                        ></button>
                      </span>
                    </div>
                    <div class="form-text">
                      {{ form.tags.length }}/10 etiket (virgül veya Enter ile ayırın)
                    </div>
                  </div>
                </div>

                <!-- Form Actions -->
                <div class="d-flex justify-content-between">
                  <router-link to="/announces" class="btn btn-outline-secondary">
                    <i class="fas fa-arrow-left me-2"></i>İptal
                  </router-link>

                  <button
                    type="submit"
                    class="btn btn-primary btn-lg"
                    :disabled="loading"
                  >
                    <span v-if="loading" class="loading-spinner me-2"></span>
                    <i v-else class="fas fa-save me-2"></i>
                    {{ loading ? 'Kaydediliyor...' : 'Duyuru Oluştur' }}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAnnouncesStore } from '@/stores/announces'
import { useToast } from 'vue-toastification'

export default {
  name: 'CreateAnnounce',
  setup() {
    const router = useRouter()
    const announcesStore = useAnnouncesStore()
    const toast = useToast()

    const form = ref({
      title: '',
      categoryId: '',
      description: '',
      content: '',
      collaborationType: '',
      maxParticipants: 1,
      expiryDate: '',
      location: '',
      contactInfo: '',
      requiredSkills: '',
      tags: []
    })

    const newTag = ref('')
    const errors = ref({})
    const loading = ref(false)
    const categories = ref([])

    const minDate = computed(() => {
      const tomorrow = new Date()
      tomorrow.setDate(tomorrow.getDate() + 1)
      return tomorrow.toISOString().slice(0, 16)
    })

    const validateForm = () => {
      errors.value = {}

      if (!form.value.title.trim()) {
        errors.value.title = 'Başlık gerekli'
      }

      if (!form.value.categoryId) {
        errors.value.categoryId = 'Kategori seçimi gerekli'
      }

      if (!form.value.content.trim()) {
        errors.value.content = 'Detaylı içerik gerekli'
      }

      if (form.value.collaborationType === '') {
        errors.value.collaborationType = 'İşbirliği türü seçimi gerekli'
      }

      if (form.value.maxParticipants < 1 || form.value.maxParticipants > 100) {
        errors.value.maxParticipants = 'Katılımcı sayısı 1-100 arasında olmalı'
      }

      if (form.value.expiryDate) {
        const expiryDate = new Date(form.value.expiryDate)
        const now = new Date()
        if (expiryDate <= now) {
          errors.value.expiryDate = 'Son başvuru tarihi gelecekte olmalı'
        }
      }

      return Object.keys(errors.value).length === 0
    }

    const addTag = () => {
      const tag = newTag.value.trim()
      if (tag && !form.value.tags.includes(tag) && form.value.tags.length < 10) {
        form.value.tags.push(tag)
        newTag.value = ''
      }
    }

    const removeTag = (index) => {
      form.value.tags.splice(index, 1)
    }

    const handleSubmit = async () => {
      if (!validateForm()) return

      loading.value = true
      try {
        const result = await announcesStore.createAnnounce({
          ...form.value,
          collaborationType: parseInt(form.value.collaborationType),
          expiryDate: form.value.expiryDate || null
        })

        if (result.success) {
          router.push(`/announces/${result.data.id}`)
        }
      } catch (error) {
        console.error('Error creating announce:', error)
      } finally {
        loading.value = false
      }
    }

    const fetchCategories = async () => {
      await announcesStore.fetchCategories()
      categories.value = announcesStore.categories
    }

    onMounted(() => {
      fetchCategories()
    })

    return {
      form,
      newTag,
      errors,
      loading,
      categories,
      minDate,
      addTag,
      removeTag,
      handleSubmit
    }
  }
}
</script>

<style scoped>
.create-announce-page {
  padding: 2rem 0;
  min-height: 80vh;
}

.form-control,
.form-select {
  border-radius: 8px;
  border: 2px solid #e9ecef;
  transition: all 0.3s ease;
}

.form-control:focus,
.form-select:focus {
  border-color: #007bff;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}

.tag-badge {
  position: relative;
  padding-right: 2rem;
  cursor: default;
}

.tag-badge .btn-close {
  position: absolute;
  right: 0.5rem;
  top: 50%;
  transform: translateY(-50%);
}

.tag-input-container {
  border: 2px dashed #dee2e6;
  border-radius: 8px;
  padding: 1rem;
  background-color: #f8f9fa;
}

.tag-list {
  max-height: 120px;
  overflow-y: auto;
}

@media (max-width: 768px) {
  .create-announce-page {
    padding: 1rem 0;
  }

  .card-body {
    padding: 1.5rem !important;
  }
}
</style>
