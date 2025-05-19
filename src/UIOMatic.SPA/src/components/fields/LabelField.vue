<template>
  <div class="label-field">
    <span class="label-field__text">{{ modelValue }}</span>
  </div>
</template>

<script>
export default {
  name: 'LabelField',
  props: {
    modelValue: {
      type: [String, Number],
      default: ''
    },
    id: {
      type: String,
      required: true
    }
  }
}
</script>

<style scoped>
.label-field {
  padding: 0.5rem;
  background-color: var(--background-color);
  border: 1px solid var(--border-color);
  border-radius: 4px;
  min-height: 2.5rem;
  display: flex;
  align-items: center;
}

.label-field__text {
  color: var(--text-color);
  font-size: 1rem;
}
</style> 