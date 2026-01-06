import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';

// Vistas públicas
import LandingView from '../views/LandingView.vue';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';

// Dashboards por rol
import TutorDashboard from '../views/tutor/TutorDashboard.vue';
import StudentDashboard from '../views/student/StudentDashboard.vue';

const routes = [
  {
    path: '/',
    name: 'landing',
    component: LandingView
  },
  {
    path: '/login',
    name: 'login',
    component: LoginView
  },
  {
    path: '/register',
    name: 'register',
    component: RegisterView
  },

  {
    path: '/tutor',
    name: 'tutor-panel',
    component: TutorDashboard,
    meta: { requiresAuth: true, role: 'Tutor' }
  },

  {
    path: '/student',
    name: 'student-panel',
    component: StudentDashboard,
    meta: { requiresAuth: true, role: 'Student' }
  },

  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// 🛡️ GUARDIA GLOBAL (ROBUSTA)
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();

  const isAuthenticated = authStore.isAuthenticated;
  const userRole = authStore.role;

  // 1️⃣ Ruta protegida sin login
  if (to.meta.requiresAuth && !isAuthenticated) {
    return next({ name: 'login' });
  }

  // 2️⃣ Usuario logueado intentando ver rutas públicas
  if (
    isAuthenticated &&
    (to.name === 'login' || to.name === 'register' || to.name === 'landing')
  ) {
    return next(
      userRole === 'Tutor'
        ? { name: 'tutor-panel' }
        : { name: 'student-panel' }
    );
  }

  // 3️⃣ Protección por rol
  if (to.meta.role && userRole && to.meta.role !== userRole) {
    return next(
      userRole === 'Tutor'
        ? { name: 'tutor-panel' }
        : { name: 'student-panel' }
    );
  }

  next();
});

export default router;
