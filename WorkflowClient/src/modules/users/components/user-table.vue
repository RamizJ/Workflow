<template>
  <BaseTable
    ref="baseTable"
    :data="tableData"
    @space="onSpaceClick"
    @double-click="onDoubleClick"
    @right-click="openContextMenu"
    @load="onLoad"
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
      @open="onDoubleClick"
      @edit="edit"
      @create="create"
      @remove="remove"
      @restore="restore"
    />
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue, Watch } from 'vue-property-decorator'
import { StateChanger } from 'vue-infinite-loading'
import { ElTableColumn } from 'element-ui/types/table-column'

import usersStore from '@/modules/users/store/users.store'
import tableStore from '@/core/store/table.store'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import Query, { FilterField } from '@/core/types/query.type'
import Entity from '@/core/types/entity.type'
import User from '@/modules/users/models/user.type'
import UserContextMenu from '@/modules/users/components/user-context-menu.vue'
import teamsStore from '@/modules/teams/store/teams.store'

@Component({
  components: {
    UserContextMenu,
    BaseTable,
    BaseTableColumn,
  },
})
export default class UserTable extends Vue {
  @Ref() readonly contextMenu!: UserContextMenu
  @Ref() readonly baseTable!: BaseTable

  private get tableData(): User[] {
    return tableStore.data as User[]
  }

  private get query(): Query {
    return tableStore.query
  }

  private get isReloadRequired(): boolean {
    return tableStore.isReloadRequired
  }

  private get openedTeamId(): number {
    const teamId = this.$route.params.teamId ? parseInt(this.$route.params.teamId) : 0
    if (teamId) this.query.teamId = teamId
    return teamId
  }

  protected beforeDestroy(): void {
    tableStore.setData([])
    tableStore.setQuery(new Query())
  }

  @Watch('isReloadRequired')
  onReloadRequired(): void {
    tableStore.setData([])
    tableStore.setPage(0)
    this.baseTable.loader.stateChanger.reset()
    tableStore.completeReload()
  }

  private async onLoad($state: StateChanger): Promise<void> {
    ;(this as any).$insProgress.start()
    let data: User[] = []
    if (this.openedTeamId) data = await teamsStore.findUsers(this.query)
    else data = await usersStore.findAll(this.query)
    if (data.length) {
      tableStore.increasePage()
      tableStore.appendData(data)
      $state.loaded()
    } else $state.complete()
    ;(this as any).$insProgress.finish()
  }

  private openContextMenu(row: User, selection: User[], event: Event) {
    tableStore.setSelectedRow(row)
    tableStore.setSelectedRows(selection)
    this.contextMenu.open(event, row)
  }

  private async onSpaceClick(): Promise<void> {
    await usersStore.openUserWindow()
  }

  private async onDoubleClick(row: User): Promise<void> {
    await this.edit(row)
  }

  private async edit(row?: User): Promise<void> {
    const user = row || (tableStore.selectedRow as User)
    await usersStore.openUserWindow(user)
  }

  private async create(): Promise<void> {
    await usersStore.openUserWindow()
  }

  private async remove(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as User[]
      const ids = selection.map((user: User) => user.id!)
      await usersStore.deleteMany(ids)
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      await usersStore.deleteOne(tableStore.selectedRow.id as string)
    }
    tableStore.requireReload()
  }

  private async restore(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as User[]
      const ids = selection.map((user: User) => user.id!)
      await usersStore.restoreMany(ids)
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      await usersStore.restoreOne(tableStore.selectedRow.id as string)
    }
    tableStore.requireReload()
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
