import axios from 'axios';
import { Message } from 'element-ui';

const getToken = () => localStorage.getItem('access_token');

const httpClient = axios.create({
  baseURL: process.env.VUE_APP_API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

const authInterceptor = config => {
  config.headers['Authorization'] = `Bearer ${getToken()}`;
  return config;
};

const errorResponseHandler = error => {
  const isDebugMode = localStorage.debugMode === 'true';
  if (!isDebugMode)
    Message({
      message: `Ошибка отправки запроса`,
      type: 'error',
      duration: 5000
    });
  else {
    const url = error.config.url;
    if (error.response) {
      const data = error.response?.data;
      const statusCode = error.response?.status;
      switch (statusCode) {
        case 400:
          Message({
            message: `Синтаксическая ошибка в запросе к «${url}»`,
            type: 'error',
            duration: 5000
          });
          break;
        case 401:
          Message({
            message: `Для выполнения запроса к «${url}» требуется аутентификация`,
            type: 'error',
            duration: 5000
          });
          break;
        case 403:
          Message({
            message: `Доступ к «${url}» запрещён`,
            type: 'error',
            duration: 5000
          });
          break;
        case 404:
          Message({
            message: `Запрашиваемый ресурс «${url}» не найден`,
            type: 'error',
            duration: 5000
          });
          break;
        case 405:
          Message({
            message: `API метод по адресу «${url}» неактивен`,
            type: 'error',
            duration: 5000
          });
          break;
        default:
          Message({
            message: `Неизвестная ошибка (${statusCode})`,
            type: 'error',
            duration: 5000
          });
          break;
      }
      console.error(data);
    } else {
      Message({
        message: `Неизвестная ошибка при запросе «${url}»`,
        type: 'error',
        duration: 5000
      });
      console.error(error);
    }
  }
};

httpClient.interceptors.request.use(authInterceptor);
httpClient.interceptors.response.use(
  response => response,
  errorResponseHandler
);

export default httpClient;
