export type Config = {
  baseURL?: string
  headers: { [key: string]: string }
}

const config: Config = {
  baseURL: process.env.VUE_APP_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
}

export const baseUrl = config.baseURL

export const setToken = (token: string): void => {
  config.headers = { ...config.headers, authorization: `Bearer ${token}` }
}

export const setBaseURL = (baseURL: string): void => {
  config.baseURL = baseURL
}

export const setHeaders = (headers: { [key: string]: string }): void => {
  config.headers = { ...headers, authorization: config.headers.authorization }
}

export default config
