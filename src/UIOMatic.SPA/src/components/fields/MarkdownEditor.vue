<template>
  <div class="markdown-editor">
    <textarea ref="editor"></textarea>
  </div>
</template>

<script>
import EasyMDE from 'easymde'
import 'easymde/dist/easymde.min.css'

export default {
  name: 'MarkdownEditor',
  props: {
    modelValue: {
      type: String,
      default: ''
    },
    options: {
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
        if (this.editor && newValue !== this.editor.value()) {
          console.log('Updating editor value:', newValue);
          this.editor.value(newValue || '');
        }
      },
      immediate: true
    }
  },
  mounted() {
    console.log('MarkdownEditor mounted with value:', this.modelValue);
    const defaultOptions = {
      element: this.$refs.editor,
      initialValue: this.modelValue || '',
      spellChecker: false,
      status: false,
      autofocus: false,
      toolbar: [
        'bold', 'italic', 'heading', '|',
        'quote', 'unordered-list', 'ordered-list', '|',
        'link', 'image', '|',
        'preview', 'side-by-side', 'fullscreen', '|',
        'guide'
      ]
    }

    this.editor = new EasyMDE({
      ...defaultOptions,
      ...this.options
    })

    this.editor.codemirror.on('change', () => {
      const value = this.editor.value();
      console.log('Editor value changed:', value);
      this.$emit('update:modelValue', value);
    })
  },
  beforeUnmount() {
    if (this.editor) {
      this.editor.toTextArea()
      this.editor = null
    }
  }
}
</script>

<style>
.markdown-editor {
  width: 100%;
}

.markdown-editor .EasyMDEContainer {
  width: 100%;
}

.markdown-editor .CodeMirror {
  min-height: 300px;
}

.markdown-editor .editor-toolbar {
  border-radius: 4px 4px 0 0;
}

.markdown-editor .CodeMirror {
  border-radius: 0 0 4px 4px;
}

.markdown-editor .editor-preview {
  background-color: #fff;
}

.markdown-editor .editor-preview-side {
  border-left: 1px solid #ddd;
}
</style> 