<template>
  <input
    type="number"
    :id="id"
    :value="modelValue"
    @input="$emit('update:modelValue', $event.target.value)"
    class="form-control"
  />
</template>

<script>
export default {
  name: 'NumberField',
  props: {
    modelValue: {
      type: [Number, String],
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
}

.form-control:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.2);
}

/* Remove spinner buttons from number input */
.form-control::-webkit-outer-spin-button,
.form-control::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.form-control[type=number] {
  -moz-appearance: textfield;
}
</style> 