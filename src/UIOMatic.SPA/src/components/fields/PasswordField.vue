<template>
  <div class="password-field">
    <div class="password-field__input-wrapper">
      <input
        :type="showPassword ? 'text' : 'password'"
        :id="id"
        :value="modelValue"
        @input="$emit('update:modelValue', $event.target.value)"
        class="form-control"
        :required="required"
      />
      <button
        type="button"
        class="password-field__toggle"
        @click="togglePassword"
        tabindex="-1"
      >
        <i :class="['fas', showPassword ? 'fa-eye-slash' : 'fa-eye']"></i>
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PasswordField',
  props: {
    modelValue: {
      type: String,
      default: ''
    },
    id: {
      type: String,
      required: true
    },
    required: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      showPassword: false
    }
  },
  methods: {
    togglePassword() {
      this.showPassword = !this.showPassword;
    }
  },
  emits: ['update:modelValue']
}
</script>

<style scoped>
.password-field {
  position: relative;
}

.password-field__input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.password-field__toggle {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  padding: 0.25rem;
  cursor: pointer;
  color: var(--text-light);
  transition: color 0.2s ease;
}

.password-field__toggle:hover {
  color: var(--text-color);
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  padding-right: 2.5rem;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  font-size: 1rem;
  transition: all 0.2s ease;
}

.form-control:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.2);
}
</style> 