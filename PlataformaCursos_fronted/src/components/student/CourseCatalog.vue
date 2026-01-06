<template>
  <div class="catalog-wrapper">
    <!-- HEADER -->
    <div class="catalog-header">
      <div>
        <h2>Explorar Cursos</h2>
        <p>Descubre cursos disponibles y empieza a aprender</p>
      </div>
    </div>

    <!-- LOADING -->
    <div v-if="loading" class="state-box">
      Cargando cursos disponibles...
    </div>

    <!-- EMPTY -->
    <div v-if="!loading && courses.length === 0" class="state-box">
      No hay cursos publicados por ahora
    </div>

    <!-- COURSES GRID -->
    <div v-if="!loading && courses.length > 0" class="catalog-grid">
      <div
        v-for="course in courses"
        :key="course.id"
        class="course-card"
      >
        <div class="card-top">
          <span class="badge">Publicado</span>
        </div>

        <h3>{{ course.title }}</h3>

        <p class="course-meta">
          Curso creado por un tutor
        </p>

        <button
          class="join-btn"
          @click="joinCourse(course.id)"
          :disabled="joiningId === course.id || isEnrolled(course.id)"
        >
          <span v-if="joiningId !== course.id && !isEnrolled(course.id)">Unirse al curso</span>
          <span v-else-if="joiningId === course.id">Uniéndose...</span>
          <span v-else>Unido ✓</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../api/axios'
import jwtDecode from 'jwt-decode'

const courses = ref([])
const loading = ref(false)
const joiningId = ref(null)
const enrolled = ref(new Set())

const getUserIdFromToken = () => {
  const token = localStorage.getItem('token')
  if (!token) return null
  try {
    const decoded = jwtDecode(token)
    // varios nombres posibles para el id según cómo el backend genere el JWT
    return decoded.sub || decoded.nameid || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null
  } catch (e) {
    return null
  }
}

/**
 * Trae solo cursos PUBLICADOS
 */
const fetchPublishedCourses = async () => {
  loading.value = true
  try {
    const res = await api.get('/courses/search', {
      params: {
        status: 'Published'
      }
    })
    courses.value = res.data.items

    // Comprobar para cada curso si el usuario actual está inscrito
    const userId = getUserIdFromToken()
    if (userId) {
      // para listados cortos esto está bien; si la lista es grande, conviene un endpoint que devuelva enrolamientos del usuario
      await Promise.all(courses.value.map(async (c) => {
        try {
          const r = await api.get(`/courses/${c.id}/students`)
          if (Array.isArray(r.data) && r.data.includes(userId)) {
            enrolled.value.add(c.id)
          }
        } catch (err) {
          // ignore individual failures
          console.debug('No se pudo comprobar inscripción para', c.id, err)
        }
      }))
    }
  } catch (error) {
    console.error('Error cargando cursos publicados', error)
  } finally {
    loading.value = false
  }
}

/**
 * Unirse a un curso
 * Llama al endpoint POST /api/courses/{id}/join
 */
const joinCourse = async (courseId) => {
  if (isEnrolled(courseId)) return
  joiningId.value = courseId
  try {
    await api.post(`/courses/${courseId}/join`)
    enrolled.value.add(courseId)
    alert('Te has unido al curso 🎉')
  } catch (error) {
    console.error('Error al unirse al curso', error)
    const message = error.response?.data?.message || 'No se pudo unir al curso'
    alert(message)
  } finally {
    joiningId.value = null
  }
}

const isEnrolled = (courseId) => enrolled.value.has(courseId)

onMounted(fetchPublishedCourses)
</script>

<style scoped>
/* ===== WRAPPER ===== */
.catalog-wrapper {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

/* ===== HEADER ===== */
.catalog-header h2 {
  font-size: 1.7rem;
  font-weight: 900;
  margin: 0;
  color: #0f172a;
}

.catalog-header p {
  font-size: 0.85rem;
  color: #64748b;
}

/* ===== GRID ===== */
.catalog-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 26px;
}

/* ===== CARD ===== */
.course-card {
  background: white;
  border-radius: 24px;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.06);
  transition: all 0.25s ease;
}

.course-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 20px 45px rgba(0, 0, 0, 0.1);
}

.card-top {
  display: flex;
  justify-content: flex-end;
}

.badge {
  font-size: 0.7rem;
  font-weight: 800;
  padding: 6px 12px;
  border-radius: 999px;
  background: #dcfce7;
  color: #166534;
}

/* ===== TEXT ===== */
.course-card h3 {
  font-size: 1.15rem;
  font-weight: 800;
  margin: 0;
  color: #0f172a;
}

.course-meta {
  font-size: 0.8rem;
  color: #64748b;
}

/* ===== BUTTON ===== */
.join-btn {
  margin-top: auto;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  border: none;
  padding: 14px;
  border-radius: 14px;
  font-weight: 800;
  cursor: pointer;
  transition: all 0.25s ease;
}

.join-btn:hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.join-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* ===== STATES ===== */
.state-box {
  padding: 60px;
  text-align: center;
  color: #94a3b8;
}
</style>
