import axios from 'axios';
import { config } from '../config';
import { toast } from '../components/Toast.vue';
import { AuthService } from './auth.service';

const api = axios.create({
  baseURL: config.api.root,
  timeout: config.api.timeout,
  headers: config.api.headers
});

// Add request interceptor to add auth token
api.interceptors.request.use(
  config => {
    const token = AuthService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Add response interceptor for error handling
api.interceptors.response.use(
  response => response,
  error => {
    console.error('API Error:', error);
    
    let errorMessage = 'An unexpected error occurred. Please try again.';
    
    if (error.response) {
      const status = error.response.status;
      const message = error.response.data?.message || 'An error occurred while communicating with the server.';

      switch (status) {
        case 401:
          errorMessage = 'Your session has expired. Please log in again.';
          // Clear token and redirect to login
          AuthService.logout();
          window.location.href = '/login';
          break;
        case 403:
          errorMessage = 'You do not have permission to perform this action.';
          break;
        case 404:
          errorMessage = 'The requested resource was not found.';
          break;
        case 500:
          errorMessage = 'A server error occurred. Please try again later.';
          break;
        default:
          errorMessage = message;
      }
    } else if (error.request) {
      errorMessage = 'Unable to connect to the server. Please check your internet connection.';
    }

    // Show error toast
    toast.error(errorMessage);
    return Promise.reject(error);
  }
);

export const UIOMaticService = {
  async getAllTypes() {
    try {
      const response = await api.get('/UIOMatic/GetAll');
      return response.data;
    } catch (error) {
      console.error('Error fetching types:', error);
      throw error; // Let the interceptor handle the error
    }
  },

  async getItems(typeAlias, page = 1, itemsPerPage = config.listView.itemsPerPage, sortColumn = config.listView.defaultSortColumn, sortOrder = config.listView.defaultSortOrder, filters = {}, searchTerm = '') {
    try {
      console.log('Fetching items for type:', typeAlias);
      const response = await api.get('/Object/GetPaged', {
        params: {
          typeAlias,
          pageNumber: page,
          itemsPerPage,
          sortColumn: sortColumn || config.listView.defaultSortColumn,
          sortOrder: sortOrder || config.listView.defaultSortOrder,
          filters: Object.entries(filters).map(([key, value]) => `${key}|${value}`).join('|'),
          searchTerm: searchTerm || ''
        }
      });
      
      console.log('API Response:', response.data);
      
      // Ensure we're using the correct response structure
      const items = Array.isArray(response.data.items) ? response.data.items : [];
      const totalItems = response.data.totalItems || items.length;
      
      return {
        items,
        totalItems,
        currentPage: page,
        itemsPerPage,
        totalPages: Math.ceil(totalItems / itemsPerPage)
      };
    } catch (error) {
      console.error('Error fetching items:', error);
      throw error; // Let the interceptor handle the error
    }
  },

  async getItem(typeAlias, id) {
    try {
      const response = await api.get(`/Object/GetById`, {
        params: { typeAlias, id }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching item:', error);
      throw error; // Let the interceptor handle the error
    }
  },

  async getTypeInfo(typeAlias) {
    try {
      const response = await api.get(`/Object/GetTypeInfo`, {
        params: { typeAlias, includePropertyInfo: true }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching type info:', error);
      throw error; // Let the interceptor handle the error
    }
  },

  async createItem(typeAlias, data) {
    try {
      const response = await api.post('/Object/Create', {
        typeAlias,
        value: data
      });
      return response.data;
    } catch (error) {
      console.error('Error creating item:', error);
      throw error; // Let the interceptor handle the error
    }
  },

  async updateItem(typeAlias, data) {
    try {
      const response = await api.put('/Object/Update', {
        typeAlias,
        value: data
      });
      return response.data;
    } catch (error) {
      console.error('Error updating item:', error);
      throw error; // Let the interceptor handle the error
    }
  },

  async deleteItems(typeAlias, ids) {
    try {
      const response = await api.delete(`/Object/Delete`, {
        params: { typeAlias, ids: ids.join(',') }
      });
      return response.data;
    } catch (error) {
      console.error('Error deleting items:', error);
      throw error; // Let the interceptor handle the error
    }
  },

  async uploadImage(file) {
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await api.post('/Object/Upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error uploading image:', error);
      throw error; // Let the interceptor handle the error
    }
  }
}; 