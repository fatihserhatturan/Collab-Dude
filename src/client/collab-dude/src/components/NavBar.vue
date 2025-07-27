<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-gradient-primary shadow-sm">
    <div class="container">
      <router-link class="navbar-brand fw-bold" to="/">
        <i class="fas fa-bullhorn me-2"></i>
        AnnounceHub
      </router-link>

      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav me-auto">
          <li class="nav-item">
            <router-link class="nav-link" to="/">
              <i class="fas fa-home me-1"></i>Ana Sayfa
            </router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" to="/announces">
              <i class="fas fa-list me-1"></i>Duyurular
            </router-link>
          </li>
          <li class="nav-item" v-if="authStore.isLoggedIn">
            <router-link class="nav-link" to="/announces/create">
              <i class="fas fa-plus me-1"></i>Duyuru Oluştur
            </router-link>
          </li>
        </ul>

        <ul class="navbar-nav ms-auto">
          <!-- Authenticated User Menu -->
          <template v-if="authStore.isLoggedIn">
            <!-- Notifications -->
            <li class="nav-item dropdown">
              <a
                class="nav-link position-relative"
                href="#"
                id="notificationsDropdown"
                role="button"
                data-bs-toggle="dropdown"
              >
                <i class="fas fa-bell"></i>
                <span
                  v-if="unreadCount > 0"
                  class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
                >
                  {{ unreadCount > 99 ? '99+' : unreadCount }}
                </span>
              </a>
              <ul class="dropdown-menu dropdown-menu-end notification-dropdown">
                <li class="dropdown-header">
                  Bildirimler
                  <router-link to="/notifications" class="btn btn-sm btn-link">
                    Tümünü Gör
                  </router-link>
                </li>
                <li><hr class="dropdown-divider"></li>
                <li v-if="notifications.length === 0" class="dropdown-item-text text-muted">
                  Yeni bildirim yok
                </li>
                <li v-for="notification in notifications.slice(0, 5)" :key="notification.id">
                  <div class="dropdown-item notification-item" :class="{ 'unread': !notification.isRead }">
                    <div class="d-flex">
                      <div class="notification-icon me-2">
                        <i :class="getNotificationIcon(notification.type)"></i>
                      </div>
                      <div class="flex-grow-1">
                        <div class="notification-title">{{ notification.title }}</div>
                        <div class="notification-message">{{ notification.message }}</div>
                        <small class="text-muted">{{ formatDate(notification.createdAt) }}</small>
                      </div>
                    </div>
                  </div>
                </li>
              </ul>
            </li>

            <!-- User Menu -->
            <li class="nav-item dropdown">
              <a
                class="nav-link dropdown-toggle d-flex align-items-center"
                href="#"
                id="userDropdown"
                role="button"
                data-bs-toggle="dropdown"
              >
                <div class="user-avatar me-2">
                  <i class="fas fa-user-circle fa-lg"></i>
                </div>
                <span class="d-none d-md-inline">{{ authStore.fullName }}</span>
              </a>
              <ul class="dropdown-menu dropdown-menu-end">
                <li class="dropdown-header">
                  {{ authStore.fullName }}
                  <br>
                  <small class="text-muted">{{ authStore.user?.email }}</small>
                </li>
                <li><hr class="dropdown-divider"></li>
                <li>
                  <router-link class="dropdown-item" to="/dashboard">
                    <i class="fas fa-tachometer-alt me-2"></i>Panel
                  </router-link>
                </li>
                <li>
                  <router-link class="dropdown-item" to="/my-announces">
                    <i class="fas fa-bullhorn me-2"></i>Duyurularım
                  </router-link>
                </li>
                <li>
                  <router-link class="dropdown-item" to="/my-applications">
                    <i class="fas fa-file-alt me-2"></i>Başvurularım
                  </router-link>
                </li>
                <li>
                  <router-link class="dropdown-item" to="/profile">
                    <i class="fas fa-user me-2"></i>Profil
                  </router-link>
                </li>

                <!-- Admin Menu -->
                <template v-if="authStore.isAdmin">
                  <li><hr class="dropdown-divider"></li>
                  <li class="dropdown-header">Yönetim</li>
                  <li>
                    <router-link class="dropdown-item" to="/users">
                      <i class="fas fa-users me-2"></i>Kullanıcılar
                    </router-link>
                  </li>
                  <li>
                    <router-link class="dropdown-item" to="/categories">
                      <i class="fas fa-folder me-2"></i>Kategoriler
                    </router-link>
                  </li>
                  <li>
                    <router-link class="dropdown-item" to="/roles">
                      <i class="fas fa-user-shield me-2"></i>Roller
                    </router-link>
                  </li>
                </template>

                <li><hr class="dropdown-divider"></li>
                <li>
                  <a class="dropdown-item text-danger" @click="logout" href="#">
                    <i class="fas fa-sign-out-alt me-2"></i>Çıkış Yap
                  </a>
                </li>
              </ul>
            </li>
          </template>

          <!-- Guest Menu -->
          <template v-else>
            <li class="nav-item">
              <router-link class="nav-link" to="/login">
                <i class="fas fa-sign-in-alt me-1"></i>Giriş Yap
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/register">
                <i class="fas fa-user-plus me-1"></i>Kayıt Ol
              </router-link>
            </li>
          </template>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script>
import { useAuthStore } from '@/stores/auth'
import { ref, onMounted } from 'vue'
import axios from 'axios'
import moment from 'moment'

export default {
  name: 'NavBar',
  setup() {
    const authStore = useAuthStore()
    const notifications = ref([])
    const unreadCount = ref(0)

    const fetchNotifications = async () => {
      if (!authStore.isLoggedIn) return

      try {
        const response = await axios.get('/notifications')
        if (response.data.success) {
          notifications.value = response.data.data
        }

        const unreadResponse = await axios.get('/notifications/unread-count')
        if (unreadResponse.data.success) {
          unreadCount.value = unreadResponse.data.data
        }
      } catch (error) {
        console.error('Error fetching notifications:', error)
      }
    }

    const logout = async () => {
      await authStore.logout()
      window.location.href = '/'
    }

    const getNotificationIcon = (type) => {
      const icons = {
        0: 'fas fa-info-circle text-info',
        1: 'fas fa-check-circle text-success',
        2: 'fas fa-exclamation-triangle text-warning',
        3: 'fas fa-times-circle text-danger',
        4: 'fas fa-user-plus text-primary',
        5: 'fas fa-refresh text-info',
        6: 'fas fa-comment text-secondary',
        7: 'fas fa-clock text-warning'
      }
      return icons[type] || 'fas fa-bell'
    }

    const formatDate = (date) => {
      return moment(date).fromNow()
    }

    onMounted(() => {
      if (authStore.isLoggedIn) {
        fetchNotifications()
        // Poll for new notifications every 30 seconds
        setInterval(fetchNotifications, 30000)
      }
    })

    return {
      authStore,
      notifications,
      unreadCount,
      logout,
      getNotificationIcon,
      formatDate
    }
  }
}
</script>

<style scoped>
.navbar-brand {
  font-size: 1.5rem;
  letter-spacing: -0.5px;
}

.nav-link {
  font-weight: 500;
  transition: all 0.3s ease;
  border-radius: 6px;
  margin: 0 2px;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  transform: translateY(-1px);
}

.user-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: rgba(255, 255, 255, 0.2);
}

.notification-dropdown {
  min-width: 350px;
  max-height: 400px;
  overflow-y: auto;
}

.notification-item {
  border-bottom: 1px solid #f0f0f0;
  padding: 12px 16px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.notification-item:hover {
  background-color: #f8f9fa;
}

.notification-item.unread {
  background-color: #e3f2fd;
  border-left: 3px solid #2196f3;
}

.notification-icon {
  width: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.notification-title {
  font-weight: 600;
  font-size: 0.9rem;
  margin-bottom: 4px;
}

.notification-message {
  font-size: 0.8rem;
  color: #666;
  margin-bottom: 4px;
  line-height: 1.3;
}

.dropdown-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 16px;
}

@media (max-width: 768px) {
  .notification-dropdown {
    min-width: 300px;
  }

  .d-none.d-md-inline {
    display: none !important;
  }
}
</style>


