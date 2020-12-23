<template>
  <BaseTable
    ref="baseTable"
    :data="tableService.tableData"
    :infinite="!tableService.groupId"
    @double-click.capture="tableService.openEntity"
    @right-click.capture="tableService.openContextMenu"
    @load.capture="tableService.load"
  >
    <BaseTableColumn prop="name" label="Название" />
    <BaseTableColumn prop="ownerFullName" label="Руководитель" width="250"></BaseTableColumn>
    <BaseTableColumn
      prop="creationDate"
      label="Дата создания"
      width="180"
      :formatter="tableService.dateFormatter"
    ></BaseTableColumn>
    <ProjectContextMenu
      slot="footer"
      ref="contextMenu"
      @open.capture="tableService.openEntity"
      @edit.capture="tableService.editEntity"
      @create.capture="tableService.createEntity"
      @remove.capture="tableService.deleteEntities"
      @remove-from-group.capture="tableService.deleteEntitiesFromGroup"
      @restore.capture="tableService.restoreEntities"
    />
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue, Watch } from 'vue-property-decorator'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import ProjectContextMenu from '@/modules/projects/components/project-context-menu.vue'
import ProjectTableService from '@/modules/projects/services/project-table.service'
import { ProgressBar } from '@/core/services/table.service'

@Component({
  components: {
    BaseTable,
    BaseTableColumn,
    ProjectContextMenu,
  },
})
export default class ProjectTable extends Vue {
  @Ref() readonly contextMenu!: ProjectContextMenu
  @Ref() readonly baseTable!: BaseTable
  private readonly tableService: ProjectTableService

  constructor() {
    super()
    const progressBar = (this as any).$insProgress as ProgressBar
    this.tableService = new ProjectTableService(progressBar)
  }

  protected async mounted(): Promise<void> {
    this.tableService.contextMenu = (this.contextMenu as unknown) as BaseContextMenu
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
}
</script>
