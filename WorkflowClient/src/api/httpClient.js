import axios from 'axios';

const httpClient = axios.create({
  baseURL: process.env.VUE_APP_API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

const getToken = () => localStorage.getItem('access_token');
const authInterceptor = config => {
  config.headers['Authorization'] = `Bearer ${getToken()}`;
  return config;
};

httpClient.interceptors.request.use(authInterceptor);

export default httpClient;