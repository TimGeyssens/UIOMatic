<template>
  <div class="edit-form">
    <div class="card">
      <div class="card__header">
        <div class="header-content">
          <i :class="['fas', getIconClass(typeInfo?.itemIcon)]"></i>
          <h1 class="card__title">
            {{ isEdit ? 'Edit' : 'Create' }} {{ typeInfo?.displayName }}
          </h1>
        </div>
      </div>

      <div class="card__body">
        <form @submit.prevent="handleSubmit">
          <div
            v-for="property in typeInfo?.editableProperties"
            :key="property.columnName"
            class="form-group"
          >
            <label :for="property.columnName" class="form-label">
              {{ property.label }}
              <span v-if="property.required" class="required">*</span>
            </label>
            <component
              :is="getFieldComponent(property)"
              :id="property.columnName"
              v-model="formData[property.columnName]"
              :required="property.required"
              :config="property.config"
            />
            <p v-if="property.description" class="form-help">
              {{ property.description }}
            </p>
          </div>

          <div class="form-actions">
            <button
              type="button"
              class="btn btn-secondary"
              @click="handleCancel"
              :disabled="isSubmitting"
            >
              <i class="fas fa-times"></i>
              Cancel
            </button>
            <button
              type="submit"
              class="btn btn-primary"
              :disabled="isSubmitting"
            >
              <i class="fas" :class="isSubmitting ? 'fa-spinner fa-spin' : 'fa-save'"></i>
              {{ isSubmitting ? 'Saving...' : 'Save' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { UIOMaticService } from '../services/uiomatic.service';
import { getIconClass } from '../utils/icons';
import { toast } from './Toast.vue';
import TextField from './fields/TextField.vue';
import TextAreaField from './fields/TextAreaField.vue';
import NumberField from './fields/NumberField.vue';
import DateTimeField from './fields/DateTimeField.vue';
import DropdownField from './fields/DropdownField.vue';
import ImageField from './fields/ImageField.vue';
import MarkdownEditor from './fields/MarkdownEditor.vue';
import LabelField from './fields/LabelField.vue';
import PasswordField from './fields/PasswordField.vue';
import RadioField from './fields/RadioField.vue';
import RichTextField from './fields/RichTextField.vue';

export default {
  name: 'EditForm',
  components: {
    TextField,
    TextAreaField,
    NumberField,
    DateTimeField,
    DropdownField,
    ImageField,
    MarkdownEditor,
    LabelField,
    PasswordField,
    RadioField,
    RichTextField
  },
  props: {
    type: {
      type: String,
      required: true
    }
  },
  setup(props) {
    const route = useRoute();
    const router = useRouter();
    const typeInfo = ref(null);
    const formData = ref({});
    const errors = ref({});
    const isSubmitting = ref(false);

    console.log('EditForm mounted with props:', props);
    console.log('Route params:', route.params);

    const isEdit = computed(() => !!route.params.id);

    const loadTypeInfo = async () => {
      try {
        console.log('Loading type info for:', props.type);
        console.log('Props in loadTypeInfo:', props);
        const types = await UIOMaticService.getAllTypes();
        console.log('Available types:', types);
        typeInfo.value = types.find(t => t.alias === props.type);
        if (!typeInfo.value) {
          console.error('Type not found:', props.type);
          console.error('Available types:', types.map(t => t.alias));
          router.push('/');
          return;
        }
        console.log('Type info loaded:', typeInfo.value);

        if (isEdit.value) {
          console.log('Loading item for edit, ID:', route.params.id);
          const item = await UIOMaticService.getItem(props.type, route.params.id);
          console.log('Loaded item:', item);
          if (item) {
            // Initialize form data with empty values first
            formData.value = {};
            typeInfo.value.editableProperties.forEach(prop => {
              // Initialize with null instead of empty string to properly handle different field types
              formData.value[prop.columnName] = null;
            });
            // Then populate with actual values
            typeInfo.value.editableProperties.forEach(prop => {
              // Convert to camelCase (first letter lowercase)
              const itemKey = prop.columnName.charAt(0).toLowerCase() + prop.columnName.slice(1);
              if (itemKey in item) {
                // For markdown fields, ensure we're passing the content as a string
                if (prop.view?.includes('markdown')) {
                  formData.value[prop.columnName] = String(item[itemKey] || '');
                } else {
                  formData.value[prop.columnName] = item[itemKey];
                }
                console.log(`Setting ${prop.columnName} to:`, formData.value[prop.columnName]);
              }
            });
            console.log('Form data populated:', formData.value);
          }
        } else {
          // For create, initialize form data with empty values
          formData.value = {};
          typeInfo.value.editableProperties.forEach(prop => {
            // Initialize with null instead of empty string
            formData.value[prop.columnName] = null;
          });
        }
      } catch (error) {
        console.error('Error loading type info:', error);
      }
    };

    const getFieldComponent = (prop) => {
      // Extract the field type from the view path
      const viewPath = prop.view || '';
      const fieldType = viewPath.split('/').pop()?.replace('.html', '') || 'textfield';
      console.log('Field type for', prop.columnName, ':', fieldType);

      switch (fieldType) {
        case 'textarea':
          return 'TextAreaField';
        case 'number':
          return 'NumberField';
        case 'datetime':
          return 'DateTimeField';
        case 'dropdown':
          return 'DropdownField';
        case 'image':
          return 'ImageField';
        case 'markdown':
          return 'MarkdownEditor';
        case 'label':
          return 'LabelField';
        case 'password':
          return 'PasswordField';
        case 'radio':
          return 'RadioField';
        case 'rte':
          return 'RichTextField';
        case 'textfield':
        default:
          return 'TextField';
      }
    };

    const handleSubmit = async () => {
      isSubmitting.value = true;
      errors.value = {};

      try {
        if (isEdit.value) {
          await UIOMaticService.updateItem(props.type, formData.value);
          toast.success(`${typeInfo.value.displayName} updated successfully`);
        } else {
          await UIOMaticService.createItem(props.type, formData.value);
          toast.success(`${typeInfo.value.displayName} created successfully`);
        }
        router.push(`/${props.type}/list`);
      } catch (error) {
        if (error.response?.data) {
          errors.value = error.response.data;
          toast.error('Failed to save. Please check the form for errors.');
        } else {
          console.error('Error saving item:', error);
          toast.error('An unexpected error occurred while saving.');
        }
      } finally {
        isSubmitting.value = false;
      }
    };

    const handleCancel = () => {
      router.push(`/${props.type}/list`);
    };

    onMounted(loadTypeInfo);

    return {
      typeInfo,
      formData,
      errors,
      isSubmitting,
      isEdit,
      getFieldComponent,
      handleSubmit,
      handleCancel,
      getIconClass
    };
  }
};
</script>

<style scoped>
.edit-form {
  max-width: 800px;
  margin: 0 auto;
}

.header-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.header-content i {
  font-size: 1.5rem;
  color: var(--primary-color);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: var(--text-color);
}

.required {
  color: var(--danger-color);
  margin-left: 0.25rem;
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

.form-checkbox {
  width: 1.25rem;
  height: 1.25rem;
  margin-top: 0.25rem;
  border-radius: 4px;
  border: 1px solid var(--border-color);
  cursor: pointer;
}

.form-help {
  margin-top: 0.5rem;
  font-size: 0.875rem;
  color: var(--text-light);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--border-color);
}

/* Button hover effects */
.btn:hover {
  transform: translateY(-1px);
}

.btn:active {
  transform: translateY(0);
}

/* Disabled state */
.btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

/* Loading spinner */
.fa-spinner {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style> 