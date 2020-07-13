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
  const url = error.response.config.url;
  const data = error.response.data;
  const statusCode = error.response.status;
  switch (statusCode) {
    case 400:
      Message({
        message: `Синтаксическая ошибка в запросе «${url}»`,
        type: 'error',
        duration: 5000
      });
      break;
    case 401:
      Message({
        message: `Для выполнения запроса требуется аутентификация «${url}»`,
        type: 'error',
        duration: 5000
      });
      break;
    case 403:
      Message({
        message: `Доступ к содержимому запрещён «${url}»`,
        type: 'error',
        duration: 5000
      });
      break;
    case 404:
      Message({
        message: `Запрашиваемый ресурс не найден «${url}»`,
        type: 'error',
        duration: 5000
      });
      break;
    case 405:
      Message({
        message: `API метод неактивен «${url}»`,
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
  if (data)
    Message({
      message: `${data}`,
      type: 'error',
      duration: 6000,
      showClose: true,
      offset: 55
    });
};

httpClient.interceptors.request.use(authInterceptor);
httpClient.interceptors.response.use(
  response => response,
  errorResponseHandler
);

export default httpClient;
