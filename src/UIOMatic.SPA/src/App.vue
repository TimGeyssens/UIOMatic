<template>
  <div class="app">
    <nav v-if="!isLoginPage" class="nav">
      <div class="nav__content">
        <router-link to="/" class="nav__link">
          <i class="fas fa-home"></i> Home
        </router-link>
        <template v-if="contentTypes.length > 0">
          <router-link 
            v-for="type in contentTypes" 
            :key="type.alias"
            :to="`/${type.alias}/list`"
            class="nav__link"
          >
            <i :class="['fas', getIconClass(type.folderIcon)]"></i>
            {{ type.displayNamePlural }}
          </router-link>
        </template>
      </div>
      <div class="nav__footer">
        <button class="nav__link nav__link--logout" @click="handleLogout">
          <i class="fas fa-sign-out-alt"></i>
          Logout
        </button>
      </div>
    </nav>
    <main :class="['main', { 'main--full': isLoginPage }]">
      <Breadcrumb v-if="!isLoginPage" />
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>
    <Toast />
  </div>
</template>

<script>
import { ref, onMounted, computed, onErrorCaptured } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { UIOMaticService } from './services/uiomatic.service';
import { getIconClass } from './utils/icons';
import Toast from './components/Toast.vue';
import { toast } from './components/Toast.vue';
import axios from 'axios';
import Breadcrumb from './components/Breadcrumb.vue';

export default {
  name: 'App',
  components: {
    Toast,
    Breadcrumb
  },
  setup() {
    const router = useRouter();
    const route = useRoute();
    const contentTypes = ref([]);

    const isLoginPage = computed(() => route.path === '/login');

    // Global error handler for JavaScript errors
    onErrorCaptured((error, instance, info) => {
      console.error('Vue Error:', error);
      console.error('Component:', instance);
      console.error('Error Info:', info);
      
      // Show error toast
      toast.error(error.message || 'An unexpected error occurred. Please try again.');
      return false; // Prevent error from propagating
    });

    // Global error handler for API errors
    axios.interceptors.response.use(
      response => response,
      error => {
        console.error('API Error:', error);
        
        let errorMessage = 'An unexpected error occurred. Please try again.';
        
        // Handle different types of API errors
        if (error.response) {
          // The request was made and the server responded with a status code
          // that falls out of the range of 2xx
          const status = error.response.status;
          const message = error.response.data?.message || 'An error occurred while communicating with the server.';

          switch (status) {
            case 401:
              errorMessage = 'Your session has expired. Please log in again.';
              router.push('/login');
              break;
            case 403:
              errorMessage = 'You do not have permission to perform this action.';
              break;
            case 404:
              errorMessage = 'The requested resource was not found.';
              break;
            case 500:
              errorMessage = 'A server error occurred. Please try again later.';
              break;
            default:
              errorMessage = message;
          }
        } else if (error.request) {
          // The request was made but no response was received
          errorMessage = 'Unable to connect to the server. Please check your internet connection.';
        }

        // Show error toast
        toast.error(errorMessage);
        return Promise.reject(error);
      }
    );

    const fetchContentTypes = async () => {
      // Only fetch content types if user is logged in
      if (!localStorage.getItem('token')) {
        return;
      }

      try {
        const types = await UIOMaticService.getAllTypes();
        console.log('Fetched types:', types);
        contentTypes.value = types.map(type => {
          console.log('Type:', type);
          console.log('Folder icon:', type.folderIcon);
          return {
            ...type,
            label: type.displayNamePlural,
            fields: type.editableProperties || [],
            displayNamePlural: type.displayNamePlural || type.displayName + 's'
          };
        });
      } catch (error) {
        console.error('Error fetching content types:', error);
        // The error will be handled by the axios interceptor
      }
    };

    const handleLogout = () => {
      try {
        // Clear any stored tokens or session data
        localStorage.removeItem('token');
        // Redirect to login page
        router.push('/login');
      } catch (error) {
        console.error('Error during logout:', error);
        toast.error('An error occurred during logout. Please try again.');
      }
    };

    onMounted(() => {
      if (!isLoginPage.value) {
        fetchContentTypes();
      }
    });

    return {
      contentTypes,
      getIconClass,
      handleLogout,
      isLoginPage
    };
  }
};
</script>

<style>
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css');

:root {
  --primary-color: #4f46e5;
  --primary-hover: #4338ca;
  --secondary-color: #10b981;
  --secondary-hover: #059669;
  --background-color: #f9fafb;
  --text-color: #1f2937;
  --text-light: #6b7280;
  --border-color: #e5e7eb;
  --danger-color: #ef4444;
  --danger-hover: #dc2626;
  --success-color: #10b981;
  --success-hover: #059669;
  --warning-color: #f59e0b;
  --warning-hover: #d97706;
}

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
  line-height: 1.5;
  color: var(--text-color);
  background-color: var(--background-color);
}

.app {
  display: flex;
  min-height: 100vh;
}

.nav {
  width: 280px;
  background: white;
  border-right: 1px solid var(--border-color);
  padding: 1.5rem;
  position: fixed;
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.nav__content {
  flex: 1;
  overflow-y: auto;
}

.nav__footer {
  padding-top: 1rem;
  border-top: 1px solid var(--border-color);
  margin-top: auto;
}

.nav__link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  color: var(--text-color);
  text-decoration: none;
  border-radius: 8px;
  transition: all 0.2s ease;
  font-weight: 500;
  cursor: pointer;
  border: none;
  background: none;
  width: 100%;
  text-align: left;
}

.nav__link:hover {
  background-color: var(--background-color);
  color: var(--primary-color);
}

.nav__link.router-link-active {
  background-color: var(--primary-color);
  color: white;
}

.nav__link--logout {
  color: var(--danger-color);
}

.nav__link--logout:hover {
  background-color: var(--danger-color);
  color: white;
}

.nav__link i {
  font-size: 1.25rem;
  width: 1.5rem;
  text-align: center;
}

.main {
  flex: 1;
  margin-left: 280px;
  padding: 2rem;
  background-color: var(--background-color);
}

.main--full {
  margin-left: 0;
  max-width: 100%;
}

.page-enter-active,
.page-leave-active {
  transition: opacity 0.3s ease;
}

.page-enter-from,
.page-leave-to {
  opacity: 0;
}

/* Button styles */
.btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  border: none;
  font-size: 0.875rem;
}

.btn-primary {
  background-color: var(--primary-color);
  color: white;
}

.btn-primary:hover {
  background-color: var(--primary-hover);
}

.btn-secondary {
  background-color: var(--secondary-color);
  color: white;
}

.btn-secondary:hover {
  background-color: var(--secondary-hover);
}

.btn-danger {
  background-color: var(--danger-color);
  color: white;
}

.btn-danger:hover {
  background-color: var(--danger-hover);
}

.btn-success {
  background-color: var(--success-color);
  color: white;
}

.btn-success:hover {
  background-color: var(--success-hover);
}

.btn-warning {
  background-color: var(--warning-color);
  color: white;
}

.btn-warning:hover {
  background-color: var(--warning-hover);
}

/* Form styles */
.form-group {
  margin-bottom: 1.5rem;
}

.form-label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: var(--text-color);
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid var(--border-color);
  border-radius: 6px;
  font-size: 0.875rem;
  transition: all 0.2s ease;
}

.form-control:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

/* Card styles */
.card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
  border: 1px solid var(--border-color);
  overflow: hidden;
}

.card__header {
  padding: 1.5rem;
  border-bottom: 1px solid var(--border-color);
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.card__title {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 0;
}

.card__body {
  padding: 1.5rem;
}

/* Table styles */
.table {
  width: 100%;
  border-collapse: collapse;
}

.table th {
  background-color: var(--background-color);
  font-weight: 600;
  text-align: left;
  padding: 1rem;
  border-bottom: 2px solid var(--border-color);
}

.table td {
  padding: 1rem;
  border-bottom: 1px solid var(--border-color);
}

.table tr:hover {
  background-color: var(--background-color);
}

/* Badge styles */
.badge {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
}

.badge-primary {
  background-color: rgba(79, 70, 229, 0.1);
  color: var(--primary-color);
}

.badge-success {
  background-color: rgba(16, 185, 129, 0.1);
  color: var(--success-color);
}

.badge-warning {
  background-color: rgba(245, 158, 11, 0.1);
  color: var(--warning-color);
}

.badge-danger {
  background-color: rgba(239, 68, 68, 0.1);
  color: var(--danger-color);
}
</style>
