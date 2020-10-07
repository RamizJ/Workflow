import { AxiosResponse } from 'axios'
import http from './http'
import Credentials from '@/types/credentials.type'
import User from '@/types/user.type'

export default {
  login(credentials: Credentials): Promise<AxiosResponse<void>> {
    return http.post<void>(`/api/Authentication/Login`, credentials)
  },
  logout(): Promise<AxiosResponse<void>> {
    return http.post<void>(`/api/Authentication/Logout`)
  },
  changePassword(currentPassword: string, newPassword: string): Promise<AxiosResponse<void>> {
    return http.post<void>(`/api/Users/ChangePassword`, { currentPassword, newPassword })
  },
  getMe(): Promise<AxiosResponse<User>> {
    return http.get(`/api/Users/GetCurrent`)
  },
}
