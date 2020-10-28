import storage from '../storage'

export interface BreadcrumbsStorage {
  setBreadcrumbs(name: string, breadcrumbs: { path: string; label: string }[]): Promise<void>
  getBreadcrumbs(name: string): Promise<{ path: string; label: string }[] | undefined>
}

const breadcrumbsStorage: BreadcrumbsStorage = {
  async setBreadcrumbs(
    name: string,
    breadcrumbs: { path: string; label: string }[]
  ): Promise<void> {
    return storage.setItem(`${name}_breadcrumbs`, JSON.stringify([...breadcrumbs]))
  },
  async getBreadcrumbs(name: string): Promise<{ path: string; label: string }[] | undefined> {
    const breadcrumbs = await storage.getItem(`${name}_breadcrumbs`)
    if (!breadcrumbs) return
    else return JSON.parse(breadcrumbs)
  },
}

export default breadcrumbsStorage
