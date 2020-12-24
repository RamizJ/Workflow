import { AxiosPromise, AxiosResponse } from 'axios'
import qs from 'qs'
import api from '@/core/api'
import Credentials from '@/core/types/credentials.type'
import User from '@/modules/users/models/user.type'
import Query from '@/core/types/query.type'
import { Statistics } from '@/core/types/statistics.model'

export default {
  login: (credentials: Credentials): AxiosPromise<void> => {
    return api.request({
      url: `/api/Authentication/Login`,
      method: 'POST',
      data: credentials,
    })
  },
  logout: (): AxiosPromise<void> => {
    return api.request({
      url: `/api/Authentication/Logout`,
      method: 'POST',
    })
  },
  setToken: (token: string): void => {
    api.setToken(token)
  },
  get: (id: string): AxiosPromise<User> => {
    return api.request({
      url: `/api/Users/Get/${id}`,
      method: 'GET',
    })
  },
  getCurrent: (): AxiosPromise<User> => {
    return api.request({
      url: `/api/Users/GetCurrent`,
      method: 'GET',
    })
  },
  getPage: (query: Query): AxiosPromise<User[]> => {
    return api.request({
      url: `/api/Users/GetPage`,
      method: 'POST',
      data: query,
    })
  },
  getRange: (ids: string[]): AxiosPromise<User[]> => {
    return api.request({
      url: `/api/Users/GetRange?ids=${qs.stringify(ids)}`,
      method: 'GET',
    })
  },
  isEmailExist: (email: string): AxiosPromise<boolean> => {
    return api.request({
      url: `/api/Users/IsEmailExist/${email}?email=${email}`,
      method: 'GET',
    })
  },
  isUserNameExist: (userName: string): AxiosPromise<boolean> => {
    return api.request({
      url: `/api/Users/IsUserNameExist/${userName}`,
      method: 'GET',
    })
  },
  create: (user: User): AxiosPromise<User> => {
    return api.request({
      url: `/api/Users/Create`,
      method: 'POST',
      data: user,
    })
  },
  update: (user: User): AxiosPromise<void> => {
    return api.request({
      url: `/api/Users/Update`,
      method: 'PUT',
      data: user,
    })
  },
  remove: (id: string): AxiosPromise<void> => {
    return api.request({
      url: `/api/Users/Delete?id=${id}`,
      method: 'DELETE',
    })
  },
  removeRange: (ids: string[]): AxiosPromise<void> => {
    return api.request({
      url: `/api/Users/DeleteRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  restore: (id: string): AxiosPromise<void> => {
    return api.request({
      url: `/api/Users/Restore/${id}`,
      method: 'PATCH',
    })
  },
  restoreRange: (ids: string[]): AxiosPromise<void> => {
    return api.request({
      url: `/api/Users/RestoreRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  changePassword: (currentPassword: string, newPassword: string): AxiosPromise<void> => {
    return api.request({
      url: `/api/Users/ChangePassword?currentPassword=${currentPassword}&newPassword=${newPassword}`,
      method: 'PATCH',
    })
  },
  resetPassword: (id: string, newPassword: string): AxiosPromise<void> => {
    return api.request({
      url: `/api/Users/ResetPassword/${id}?newPassword=${newPassword}`,
      method: 'PATCH',
    })
  },
  getStatistics: (userId: string, dateBegin: string, dateEnd: string): AxiosPromise<Statistics> => {
    return api.request({
      url: `/api/Users/GetStatistic/${userId}`,
      method: 'POST',
      data: { dateBegin, dateEnd },
    })
  },
  getProjectStatistics: (
    userId: string,
    projectId: number,
    dateBegin: string,
    dateEnd: string
  ): AxiosPromise<Statistics> => {
    return api.request({
      url: `/api/Users/GetProjectStatistic/${userId}?projectId=${projectId}`,
      method: 'POST',
      data: { dateBegin, dateEnd },
    })
  },
}
