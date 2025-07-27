<template>
  <div id="app">
    <NavBar />
    <main class="container-fluid">
      <router-view />
    </main>
    <FooterComponent />
  </div>
</template>

<script>
import NavBar from '@/components/NavBar.vue'
import FooterComponent from '@/components/Footer.vue'

export default {
  name: 'App',
  components: {
    NavBar,
    FooterComponent
  }
}
</script>

<style>
#app {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

main {
  flex: 1;
  padding: 20px 0;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.btn-custom {
  border-radius: 8px;
  font-weight: 500;
  transition: all 0.3s ease;
}

.btn-custom:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}

.card-custom {
  border: none;
  border-radius: 12px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  transition: all 0.3s ease;
}

.card-custom:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15);
}

.loading-spinner {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 3px solid #f3f3f3;
  border-top: 3px solid #007bff;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.badge-custom {
  font-size: 0.75em;
  padding: 0.5em 0.8em;
  border-radius: 20px;
  font-weight: 500;
}

.text-muted-custom {
  color: #6c757d !important;
  font-size: 0.9em;
}

.bg-gradient-primary {
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
}

.bg-gradient-success {
  background: linear-gradient(135deg, #28a745 0%, #1e7e34 100%);
}

.bg-gradient-warning {
  background: linear-gradient(135deg, #ffc107 0%, #e0a800 100%);
}

.bg-gradient-danger {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
}
</style>
