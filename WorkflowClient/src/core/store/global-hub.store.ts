import { getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators'
import store from '@/core/store/index'
import GlobalHub from '@/core/hubs/global-hub'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'globalHubStore',
  store,
})
class GlobalHubStore extends VuexModule {
  _globalHub: GlobalHub = new GlobalHub()

  public get isActive(): boolean {
    return this._globalHub.active
  }

  @Mutation
  async start(): Promise<void> {
    await this._globalHub.start()
  }

  @Mutation
  async stop(): Promise<void> {
    await this._globalHub.stop()
  }
}

export default getModule(GlobalHubStore)
