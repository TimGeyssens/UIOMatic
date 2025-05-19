<template>
  <textarea
    :id="id"
    :value="modelValue"
    @input="$emit('update:modelValue', $event.target.value)"
    class="form-control"
    rows="4"
  ></textarea>
</template>

<script>
export default {
  name: 'TextAreaField',
  props: {
    modelValue: {
      type: String,
      default: ''
    },
    id: {
      type: String,
      required: true
    }
  },
  emits: ['update:modelValue']
};
</script>

<style scoped>
.form-control {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  resize: vertical;
  min-height: 100px;
}

.form-control:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.2);
}
</style> 