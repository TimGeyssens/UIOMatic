import axios from 'axios';

const API_ROOT = import.meta.env.VITE_API_ROOT || 'https://localhost:7170';

export const AuthService = {
  async login(username, password) {
    try {
      const response = await axios.post(`${API_ROOT}/auth/login`, {
        username,
        password
      });
      
      if (response.data.token) {
        localStorage.setItem('token', response.data.token);
        this.setAuthHeader(response.data.token);
      }
      
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  logout() {
    localStorage.removeItem('token');
    delete axios.defaults.headers.common['Authorization'];
  },

  getToken() {
    return localStorage.getItem('token');
  },

  setAuthHeader(token) {
    if (token) {
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    } else {
      delete axios.defaults.headers.common['Authorization'];
    }
  },

  isAuthenticated() {
    const token = this.getToken();
    if (!token) return false;

    try {
      // Check if token is expired
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000; // Convert to milliseconds
      const currentTime = Date.now();
      
      // If token is about to expire in the next 5 minutes, refresh it
      if (expirationTime - currentTime < 5 * 60 * 1000) {
        this.refreshToken();
      }
      
      return expirationTime > currentTime;
    } catch (error) {
      console.error('Error checking token:', error);
      return false;
    }
  },

  async refreshToken() {
    try {
      const response = await axios.post(`${API_ROOT}/auth/refresh`, {}, {
        headers: {
          'Authorization': `Bearer ${this.getToken()}`
        }
      });
      
      if (response.data.token) {
        localStorage.setItem('token', response.data.token);
        this.setAuthHeader(response.data.token);
      }
    } catch (error) {
      console.error('Error refreshing token:', error);
      this.logout();
    }
  }
}; 