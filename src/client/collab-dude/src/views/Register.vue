<template>
  <div class="auth-container">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
          <div class="card card-custom shadow-lg">
            <div class="card-body p-5">
              <div class="text-center mb-4">
                <i class="fas fa-user-plus fa-3x text-success mb-3"></i>
                <h2 class="fw-bold">Kayıt Ol</h2>
                <p class="text-muted">Yeni hesap oluşturun</p>
              </div>

              <form @submit.prevent="handleRegister">
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label for="firstName" class="form-label">
                      <i class="fas fa-user me-1"></i>Ad
                    </label>
                    <input
                      id="firstName"
                      type="text"
                      class="form-control"
                      v-model="form.firstName"
                      :class="{ 'is-invalid': errors.firstName }"
                      placeholder="Adınız"
                      required
                    >
                    <div v-if="errors.firstName" class="invalid-feedback">
                      {{ errors.firstName }}
                    </div>
                  </div>

                  <div class="col-md-6 mb-3">
                    <label for="lastName" class="form-label">
                      <i class="fas fa-user me-1"></i>Soyad
                    </label>
                    <input
                      id="lastName"
                      type="text"
                      class="form-control"
                      v-model="form.lastName"
                      :class="{ 'is-invalid': errors.lastName }"
                      placeholder="Soyadınız"
                      required
                    >
                    <div v-if="errors.lastName" class="invalid-feedback">
                      {{ errors.lastName }}
                    </div>
                  </div>
                </div>

                <div class="mb-3">
                  <label for="email" class="form-label">
                    <i class="fas fa-envelope me-1"></i>Email
                  </label>
                  <input
                    id="email"
                    type="email"
                    class="form-control"
                    v-model="form.email"
                    :class="{ 'is-invalid': errors.email }"
                    placeholder="email@example.com"
                    required
                  >
                  <div v-if="errors.email" class="invalid-feedback">
                    {{ errors.email }}
                  </div>
                </div>

                <div class="mb-3">
                  <label for="userName" class="form-label">
                    <i class="fas fa-at me-1"></i>Kullanıcı Adı
                  </label>
                  <input
                    id="userName"
                    type="text"
                    class="form-control"
                    v-model="form.userName"
                    :class="{ 'is-invalid': errors.userName }"
                    placeholder="kullaniciadi"
                    required
                  >
                  <div v-if="errors.userName" class="invalid-feedback">
                    {{ errors.userName }}
                  </div>
                </div>

                <div class="mb-3">
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
                      placeholder="Güçlü bir şifre seçin"
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
                  <div class="form-text">
                    <small>En az 8 karakter, büyük harf, küçük harf ve rakam içermeli</small>
                  </div>
                </div>

                <div class="mb-3">
                  <label for="confirmPassword" class="form-label">
                    <i class="fas fa-lock me-1"></i>Şifre Tekrarı
                  </label>
                  <input
                    id="confirmPassword"
                    type="password"
                    class="form-control"
                    v-model="form.confirmPassword"
                    :class="{ 'is-invalid': errors.confirmPassword }"
                    placeholder="Şifrenizi tekrar girin"
                    required
                  >
                  <div v-if="errors.confirmPassword" class="invalid-feedback">
                    {{ errors.confirmPassword }}
                  </div>
                </div>

                <div class="mb-4">
                  <div class="form-check">
                    <input
                      class="form-check-input"
                      type="checkbox"
                      id="acceptTerms"
                      v-model="form.acceptTerms"
                      :class="{ 'is-invalid': errors.acceptTerms }"
                    >
                    <label class="form-check-label" for="acceptTerms">
                      <a href="#" class="text-decoration-none">Kullanım şartlarını</a>
                      ve <a href="#" class="text-decoration-none">gizlilik politikasını</a> kabul ediyorum
                    </label>
                    <div v-if="errors.acceptTerms" class="invalid-feedback">
                      {{ errors.acceptTerms }}
                    </div>
                  </div>
                </div>

                <div class="d-grid">
                  <button
                    type="submit"
                    class="btn btn-success btn-custom"
                    :disabled="authStore.loading"
                  >
                    <span v-if="authStore.loading" class="loading-spinner me-2"></span>
                    <i v-else class="fas fa-user-plus me-2"></i>
                    {{ authStore.loading ? 'Kayıt oluşturuluyor...' : 'Kayıt Ol' }}
                  </button>
                </div>
              </form>

              <div class="text-center mt-4">
                <p class="text-muted">
                  Zaten hesabınız var mı?
                  <router-link to="/login" class="text-decoration-none">
                    Giriş yapın
                  </router-link>
                </p>
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
  name: 'RegisterView',
  setup() {
    const router = useRouter()
    const authStore = useAuthStore()

    const form = ref({
      firstName: '',
      lastName: '',
      email: '',
      userName: '',
      password: '',
      confirmPassword: '',
      roleName: 'User',
      acceptTerms: false
    })

    const errors = ref({})
    const showPassword = ref(false)

    const validateForm = () => {
      errors.value = {}

      if (!form.value.firstName) {
        errors.value.firstName = 'Ad gerekli'
      }

      if (!form.value.lastName) {
        errors.value.lastName = 'Soyad gerekli'
      }

      if (!form.value.email) {
        errors.value.email = 'Email gerekli'
      } else if (!/\S+@\S+\.\S+/.test(form.value.email)) {
        errors.value.email = 'Geçerli bir email adresi girin'
      }

      if (!form.value.userName) {
        errors.value.userName = 'Kullanıcı adı gerekli'
      } else if (form.value.userName.length < 3) {
        errors.value.userName = 'Kullanıcı adı en az 3 karakter olmalı'
      }

      if (!form.value.password) {
        errors.value.password = 'Şifre gerekli'
      } else if (form.value.password.length < 8) {
        errors.value.password = 'Şifre en az 8 karakter olmalı'
      } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(form.value.password)) {
        errors.value.password = 'Şifre büyük harf, küçük harf ve rakam içermeli'
      }

      if (!form.value.confirmPassword) {
        errors.value.confirmPassword = 'Şifre tekrarı gerekli'
      } else if (form.value.password !== form.value.confirmPassword) {
        errors.value.confirmPassword = 'Şifreler eşleşmiyor'
      }

      if (!form.value.acceptTerms) {
        errors.value.acceptTerms = 'Kullanım şartlarını kabul etmelisiniz'
      }

      return Object.keys(errors.value).length === 0
    }

    const handleRegister = async () => {
      if (!validateForm()) return

      const { ...userData } = form.value
      const result = await authStore.register(userData)

      if (result.success) {
        router.push('/login')
      }
    }

    return {
      form,
      errors,
      showPassword,
      authStore,
      handleRegister
    }
  }
}
</script>

<style scoped>
.auth-container {
  min-height: 90vh;
  display: flex;
  align-items: center;
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
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
  border-color: #28a745;
  box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25);
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
  box-shadow: 0 8px 25px rgba(40, 167, 69, 0.3);
}

.form-check-input:checked {
  background-color: #28a745;
  border-color: #28a745;
}

.input-group .btn {
  border-radius: 0 8px 8px 0;
}
</style>
