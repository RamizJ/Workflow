import { StateChanger } from 'vue-infinite-loading'
import TableService from '@/core/services/table.service'
import tableStore from '@/core/store/table.store'
import goalsStore from '@/modules/goals/store/goals.store'
import breadcrumbStore from '@/modules/goals/store/breadcrumb.store'
import Goal, { Status } from '@/modules/goals/models/goal.type'
import { router } from '@/core'

export default class GoalTableService extends TableService {
  public async initialize(): Promise<void> {
    if (!this.goalId) await this.resetBreadcrumbs()
    await this.updateBreadcrumbs()
    if (this.projectId) this.extendQuery({ projectId: this.projectId })
    this.reloadTable()
  }

  public async load(tableLoader: StateChanger): Promise<void> {
    let fetchMethod
    let fetchPayload

    if (this.goalId) {
      fetchMethod = goalsStore.findChild
      fetchPayload = { id: this.goalId, query: this.query }
    } else fetchMethod = goalsStore.findAll

    await this.loadData(tableLoader, fetchMethod, fetchPayload)
  }

  public async openEntity(row: Goal): Promise<void> {
    if (row.hasChildren) {
      const path = `${this.route.path}/${row.id}`
      await router.push(path)
      await breadcrumbStore.addBreadcrumb({ path: path, label: `${row.title}` })
      this.reloadTable()
    } else {
      tableStore.setSelectedRow(row)
      const goal = await goalsStore.findOneById(row.id!)
      goalsStore.setTask(goal)
      await goalsStore.openGoalWindow(row)
    }
  }

  public async editEntity(): Promise<void> {
    if (!tableStore.selectedRow) return
    await goalsStore.openGoalWindow(tableStore.selectedRow as Goal)
  }

  public async editEntityStatus(status: string): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Goal[]
      selection.forEach((goal: Goal) => (goal.state = status.toLowerCase() as Status))
      await goalsStore.updateMany(selection)
    } else {
      if (!tableStore.selectedRow) return
      const row: Goal = tableStore.selectedRow as Goal
      row.state = status.toLowerCase() as Status
      await goalsStore.openGoalChangeStatusWindow(row)
    }
  }

  public async deleteEntities(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await goalsStore.deleteMany(ids)
    tableStore.requireReload()
  }

  public async restoreEntities(): Promise<void> {
    const ids = this.selectedIds as Array<number>
    await goalsStore.restoreMany(ids)
    tableStore.requireReload()
  }

  public async createChild(): Promise<void> {
    const parentGoal = tableStore.selectedRow as Goal
    const goal = new Goal()
    goal.parentGoalId = parentGoal.id
    goal.projectId = parentGoal.projectId
    await goalsStore.openGoalWindow(goal)
  }
}
