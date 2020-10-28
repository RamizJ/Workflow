<template>
  <BaseTable
    ref="baseTable"
    :data="tableData"
    :row-class="setRowClass"
    @space="onSpaceClick"
    @double-click="onDoubleClick"
    @right-click="openContextMenu"
    @load="onLoad"
    infinite
  >
    <BaseTableColumn prop="title" label="Название" :custom="true">
      <template v-slot:default="scope">
        <GoalTitleCell :row="scope.row"></GoalTitleCell>
      </template>
    </BaseTableColumn>
    <BaseTableColumn prop="performerFio" label="Исполнитель" width="150" :formatter="formatFio" />
    <BaseTableColumn v-if="!isProjectPage" prop="projectName" label="Проект" width="150" />
    <BaseTableColumn prop="state" label="Статус" width="120" :formatter="formatStatus" />
    <BaseTableColumn prop="priority" label="Приоритет" width="120" :formatter="formatPriority" />
    <BaseTableColumn
      prop="creationDate"
      label="Дата создания"
      width="180"
      :formatter="formatDate"
    />
    <GoalContextMenu
      slot="footer"
      ref="contextMenu"
      @edit="edit"
      @create-child="createChild"
      @edit-status="editStatus"
      @remove="remove"
      @restore="restore"
    />
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue, Watch } from 'vue-property-decorator'
import { StateChanger } from 'vue-infinite-loading'
import { ElTableColumn } from 'element-ui/types/table-column'

import goalsStore from '@/modules/goals/store/goals.store'
import tableStore from '@/core/store/table.store'
import breadcrumbStore from '@/modules/goals/store/breadcrumb.store'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import GoalTitleCell from '@/modules/goals/components/goal-table/goal-title-cell.vue'
import Query, { FilterField } from '@/core/types/query.type'
import Entity from '@/core/types/entity.type'
import Goal, { Priority, Status, priorities, statuses } from '@/modules/goals/models/goal.type'
import GoalContextMenu from '@/modules/goals/components/goal-table/goal-context-menu.vue'

@Component({
  components: {
    BaseTable,
    BaseTableColumn,
    GoalTitleCell,
    GoalContextMenu,
  },
})
export default class GoalTableNew extends Vue {
  @Ref() readonly contextMenu!: GoalContextMenu
  @Ref() readonly baseTable!: BaseTable

  private get tableData(): Goal[] {
    return tableStore.data as Goal[]
  }

  private get query(): Query {
    return tableStore.query
  }

  private get isReloadRequired(): boolean {
    return tableStore.isReloadRequired
  }

  protected async mounted(): Promise<void> {
    await this.updateBreadcrumbs()
  }

  private async updateBreadcrumbs(): Promise<void> {
    await breadcrumbStore.updateBreadcrumbs()
  }

  @Watch('isReloadRequired')
  onReloadRequired(): void {
    tableStore.setData([])
    tableStore.setPage(0)
    this.baseTable.loader.stateChanger.reset()
    tableStore.completeReload()
  }

  private get isProjectPage(): boolean {
    return !!this.$route.params.projectId
  }

  private async onLoad($state: StateChanger): Promise<void> {
    ;(this as any).$insProgress.start()
    let data: Goal[] = []
    if (this.openedGoalId)
      data = await goalsStore.findChild({ id: this.openedGoalId, query: this.query })
    else data = await goalsStore.findAll(this.query)
    tableStore.increasePage()
    if (data.length) $state.loaded()
    else $state.complete()
    tableStore.appendData(data)
    ;(this as any).$insProgress.finish()
  }

  private get openedGoalId(): number {
    const pathElements = this.$route.path.split('/')
    const goalId = pathElements[pathElements.length - 1]
    try {
      return parseInt(goalId)
    } catch (e) {
      return 0
    }
  }

  private openContextMenu(row: Goal, selection: Goal[], event: Event) {
    tableStore.setSelectedRow(row)
    tableStore.setSelectedRows(selection)
    this.contextMenu.open(event, row)
  }

  private setRowClass({ row, rowIndex }: { row: Entity; rowIndex: number }): string {
    const goal = row as Goal
    const isSection = goal.metadataList?.some(
      (metadata) => metadata.key === 'isSection' && metadata.value === 'true'
    )
    if (isSection) return 'section'
    else return ''
  }

  private async onSpaceClick(): Promise<void> {
    await goalsStore.openGoalWindow()
  }

  private async onDoubleClick(row: Goal): Promise<void> {
    if (row.hasChildren) {
      const path = `${this.$route.path}/${row.id}`
      await this.$router.push(path)
      await breadcrumbStore.addBreadcrumb({ path: path, label: `${row.title}` })
    } else {
      tableStore.setSelectedRow(row)
      const goal = await goalsStore.findOneById(row.id!)
      goalsStore.setTask(goal)
      await goalsStore.openGoalWindow(row)
    }
  }

  private async edit(): Promise<void> {
    if (!tableStore.selectedRow) return
    await goalsStore.openGoalWindow(tableStore.selectedRow as Goal)
  }

  private async editStatus(status: string): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Goal[]
      selection.forEach((goal: Goal) => (goal.state = status as Status))
      await goalsStore.updateMany(selection)
    } else {
      if (!tableStore.selectedRow) return
      const row: Goal = tableStore.selectedRow as Goal
      row.state = status as Status
      await goalsStore.updateOne(row)
    }
  }

  private async createChild(): Promise<void> {
    const parentGoal = tableStore.selectedRow as Goal
    const goal = new Goal()
    goal.parentGoalId = parentGoal.id
    goal.projectId = parentGoal.projectId
    await goalsStore.openGoalWindow(goal)
  }

  private async remove(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Goal[]
      const ids = selection.map((goal: Goal) => goal.id!)
      await goalsStore.deleteMany(ids)
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      await goalsStore.deleteOne(tableStore.selectedRow.id as number)
    }
    tableStore.requireReload()
  }

  private async restore(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Goal[]
      const ids = selection.map((goal: Goal) => goal.id!)
      await goalsStore.restoreMany(ids)
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      await goalsStore.restoreOne(tableStore.selectedRow.id as number)
    }
    tableStore.requireReload()
  }

  public formatStatus(row: Entity, column: ElTableColumn, value: string): string {
    return statuses.find((status) => status.value === (value as Status))?.label || ''
  }

  public formatPriority(row: Entity, column: ElTableColumn, value: string): string {
    return priorities.find((priority) => priority.value == (value as Priority))?.label || ''
  }

  public formatDate(row: Entity, column: ElTableColumn, value: string): string {
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

  public formatFio(row: Entity, column: ElTableColumn, value: string): string {
    return this.shortenFullName(value)
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
</script>

<style lang="scss"></style>
