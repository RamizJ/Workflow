<template>
  <BaseTable
    ref="baseTable"
    :data="tableService.tableData"
    @double-click.capture="tableService.openEntity"
    @right-click.capture="tableService.openContextMenu"
    @load.capture="tableService.load"
    infinite
  >
    <BaseTableColumn prop="lastName" label="Фамилия" width="150"></BaseTableColumn>
    <BaseTableColumn prop="firstName" label="Имя" width="150"></BaseTableColumn>
    <BaseTableColumn prop="middleName" label="Отчество" width="150"></BaseTableColumn>
    <BaseTableColumn prop="userName" label="Логин" width="150"></BaseTableColumn>
    <BaseTableColumn prop="email" label="Почта"></BaseTableColumn>
    <BaseTableColumn prop="phone" label="Телефон" width="120"></BaseTableColumn>
    <BaseTableColumn prop="position" label="Должность" width="180"></BaseTableColumn>
    <UserContextMenu
      slot="footer"
      ref="contextMenu"
      @open.capture="tableService.openEntity"
      @edit.capture="tableService.editEntity"
      @create.capture="tableService.createEntity"
      @remove.capture="tableService.deleteEntities"
      @remove-from-team.capture="tableService.deleteEntitiesFromTeam"
      @restore.capture="tableService.restoreEntities"
    />
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue, Watch } from 'vue-property-decorator'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import UserContextMenu from '@/modules/users/components/user-context-menu.vue'
import UserTableService from '@/modules/users/services/user-table.service'
import { ProgressBar } from '@/core/services/table.service'

@Component({
  components: {
    BaseTable,
    BaseTableColumn,
    UserContextMenu,
  },
})
export default class UserTable extends Vue {
  @Ref() readonly contextMenu!: UserContextMenu
  @Ref() readonly baseTable!: BaseTable
  private readonly tableService: UserTableService

  constructor() {
    super()
    const progressBar = (this as any).$insProgress as ProgressBar
    this.tableService = new UserTableService(progressBar)
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
