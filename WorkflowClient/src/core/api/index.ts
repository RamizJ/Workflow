import request from './request'
import { setToken, setBaseURL, setHeaders, baseUrl } from './config'
import { AxiosPromise, AxiosRequestConfig } from 'axios'

interface API {
  request: (config: AxiosRequestConfig) => AxiosPromise
  setToken: (token: string) => void
  setBaseURL: (baseURL: string) => void
  setHeaders: (headers: { [key: string]: string }) => void
  baseUrl?: string
}

const api: API = {
  request,
  setToken,
  setBaseURL,
  setHeaders,
  baseUrl,
}

export default api
