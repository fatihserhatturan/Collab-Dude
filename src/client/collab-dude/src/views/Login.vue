<template>
  <div class="auth-container">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
          <div class="card card-custom shadow-lg">
            <div class="card-body p-5">
              <div class="text-center mb-4">
                <i class="fas fa-bullhorn fa-3x text-primary mb-3"></i>
                <h2 class="fw-bold">Giriş Yap</h2>
                <p class="text-muted">Hesabınıza erişim sağlayın</p>
              </div>

              <form @submit.prevent="handleLogin">
                <div class="mb-3">
                  <label for="emailOrUsername" class="form-label">
                    <i class="fas fa-user me-1"></i>Email veya Kullanıcı Adı
                  </label>
                  <input
                    id="emailOrUsername"
                    type="text"
                    class="form-control"
                    v-model="form.emailOrUsername"
                    :class="{ 'is-invalid': errors.emailOrUsername }"
                    placeholder="email@example.com veya kullaniciadi"
                    required
                  >
                  <div v-if="errors.emailOrUsername" class="invalid-feedback">
                    {{ errors.emailOrUsername }}
                  </div>
                </div>

                <div class="mb-4">
                  <label for="password" class="form-label">
                    <i class="fas fa-lock me-1"></i>Şifre
                  </label>
                  <div class="input-group">
                    <input
                      id="password"
                      :type="showPassword ? 'text' : 'password'"
                      class="form-control"
                      v-model="form.password"
                      :class="{ 'is-invalid': errors.password }"
                      placeholder="Şifrenizi girin"
                      required
                    >
                    <button
                      type="button"
                      class="btn btn-outline-secondary"
                      @click="showPassword = !showPassword"
                    >
                      <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
                    </button>
                    <div v-if="errors.password" class="invalid-feedback">
                      {{ errors.password }}
                    </div>
                  </div>
                </div>

                <div class="d-grid">
                  <button
                    type="submit"
                    class="btn btn-primary btn-custom"
                    :disabled="authStore.loading"
                  >
                    <span v-if="authStore.loading" class="loading-spinner me-2"></span>
                    <i v-else class="fas fa-sign-in-alt me-2"></i>
                    {{ authStore.loading ? 'Giriş yapılıyor...' : 'Giriş Yap' }}
                  </button>
                </div>
              </form>

              <div class="text-center mt-4">
                <p class="text-muted">
                  Hesabınız yok mu?
                  <router-link to="/register" class="text-decoration-none">
                    Kayıt olun
                  </router-link>
                </p>
              </div>

              <!-- Test Credentials Info -->
              <div class="alert alert-info mt-4">
                <strong>Test Bilgileri:</strong><br>
                <small>
                  Admin: admin@example.com / Password123!<br>
                  User: ahmet.kaya@example.com / Password123!
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

export default {
  name: 'LoginView',
  setup() {
    const router = useRouter()
    const authStore = useAuthStore()

    const form = ref({
      emailOrUsername: '',
      password: ''
    })

    const errors = ref({})
    const showPassword = ref(false)

    const validateForm = () => {
      errors.value = {}

      if (!form.value.emailOrUsername) {
        errors.value.emailOrUsername = 'Email veya kullanıcı adı gerekli'
      }

      if (!form.value.password) {
        errors.value.password = 'Şifre gerekli'
      } else if (form.value.password.length < 6) {
        errors.value.password = 'Şifre en az 6 karakter olmalı'
      }

      return Object.keys(errors.value).length === 0
    }

    const handleLogin = async () => {
      if (!validateForm()) return

      const result = await authStore.login(form.value)

      if (result.success) {
        router.push('/')
      }
    }

    return {
      form,
      errors,
      showPassword,
      authStore,
      handleLogin
    }
  }
}
</script>

<style scoped>
.auth-container {
  min-height: 80vh;
  display: flex;
  align-items: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 40px 0;
}

.card-custom {
  border: none;
  border-radius: 15px;
  backdrop-filter: blur(10px);
  background: rgba(255, 255, 255, 0.95);
}

.form-control {
  border-radius: 8px;
  border: 2px solid #e9ecef;
  padding: 12px 16px;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.form-control:focus {
  border-color: #007bff;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}

.btn-custom {
  padding: 12px 24px;
  font-size: 1.1rem;
  font-weight: 600;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.btn-custom:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 123, 255, 0.3);
}

.input-group .btn {
  border-radius: 0 8px 8px 0;
}
</style>
