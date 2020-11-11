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
    <BaseTableColumn prop="ownerFio" label="Руководитель" width="250"></BaseTableColumn>
    <BaseTableColumn
      prop="creationDate"
      label="Дата создания"
      width="180"
      :formatter="formatDate"
    ></BaseTableColumn>
    <ProjectContextMenu
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

import projectsStore from '@/modules/projects/store/projects.store'
import tableStore from '@/core/store/table.store'
import BaseTable from '@/core/components/base-table/base-table.vue'
import BaseTableColumn from '@/core/components/base-table/base-table-column.vue'
import Query, { FilterField } from '@/core/types/query.type'
import Entity from '@/core/types/entity.type'
import ProjectContextMenu from '@/modules/projects/components/project-context-menu.vue'
import Project from '@/modules/projects/models/project.type'
import teamsStore from '@/modules/teams/store/teams.store'

@Component({
  components: {
    ProjectContextMenu,
    BaseTable,
    BaseTableColumn,
  },
})
export default class ProjectTable extends Vue {
  @Ref() readonly contextMenu!: ProjectContextMenu
  @Ref() readonly baseTable!: BaseTable

  private get tableData(): Project[] {
    return tableStore.data as Project[]
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
    let data: Project[] = []
    if (this.openedTeamId) data = await teamsStore.findProjects(this.query)
    else data = await projectsStore.findAll(this.query)
    if (data.length) {
      tableStore.increasePage()
      tableStore.appendData(data)
      $state.loaded()
    } else $state.complete()
    ;(this as any).$insProgress.finish()
  }

  private openContextMenu(row: Project, selection: Project[], event: Event) {
    tableStore.setSelectedRow(row)
    tableStore.setSelectedRows(selection)
    if (!this.$route.params.teamId) this.contextMenu.open(event, row)
  }

  private async onSpaceClick(): Promise<void> {
    await projectsStore.openProjectWindow()
  }

  private async onDoubleClick(row: Project): Promise<void> {
    if (!row.isRemoved && !this.$route.params.teamId) await this.$router.push(`/projects/${row.id}`)
  }

  private async edit(): Promise<void> {
    if (!tableStore.selectedRow) return
    const project = tableStore.selectedRow as Project
    // console.log(project)
    await projectsStore.openProjectWindow(project)
  }

  private async create(): Promise<void> {
    await projectsStore.openProjectWindow()
  }

  private async remove(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Project[]
      const ids = selection.map((project: Project) => project.id!)
      await projectsStore.deleteMany(ids)
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      await projectsStore.deleteOne(tableStore.selectedRow.id as number)
    }
    tableStore.requireReload()
  }

  private async restore(): Promise<void> {
    if (tableStore.isMultiselect) {
      const selection = tableStore.selectedRows as Project[]
      const ids = selection.map((project: Project) => project.id!)
      await projectsStore.restoreMany(ids)
    } else {
      if (!tableStore.selectedRow) return
      if (!tableStore.selectedRow?.id) return
      await projectsStore.restoreOne(tableStore.selectedRow.id as number)
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
