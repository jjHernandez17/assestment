<script setup>
import { ref } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const form = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  role: 'Student' // Valor por defecto
});

const error = ref('');
const loading = ref(false);

const handleRegister = async () => {
  error.value = '';
  loading.value = true;
  
  const result = await authStore.register(form.value);
  
  if (result.success) {
    alert('¡Cuenta creada con éxito! Ahora puedes iniciar sesión.');
    router.push('/login');
  } else {
    error.value = result.message;
  }
  loading.value = false;
};
</script>

<template>
  <div class="auth-wrapper">
    <div class="auth-container">
      <!-- Encabezado del Formulario -->
      <header class="auth-header">
        <div class="logo-icon" @click="router.push('/')">E</div>
        <h1>Crear cuenta</h1>
        <p>Únete a la comunidad de aprendizaje de EduPlatform</p>
      </header>

      <!-- Card de Registro -->
      <div class="auth-card">
        <form @submit.prevent="handleRegister" class="register-form">
          
          <div class="form-row">
            <div class="form-group">
              <label>Nombre</label>
              <input v-model="form.firstName" type="text" placeholder="Ej. Juan" required />
            </div>
            <div class="form-group">
              <label>Apellido</label>
              <input v-model="form.lastName" type="text" placeholder="Ej. Pérez" required />
            </div>
          </div>

          <div class="form-group">
            <label>Correo electrónico</label>
            <input v-model="form.email" type="email" placeholder="nombre@ejemplo.com" required />
          </div>

          <div class="form-group">
            <label>Contraseña</label>
            <input v-model="form.password" type="password" placeholder="Mínimo 6 caracteres" required />
          </div>

          <div class="form-group">
            <label>¿Cuál es tu rol?</label>
            <select v-model="form.role" class="role-select" required>
              <option value="Student">Quiero aprender (Estudiante)</option>
              <option value="Tutor">Quiero enseñar (Tutor)</option>
            </select>
          </div>

          <button type="submit" class="btn-submit" :disabled="loading">
            {{ loading ? 'Creando cuenta...' : 'Registrarse' }}
          </button>
        </form>

        <p v-if="error" class="error-msg">{{ error }}</p>
      </div>

      <!-- Footer del Formulario -->
      <footer class="auth-footer">
        <p>¿Ya tienes una cuenta? <router-link to="/login">Inicia sesión</router-link></p>
      </footer>
    </div>
  </div>
</template>

<style scoped>
/* Contenedor principal centrado */
.auth-wrapper {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f8fafc;
  padding: 20px;
  font-family: 'Inter', sans-serif;
}

.auth-container {
  width: 100%;
  max-width: 480px;
}

/* Header */
.auth-header {
  text-align: center;
  margin-bottom: 30px;
}

.logo-icon {
  background: #42b983;
  color: white;
  width: 45px;
  height: 45px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 1.4rem;
  margin: 0 auto 20px;
  cursor: pointer;
  transition: transform 0.2s;
}

.logo-icon:hover { transform: scale(1.05); }

h1 {
  font-size: 1.8rem;
  font-weight: 800;
  color: #1e293b;
  margin: 0 0 10px;
}

p {
  color: #64748b;
  font-size: 0.95rem;
}

/* Card Estilizada */
.auth-card {
  background: white;
  padding: 35px;
  border-radius: 24px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.04);
  border: 1px solid #f1f5f9;
}

/* Formulario */
.form-row {
  display: flex;
  gap: 15px;
}

.form-group {
  margin-bottom: 20px;
  text-align: left;
  flex: 1;
}

label {
  display: block;
  font-size: 0.85rem;
  font-weight: 600;
  color: #475569;
  margin-bottom: 8px;
}

input, .role-select {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  font-size: 0.95rem;
  transition: all 0.3s ease;
  box-sizing: border-box;
}

input:focus, .role-select:focus {
  outline: none;
  border-color: #42b983;
  box-shadow: 0 0 0 4px rgba(66, 185, 131, 0.1);
}

.role-select {
  background-color: white;
  cursor: pointer;
}

/* Botón */
.btn-submit {
  width: 100%;
  padding: 14px;
  background-color: #42b983;
  color: white;
  border: none;
  border-radius: 12px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 10px;
}

.btn-submit:hover {
  background-color: #3aa876;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(66, 185, 131, 0.3);
}

.btn-submit:disabled {
  background-color: #cbd5e1;
  cursor: not-allowed;
  transform: none;
}

/* Mensajes */
.error-msg {
  background-color: #fef2f2;
  color: #ef4444;
  padding: 12px;
  border-radius: 10px;
  font-size: 0.85rem;
  margin-top: 20px;
  text-align: center;
}

.auth-footer {
  text-align: center;
  margin-top: 25px;
}

.auth-footer a {
  color: #42b983;
  text-decoration: none;
  font-weight: 700;
}

.auth-footer a:hover {
  text-decoration: underline;
}

/* Ajustes para móviles */
@media (max-width: 480px) {
  .form-row { flex-direction: column; gap: 0; }
  .auth-card { padding: 25px; }
}
</style>