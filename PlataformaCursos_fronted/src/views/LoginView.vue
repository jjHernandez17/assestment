<script setup>
import { ref } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';

const email = ref('admin@test.com');
const password = ref('Admin123!');
const errorMessage = ref('');
const loading = ref(false);

const authStore = useAuthStore();
const router = useRouter();

const handleLogin = async () => {
  if (loading.value) return;

  errorMessage.value = '';
  loading.value = true;

  try {
    const result = await authStore.login(email.value, password.value);

    if (!result.success) {
      errorMessage.value = result.message;
      return;
    }

    const role = authStore.role;

    if (role === 'Tutor') {
      router.replace('/tutor');
    } else if (role === 'Student') {
      router.replace('/student');
    } else {
      errorMessage.value = 'Rol de usuario no reconocido';
      authStore.logout();
    }
  } catch (err) {
    console.error(err);
    errorMessage.value = 'Error inesperado al iniciar sesión';
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="login-page">
    <div class="auth-card">
      <div class="logo-container">
        <div class="logo-icon">P</div>
        <h1>EduPlatform</h1>
      </div>

      <h2>¡Bienvenido de nuevo!</h2>
      <p class="subtitle">Ingresa tus credenciales para continuar</p>

      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label>Correo Electrónico</label>
          <input v-model="email" type="email" required class="input-field" />
        </div>

        <div class="form-group">
          <label>Contraseña</label>
          <input v-model="password" type="password" required class="input-field" />
        </div>

        <button type="submit" :disabled="loading" class="btn-login">
          <span v-if="!loading">Iniciar Sesión</span>
          <span v-else class="loader"></span>
        </button>
      </form>

      <transition name="fade">
        <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>
      </transition>

      <div class="footer-links">
        <p>¿No tienes cuenta? <router-link to="/register">Crea una cuenta</router-link></p>
        <router-link to="/" class="back-link">← Volver al inicio</router-link>
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f8fafc; /* Gris muy claro profesional */
  font-family: 'Inter', sans-serif;
}

.auth-card {
  background: white;
  padding: 48px;
  border-radius: 24px;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.05), 0 10px 10px -5px rgba(0, 0, 0, 0.02);
  width: 100%;
  max-width: 440px;
}

.logo-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
  margin-bottom: 24px;
}

.logo-icon {
  width: 40px;
  height: 40px;
  background: #10b981; /* Esmeralda */
  color: white;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 1.2rem;
}

h1 { font-size: 1.25rem; font-weight: 800; color: #1e293b; margin: 0; }
h2 { font-size: 1.5rem; font-weight: 700; color: #1e293b; margin-bottom: 8px; text-align: center; }
.subtitle { color: #64748b; margin-bottom: 32px; font-size: 0.95rem; text-align: center; }

.form-group {
  text-align: left;
  margin-bottom: 24px;
}

label { 
  display: block; 
  margin-bottom: 8px; 
  font-weight: 600; 
  font-size: 0.85rem; 
  color: #475569;
}

.input-field {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  font-size: 0.95rem;
  transition: all 0.2s;
  background: #fcfcfd;
}

.input-field:focus {
  outline: none;
  border-color: #10b981;
  box-shadow: 0 0 0 4px rgba(16, 185, 129, 0.1);
  background: white;
}

.btn-login {
  width: 100%;
  padding: 14px;
  background-color: #10b981;
  color: white;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  justify-content: center;
  align-items: center;
}

.btn-login:hover { background-color: #059669; transform: translateY(-1px); }
.btn-login:active { transform: translateY(0); }
.btn-login:disabled { background-color: #94a3b8; cursor: not-allowed; transform: none; }

.error-text { 
  color: #ef4444; 
  margin-top: 20px; 
  font-size: 0.85rem; 
  background: #fef2f2;
  padding: 10px;
  border-radius: 8px;
  border: 1px solid #fee2e2;
}

.footer-links { margin-top: 32px; font-size: 0.9rem; text-align: center; color: #64748b; }
.footer-links a { color: #10b981; text-decoration: none; font-weight: 600; }
.footer-links a:hover { text-decoration: underline; }

.back-link { display: block; margin-top: 20px; color: #94a3b8 !important; font-weight: 400 !important; }

/* Animación simple de carga */
.loader {
  width: 20px;
  height: 20px;
  border: 3px solid rgba(255,255,255,0.3);
  border-radius: 50%;
  border-top-color: #fff;
  animation: spin 0.8s ease-in-out infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

/* Transición de error */
.fade-enter-active, .fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>