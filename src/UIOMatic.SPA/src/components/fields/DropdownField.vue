<template>
  <select
    :id="id"
    :value="modelValue"
    @input="$emit('update:modelValue', $event.target.value)"
    class="form-control"
  >
    <option value="">Select an option</option>
    <option
      v-for="option in options"
      :key="option.key"
      :value="option.key"
    >
      {{ option.value }}
    </option>
  </select>
</template>

<script>
import { ref, onMounted } from 'vue';
import { UIOMaticService } from '../../services/uiomatic.service';

export default {
  name: 'DropdownField',
  props: {
    modelValue: {
      type: [String, Number],
      default: ''
    },
    id: {
      type: String,
      required: true
    },
    config: {
      type: Object,
      required: true
    }
  },
  emits: ['update:modelValue'],
  setup(props) {
    const options = ref([]);

    const loadOptions = async () => {
      try {
        const data = await UIOMaticService.getFilterLookup(
          props.config.typeAlias,
          props.config.valueColumn,
          props.config.textTemplate
        );
        options.value = data;
      } catch (error) {
        console.error('Error loading dropdown options:', error);
      }
    };

    onMounted(loadOptions);

    return {
      options
    };
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
  background-color: white;
}

.form-control:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.2);
}
</style> 