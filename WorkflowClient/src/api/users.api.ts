import qs from 'qs'
import http from './http'
import Query from '@/types/query.type'
import User from '@/types/user.type'
import { AxiosResponse } from 'axios'

export default {
  findAll(query: Query): Promise<AxiosResponse<User[]>> {
    return http.post(`/api/Users/GetPage`, query)
  },
  findAllByIds(ids: string[]): Promise<AxiosResponse<User[]>> {
    return http.get(`/api/Users/GetRange?${qs.stringify(ids)}`)
  },
  findOneById(id: string): Promise<AxiosResponse<User>> {
    return http.get(`/api/Users/Get/${id}`)
  },
  createOne(entity: User): Promise<AxiosResponse<User>> {
    return http.post(`/api/Users/Create`, entity)
  },
  updateOne(entity: User): Promise<AxiosResponse<void>> {
    return http.put(`/api/Users/Update`, entity)
  },
  updateMany(entities: User[]): Promise<AxiosResponse<void>> {
    return http.put(`/api/Users/UpdateRange`, entities)
  },
  deleteOne(id: string): Promise<AxiosResponse<void>> {
    return http.delete(`/api/Users/Delete?id=${id}`)
  },
  deleteMany(ids: string[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Users/DeleteRange`, ids)
  },
  restoreOne(id: string): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Users/Restore/${id}`)
  },
  restoreMany(ids: string[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Users/RestoreRange`, ids)
  },
  resetPassword(userId: string, newPassword: string): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Users/ResetPassword/${userId}?newPassword=${newPassword}`)
  },
  isEmailExist(email: string): Promise<AxiosResponse<boolean>> {
    return http.get(`/api/Users/IsEmailExist/${email}?email=${email}`)
  },
  isUserNameExist(userName: string): Promise<AxiosResponse<boolean>> {
    return http.get(`/api/Users/IsUserNameExist/${userName}`)
  },
}
