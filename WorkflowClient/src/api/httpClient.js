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
  const url = error.config.url;
  if (error.response) {
    const data = error.response?.data;
    const statusCode = error.response?.status;
    switch (statusCode) {
      case 400:
        Message({
          message: `Синтаксическая ошибка в отправляемых данных`,
          type: 'error',
          duration: 5000
        });
        console.error(`Синтаксическая ошибка в запросе к «${url}»`);
        break;
      case 401:
        Message({
          message: `Неверные учётные данные`,
          type: 'error',
          duration: 5000
        });
        console.error(
          `Для выполнения запроса к «${url}» требуется аутентификация`
        );
        break;
      case 403:
        Message({
          message: `Попытка доступа к запрещённному содержимому`,
          type: 'error',
          duration: 5000
        });
        console.error(`Доступ к «${url}» запрещён`);
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
  } else {
    // Message({
    //   message: `Неизвестная ошибка при запросе «${url}»`,
    //   type: 'error',
    //   duration: 5000
    // });
    console.error(`Неизвестная ошибка при запросе «${url}»`);
  }
};

httpClient.interceptors.request.use(authInterceptor);
httpClient.interceptors.response.use(
  response => response,
  errorResponseHandler
);

export default httpClient;
