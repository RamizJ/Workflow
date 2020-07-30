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
  if (error.response)
    Message({
      message: `Ошибка отправки запроса`,
      type: 'error',
      duration: 5000
    });
  if (isDebugMode) pushErrorMessage(error);
};

const pushErrorMessage = error => {
  const url = error.config.url;
  const data = error.response?.data;
  const statusCode = error.response?.status;

  console.error(
    `Ошибка ${statusCode || ''} во время выполнения запроса по адресу «${url}»`
  );
  if (data) console.log(data);

  if (!statusCode)
    Message({
      message: `Неизвестная ошибка при запросе к «${url}»`,
      type: 'error',
      duration: 5000
    });

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
      break;
  }
};

httpClient.interceptors.request.use(authInterceptor);
httpClient.interceptors.response.use(
  response => response,
  errorResponseHandler
);

export default httpClient;
