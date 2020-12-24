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
    <BaseTableColumn prop="ownerFullName" label="Создатель" width="250" />
    <BaseTableColumn
      prop="creationDate"
      label="Дата создания"
      width="180"
      :formatter="tableService.dateFormatter"
    />
    <GroupContextMenu
      slot="footer"
      ref="contextMenu"
      @open.capture="tableService.openEntity"
      @edit.capture="tableService.editEntity"
      @create.capture="tableService.createEntity"
      @remove.capture="tableService.deleteEntities"
      @restore.capture="tableService.restoreEntities"
    />
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue, Watch } from 'vue-property-decorator'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import GroupTableService from '@/modules/groups/services/group-table.service'
import GroupContextMenu from '@/modules/groups/components/group-context-menu.vue'
import { ProgressBar } from '@/core/services/table.service'

@Component({ components: { GroupContextMenu, BaseTable, BaseTableColumn } })
export default class GroupTable extends Vue {
  @Ref() readonly contextMenu!: GroupContextMenu
  @Ref() readonly baseTable!: BaseTable
  private readonly tableService: GroupTableService

  constructor() {
    super()
    const progressBar = (this as any).$insProgress as ProgressBar
    this.tableService = new GroupTableService(progressBar)
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

<style scoped></style>
