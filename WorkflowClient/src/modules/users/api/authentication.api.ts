import { AxiosPromise, AxiosResponse } from 'axios'
import api from '@/core/api'
import Credentials from '@/core/types/credentials.type'

export default {
  login: (credentials: Credentials): AxiosPromise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Authentication/Login`,
      method: 'POST',
      data: credentials,
    })
  },
  logout: (): AxiosPromise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Authentication/Logout`,
      method: 'POST',
    })
  },
  setToken: (token: string): void => {
    api.setToken(token)
  },
}
