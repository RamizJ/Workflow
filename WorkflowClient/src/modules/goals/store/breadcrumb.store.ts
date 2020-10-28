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
  _breadcrumbs: { path: string; label: string }[] = []

  public get breadcrumbs(): { path: string; label: string }[] {
    return this._breadcrumbs
  }

  @Mutation
  async updateBreadcrumbs(): Promise<void> {
    const breadcrumbs = await breadcrumbsStorage.getBreadcrumbs('goals')
    if (breadcrumbs) this._breadcrumbs = breadcrumbs
    if (!this._breadcrumbs.length) this._breadcrumbs = [{ path: '/goals', label: 'Задачи' }]
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
    const breadcrumbs = this._breadcrumbs
    const alreadyExist = breadcrumbs.some((b) => b.path === breadcrumb.path)
    if (alreadyExist) return breadcrumbs
    else return [...breadcrumbs, breadcrumb]
  }

  @Mutation
  async removeBreadcrumb(path: string): Promise<void> {
    this._breadcrumbs = this._breadcrumbs.filter((breadcrumb) => breadcrumb.path !== path)
    await breadcrumbsStorage.setBreadcrumbs('goals', this._breadcrumbs)
  }
}

export default getModule(BreadcrumbStore)
