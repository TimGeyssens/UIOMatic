<template>
  <div class="rich-text-field">
    <textarea ref="editor"></textarea>
  </div>
</template>

<script>
import { Editor as TinyMCEEditor } from '@tinymce/tinymce-vue'
import { rteConfig } from '../../config'

export default {
  name: 'RichTextField',
  components: {
    Editor: TinyMCEEditor
  },
  props: {
    modelValue: {
      type: String,
      default: ''
    },
    id: {
      type: String,
      required: true
    },
    config: {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {
      editor: null
    }
  },
  watch: {
    modelValue: {
      handler(newValue) {
        if (this.editor && newValue !== this.editor.getContent()) {
          this.editor.setContent(newValue || '');
        }
      },
      immediate: true
    }
  },
  mounted() {
    // Merge the default config with any custom config
    const mergedConfig = {
      ...rteConfig,
      ...this.config,
      target: this.$refs.editor,
      setup: (editor) => {
        this.editor = editor;
        editor.on('change', () => {
          this.$emit('update:modelValue', editor.getContent());
        });
      }
    };

    // Initialize TinyMCE
    tinymce.init(mergedConfig);
  },
  beforeUnmount() {
    if (this.editor) {
      this.editor.destroy();
    }
  },
  emits: ['update:modelValue']
}
</script>

<style>
.rich-text-field {
  width: 100%;
}

.rich-text-field .tox-tinymce {
  border-radius: 4px;
  border-color: var(--border-color) !important;
}

.rich-text-field .tox-tinymce:focus-within {
  border-color: var(--primary-color) !important;
  box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.2);
}

.rich-text-field .tox-toolbar__primary {
  background-color: var(--background-color) !important;
}

.rich-text-field .tox-tbtn {
  color: var(--text-color) !important;
}

.rich-text-field .tox-tbtn:hover {
  background-color: var(--border-color) !important;
}
</style> 