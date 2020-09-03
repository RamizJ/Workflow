import qs from 'qs'
import http from './http'
import Query from '@/types/query.type'
import User from '@/types/user.type'

export default {
  findAll: (query: Query) => http.post(`/api/Users/GetPage`, query),
  findAllByIds: (ids: string[]) => http.get(`/api/Users/GetRange?${qs.stringify(ids)}`),
  findOneById: (id: string) => http.get(`/api/Users/Get/${id}`),
  createOne: (entity: User) => http.post(`/api/Users/Create`, entity),
  updateOne: (entity: User) => http.put(`/api/Users/Update`, entity),
  updateMany: (entities: User[]) => http.put(`/api/Users/UpdateRange`, entities),
  deleteOne: (id: string) => http.delete(`/api/Users/Delete?id=${id}`),
  deleteMany: (ids: string[]) => http.patch(`/api/Users/DeleteRange`, ids),
  restoreOne: (id: string) => http.patch(`/api/Users/Restore/${id}`),
  restoreMany: (ids: string[]) => http.patch(`/api/Users/RestoreRange`, ids),
  resetPassword: (userId: string, newPassword: string) =>
    http.patch(`/api/Users/ResetPassword/${userId}?newPassword=${newPassword}`),
  isEmailExist: (email: string) => http.get(`/api/Users/IsEmailExist/${email}?email=${email}`),
  isUserNameExist: (userName: string) => http.get(`/api/Users/IsUserNameExist/${userName}`)
}
