import qs from 'qs'
import httpClient from './httpClient'
import Query from '@/types/query.type'
import User from '@/types/user.type'

export default {
  findOneById: (id: string) => httpClient.get(`/api/Users/Get/${id}`),
  findAll: (query: Query) => httpClient.post(`/api/Users/GetPage`, query),
  findAllByIds: (ids: string[]) => httpClient.get(`/api/Users/GetRange?${qs.stringify(ids)}`),
  createOne: (entity: User) => httpClient.post(`/api/Users/Create`, entity),
  updateOne: (entity: User) => httpClient.put(`/api/Users/Update`, entity),
  updateMany: (entities: User[]) => httpClient.put(`/api/Users/UpdateRange`, entities),
  deleteOne: (id: string) => httpClient.delete(`/api/Users/Delete?id=${id}`),
  deleteMany: (ids: string[]) => httpClient.patch(`/api/Users/DeleteRange`, ids),
  restoreOne: (id: string) => httpClient.patch(`/api/Users/Restore/${id}`),
  restoreMany: (ids: string[]) => httpClient.patch(`/api/Users/RestoreRange`, ids),
  resetPassword: (userId: string, newPassword: string) =>
    httpClient.patch(`/api/Users/ResetPassword/${userId}?newPassword=${newPassword}`),
  isEmailExist: (email: string) =>
    httpClient.get(`/api/Users/IsEmailExist/${email}?email=${email}`),
  isUserNameExist: (userName: string) => httpClient.get(`/api/Users/IsUserNameExist/${userName}`)
}
