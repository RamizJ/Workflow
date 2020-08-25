import httpClient from './httpClient';

export default {
  login: (userName, password, rememberMe) =>
    httpClient.post(`/api/Authentication/Login`, {
      userName,
      password,
      rememberMe
    }),
  logout: () => httpClient.post(`/api/Authentication/Logout`),
  changePassword: (currentPassword, newPassword) =>
    httpClient.post(`/api/Users/ChangePassword`, {
      currentPassword,
      newPassword
    }),
  getMe: () => httpClient.get(`/api/Users/GetCurrent`)
};
