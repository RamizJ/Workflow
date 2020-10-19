import storage from '../storage'

export interface TokenStorage {
  setToken(token: string): Promise<void>
  getToken(): Promise<string>
}

const tokenStorage: TokenStorage = {
  async setToken(token: string): Promise<void> {
    return storage.setItem('workflow_access_token', token)
  },
  async getToken(): Promise<string> {
    return storage.getItem('workflow_access_token')
  },
}

export default tokenStorage
