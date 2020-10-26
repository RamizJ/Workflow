<template>
  <BaseTable
    ref="baseTable"
    :data="tableData"
    @space="openGoalWindow"
    @double-click="openGoalWindow"
    @right-click="openContextMenu"
    @load="onLoad"
    infinite
  >
    <BaseTableColumn type="selection" width="42" />
    <BaseTableColumn prop="title" label="Задача" :custom="true">
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
      @create="create"
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
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import GoalTitleCell from '@/modules/goals/components/goal-table/goal-title-cell.vue'
import Query, { FilterField } from '@/core/types/query.type'
import Entity from '@/core/types/entity.type'
import Task, { Priority, Status, priorities, statuses } from '@/modules/goals/models/task.type'
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

  private get tableData(): Task[] {
    return tableStore.data as Task[]
  }

  private get query(): Query {
    return tableStore.query
  }

  private get isReloadRequired(): boolean {
    return tableStore.isReloadRequired
  }

  @Watch('isReloadRequired')
  onReloadRequired() {
    tableStore.setData([])
    tableStore.setPage(0)
    this.baseTable.loader.stateChanger.reset()
    tableStore.completeReload()
  }

  private get isProjectPage(): boolean {
    return !!this.$route.params.projectId
  }

  private async onLoad($state: StateChanger): Promise<void> {
    const data = await goalsStore.findAll(this.query)
    tableStore.increasePage()
    if (data.length) $state.loaded()
    else $state.complete()
    tableStore.appendData(data)
  }

  private openContextMenu(row: Task, selection: Task[], event: Event) {
    tableStore.setSelectedRow(row)
    tableStore.setSelectedRows(selection)
    this.contextMenu.open(event, row)
  }

  private openGoalWindow(row: Task): void {
    tableStore.setSelectedRow(row)
    goalsStore.setTask(tableStore.selectedRow as Task)
    goalsStore.openGoalWindow(row)
  }

  private async edit(): Promise<void> {
    if (!tableStore.selectedRow) return
    await goalsStore.openGoalWindow(tableStore.selectedRow as Task)
  }

  private async editStatus(status: string): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Task[]
      selection.forEach((goal: Task) => (goal.state = status as Status))
      await goalsStore.updateMany(selection)
    } else {
      if (!tableStore.selectedRow) return
      const row: Task = tableStore.selectedRow as Task
      row.state = status as Status
      await goalsStore.updateOne(row)
    }
  }

  private async create(): Promise<void> {
    goalsStore.setTask(new Task())
    await goalsStore.openGoalWindow()
  }

  private async remove(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Task[]
      const ids = selection.map((goal: Task) => goal.id!)
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
      const selection = tableStore.selectedRows as Task[]
      const ids = selection.map((goal: Task) => goal.id!)
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
