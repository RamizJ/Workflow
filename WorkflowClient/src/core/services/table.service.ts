import { Route } from 'vue-router'
import { StateChanger } from 'vue-infinite-loading'
import { ElTableColumn } from 'element-ui/types/table-column'
import tableStore from '@/core/store/table.store'
import breadcrumbStore from '@/modules/goals/store/breadcrumb.store'
import Entity from '@/core/types/entity.type'
import Query from '@/core/types/query.type'
import { priorities, Priority, Status, statuses } from '@/modules/goals/models/goal.type'
import { router } from '@/core'
import { Message } from 'element-ui'

export default class TableService {
  protected progressBar: ProgressBar
  public tableLoader?: StateChanger
  public contextMenu?: {
    open: (event: Event, data: any) => void
    confirmDelete: () => Promise<boolean>
  }

  constructor(progressBar: ProgressBar) {
    this.progressBar = progressBar
    this.resetTable()
  }

  public get route(): Route {
    return router.currentRoute
  }

  public get tableData(): Entity[] {
    return tableStore.data
  }

  public get query(): Query {
    return tableStore.query
  }

  public get isReloadRequired(): boolean {
    return tableStore.isReloadRequired
  }

  public get goalId(): number {
    if (!this.route.path.includes('/goals/')) return 0
    const pathElements = this.route.path.split('/')
    const goalId = pathElements[pathElements.length - 1]
    if (isNaN(parseInt(goalId))) return 0
    else return parseInt(goalId)
  }

  public get groupId(): number {
    return this.route.params.groupId ? parseInt(this.route.params.groupId) : 0
  }

  public get projectId(): number {
    return this.route.params.projectId ? parseInt(this.route.params.projectId) : 0
  }

  public get teamId(): number {
    return this.route.params.teamId ? parseInt(this.route.params.teamId) : 0
  }

  public get selectedIds(): Array<number> | Array<string> {
    let ids
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Entity[]
      ids = selection.map((entity: Entity) => entity.id!)
    } else {
      if (!tableStore.selectedRow) return []
      if (!tableStore.selectedRow?.id) return []
      ids = [tableStore.selectedRow.id]
    }
    if (typeof ids[0] === 'number') return ids as Array<number>
    else return ids as Array<string>
  }

  public async loadData(
    tableLoader: StateChanger,
    fetchMethod: (payload: Record<string, any>) => Promise<Entity[]>,
    fetchPayload?: Record<string, any>
  ): Promise<void> {
    this.tableLoader = tableLoader
    this.progressBar.start()
    if (this.projectId) this.extendQuery({ projectId: this.projectId })
    if (this.teamId) this.extendQuery({ teamId: this.teamId })
    try {
      const data: Entity[] = await fetchMethod(fetchPayload || this.query)
      if (data.length) {
        tableStore.increasePage()
        tableStore.appendData(data)
        this.tableLoader?.loaded()
      } else this.tableLoader?.complete()
    } catch (e) {
      Message.error('Ошибка загрузки данных')
    }
    this.progressBar.finish()
  }

  public extendQuery(property: Record<string, string | number | boolean>): void {
    tableStore.extendQuery(property)
  }

  public openContextMenu({
    row,
    selection,
    event,
  }: {
    row: Entity
    selection: Entity[]
    event: Event
  }): void {
    tableStore.setSelectedRow(row)
    tableStore.setSelectedRows(selection)
    this.contextMenu?.open(event, row)
  }

  public resetTable(): void {
    tableStore.setData([])
    tableStore.setPage(0)
    this.resetFilters()
  }

  public resetFilters(): void {
    tableStore.extendQuery({ projectId: this.projectId || undefined })
    tableStore.extendQuery({ teamId: this.teamId || undefined })
    tableStore.extendQuery({ filterFields: [] })
    tableStore.extendQuery({ filter: '' })
    tableStore.extendQuery({ withRemoved: false })
  }

  public reloadTable(reloadRequired?: boolean): void {
    if (reloadRequired === false) return
    tableStore.setData([])
    tableStore.setPage(0)
    this.tableLoader?.reset()
    tableStore.completeReload()
  }

  public async updateBreadcrumbs(): Promise<void> {
    await breadcrumbStore.updateBreadcrumbs()
  }

  public async resetBreadcrumbs(): Promise<void> {
    await breadcrumbStore.resetBreadcrumbs()
  }

  public statusFormatter(row: Entity, column: ElTableColumn, value: string): string {
    return statuses.find((status) => status.value === (value as Status))?.label || ''
  }

  public priorityFormatter(row: Entity, column: ElTableColumn, value: string): string {
    return priorities.find((priority) => priority.value == (value as Priority))?.label || ''
  }

  public dateFormatter(row: Entity, column: ElTableColumn, value: string): string {
    const date = new Date(value)
    return date.toLocaleString('ru', {
      timeZone: 'UTC',
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  public fioFormatter(row: Entity, column: ElTableColumn, value: string): string {
    if (!value) return value
    const fioArray = value.split(' ')
    const lastName = fioArray[0]
    const firstNameInitial = fioArray[1][0] ? `${fioArray[1][0]}.` : ''
    const middleNameInitial = fioArray[2][0] ? `${fioArray[2][0]}.` : ''
    return `${lastName} ${firstNameInitial} ${middleNameInitial}`
  }

  public shortenFullName(value: string): string {
    if (!value) return value
    const fioArray = value.split(' ')
    const lastName = fioArray[0]
    const firstNameInitial = fioArray[1][0] ? `${fioArray[1][0]}.` : ''
    const middleNameInitial = fioArray[2][0] ? `${fioArray[2][0]}.` : ''
    return `${lastName} ${firstNameInitial} ${middleNameInitial}`
  }
}

export interface ProgressBar {
  start: () => void
  finish: () => void
}
