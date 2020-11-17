import { StateChanger } from 'vue-infinite-loading'
import TableService from '@/core/services/table.service'
import tableStore from '@/core/store/table.store'
import teamsStore from '@/modules/teams/store/teams.store'
import usersStore from '../store/users.store'
import User from '../models/user.type'
import Entity from '@/core/types/entity.type'

export default class UserTableService extends TableService {
  public async load(tableLoader: StateChanger): Promise<void> {
    let fetchMethod
    if (this.teamId) fetchMethod = teamsStore.findUsers
    else fetchMethod = usersStore.findAll
    await this.loadData(tableLoader, fetchMethod)
  }

  public async createEntity(): Promise<void> {
    usersStore.setUser(null)
    await usersStore.openUserWindow()
  }

  public async editEntity(row?: Entity): Promise<void> {
    const user = row ? (row as User) : (tableStore.selectedRow as User)
    await usersStore.openUserWindow(user)
  }

  public async deleteEntities(): Promise<void> {
    const ids = this.selectedIds as Array<string>
    await usersStore.deleteMany(ids)
    tableStore.requireReload()
  }

  public async deleteEntitiesFromTeam(): Promise<void> {
    const teamId = this.teamId
    const userIds = this.selectedIds as Array<string>
    await teamsStore.removeUsers({ teamId, userIds })
    tableStore.requireReload()
  }

  public async restoreEntities(): Promise<void> {
    const ids = this.selectedIds as Array<string>
    await usersStore.restoreMany(ids)
    tableStore.requireReload()
  }
}
