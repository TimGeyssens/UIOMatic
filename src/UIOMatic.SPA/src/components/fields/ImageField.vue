<template>
  <div class="image-field">
    <div v-if="previewUrl" class="image-field__preview">
      <img :src="previewUrl" alt="Preview" class="image-field__image" />
      <button @click="removeImage" class="btn btn-small btn-danger image-field__remove">
        Remove
      </button>
    </div>
    
    <div v-else class="image-field__upload">
      <label :for="id" class="image-field__label">
        <div class="image-field__dropzone">
          <span class="image-field__icon">📁</span>
          <span class="image-field__text">Drop image here or click to upload</span>
        </div>
      </label>
      <input
        :id="id"
        type="file"
        accept="image/*"
        class="image-field__input"
        @change="handleFileChange"
        @dragover.prevent
        @drop.prevent="handleDrop"
      />
    </div>

    <div v-if="error" class="image-field__error">{{ error }}</div>
  </div>
</template>

<script>
import { ref, watch } from 'vue';
import { UIOMaticService } from '../../services/uiomatic.service';

export default {
  name: 'ImageField',
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
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const previewUrl = ref('');
    const error = ref('');

    // Watch for changes in modelValue to update preview
    watch(() => props.modelValue, (newValue) => {
      if (newValue) {
        previewUrl.value = newValue;
      } else {
        previewUrl.value = '';
      }
    }, { immediate: true });

    const handleFileChange = async (event) => {
      const file = event.target.files[0];
      if (file) {
        await uploadFile(file);
      }
    };

    const handleDrop = async (event) => {
      const file = event.dataTransfer.files[0];
      if (file && file.type.startsWith('image/')) {
        await uploadFile(file);
      } else {
        error.value = 'Please drop an image file';
      }
    };

    const uploadFile = async (file) => {
      try {
        error.value = '';
        const result = await UIOMaticService.uploadImage(file);
        emit('update:modelValue', result.path);
      } catch (err) {
        error.value = 'Failed to upload image';
        console.error('Error uploading image:', err);
      }
    };

    const removeImage = () => {
      previewUrl.value = '';
      emit('update:modelValue', '');
    };

    return {
      previewUrl,
      error,
      handleFileChange,
      handleDrop,
      removeImage
    };
  }
};
</script>

<style scoped>
.image-field {
  width: 100%;
}

.image-field__preview {
  position: relative;
  width: 200px;
  height: 200px;
  border: 2px dashed #ddd;
  border-radius: 4px;
  overflow: hidden;
}

.image-field__image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.image-field__remove {
  position: absolute;
  top: 8px;
  right: 8px;
  background-color: rgba(244, 67, 54, 0.9);
}

.image-field__upload {
  width: 100%;
}

.image-field__label {
  display: block;
  width: 100%;
  cursor: pointer;
}

.image-field__dropzone {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 200px;
  border: 2px dashed #ddd;
  border-radius: 4px;
  background-color: #fafafa;
  transition: border-color 0.3s ease;
}

.image-field__dropzone:hover {
  border-color: #4CAF50;
}

.image-field__icon {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}

.image-field__text {
  color: #666;
}

.image-field__input {
  display: none;
}

.image-field__error {
  color: #f44336;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  color: white;
}

.btn-small {
  padding: 0.25rem 0.5rem;
  font-size: 0.875rem;
}

.btn-danger {
  background-color: #f44336;
}
</style> 