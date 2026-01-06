JUAN JOSE HERNANDEZ VARGAS 
VAN ROSSUM
RIWI
CONTACTO: 3217895801


# PlataformaCursos_fronted

Proyecto frontend de una plataforma de cursos (SPA) construido con Vite y Vue 3. Este repositorio contiene la interfaz de usuario para estudiantes y tutores, incluye gestión de autenticación, navegación por rutas, consumo de una API (vía axios), y componentes organizados por rol.

Índice
-
- Descripción
- Tecnologías
- Requisitos previos
- Instalación (local)
- Scripts disponibles
- Desarrollo y despliegue
- Docker
- Variables de entorno
- Estructura del proyecto
- Componentes y vistas principales
- Router y stores
- API (axios)
- Estilos y assets
- Buenas prácticas y convenciones
- Contribución
- Resolución de problemas comunes
- Licencia y contacto

## Descripción

Interfaz de usuario para una plataforma de cursos con soporte para usuarios estudiantes y tutores. Permite: registro e inicio de sesión, ver catálogo de cursos, seguir progreso, gestión de cursos por parte de tutores y estadísticas.

Este README documenta cómo instalar, ejecutar, probar, construir y desplegar la aplicación, además de detallar la estructura del código y las piezas clave.

## Tecnologías

- Framework: Vue 3 (Single File Components)
- Bundler / Dev server: Vite
- HTTP client: axios (directorio `src/api/axios.js`)
- Estilos: CSS nativo y Bootstrap (instalado via npm)
- Contenido: HTML, JavaScript moderno (ES modules)
- Contenedor: Dockerfile incluido

## Requisitos previos

- Node.js >= 14 (preferible 16+)
- npm o yarn
- Docker (opcional, para build/imagen)

## Instalación (local)

Clona el repositorio y entra en la carpeta:

```bash
git clone <repo-url>
cd PlataformaCursos_fronted
```

Instala dependencias:

```bash
npm install
# o
# yarn
```

## Scripts disponibles

Los scripts definidos en `package.json` (asegúrate de revisar `package.json` para confirmar nombres exactos):

- `npm run dev` — inicia el servidor de desarrollo (Vite) en modo HMR.
- `npm run build` — construye la aplicación para producción en `dist/`.
- `npm run preview` — sirve la build de producción localmente para pruebas.

Ejemplo para desarrollo:

```bash
npm run dev
# abrir http://localhost:5173 (puerto por defecto de Vite)
```

## Desarrollo y despliegue

- Durante el desarrollo usa `npm run dev`.
- Para producción, genera la build con `npm run build` y despliega la carpeta `dist/` en cualquier servidor estático o CDN.
- Recomendación de despliegue: servir `dist/` desde un CDN o un servidor (NGINX) con configuración para history API fallback (SPA).

### Ejemplo de NGINX (resumen)

```
server {
  listen 80;
  server_name example.com;
  root /var/www/plataformacursos/dist;

  location / {
    try_files $uri $uri/ /index.html;
  }
}
```

## Docker

El repositorio incluye un `Dockerfile`. Flujo típico:

```bash
# construir imagen
docker build -t plataforma-cursos-frontend:latest .

# ejecutar contenedor (sirve archivos estáticos si el Dockerfile lo hace)
docker run -p 8080:80 plataforma-cursos-frontend:latest
```

Revisa `Dockerfile` para confirmar puertos y el comando `CMD` que sirve la aplicación.

## Variables de entorno

La aplicación puede necesitar variables de entorno para apuntar a la API (por ejemplo `VITE_API_BASE_URL`). Con Vite, las variables suelen empezar por `VITE_`. Añade un `.env` o `.env.local` en la raíz con:

```
VITE_API_BASE_URL=https://api.example.com
# Otros flags (ejemplo)
VITE_APP_TITLE=PlataformaCursos
```

Reinicia el servidor de desarrollo si cambias las variables.

## Estructura del proyecto

Raíz relevante (resumida):

- `index.html` — punto de entrada HTML.
- `package.json` — scripts y dependencias.
- `vite.config.js` — configuración de Vite.
- `Dockerfile` — imagen para producción.
- `src/` — código fuente de la aplicación.
  - `main.js` — bootstrap de Vue, montaje.
  - `App.vue` — componente raíz.
  - `api/axios.js` — instancia de axios configurada.
  - `assets/` — imágenes y recursos.
  - `components/` — componentes reutilizables y por rol.
    - `HelloWorld.vue`, `UserProfile.vue`.
    - `student/` — `CourseCatalog.vue`, `MyLearning.vue`, `StudentStats.vue`.
    - `tutor/` — `TutorCourses.vue`, `TutorStats.vue`, `TutorStudents.vue`.
  - `router/index.js` — definición de rutas / protección por roles.
  - `stores/` — Pinia / Vuex? (en este repo hay `auth.js`) — gestión de auth y estado.
  - `views/` — vistas de página: `LandingView.vue`, `LoginView.vue`, `RegisterView.vue`, `student/StudentDashboard.vue`, `tutor/TutorDashboard.vue`.

## Componentes y vistas principales

- `LandingView.vue` — página pública de bienvenida.
- `LoginView.vue` / `RegisterView.vue` — auth.
- `StudentDashboard.vue` — panel principal para estudiantes.
- `TutorDashboard.vue` — panel para tutores.
- `CourseCatalog.vue` — listado de cursos disponibles.
- `MyLearning.vue` — seguimiento de progreso del estudiante.

Los componentes en `src/components/tutor` y `src/components/student` agrupan la funcionalidad por rol. Mantén componentes pequeños y con una sola responsabilidad.

## Router y stores

- `src/router/index.js`: define rutas públicas y privadas y la lógica de guardia basada en `auth`.
- `src/stores/auth.js`: estado de autenticación, tokens y helper para protección de rutas.

Consejo: centraliza la lógica de validación de roles en un middleware/guard para no duplicarla en las vistas.

## API (axios)

`src/api/axios.js` contiene la instancia preconfigurada de axios con baseURL y posiblemente interceptores para añadir el token de autorización y manejar errores 401 (refresh token o logout).

Ejemplo de uso:

```js
import api from '../api/axios'

export function fetchCourses() {
  return api.get('/courses')
}
```

Comprueba que los endpoints coinciden con tu backend y maneja errores con try/catch en los componentes.

## Estilos y assets

- El proyecto usa CSS y Bootstrap (instalado). Para personalizar temas, prefiera variables CSS o importar un archivo SCSS/LESS si decides introducir preprocesador.
- Coloca imágenes y fuentes en `src/assets`.

## Buenas prácticas y convenciones

- Mantener componentes lo más pequeños posible y dividir en presentational + container cuando aplique.
- Evitar lógica de negocio en componentes; mover a composables o stores.
- Usar `async/await` y centralizar manejo de errores en helpers.
- Proteger rutas y comprobar roles en guards del router.

## Contribución

Si vas a contribuir:

1. Crea un fork y trabaja en una branch con nombre claro: `feature/mi-cambio` o `fix/tema`.
2. Ejecuta tests (si existen) y el linter antes de abrir PR.
3. Describe los cambios en el PR y el motivo.

Archivo de ejemplo para checklist local:

```bash
# instalar dependencias
npm install

# ejecutar dev server
npm run dev

# build producción
npm run build
```

## Resolución de problemas comunes

- Problema: CORS al conectar con la API. Solución: configurar CORS en el backend o usar un proxy en desarrollo en `vite.config.js`.
- Problema: rutas 404 en producción. Solución: configurar server (NGINX) con try_files para redirigir a `index.html`.
- Problema: variables de entorno no cargan. Solución: con Vite, usar `VITE_` prefijo y reiniciar el servidor.

## Tests y Linting

Actualmente no hay tests incluidos en el repositorio (revisar `package.json`). Se recomienda añadir:

- Linting con `eslint` configurado para Vue 3.
- Tests unitarios con `vitest` o `jest` + `vue-test-utils`.

## Checklist antes de PR/Deploy

- [ ] Todas las funcionalidades manualmente probadas en modo dev.
- [ ] Lint y formateo aplicados.
- [ ] Variables sensibles no incluidas en el repo.
- [ ] Build de producción pasa (`npm run build`).

## Licencia

Añade una licencia al repositorio (ej. MIT) si la organización lo permite. Actualmente no se incluye archivo `LICENSE` en este repo (revisar y añadir si procede).

## Contacto

Si necesitas ayuda adicional con este repo, prueba a:

- Abrir un issue en el repositorio con descripción y pasos para reproducir.
- Contactar al autor responsable del frontend en tu equipo.

---

Si quieres, puedo:

- traducir este `README.md` al inglés,
- añadir badges (build, npm version, license),
- o ejecutar `npm run build` y comprobar la salida.

Archivo creado: [README.md](README.md)
# Vue 3 + Vite

