import { getModule, Module, MutationAction, VuexModule } from 'vuex-module-decorators'

import store from '@/core/store'
import tokenStorage from '@/core/storage/adapters/token'
import api from '../api'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'tokenStore',
  store: store,
})
class TokenStore extends VuexModule {
  _token: string | null = null

  public get token(): string | null {
    return this._token
  }

  @MutationAction({ mutate: ['_token'] })
  public async updateToken(token?: string) {
    if (token) {
      await tokenStorage.setToken(token)
      api.setToken(token)
      return { _token: token }
    } else {
      const localToken = await tokenStorage.getToken()
      api.setToken(localToken)
      return { _token: localToken }
    }
  }

  @MutationAction({ mutate: ['_token'] })
  public async removeToken() {
    await tokenStorage.setToken('')
    return { _token: null }
  }
}

export default getModule(TokenStore)
