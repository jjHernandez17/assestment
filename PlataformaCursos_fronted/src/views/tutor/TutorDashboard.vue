<template>
  <div class="dashboard-root">
    <!-- SIDEBAR -->
    <aside class="sidebar">
      <div class="sidebar-header">
        <div class="logo-circle">P</div>
        <h2>EduPlatform</h2>
        <span>Panel Tutor</span>
      </div>

      <nav class="menu">
        <button
          v-for="item in menuItems"
          :key="item.id"
          @click="activeComponent = item.component"
          :class="['menu-item', activeComponent === item.component ? 'active' : '']"
        >
          <component :is="item.icon" class="menu-icon" />
          <span>{{ item.label }}</span>
        </button>
      </nav>

      <div class="sidebar-footer">
        <button class="logout-btn" @click="handleLogout">
          <LogOutIcon />
          Cerrar sesión
        </button>
      </div>
    </aside>

    <!-- MAIN -->
    <main class="main-content">
      <!-- HEADER -->
      <header class="topbar">
        <div>
          <h1>Panel de Control</h1>
          <p>Gestiona tus cursos y estudiantes</p>
        </div>

        <div class="user-box">
          <span>{{ authStore.user.email }}</span>
          <div class="avatar">
            {{ authStore.user.firstName?.charAt(0) || 'U' }}
          </div>
        </div>
      </header>

      <!-- CONTENT -->
      <section class="content-area">
        <transition name="fade" mode="out-in">
          <component :is="activeComponent" />
        </transition>
      </section>
    </main>
  </div>
</template>

<script setup>
import { shallowRef } from 'vue';
import {
  LayoutDashboardIcon,
  BookOpenIcon,
  UsersIcon,
  UserCircleIcon,
  LogOutIcon
} from 'lucide-vue-next';

import { useAuthStore } from '../../stores/auth';
import { useRouter } from 'vue-router';

import TutorStats from '../../components/tutor/TutorStats.vue';
import TutorCourses from '../../components/tutor/TutorCourses.vue';
import TutorStudents from '../../components/tutor/TutorStudents.vue';
import UserProfile from '../../components/UserProfile.vue';

const authStore = useAuthStore();
const router = useRouter();

const activeComponent = shallowRef(TutorStats);

const menuItems = [
  { id: 1, label: 'Dashboard', icon: LayoutDashboardIcon, component: TutorStats },
  { id: 2, label: 'Cursos', icon: BookOpenIcon, component: TutorCourses },
  { id: 3, label: 'Estudiantes', icon: UsersIcon, component: TutorStudents },
  { id: 4, label: 'Perfil', icon: UserCircleIcon, component: UserProfile }
];

const handleLogout = () => {
  authStore.logout();
  router.replace('/login');
};
</script>

<style scoped>
/* ====== ROOT ====== */
.dashboard-root {
  display: flex;
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Inter', sans-serif;
}

/* ====== SIDEBAR ====== */
.sidebar {
  width: 280px;
  background: linear-gradient(180deg, #10b981, #059669);
  color: white;
  display: flex;
  flex-direction: column;
  padding: 24px 0;
  box-shadow: 4px 0 20px rgba(0, 0, 0, 0.08);
}

.sidebar-header {
  text-align: center;
  padding: 20px;
}

.logo-circle {
  width: 56px;
  height: 56px;
  background: white;
  color: #10b981;
  border-radius: 18px;
  font-weight: 900;
  font-size: 1.6rem;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 12px;
}

.sidebar-header h2 {
  font-size: 1.3rem;
  font-weight: 800;
  margin: 0;
}

.sidebar-header span {
  font-size: 0.75rem;
  opacity: 0.8;
}

/* ====== MENU ====== */
.menu {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 30px 20px;
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 14px 18px;
  border-radius: 16px;
  background: transparent;
  border: none;
  color: white;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.25s ease;
}

.menu-item:hover {
  background: rgba(255, 255, 255, 0.18);
  transform: translateX(4px);
}

.menu-item.active {
  background: white;
  color: #10b981;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
}

.menu-icon {
  width: 22px;
  height: 22px;
}

/* ====== FOOTER ====== */
.sidebar-footer {
  padding: 20px;
}

.logout-btn {
  width: 100%;
  padding: 12px;
  border-radius: 14px;
  background: rgba(255, 255, 255, 0.15);
  border: none;
  color: white;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.25);
}

/* ====== MAIN ====== */
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* ====== TOPBAR ====== */
.topbar {
  height: 90px;
  background: white;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 40px;
  border-bottom: 1px solid #e5e7eb;
}

.topbar h1 {
  font-size: 1.4rem;
  font-weight: 800;
  color: #1e293b;
  margin: 0;
}

.topbar p {
  font-size: 0.85rem;
  color: #64748b;
}

.user-box {
  display: flex;
  align-items: center;
  gap: 14px;
  font-size: 0.85rem;
  color: #475569;
}

.avatar {
  width: 42px;
  height: 42px;
  background: #10b981;
  color: white;
  border-radius: 50%;
  font-weight: 800;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* ====== CONTENT ====== */
.content-area {
  flex: 1;
  padding: 40px;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
}

/* ====== TRANSITION ====== */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
