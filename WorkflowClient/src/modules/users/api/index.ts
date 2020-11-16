import { AxiosPromise, AxiosResponse } from 'axios'
import qs from 'qs'
import api from '@/core/api'
import Credentials from '@/core/types/credentials.type'
import User from '@/modules/users/models/user.type'
import Query from '@/core/types/query.type'

export default {
  /**
   * Authentication
   * @param credentials - userName: string, password: string, rememberMe: boolean
   */
  login: (credentials: Credentials): AxiosPromise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Authentication/Login`,
      method: 'POST',
      data: credentials,
    })
  },

  /**
   * Logout
   */
  logout: (): AxiosPromise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Authentication/Logout`,
      method: 'POST',
    })
  },

  /**
   * Save token in the API client
   * @param token - string
   */
  setToken: (token: string): void => {
    api.setToken(token)
  },

  /**
   * Get user by ID
   * @param id - string (example: 72cdbd9d-4687-4774-a91a-9bcc71a8668e)
   */
  get: (id: string): Promise<AxiosResponse<User>> => {
    return api.request({
      url: `/api/Users/Get/${id}`,
      method: 'GET',
    })
  },

  /**
   * Get currently authorized user
   */
  getCurrent: (): AxiosPromise<AxiosResponse<User>> => {
    return api.request({
      url: `/api/Users/GetCurrent`,
      method: 'GET',
    })
  },

  /**
   * Get a list of users
   * @param query - {
   *   pageNumber: number,
   *   pageSize: number,
   *   filter: string,
   *   filterFields: Array<FilterField>,
   *   sortFields: Array<SortField>,
   *   withRemoved: boolean
   * }
   */
  getPage: (query: Query): Promise<AxiosResponse<User[]>> => {
    return api.request({
      url: `/api/Users/GetPage`,
      method: 'POST',
      data: query,
    })
  },

  /**
   * Get a list of users by specified IDs
   * @param ids - Array<string>
   */
  getRange: (ids: string[]): Promise<AxiosResponse<User[]>> => {
    return api.request({
      url: `/api/Users/GetRange?ids=${qs.stringify(ids)}`,
      method: 'GET',
    })
  },

  /**
   * Check whether the user with the specified name exists
   * @param email - string
   */
  isEmailExist: (email: string): Promise<AxiosResponse<boolean>> => {
    return api.request({
      url: `/api/Users/IsEmailExist/${email}?email=${email}`,
      method: 'GET',
    })
  },

  /**
   * Check whether the user with the specified username exists
   * @param userName - string
   */
  isUserNameExist: (userName: string): Promise<AxiosResponse<boolean>> => {
    return api.request({
      url: `/api/Users/IsUserNameExist/${userName}`,
      method: 'GET',
    })
  },

  /**
   * Create new user
   * @param user - User
   */
  create: (user: User): Promise<AxiosResponse<User>> => {
    return api.request({
      url: `/api/Users/Create`,
      method: 'POST',
      data: user,
    })
  },

  /**
   * Update existing user
   * @param user - User
   */
  update: (user: User): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/Update`,
      method: 'PUT',
      data: user,
    })
  },

  /**
   * Delete user by ID
   * @param id - string
   */
  remove: (id: string): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/Delete?id=${id}`,
      method: 'DELETE',
    })
  },

  /**
   * Delete multiple users by IDs
   * @param ids - Array<string>
   */
  removeRange: (ids: string[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/DeleteRange`,
      method: 'PATCH',
      data: ids,
    })
  },

  /**
   * Restore user by ID
   * @param id - string
   */
  restore: (id: string): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/Restore/${id}`,
      method: 'PATCH',
    })
  },

  /**
   * Restore multiple users by IDs
   * @param ids - Array<string>
   */
  restoreRange: (ids: string[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/RestoreRange`,
      method: 'PATCH',
      data: ids,
    })
  },

  /**
   * Change password for current user
   * @param currentPassword - string
   * @param newPassword - string
   */
  changePassword: (
    currentPassword: string,
    newPassword: string
  ): AxiosPromise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/ChangePassword?currentPassword=${currentPassword}&newPassword=${newPassword}`,
      method: 'PATCH',
    })
  },

  /**
   * Set new password for user with specified ID
   * @param id - string
   * @param newPassword - string
   */
  resetPassword: (id: string, newPassword: string): AxiosPromise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Users/ResetPassword/${id}?newPassword=${newPassword}`,
      method: 'PATCH',
    })
  },
}
