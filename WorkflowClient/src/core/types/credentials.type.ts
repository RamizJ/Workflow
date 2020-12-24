export default class Credentials {
  userName?: string
  password?: string
  rememberMe = true

  constructor(userName: string, password: string, rememberMe = true) {
    this.userName = userName
    this.password = password
    this.rememberMe = rememberMe
  }
}
