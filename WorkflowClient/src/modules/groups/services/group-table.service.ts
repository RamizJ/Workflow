import { StateChanger } from 'vue-infinite-loading'
import TableService from '@/core/services/table.service'
import tableStore from '@/core/store/table.store'
import groupsStore from '@/modules/groups/store/groups.store'
import Group from '@/modules/groups/models/group.model'

export default class GroupTableService extends TableService {
  public async load(tableLoader: StateChanger): Promise<void> {
    await this.loadData(tableLoader, groupsStore.getPage)
  }

  public async createEntity(): Promise<void> {
    groupsStore.setGroup(null)
    await groupsStore.openGroupWindow()
  }

  public async editEntity(row?: Group): Promise<void> {
    const entity = row || (tableStore.selectedRow as Group)
    if (!entity) return
    groupsStore.setGroup(entity)
    await groupsStore.openGroupWindow(entity)
  }

  public async deleteEntities(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await groupsStore.removeRange(ids)
    tableStore.requireReload()
  }

  public async restoreEntities(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await groupsStore.restoreRange(ids)
    tableStore.requireReload()
  }
}
