
<template>
  <footer class="bg-dark text-light py-4 mt-auto">
    <div class="container">
      <div class="row">
        <div class="col-md-6">
          <h5>AnnounceHub</h5>
          <p class="text-muted">İşbirliği ve proje duyuruları için modern platform</p>
        </div>
        <div class="col-md-6 text-md-end">
          <div class="social-links mb-3">
            <a href="#" class="text-light me-3"><i class="fab fa-twitter"></i></a>
            <a href="#" class="text-light me-3"><i class="fab fa-linkedin"></i></a>
            <a href="#" class="text-light me-3"><i class="fab fa-github"></i></a>
            <a href="#" class="text-light"><i class="fab fa-instagram"></i></a>
          </div>
          <small class="text-muted">
            © 2024 AnnounceHub. Tüm hakları saklıdır.
          </small>
        </div>
      </div>
    </div>
  </footer>
</template>

<script>
export default {
  name: 'FooterComponent'
}
</script>

<style scoped>
footer {
  border-top: 1px solid #dee2e6;
}

.social-links a {
  font-size: 1.2rem;
  transition: color 0.3s ease;
}

.social-links a:hover {
  color: #007bff !important;
}
</style>
