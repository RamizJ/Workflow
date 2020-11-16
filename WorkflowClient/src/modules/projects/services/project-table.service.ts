import { StateChanger } from 'vue-infinite-loading'
import TableService from '@/core/services/table.service'
import tableStore from '@/core/store/table.store'
import projectsStore from '../store/projects.store'
import teamsStore from '@/modules/teams/store/teams.store'
import Project from '@/modules/projects/models/project.type'
import { router } from '@/core'

export default class ProjectTableService extends TableService {
  public async load(tableLoader: StateChanger): Promise<void> {
    let fetchMethod
    if (this.teamId) fetchMethod = teamsStore.findProjects
    else fetchMethod = projectsStore.findAll
    await this.loadData(tableLoader, fetchMethod)
  }

  public async openRow(row: Project): Promise<void> {
    row = row || (tableStore.selectedRow as Project)
    if (!row.isRemoved && !this.teamId) await router.push(`/projects/${row.id}`)
  }

  public async createRow(): Promise<void> {
    projectsStore.setProject(null)
    await projectsStore.openProjectWindow()
  }

  public async editRow(): Promise<void> {
    if (!tableStore.selectedRow) return
    await projectsStore.openProjectWindow(tableStore.selectedRow as Project)
  }

  public async deleteRows(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await projectsStore.deleteMany(ids)
    tableStore.requireReload()
  }

  public async restoreRows(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await projectsStore.restoreMany(ids)
    tableStore.requireReload()
  }
}
