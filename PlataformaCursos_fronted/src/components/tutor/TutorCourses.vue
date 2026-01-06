<template>
    <div class="courses-wrapper">
        <!-- ================= HEADER ================= -->
        <div class="courses-header">
            <div>
                <h2>Mis Cursos</h2>
                <p>Administra y crea tus cursos</p>
            </div>

            <button class="primary-btn" @click="showCreate = true">
                + Crear curso
            </button>
        </div>

        <!-- ================= MODAL CREAR CURSO ================= -->
        <div v-if="showCreate" class="modal-overlay">
            <div class="modal-card">
                <h3>Crear nuevo curso</h3>

                <input
                    v-model="newCourseTitle"
                    placeholder="Título del curso"
                    class="input"
                />

                <div class="modal-actions">
                    <button class="ghost-btn" @click="closeModal">
                        Cancelar
                    </button>
                    <button
                        class="primary-btn"
                        @click="createCourse"
                        :disabled="loading"
                    >
                        Crear
                    </button>
                </div>
            </div>
        </div>

        <!-- ================= MODAL CREAR LECCIÓN ================= -->
        <div v-if="showLessonModal" class="modal-overlay">
            <div class="modal-card">
                <h3>Nueva lección</h3>

                <input
                    v-model="newLessonTitle"
                    placeholder="Título de la lección"
                    class="input"
                />

                <div class="modal-actions">
                    <button class="ghost-btn" @click="closeLessonModal">
                        Cancelar
                    </button>
                    <button
                        class="primary-btn"
                        @click="createLesson"
                        :disabled="loading"
                    >
                        Crear
                    </button>
                </div>
            </div>
        </div>

        <!-- ================= MODAL VER LECCIONES ================= -->
        <div v-if="showLessonsModal" class="modal-overlay">
            <div class="modal-card">
                <h3>Lecciones del curso</h3>

                <p class="lesson-course-title">
                    {{ selectedCourse?.title }}
                </p>

                <div v-if="lessons.length === 0" class="state-box">
                    Este curso aún no tiene lecciones
                </div>

                <ul v-else class="lessons-list">
                    <li v-for="lesson in lessons" :key="lesson.id">
                        📘 {{ lesson.title }}
                    </li>
                </ul>

                <div class="modal-actions">
                    <button class="ghost-btn" @click="closeLessonsModal">
                        Cerrar
                    </button>
                </div>
            </div>
        </div>

        <!-- ================= LOADING ================= -->
        <div v-if="loading && courses.length === 0" class="state-box">
            Cargando cursos...
        </div>

        <!-- ================= EMPTY ================= -->
        <div v-if="!loading && courses.length === 0" class="state-box">
            Aún no has creado ningún curso
        </div>

        <!-- ================= COURSES GRID ================= -->
        <div class="courses-grid">
            <div
                v-for="course in courses"
                :key="course.id"
                class="course-card"
            >
                <div class="course-top">
                    <span
                        class="status"
                        :class="course.status === 'Published' ? 'published' : 'draft'"
                    >
                        {{ course.status }}
                    </span>
                </div>

                <h3>{{ course.title }}</h3>

                <div class="course-actions">
                    <button
                        v-if="course.status !== 'Published'"
                        class="success-btn"
                        @click="publishCourse(course.id)"
                    >
                        Publicar
                    </button>

                    <button
                        class="info-btn"
                        @click="openLessonModal(course.id)"
                    >
                        + Lección
                    </button>

                    <button
                        class="secondary-btn"
                        @click="openLessons(course)"
                    >
                        Ver lecciones
                    </button>

                    <button
                        v-if="course.status === 'Published'"
                        class="warning-btn"
                        @click="unpublishCourse(course.id)"
                    >
                        Ocultar
                    </button>

                    <button
                        class="danger-btn"
                        @click="deleteCourse(course.id)"
                    >
                        Eliminar
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../api/axios'

// =================== STATE ===================
const courses = ref([])
const lessons = ref([])

const loading = ref(false)

// Modales
const showCreate = ref(false)
const showLessonModal = ref(false)
const showLessonsModal = ref(false)

// Curso / lección
const newCourseTitle = ref('')
const newLessonTitle = ref('')
const selectedCourseId = ref(null)
const selectedCourse = ref(null)

// =================== API ===================
const fetchCourses = async () => {
    loading.value = true
    try {
        const res = await api.get('/courses/search')
        courses.value = res.data.items
    } catch (e) {
        console.error('Error cargando cursos', e)
    } finally {
        loading.value = false
    }
}

const createCourse = async () => {
    if (!newCourseTitle.value.trim()) return

    loading.value = true
    try {
        await api.post('/courses', { title: newCourseTitle.value })
        closeModal()
        await fetchCourses()
    } catch (e) {
        console.error('Error creando curso', e)
    } finally {
        loading.value = false
    }
}

const publishCourse = async (id) => {
    await api.patch(`/courses/${id}/publish`)
    fetchCourses()
}

const unpublishCourse = async (id) => {
    await api.patch(`/courses/${id}/unpublish`)
    fetchCourses()
}

const deleteCourse = async (id) => {
    if (!confirm('¿Eliminar este curso?')) return
    await api.delete(`/courses/${id}`)
    fetchCourses()
}

// =================== LECCIONES ===================
const openLessonModal = (courseId) => {
    selectedCourseId.value = courseId
    newLessonTitle.value = ''
    showLessonModal.value = true
}

const createLesson = async () => {
    if (!newLessonTitle.value.trim()) return

    loading.value = true
    try {
        await api.post(`/courses/${selectedCourseId.value}/lessons`, {
            title: newLessonTitle.value
        })
        closeLessonModal()
    } catch (e) {
        console.error('Error creando lección', e)
    } finally {
        loading.value = false
    }
}

// 🔥 ESTA ES LA FUNCIÓN CLAVE
const openLessons = async (course) => {
    selectedCourse.value = course
    showLessonsModal.value = true
    lessons.value = []

    try {
        const res = await api.get(`/courses/${course.id}/lessons`)
        lessons.value = res.data
    } catch (e) {
        console.error('Error cargando lecciones', e)
    }
}

// =================== MODALS ===================
const closeModal = () => {
    showCreate.value = false
    newCourseTitle.value = ''
}

const closeLessonModal = () => {
    showLessonModal.value = false
    newLessonTitle.value = ''
    selectedCourseId.value = null
}

const closeLessonsModal = () => {
    showLessonsModal.value = false
    lessons.value = []
    selectedCourse.value = null
}

// =================== INIT ===================
onMounted(fetchCourses)
</script>



<style scoped>
/* ===== WRAPPER ===== */
.courses-wrapper {
    display: flex;
    flex-direction: column;
    gap: 30px;
}

/* ===== HEADER ===== */
.courses-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.courses-header h2 {
    font-size: 1.6rem;
    font-weight: 800;
    margin: 0;
    color: #1e293b;
}

.courses-header p {
    font-size: 0.85rem;
    color: #64748b;
}

/* ===== BUTTONS ===== */
.primary-btn {
    background: #10b981;
    color: white;
    border: none;
    padding: 12px 20px;
    border-radius: 14px;
    font-weight: 700;
    cursor: pointer;
}

.primary-btn:hover {
    background: #059669;
}

.ghost-btn {
    background: transparent;
    border: 1px solid #e5e7eb;
    padding: 10px 18px;
    border-radius: 14px;
    cursor: pointer;
}

.success-btn {
    background: #10b981;
    color: white;
}

.warning-btn {
    background: #f59e0b;
    color: white;
}

.danger-btn {
    background: #ef4444;
    color: white;
}

.course-actions button {
    border: none;
    padding: 8px 14px;
    border-radius: 10px;
    font-size: 0.75rem;
    font-weight: 700;
    cursor: pointer;
}

/* ===== GRID ===== */
.courses-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
    gap: 24px;
}

.course-card {
    background: white;
    border-radius: 22px;
    padding: 22px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.06);
    display: flex;
    flex-direction: column;
    gap: 16px;
}

.course-card h3 {
    font-size: 1.1rem;
    font-weight: 700;
    color: #1e293b;
    margin: 0;
}

.course-top {
    display: flex;
    justify-content: flex-end;
}

.status {
    font-size: 0.7rem;
    font-weight: 800;
    padding: 4px 10px;
    border-radius: 999px;
}

.published {
    background: #dcfce7;
    color: #15803d;
}

.draft {
    background: #fef3c7;
    color: #92400e;
}

/* ===== STATES ===== */
.state-box {
    text-align: center;
    padding: 60px;
    color: #94a3b8;
}

/* ===== MODAL ===== */
.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.4);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 50;
}

.modal-card {
    background: white;
    padding: 30px;
    border-radius: 24px;
    width: 100%;
    max-width: 420px;
    display: flex;
    flex-direction: column;
    gap: 20px;
}

.modal-card h3 {
    margin: 0;
    font-size: 1.2rem;
    font-weight: 800;
}

/* ===== INPUT ===== */
.input {
    padding: 14px;
    border-radius: 14px;
    border: 1px solid #e5e7eb;
    font-size: 0.9rem;
}

.modal-actions {
    display: flex;
    justify-content: flex-end;
    gap: 12px;
}
</style>
