import http from './http'
import Credentials from '@/types/credentials.type'

export default {
  login: (credentials: Credentials) => http.post(`/api/Authentication/Login`, credentials),
  logout: () => http.post(`/api/Authentication/Logout`),
  changePassword: (currentPassword: string, newPassword: string) =>
    http.post(`/api/Users/ChangePassword`, { currentPassword, newPassword }),
  getMe: () => http.get(`/api/Users/GetCurrent`)
}
