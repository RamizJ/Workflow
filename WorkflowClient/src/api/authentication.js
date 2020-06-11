import httpClient from './httpClient';

export default {
  login: (userName, password, rememberMe) =>
    httpClient.post(`/api/Authentication/Login`, {
      userName,
      password,
      rememberMe
    }),
  changePassword: (currentPassword, newPassword) =>
    httpClient.post(`/api/Users/ChangePassword`, {
      currentPassword,
      newPassword
    }),
  logout: () => httpClient.post(`/api/Authentication/Logout`),
  getMe: () => httpClient.get(`/api/Users/GetCurrent`)
};
