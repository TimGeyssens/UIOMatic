export const config = {
  api: {
    root: import.meta.env.VITE_API_ROOT || 'https://localhost:7170',
    timeout: 30000, // 30 seconds
    headers: {
      'Content-Type': 'application/json'
    }
  },
  listView: {
    itemsPerPage: 10,
    defaultSortColumn: 'Id',
    defaultSortOrder: 'ASC'
  },
  editForm: {
    maxWidth: '800px'
  },
  theme: {
    colors: {
      primary: '#4CAF50',
      secondary: '#9e9e9e',
      danger: '#f44336',
      text: {
        primary: '#333',
        secondary: '#666'
      }
    },
    spacing: {
      small: '0.5rem',
      medium: '1rem',
      large: '2rem'
    }
  }
};

export const rteConfig = {
  height: 400,
  menubar: true,
  plugins: [
    'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
    'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
    'insertdatetime', 'media', 'table', 'code', 'help', 'wordcount'
  ],
  toolbar: [
    'undo redo | blocks | bold italic forecolor | alignleft aligncenter alignright alignjustify',
    'bullist numlist outdent indent | link image media table | removeformat | help'
  ].join(' | '),
  content_style: 'body { font-family: -apple-system, BlinkMacSystemFont, San Francisco, Segoe UI, Roboto, Helvetica Neue, sans-serif; font-size: 14px; }',
  branding: false,
  promotion: false,
  resize: true,
  statusbar: true,
  image_advtab: true,
  image_title: true,
  automatic_uploads: true,
  file_picker_types: 'image',
  images_upload_url: '/api/upload', // Update this with your actual upload endpoint
  images_upload_handler: null, // Will be set dynamically if needed
  setup: null // Will be set by the component
}; 