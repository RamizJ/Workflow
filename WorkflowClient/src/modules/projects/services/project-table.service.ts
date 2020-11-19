import { StateChanger } from 'vue-infinite-loading'
import TableService from '@/core/services/table.service'
import tableStore from '@/core/store/table.store'
import projectsStore from '../store/projects.store'
import teamsStore from '@/modules/teams/store/teams.store'
import Project from '@/modules/projects/models/project.type'
import { router } from '@/core'
import Query from '@/core/types/query.type'
import groupsStore from '@/modules/groups/store/groups.store'

export default class ProjectTableService extends TableService {
  public async load(tableLoader: StateChanger): Promise<void> {
    let fetchMethod
    if (this.teamId) fetchMethod = teamsStore.findProjects
    else if (this.groupId)
      fetchMethod = async () => {
        const group = await groupsStore.get(this.groupId)
        tableLoader.complete()
        return group.projects
      }
    else fetchMethod = projectsStore.findAll
    await this.loadData(tableLoader, fetchMethod)
  }

  public async openEntity(row: Project): Promise<void> {
    row = row || (tableStore.selectedRow as Project)
    if (!row.isRemoved && !this.teamId) await router.push(`/projects/${row.id}`)
  }

  public async createEntity(): Promise<void> {
    projectsStore.setProject(null)
    await projectsStore.openProjectWindow()
  }

  public async editEntity(): Promise<void> {
    if (!tableStore.selectedRow) return
    await projectsStore.openProjectWindow(tableStore.selectedRow as Project)
  }

  public async deleteEntities(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await projectsStore.deleteMany(ids)
    tableStore.requireReload()
  }

  public async deleteEntitiesFromGroup(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await groupsStore.deleteProjects({ groupId: this.groupId, projectIds: ids })
    tableStore.requireReload()
  }

  public async restoreEntities(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await projectsStore.restoreMany(ids)
    tableStore.requireReload()
  }
}
