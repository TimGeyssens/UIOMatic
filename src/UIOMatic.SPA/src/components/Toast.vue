<template>
  <div class="toast-container">
    <TransitionGroup name="toast">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        :class="['toast', `toast--${toast.type}`]"
        role="alert"
      >
        <i :class="['fas', getIcon(toast.type)]"></i>
        <span>{{ toast.message }}</span>
      </div>
    </TransitionGroup>
  </div>
</template>

<script>
import { ref } from 'vue';

const toasts = ref([]);
let toastId = 0;

const showToast = (message, type = 'info', duration = 5000) => {
  const id = ++toastId;
  toasts.value.push({ id, message, type });
  
  setTimeout(() => {
    const index = toasts.value.findIndex(t => t.id === id);
    if (index !== -1) {
      toasts.value.splice(index, 1);
    }
  }, duration);
};

export const toast = {
  success: (message, duration) => showToast(message, 'success', duration),
  error: (message, duration) => showToast(message, 'error', duration),
  warning: (message, duration) => showToast(message, 'warning', duration),
  info: (message, duration) => showToast(message, 'info', duration)
};

export default {
  name: 'Toast',
  setup() {
    const getIcon = (type) => {
      switch (type) {
        case 'success':
          return 'fa-check-circle';
        case 'error':
          return 'fa-exclamation-circle';
        case 'warning':
          return 'fa-exclamation-triangle';
        case 'info':
        default:
          return 'fa-info-circle';
      }
    };

    return {
      toasts,
      getIcon
    };
  }
};
</script>

<style scoped>
.toast-container {
  position: fixed;
  top: 1rem;
  right: 1rem;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-width: 24rem;
}

.toast {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  border-radius: 0.5rem;
  background: white;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  color: var(--text-color);
  font-size: 0.875rem;
  line-height: 1.25rem;
}

.toast--success {
  background-color: var(--success-color);
  color: white;
}

.toast--error {
  background-color: var(--danger-color);
  color: white;
}

.toast--warning {
  background-color: var(--warning-color);
  color: white;
}

.toast--info {
  background-color: var(--primary-color);
  color: white;
}

.toast i {
  font-size: 1.25rem;
}

/* Toast animations */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(30px);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
</style> 