<template>
  <div class="login-container">
    <form @submit.prevent="handleLogin" class="login-form">
      <h2>Login</h2>
      
      <div class="form-group">
        <label for="username">Username</label>
        <input
          type="text"
          id="username"
          v-model="username"
          required
          class="form-control"
        />
      </div>

      <div class="form-group">
        <label for="password">Password</label>
        <input
          type="password"
          id="password"
          v-model="password"
          required
          class="form-control"
        />
      </div>

      <div v-if="error" class="error-message">
        {{ error }}
      </div>

      <button type="submit" class="btn btn-primary" :disabled="isLoading">
        {{ isLoading ? 'Logging in...' : 'Login' }}
      </button>
    </form>
  </div>
</template>

<script>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { AuthService } from '../services/auth.service';
import { useStore } from 'vuex';

export default {
  name: 'Login',
  setup() {
    const router = useRouter();
    const store = useStore();
    const username = ref('');
    const password = ref('');
    const error = ref('');
    const isLoading = ref(false);

    const handleLogin = async () => {
      try {
        isLoading.value = true;
        error.value = '';
        
        await AuthService.login(username.value, password.value);
        await store.dispatch('fetchContentTypes');
        router.push('/');
      } catch (err) {
        error.value = 'Invalid username or password';
      } finally {
        isLoading.value = false;
      }
    };

    return {
      username,
      password,
      error,
      isLoading,
      handleLogin
    };
  }
};
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f5f5f5;
}

.login-form {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.form-control {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.error-message {
  color: #dc3545;
  margin-bottom: 1rem;
}

.btn {
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
}

.btn-primary {
  background-color: #4CAF50;
  color: white;
}

.btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}
</style> 