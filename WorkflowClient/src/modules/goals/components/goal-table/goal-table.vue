<template>
  <BaseTable
    ref="baseTable"
    :data="tableService.tableData"
    @double-click.capture="tableService.openEntity"
    @right-click.capture="tableService.openContextMenu"
    @load.capture="tableService.load"
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
      :formatter="tableService.fioFormatter"
    />
    <BaseTableColumn v-if="!tableService.projectId" prop="projectName" label="Проект" width="150" />
    <BaseTableColumn
      prop="state"
      label="Статус"
      width="120"
      :formatter="tableService.statusFormatter"
    />
    <BaseTableColumn
      prop="priority"
      label="Приоритет"
      width="120"
      :formatter="tableService.priorityFormatter"
    />
    <BaseTableColumn
      prop="creationDate"
      label="Дата создания"
      width="180"
      :formatter="tableService.dateFormatter"
    />
    <GoalContextMenu
      slot="footer"
      ref="contextMenu"
      @edit.capture="tableService.editEntity"
      @edit-status.capture="tableService.editEntityStatus"
      @create-child.capture="tableService.createChild"
      @remove.capture="tableService.deleteEntities"
      @restore.capture="tableService.restoreEntities"
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
    GoalContextMenu,
    GoalTitleCell,
  },
})
export default class GoalTable extends Vue {
  @Ref() readonly contextMenu!: GoalContextMenu
  @Ref() readonly baseTable!: BaseTable
  private tableService: GoalTableService

  constructor() {
    super()
    const progressBar = (this as any).$insProgress as ProgressBar
    this.tableService = new GoalTableService(progressBar)
  }

  protected async mounted(): Promise<void> {
    this.tableService.contextMenu = (this.contextMenu as unknown) as BaseContextMenu
    await this.tableService.initialize()
  }

  protected beforeDestroy(): void {
    this.tableService.resetTable()
  }

  private get isReloadRequired(): boolean {
    return this.tableService.isReloadRequired
  }

  @Watch('isReloadRequired')
  onReloadRequired(value: boolean): void {
    this.tableService.reloadTable(value)
  }

  @Watch('$route')
  onRouteChange(to: Route): void {
    if (to.path === '/goals') this.tableService.resetBreadcrumbs()
    this.tableService.reloadTable()
  }
}
</script>
