import { AxiosPromise, AxiosResponse } from 'axios'
import qs from 'qs'

import api from '@/core/api'
import User from '@/modules/users/models/user.type'
import Query from '@/core/types/query.type'

export default {
  get: (id: string): Promise<AxiosResponse<User>> => {
    return api.request({
      url: `/api/Users/Get/${id}`,
      method: 'GET',
    })
  },
  getCurrent: (): AxiosPromise<AxiosResponse<User>> => {
    return api.request({
      url: `/api/Users/GetCurrent`,
      method: 'GET',
    })
  },
  getPage: (query: Query): Promise<AxiosResponse<User[]>> => {
    return api.request({
      url: `/api/Users/GetPage`,
      method: 'POST',
      data: query,
    })
  },
  getRange: (ids: string[]): Promise<AxiosResponse<User[]>> => {
    return api.request({
      url: `/api/Users/GetRange?ids=${qs.stringify(ids)}`,
      method: 'GET',
    })
  },
  isEmailExist: (email: string): Promise<AxiosResponse<boolean>> => {
    return api.request({
      url: `/api/Users/IsEmailExist/${email}?email=${email}`,
      method: 'GET',
    })
  },
  isUserNameExist: (userName: string): Promise<AxiosResponse<boolean>> => {
    return api.request({
      url: `/api/Users/IsUserNameExist/${userName}`,
      method: 'GET',
    })
  },
  create: (user: User): Promise<AxiosResponse<User>> => {
    return api.request({
      url: `/api/Users/Create`,
      method: 'POST',
      data: user,
    })
  },
  update: (user: User): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/Update`,
      method: 'PUT',
      data: user,
    })
  },
  remove: (id: string): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/Delete?id=${id}`,
      method: 'DELETE',
    })
  },
  removeRange: (ids: string[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/DeleteRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  restore: (id: string): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/Restore/${id}`,
      method: 'PATCH',
    })
  },
  restoreRange: (ids: string[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/RestoreRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  changePassword: (
    currentPassword: string,
    newPassword: string
  ): AxiosPromise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/ChangePassword?currentPassword=${currentPassword}&newPassword=${newPassword}`,
      method: 'PATCH',
    })
  },
  resetPassword: (id: string, newPassword: string): AxiosPromise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/ResetPassword/${id}?newPassword=${newPassword}`,
      method: 'PATCH',
    })
  },
}
