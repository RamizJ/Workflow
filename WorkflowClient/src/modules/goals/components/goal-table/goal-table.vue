<template>
  <BaseTable
    ref="baseTable"
    :data="goalTableService.tableData"
    @double-click.self="goalTableService.openGoal"
    @right-click.self="goalTableService.openContextMenu"
    @load.self="goalTableService.load"
    infinite
  >
    <BaseTableColumn prop="title" label="Название" :custom="true">
      <template v-slot:default="scope">
        <GoalTitleCell :row="scope.row"></GoalTitleCell>
      </template>
    </BaseTableColumn>
    <BaseTableColumn
      prop="performerFio"
      label="Исполнитель"
      width="150"
      :formatter="goalTableService.fioFormatter"
    />
    <BaseTableColumn
      v-if="!goalTableService.projectId"
      prop="projectName"
      label="Проект"
      width="150"
    />
    <BaseTableColumn
      prop="state"
      label="Статус"
      width="120"
      :formatter="goalTableService.statusFormatter"
    />
    <BaseTableColumn
      prop="priority"
      label="Приоритет"
      width="120"
      :formatter="goalTableService.priorityFormatter"
    />
    <BaseTableColumn
      prop="creationDate"
      label="Дата создания"
      width="180"
      :formatter="goalTableService.dateFormatter"
    />
    <GoalContextMenu
      slot="footer"
      ref="contextMenu"
      @edit="goalTableService.editRow"
      @edit-status="goalTableService.editRowStatus"
      @create-child="goalTableService.createChild"
      @remove="goalTableService.deleteRows"
      @restore="goalTableService.restoreRows"
    />
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue, Watch } from 'vue-property-decorator'
import { Route } from 'vue-router'
import { ProgressBar } from '@/core/services/table.service'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import GoalTitleCell from '@/modules/goals/components/goal-table/goal-title-cell.vue'
import GoalContextMenu from '@/modules/goals/components/goal-table/goal-context-menu.vue'
import GoalTableService from '@/modules/goals/services/goal-table.service'

@Component({
  components: {
    BaseTable,
    BaseTableColumn,
    GoalTitleCell,
    GoalContextMenu,
  },
})
export default class GoalTable extends Vue {
  @Ref() readonly contextMenu!: GoalContextMenu
  @Ref() readonly baseTable!: BaseTable
  private goalTableService: GoalTableService

  constructor() {
    super()
    const progressBar = (this as any).$insProgress as ProgressBar
    this.goalTableService = new GoalTableService(progressBar)
  }

  protected async mounted(): Promise<void> {
    this.goalTableService.contextMenu = (this.contextMenu as unknown) as BaseContextMenu
    await this.goalTableService.initialize()
  }

  protected beforeDestroy(): void {
    this.goalTableService.resetTable()
  }

  @Watch('$route')
  onRouteChange(to: Route): void {
    if (to.path === '/goals') this.goalTableService.resetBreadcrumbs()
    this.goalTableService.reloadTable()
  }

  @Watch('isReloadRequired')
  onReloadRequired(value: boolean): void {
    this.goalTableService.reloadTable(value)
  }
}
</script>
