import { getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators'
import store from '@/core/store'

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
  setBreadcrumbs(breadcrumbs: { path: string; label: string }[]) {
    this._breadcrumbs = breadcrumbs
  }

  @Mutation
  addBreadcrumb(breadcrumb: { path: string; label: string }) {
    const alreadyExist = this._breadcrumbs.find((b) => b.path === breadcrumb.path)
    if (alreadyExist) {
      this._breadcrumbs = this._breadcrumbs.map((b) => {
        if (b.path === breadcrumb.path) b.label = breadcrumb.label
        return b
      })
    } else {
      this._breadcrumbs = [...this._breadcrumbs, { ...breadcrumb }]
    }
  }

  @Mutation
  removeBreadcrumb(path: string) {
    console.log(`Breadcrumb removed: ${path}`)
    this._breadcrumbs = this._breadcrumbs.filter((breadcrumb) => breadcrumb.path !== path)
  }
}

export default getModule(BreadcrumbStore)
