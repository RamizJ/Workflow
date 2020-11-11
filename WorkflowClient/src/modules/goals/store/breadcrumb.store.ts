import {
  Action,
  getModule,
  Module,
  Mutation,
  MutationAction,
  VuexModule,
} from 'vuex-module-decorators'
import store from '@/core/store'
import breadcrumbsStorage from '@/core/storage/adapters/breadcrumbs'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'breadcrumbStore',
  store,
})
class BreadcrumbStore extends VuexModule {
  _rootBreadcrumb: { path: string; label: string } = { path: '/goals', label: 'Задачи' }
  _breadcrumbs: { path: string; label: string }[] = []

  public get breadcrumbs(): { path: string; label: string }[] {
    return this._breadcrumbs
  }

  @Mutation
  async setRootBreadcrumb(breadcrumb: { path: string; label: string }): Promise<void> {
    this._rootBreadcrumb = breadcrumb
  }

  @Mutation
  async updateBreadcrumbs(): Promise<void> {
    const breadcrumbs = await breadcrumbsStorage.getBreadcrumbs('goals')
    if (breadcrumbs) this._breadcrumbs = breadcrumbs
    if (!this._breadcrumbs.length) this._breadcrumbs = [this._rootBreadcrumb]
  }

  @Mutation
  async setBreadcrumbs(breadcrumbs: { path: string; label: string }[]): Promise<void> {
    this._breadcrumbs = breadcrumbs
    await breadcrumbsStorage.setBreadcrumbs('goals', this._breadcrumbs)
  }

  @Action({ commit: 'setBreadcrumbs' })
  async goto(path: string) {
    const breadcrumbs = this._breadcrumbs
    const breadcrumb = breadcrumbs.find((b) => b.path === path)
    if (breadcrumb) {
      const indexOfBreadcrumb = breadcrumbs.indexOf(breadcrumb)
      return breadcrumbs.slice(0, indexOfBreadcrumb + 1)
    } else return breadcrumbs
  }

  @Action({ commit: 'setBreadcrumbs' })
  async addBreadcrumb(breadcrumb: { path: string; label: string }) {
    const alreadyExist = this._breadcrumbs.some((b) => b.path === breadcrumb.path)
    if (alreadyExist) return this._breadcrumbs
    else return [...this._breadcrumbs, breadcrumb]
  }

  @Mutation
  async removeBreadcrumb(path: string): Promise<void> {
    this._breadcrumbs = this._breadcrumbs.filter((breadcrumb) => breadcrumb.path !== path)
    await breadcrumbsStorage.setBreadcrumbs('goals', this._breadcrumbs)
  }

  @Mutation
  async resetBreadcrumbs(): Promise<void> {
    this._breadcrumbs = [this._rootBreadcrumb]
    await breadcrumbsStorage.setBreadcrumbs('goals', this._breadcrumbs)
  }
}

export default getModule(BreadcrumbStore)
