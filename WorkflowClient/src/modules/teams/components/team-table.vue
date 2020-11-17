<template>
  <BaseTable
    ref="baseTable"
    :data="tableService.tableData"
    @double-click.capture="tableService.openEntity"
    @right-click.capture="tableService.openContextMenu"
    @load.capture="tableService.load"
    infinite
  >
    <BaseTableColumn prop="name" label="Название" />
    <TeamContextMenu
      slot="footer"
      ref="contextMenu"
      @open.capture="tableService.openEntity"
      @edit.capture="tableService.editEntity"
      @create.capture="tableService.createEntity"
      @remove.capture="tableService.deleteEntities"
      @remove-from-project.capture="tableService.deleteEntitiesFromProject"
      @restore.capture="tableService.restoreEntities"
    />
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue, Watch } from 'vue-property-decorator'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import TeamContextMenu from '@/modules/teams/components/team-context-menu.vue'
import TeamTableService from '@/modules/teams/services/team-table.service'
import { ProgressBar } from '@/core/services/table.service'

@Component({
  components: {
    BaseTable,
    BaseTableColumn,
    TeamContextMenu,
  },
})
export default class TeamTable extends Vue {
  @Ref() readonly contextMenu!: TeamContextMenu
  @Ref() readonly baseTable!: BaseTable
  private readonly tableService: TeamTableService

  constructor() {
    super()
    const progressBar = (this as any).$insProgress as ProgressBar
    this.tableService = new TeamTableService(progressBar)
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
