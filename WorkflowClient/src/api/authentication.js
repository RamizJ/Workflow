import httpClient from './httpClient';

export default {
  login: (userName, password, rememberMe) =>
    httpClient.post(`/api/Authentication/Login`, {
      userName,
      password,
      rememberMe
    }),
  logout: () => httpClient.post(`/api/Authentication/Logout`),
  getMe: () => httpClient.get(`/api/Users/GetCurrentUser`)
};
