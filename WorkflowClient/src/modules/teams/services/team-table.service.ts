import { StateChanger } from 'vue-infinite-loading'
import TableService from '@/core/services/table.service'
import tableStore from '@/core/store/table.store'
import teamsStore from '../store/teams.store'
import projectsStore from '@/modules/projects/store/projects.store'
import Team from '@/modules/projects/models/project.type'
import { router } from '@/core'

export default class TeamTableService extends TableService {
  public async load(tableLoader: StateChanger): Promise<void> {
    let fetchMethod
    if (this.projectId) fetchMethod = projectsStore.findTeams
    else fetchMethod = teamsStore.findAll
    await this.loadData(tableLoader, fetchMethod)
  }

  public async openEntity(row: Team): Promise<void> {
    row = row || (tableStore.selectedRow as Team)
    if (!row.isRemoved && !this.projectId) await router.push(`/teams/${row.id}`)
  }

  public async createEntity(): Promise<void> {
    teamsStore.setTeam(null)
    await teamsStore.openTeamWindow()
  }

  public async editEntity(): Promise<void> {
    if (!tableStore.selectedRow) return
    await teamsStore.openTeamWindow(tableStore.selectedRow as Team)
  }

  public async deleteEntities(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await teamsStore.deleteMany(ids)
    tableStore.requireReload()
  }

  public async deleteEntitiesFromProject(): Promise<void> {
    const projectId = this.projectId
    const teamIds = this.selectedIds as Array<number>
    await projectsStore.removeTeams({ projectId, teamIds })
    tableStore.requireReload()
  }

  public async restoreEntities(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await teamsStore.restoreMany(ids)
    tableStore.requireReload()
  }
}
