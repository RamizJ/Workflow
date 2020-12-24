import { VuexModule, Module, Action, MutationAction, getModule } from 'vuex-module-decorators'
import { AxiosResponse } from 'axios'

import store from '@/core/store'
import api from '../api'
import tokenModule from '@/modules/users/store/token.store'
import Credentials from '@/core/types/credentials.type'
import User from '@/modules/users/models/user.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'authStore',
  store: store,
})
class AuthStore extends VuexModule {
  _user: User | null = null

  public get me(): User | null {
    return this._user
  }
  public get isLogged(): boolean {
    return !!this._user && !!tokenModule.token
  }

  @MutationAction({ mutate: ['_user'] })
  public async login(credentials: Credentials) {
    const response: AxiosResponse = await api.login(credentials)
    const user: User = response.data.user as User
    const token: string = response.data.token as string
    await tokenModule.updateToken(token)
    return { _user: user }
  }

  @MutationAction({ mutate: ['_user'] })
  public async logout() {
    await api.logout()
    await tokenModule.removeToken()
    return { _user: null }
  }

  @MutationAction({ mutate: ['_user'] })
  public async updateMe() {
    try {
      await tokenModule.updateToken()
      const response = await api.getCurrent()
      return { _user: response.data as User }
    } catch (e) {
      console.error(`Can't update current user`)
      return { _user: null }
    }
  }

  @Action
  public async changePassword({
    currentPassword,
    newPassword,
  }: {
    currentPassword: string
    newPassword: string
  }): Promise<boolean> {
    try {
      const response = await api.changePassword(currentPassword, newPassword)
      return !!response
    } catch (e) {
      console.error(`Can't change password`)
      return false
    }
  }
}

export default getModule(AuthStore)
