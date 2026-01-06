<template>
  <div class="student-layout">
    <!-- ===== SIDEBAR ===== -->
    <aside class="sidebar">
      <!-- Brand -->
      <div class="brand">
        <div class="logo">E</div>
        <div>
          <h1>EduPlatform</h1>
          <span>ESTUDIANTE</span>
        </div>
      </div>

      <!-- Menu -->
      <nav class="menu">
        <button
          v-for="item in menuItems"
          :key="item.id"
          @click="activeComponent = item.component"
          :class="['menu-item', activeComponent === item.component && 'active']"
        >
          <component :is="item.icon" class="icon" />
          <span>{{ item.label }}</span>
        </button>
      </nav>

      <!-- Progress Card -->
      <div class="progress-card">
        <p class="progress-title">Progreso General</p>
        <div class="progress-bar">
          <div class="progress-fill"></div>
        </div>
        <p class="progress-value">75% completado</p>
      </div>
    </aside>

    <!-- ===== MAIN ===== -->
    <main class="main">
      <!-- Header -->
      <header class="topbar">
        <div class="breadcrumb">
          Plataforma / <strong>Dashboard</strong>
        </div>

        <div class="user-box">
          <span class="email">{{ authStore.user?.email }}</span>
          <div class="avatar">
            {{ authStore.user?.firstName?.[0] }}
          </div>
          <button @click="handleLogout" class="logout">
            <LogOutIcon />
          </button>
        </div>
      </header>

      <!-- Content -->
      <section class="content">
        <div class="content-inner">
          <transition name="fade-slide" mode="out-in">
            <component :is="activeComponent" />
          </transition>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
import { shallowRef } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { useRouter } from 'vue-router'
import {
  LayoutDashboardIcon,
  SearchIcon,
  BookCheckIcon,
  UserCircleIcon,
  LogOutIcon
} from 'lucide-vue-next'

// Components
import StudentStats from '../../components/student/StudentStats.vue'
import CourseCatalog from '../../components/student/CourseCatalog.vue'
import MyLearning from '../../components/student/MyLearning.vue'
import UserProfile from '../../components/UserProfile.vue'

const authStore = useAuthStore()
const router = useRouter()

const activeComponent = shallowRef(StudentStats)

const menuItems = [
  { id: 1, label: 'Inicio', icon: LayoutDashboardIcon, component: StudentStats },
  { id: 2, label: 'Explorar Cursos', icon: SearchIcon, component: CourseCatalog },
  { id: 3, label: 'Mis Cursos', icon: BookCheckIcon, component: MyLearning },
  { id: 4, label: 'Mi Perfil', icon: UserCircleIcon, component: UserProfile }
]

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
/* ===== LAYOUT ===== */
.student-layout {
  display: flex;
  height: 100vh;
  background: linear-gradient(135deg, #f8fafc, #eef2ff);
  font-family: 'Inter', sans-serif;
}

/* ===== SIDEBAR ===== */
.sidebar {
  width: 280px;
  background: linear-gradient(180deg, #0f172a, #1e1b4b);
  color: white;
  display: flex;
  flex-direction: column;
  padding: 28px 22px;
}

.brand {
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 40px;
}

.logo {
  width: 44px;
  height: 44px;
  border-radius: 14px;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 900;
}

.brand h1 {
  font-size: 1.1rem;
  margin: 0;
}

.brand span {
  font-size: 0.65rem;
  letter-spacing: 2px;
  color: #c7d2fe;
}

/* ===== MENU ===== */
.menu {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 14px 18px;
  border-radius: 16px;
  color: #c7d2fe;
  background: transparent;
  border: none;
  cursor: pointer;
  transition: all 0.25s ease;
}

.menu-item .icon {
  width: 20px;
  height: 20px;
}

.menu-item:hover {
  background: rgba(255, 255, 255, 0.08);
  color: white;
}

.menu-item.active {
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  box-shadow: 0 10px 25px rgba(99, 102, 241, 0.4);
}

/* ===== PROGRESS ===== */
.progress-card {
  margin-top: 30px;
  background: rgba(255, 255, 255, 0.08);
  padding: 18px;
  border-radius: 18px;
}

.progress-title {
  font-size: 0.75rem;
  color: #c7d2fe;
}

.progress-bar {
  height: 6px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 999px;
  overflow: hidden;
  margin: 10px 0;
}

.progress-fill {
  height: 100%;
  width: 75%;
  background: linear-gradient(90deg, #3b82f6, #22d3ee);
}

.progress-value {
  font-size: 0.7rem;
  color: #e0e7ff;
}

/* ===== MAIN ===== */
.main {
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* ===== TOPBAR ===== */
.topbar {
  height: 72px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 48px;
}

.breadcrumb {
  font-size: 0.85rem;
  color: #64748b;
}

.user-box {
  display: flex;
  align-items: center;
  gap: 14px;
}

.email {
  font-size: 0.8rem;
  color: #475569;
}

.avatar {
  width: 38px;
  height: 38px;
  border-radius: 50%;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
}

.logout {
  background: none;
  border: none;
  cursor: pointer;
  color: #64748b;
}

.logout:hover {
  color: #ef4444;
}

/* ===== CONTENT ===== */
.content {
  flex: 1;
  overflow-y: auto;
  padding: 48px;
}

.content-inner {
  max-width: 1100px;
  margin: 0 auto;
}

/* ===== ANIMATION ===== */
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.25s ease;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(10px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
