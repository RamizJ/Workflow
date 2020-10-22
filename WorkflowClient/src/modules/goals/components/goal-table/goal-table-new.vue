<template>
  <BaseTable
    :data="data"
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
    <BaseContextMenu slot="footer" ref="contextMenu">
      <template v-slot:default="scope">
        <BaseContextMenuItem @click="editEntity(scope.data)">Изменить</BaseContextMenuItem>
      </template>
    </BaseContextMenu>
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue } from 'vue-property-decorator'
import { StateChanger } from 'vue-infinite-loading'
import goalsStore from '@/modules/goals/store/goals.store'
import BaseTable from '../../../../core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import Task, { Priority, Status, priorities, statuses } from '@/modules/goals/models/task.type'
import Query from '@/core/types/query.type'
import GoalTitleCell from '@/modules/goals/components/goal-table/goal-title-cell.vue'
import Entity from '@/core/types/entity.type'
import { ElTableColumn } from 'element-ui/types/table-column'
import moment from 'moment'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import BaseContextMenuItem from '@/core/components/base-context-menu/base-context-menu-item.vue'

@Component({
  components: { BaseContextMenuItem, BaseContextMenu, GoalTitleCell, BaseTableColumn, BaseTable },
})
export default class GoalTableNew extends Vue {
  @Ref() readonly contextMenu!: BaseContextMenu

  private query: Query = new Query()
  private data: Task[] = []

  private get isProjectPage(): boolean {
    return !!this.$route.params.projectId
  }

  private async onLoad($state: StateChanger): Promise<void> {
    const isFirstLoad = !this.data.length
    const data = await goalsStore.findAll(this.query)
    if (this.query.pageNumber !== undefined) this.query.pageNumber++
    if (data.length) $state.loaded()
    else $state.complete()
    this.data = isFirstLoad ? data : this.data.concat(data)
  }

  private openContextMenu(row: Entity, event: Event) {
    this.contextMenu.open(event, row)
  }

  private openGoalWindow(row: Task): void {
    goalsStore.setTask(row)
    goalsStore.openGoalWindow()
  }

  private editEntity(row: Task): void {
    console.log(row)
  }

  public formatStatus(row: Entity, column: ElTableColumn, value: string): string {
    return statuses.find((status) => status.value === (value as Status))?.label || ''
  }

  public formatPriority(row: Entity, column: ElTableColumn, value: string): string {
    return priorities.find((priority) => priority.value == (value as Priority))?.label || ''
  }

  public formatDate(row: Entity, column: ElTableColumn, value: string): string {
    const dateUtc = moment.utc(value)
    return dateUtc.format('DD.MM.YYYY HH:mm')
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
