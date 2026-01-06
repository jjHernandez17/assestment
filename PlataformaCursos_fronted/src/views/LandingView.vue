<script setup>
// No necesitamos lógica por ahora, pero mantenemos el script setup por estándar
</script>

<template>
  <div class="landing-wrapper">
    <main class="landing-container">
      <!-- Sección Hero -->
      <header class="hero">
        <div class="logo-container">
          <div class="logo-icon">E</div>
          <span class="logo-text">EduPlatform</span>
        </div>
        
        <h1 class="title">Potencia tu <span class="highlight">Aprendizaje</span></h1>
        <p class="subtitle">
          La plataforma integral para crear, gestionar y consumir cursos interactivos de nivel profesional.
        </p>
        
        <div class="actions">
          <router-link to="/login" class="btn btn-primary">Comenzar ahora</router-link>
          <router-link to="/register" class="btn btn-secondary">Crear cuenta gratuita</router-link>
        </div>
      </header>

      <!-- Sección de Características -->
      <section class="features">
        <div class="feature-card">
          <div class="icon">👨‍🏫</div>
          <h3>Para Tutores</h3>
          <p>Organiza lecciones, gestiona órdenes y publica contenido de alto impacto con un solo clic.</p>
        </div>

        <div class="feature-card">
          <div class="icon">🎓</div>
          <h3>Para Estudiantes</h3>
          <p>Aprende a tu ritmo con lecciones estructuradas y una interfaz diseñada para el enfoque.</p>
        </div>
      </section>

      <footer class="footer">
        <p>&copy; 2026 EduPlatform. Assessment Técnico .NET & Vue.js</p>
      </footer>
    </main>
  </div>
</template>

<style scoped>
/* Contenedor Principal para centrar todo en pantalla */
.landing-wrapper {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f8fafc;
  padding: 20px;
  font-family: 'Inter', system-ui, -apple-system, sans-serif;
}

.landing-container {
  max-width: 900px;
  width: 100%;
  text-align: center;
}

/* Logo */
.logo-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  margin-bottom: 30px;
}

.logo-icon {
  background: #42b983;
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 1.2rem;
}

.logo-text {
  font-size: 1.5rem;
  font-weight: 800;
  color: #2c3e50;
  letter-spacing: -0.5px;
}

/* Hero Section */
.hero {
  padding: 40px 20px;
}

.title {
  font-size: 3.5rem;
  font-weight: 800;
  color: #1e293b;
  line-height: 1.1;
  margin-bottom: 20px;
}

.highlight {
  color: #42b983;
}

.subtitle {
  font-size: 1.25rem;
  color: #64748b;
  max-width: 600px;
  margin: 0 auto 40px;
  line-height: 1.6;
}

/* Botones */
.actions {
  display: flex;
  justify-content: center;
  gap: 15px;
  margin-bottom: 60px;
}

.btn {
  padding: 14px 28px;
  text-decoration: none;
  border-radius: 12px;
  font-weight: 600;
  transition: all 0.3s ease;
  font-size: 1rem;
}

.btn-primary {
  background-color: #42b983;
  color: white;
  box-shadow: 0 4px 14px rgba(66, 185, 131, 0.4);
}

.btn-primary:hover {
  background-color: #3aa876;
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(66, 185, 131, 0.6);
}

.btn-secondary {
  background-color: white;
  color: #42b983;
  border: 2px solid #42b983;
}

.btn-secondary:hover {
  background-color: #f0fdf4;
  transform: translateY(-2px);
}

/* Características */
.features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 20px;
  margin-bottom: 40px;
}

.feature-card {
  background: white;
  padding: 30px;
  border-radius: 20px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.05);
  transition: transform 0.3s ease;
  border: 1px solid #f1f5f9;
}

.feature-card:hover {
  transform: translateY(-5px);
}

.icon {
  font-size: 2.5rem;
  margin-bottom: 15px;
}

.feature-card h3 {
  color: #1e293b;
  margin-bottom: 10px;
  font-weight: 700;
}

.feature-card p {
  color: #64748b;
  font-size: 0.95rem;
  line-height: 1.5;
}

.footer {
  color: #94a3b8;
  font-size: 0.85rem;
  margin-top: 60px;
}

/* Responsividad para móviles */
@media (max-width: 600px) {
  .title { font-size: 2.5rem; }
  .actions { flex-direction: column; }
  .btn { width: 100%; box-sizing: border-box; }
}
</style>