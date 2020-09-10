import { VuexModule, Module, Action, MutationAction, getModule } from 'vuex-module-decorators'
import { AxiosResponse } from 'axios'

import store from '@/store'
import authAPI from '@/api/auth.api'
import Credentials from '@/types/credentials.type'
import User from '@/types/user.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'authModule',
  store: store,
})
class AuthModule extends VuexModule {
  _user: User | null = null
  _token: string | null = localStorage.getItem('workflow_access_token') || null

  public get me(): User | null {
    return this._user
  }
  public get token(): string | null {
    return this._token
  }
  public get loggedIn(): boolean {
    return !!this._user && !!this._token
  }

  @MutationAction({ mutate: ['_user', '_token'] })
  public async login(credentials: Credentials) {
    const response: AxiosResponse = await authAPI.login(credentials)
    const user: User = response.data.user as User
    const token: string = response.data.token as string
    localStorage.setItem('workflow_access_token', token)
    return {
      _user: user,
      _token: token,
    }
  }

  @MutationAction({ mutate: ['_user', '_token'] })
  public async logout() {
    await authAPI.logout()
    localStorage.removeItem('workflow_access_token')
    return {
      _user: null,
      _token: null,
    }
  }

  @MutationAction({ mutate: ['_user', '_token'] })
  public async getMe() {
    const response: AxiosResponse = await authAPI.getMe()
    if (!response || response.status === 401)
      return {
        _user: null,
        _token: null,
      }
    else {
      return {
        _user: response.data as User,
        _token: localStorage.getItem('workflow_access_token') || this._token,
      }
    }
  }

  @Action
  public async changePassword({
    currentPassword,
    newPassword,
  }: {
    currentPassword: string
    newPassword: string
  }): Promise<void> {
    await authAPI.changePassword(currentPassword, newPassword)
  }
}

export default getModule(AuthModule)
