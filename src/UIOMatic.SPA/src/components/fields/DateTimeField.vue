<template>
  <input
    type="datetime-local"
    :id="id"
    :value="formattedValue"
    @input="handleInput"
    class="form-control"
  />
</template>

<script>
export default {
  name: 'DateTimeField',
  props: {
    modelValue: {
      type: [String, Date],
      default: ''
    },
    id: {
      type: String,
      required: true
    }
  },
  emits: ['update:modelValue'],
  computed: {
    formattedValue() {
      if (!this.modelValue) return '';
      const date = new Date(this.modelValue);
      return date.toISOString().slice(0, 16);
    }
  },
  methods: {
    handleInput(event) {
      const value = event.target.value;
      if (!value) {
        this.$emit('update:modelValue', '');
        return;
      }
      const date = new Date(value);
      this.$emit('update:modelValue', date.toISOString());
    }
  }
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
</style> 