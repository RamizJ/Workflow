import httpClient from './httpClient';
import Credentials from '@/types/credentials.type';
import ChangePassword from '@/types/change-password.type';

export default {
  login: (credentials: Credentials) =>
    httpClient.post(`/api/Authentication/Login`, credentials),
  logout: () => httpClient.post(`/api/Authentication/Logout`),
  changePassword: (changePasswordData: ChangePassword) =>
    httpClient.post(`/api/Users/ChangePassword`, changePasswordData),
  getMe: () => httpClient.get(`/api/Users/GetCurrent`)
};
