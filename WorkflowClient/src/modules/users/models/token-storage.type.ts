export interface TokenStorage {
  setToken(token: string): Promise<void>
  getToken(): Promise<string>
}
