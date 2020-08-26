import httpClient from './httpClient'
import Credentials from '@/types/credentials.type'

export default {
  login: (credentials: Credentials) => httpClient.post(`/api/Authentication/Login`, credentials),
  logout: () => httpClient.post(`/api/Authentication/Logout`),
  changePassword: (currentPassword: string, newPassword: string) =>
    httpClient.post(`/api/Users/ChangePassword`, { currentPassword, newPassword }),
  getMe: () => httpClient.get(`/api/Users/GetCurrent`)
}
