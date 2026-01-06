import { defineStore } from 'pinia';
import api from '../api/axios';
import { jwtDecode } from 'jwt-decode';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('token') || null,
    role: localStorage.getItem('role') || null
  }),

  getters: {
    isAuthenticated: (state) => !!state.token
  },

  actions: {
     async register(userData) {
      try {
        const response = await api.post('/auth/register', userData);
        return { success: true, message: response.data.message };
      } catch (error) {
        return {
          success: false,
          message:
            error.response?.data?.message || 'Error al registrar usuario',
        };
      }
    },

    async login(email, password) {
      try {
        const response = await api.post('/auth/login', {
          email,
          password
        });

        const token = response.data.token;
        const user = response.data.user;

        // 🔥 DECODIFICAMOS EL JWT (FUENTE REAL DEL ROL)
        const decoded = jwtDecode(token);

        const role =
          decoded.role ||
          decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        if (!role) {
          throw new Error('Rol no encontrado en el token');
        }

        // Estado
        this.token = token;
        this.user = user;
        this.role = role;

        // Persistencia
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));
        localStorage.setItem('role', role);

        return { success: true };
      } catch (error) {
        console.error('LOGIN ERROR:', error);
        return {
          success: false,
          message: 'Credenciales inválidas o error de servidor'
        };
      }
    },

    logout() {
      this.token = null;
      this.user = null;
      this.role = null;

      localStorage.removeItem('token');
      localStorage.removeItem('user');
      localStorage.removeItem('role');
    }
  }
});
