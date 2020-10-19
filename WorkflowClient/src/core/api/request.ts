import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios'
import config from './config'

const request: AxiosInstance = axios.create({
  baseURL: config.baseURL,
  headers: config.headers,
})

const requestHandler = (requestConfig: AxiosRequestConfig) => {
  requestConfig.baseURL = requestConfig.baseURL || config.baseURL
  requestConfig.headers.authorization = config.headers.authorization
  return requestConfig
}

const responseHandler = (response: AxiosResponse) => {
  return response
}

const errorHandler = (error: AxiosError) => {
  return Promise.reject(error)
}

request.interceptors.request.use(requestHandler)
request.interceptors.response.use(responseHandler, errorHandler)

export default request
