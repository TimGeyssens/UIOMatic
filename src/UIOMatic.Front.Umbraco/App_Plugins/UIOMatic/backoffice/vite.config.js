import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    outDir: '../dist',
    lib: {
      entry: {
        'components/list/uiomatic-list': './components/list/uiomatic-list.ts',
        'components/edit/uiomatic-edit': './components/edit/uiomatic-edit.ts',
        'components/delete/uiomatic-delete': './components/delete/uiomatic-delete.ts'
      },
      formats: ['es']
    },
    rollupOptions: {
      external: ['lit', '@umbraco-cms/element', '@umbraco-cms/styles', '@umbraco-cms/notification']
    }
  }
}); 