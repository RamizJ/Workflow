import { VuexModule, Module, MutationAction, getModule } from 'vuex-module-decorators'

import store from '@/store'
import { Theme } from '@/types/theme.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'settingsModule',
  store: store,
})
class SettingsModule extends VuexModule {
  _opened = false
  _theme = localStorage.theme || Theme.SYSTEM
  _confirmDialogClose = localStorage.confirmDialogClose === 'true'
  _confirmDelete = localStorage.confirmDelete === 'true'
  _debugMode = localStorage.debugMode === 'true'

  public get isSettingsOpened(): boolean {
    return this._opened
  }
  public get theme(): Theme {
    return this._theme
  }
  public get confirmDialogClose(): boolean {
    return this._confirmDialogClose
  }
  public get confirmDelete(): boolean {
    return this._confirmDelete
  }
  public get debugMode(): boolean {
    return this._debugMode
  }

  @MutationAction({ mutate: ['_opened'] })
  public async closeSettings() {
    return {
      _opened: false,
    }
  }

  @MutationAction({ mutate: ['_opened'] })
  public async openSettings() {
    return {
      _opened: true,
    }
  }

  @MutationAction({ mutate: ['_theme'] })
  public async setTheme(theme: Theme) {
    localStorage.theme = theme
    document.documentElement.setAttribute('theme', theme)
    return {
      _theme: theme,
    }
  }

  @MutationAction({ mutate: ['_confirmDialogClose'] })
  public async setConfirmDialogClose(state: boolean) {
    localStorage.confirmDialogClose = state
    return {
      _confirmDialogClose: state,
    }
  }

  @MutationAction({ mutate: ['_confirmDelete'] })
  public async setConfirmDelete(state: boolean) {
    localStorage.confirmDelete = state
    return {
      _confirmDelete: state,
    }
  }

  @MutationAction({ mutate: ['_debugMode'] })
  public async setDebugMode(state: boolean) {
    localStorage.debugMode = state
    return {
      _debugMode: state,
    }
  }
}

export default getModule(SettingsModule)
