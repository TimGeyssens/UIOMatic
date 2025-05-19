<template>
  <div class="radio-field">
    <div
      v-for="option in options"
      :key="option.value"
      class="radio-field__option"
    >
      <input
        type="radio"
        :id="`${id}-${option.value}`"
        :name="id"
        :value="option.value"
        :checked="modelValue === option.value"
        @change="$emit('update:modelValue', option.value)"
        class="radio-field__input"
        :required="required"
      />
      <label :for="`${id}-${option.value}`" class="radio-field__label">
        {{ option.label }}
      </label>
    </div>
  </div>
</template>

<script>
export default {
  name: 'RadioField',
  props: {
    modelValue: {
      type: [String, Number],
      default: ''
    },
    id: {
      type: String,
      required: true
    },
    required: {
      type: Boolean,
      default: false
    },
    options: {
      type: Array,
      required: true,
      validator: (value) => {
        return value.every(option => 
          typeof option === 'object' && 
          'value' in option && 
          'label' in option
        );
      }
    }
  },
  emits: ['update:modelValue']
}
</script>

<style scoped>
.radio-field {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.radio-field__option {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.radio-field__input {
  width: 1.25rem;
  height: 1.25rem;
  margin: 0;
  cursor: pointer;
}

.radio-field__label {
  font-size: 1rem;
  color: var(--text-color);
  cursor: pointer;
  user-select: none;
}

.radio-field__input:checked + .radio-field__label {
  color: var(--primary-color);
  font-weight: 500;
}

.radio-field__input:focus {
  outline: 2px solid var(--primary-color);
  outline-offset: 2px;
}
</style> 