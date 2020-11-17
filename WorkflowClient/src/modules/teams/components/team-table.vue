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
    <BaseTableColumn prop="name" label="Название" />
    <TeamContextMenu
      slot="footer"
      ref="contextMenu"
      @open="onDoubleClick"
      @edit="edit"
      @create="create"
      @remove="remove"
      @remove-from-project="removeFromProject"
      @restore="restore"
    />
  </BaseTable>
</template>

<script lang="ts">
import { Component, Ref, Vue, Watch } from 'vue-property-decorator'
import { StateChanger } from 'vue-infinite-loading'
import { ElTableColumn } from 'element-ui/types/table-column'

import teamsStore from '@/modules/teams/store/teams.store'
import tableStore from '@/core/store/table.store'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import Query, { FilterField } from '@/core/types/query.type'
import Entity from '@/core/types/entity.type'
import Team from '@/modules/teams/models/team.type'
import TeamContextMenu from '@/modules/teams/components/team-context-menu.vue'
import projectsStore from '@/modules/projects/store/projects.store'

@Component({
  components: {
    TeamContextMenu,
    BaseTable,
    BaseTableColumn,
  },
})
export default class TeamTable extends Vue {
  @Ref() readonly contextMenu!: TeamContextMenu
  @Ref() readonly baseTable!: BaseTable

  private get tableData(): Team[] {
    return tableStore.data as Team[]
  }

  private get query(): Query {
    return tableStore.query
  }

  private get isReloadRequired(): boolean {
    return tableStore.isReloadRequired
  }

  private get openedProjectId(): number {
    const projectId = this.$route.params.projectId ? parseInt(this.$route.params.projectId) : 0
    if (projectId) this.query.projectId = projectId
    return projectId
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
    let data: Team[] = []
    if (this.openedProjectId) data = await projectsStore.findTeams(this.query)
    else data = await teamsStore.findAll(this.query)
    if (data.length) {
      tableStore.increasePage()
      tableStore.appendData(data)
      $state.loaded()
    } else $state.complete()
    ;(this as any).$insProgress.finish()
  }

  private openContextMenu(row: Team, selection: Team[], event: Event) {
    tableStore.setSelectedRow(row)
    tableStore.setSelectedRows(selection)
    this.contextMenu.open(event, row)
  }

  private async onSpaceClick(): Promise<void> {
    await teamsStore.openTeamWindow()
  }

  private async onDoubleClick(row?: Team): Promise<void> {
    const team: Team = row || (tableStore.selectedRow as Team)
    if (!team.isRemoved && !this.$route.params.teamId) await this.$router.push(`/teams/${team.id}`)
  }

  private async edit(): Promise<void> {
    if (!tableStore.selectedRow) return
    const team = tableStore.selectedRow as Team
    await teamsStore.openTeamWindow(team)
  }

  private async create(): Promise<void> {
    await teamsStore.openTeamWindow()
  }

  private async remove(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Team[]
      const ids = selection.map((team: Team) => team.id!)
      await teamsStore.deleteMany(ids)
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      await teamsStore.deleteOne(tableStore.selectedRow.id as number)
    }
    tableStore.requireReload()
  }

  private async removeFromProject(): Promise<void> {
    const projectId = parseInt(this.$route.params.projectId)
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Team[]
      const teamIds = selection.map((team: Team) => team.id!)
      await projectsStore.removeTeams({ projectId, teamIds })
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      const teamId = tableStore.selectedRow.id as number
      await projectsStore.removeTeam({ projectId, teamId })
    }
    tableStore.requireReload()
  }

  private async restore(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Team[]
      const ids = selection.map((team: Team) => team.id!)
      await teamsStore.restoreMany(ids)
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      await teamsStore.restoreOne(tableStore.selectedRow.id as number)
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
