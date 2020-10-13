<template>
  <div class="table-container">
    <el-table
      ref="table"
      height="100%"
      v-loading="loading"
      :data="data"
      :row-class-name="setIndex"
      @select="onRowSelect"
      @row-click="onRowSingleClick"
      @row-contextmenu="onRowRightClick"
      @row-dblclick="onRowDoubleClick"
      highlight-current-row="highlight-current-row"
      border="border"
    >
      <el-table-column type="selection" width="42"></el-table-column>
      <el-table-column prop="name" label="Команда"></el-table-column>
      <infinite-loading
        slot="append"
        ref="loader"
        spinner="waveDots"
        :distance="300"
        @infinite="loadData"
        force-use-infinite-wrapper=".el-table__body-wrapper"
      >
        <div slot="no-more"></div>
        <div slot="no-results"></div>
      </infinite-loading>
    </el-table>
    <vue-context ref="contextMenu">
      <template slot-scope="child">
        <li>
          <a
            v-if="isRowEditable && !$route.params.projectId"
            @click.prevent="onRowDoubleClick(child.data.row)"
            >Открыть</a
          >
        </li>
        <li>
          <a
            v-if="!isRowEditable && !$route.params.projectId"
            @click.prevent="editEntity(child.data.row)"
          >
            {{ 'Информация' }}
          </a>
        </li>
        <li>
          <a v-if="isRowEditable && $route.params.projectId" @click.prevent="editProjectTeamRights"
            >Изменить права</a
          >
        </li>
        <el-divider v-if="isRowEditable && !$route.params.projectId"></el-divider>
        <li>
          <a v-if="isRowEditable && !$route.params.projectId" @click.prevent="createEntity"
            >Новая команда</a
          >
        </li>
        <li>
          <a v-if="isRowEditable && $route.params.projectId" @click.prevent="addTeam"
            >Добавить команду</a
          >
        </li>
        <el-divider v-if="isRowEditable"></el-divider>
        <li>
          <a v-if="$route.params.projectId" @click.prevent="removeEntityFromProject(child.data.row)"
            >Убрать из проекта</a
          >
        </li>
        <li>
          <el-popconfirm
            v-if="isRowEditable && !$route.params.projectId && isConfirmDelete"
            :title="
              isMultipleSelected ? 'Удалить выбранные элементы?' : 'Удалить выбранный элемент?'
            "
            @onConfirm="deleteEntity"
          >
            <a slot="reference">Переместить в корзину</a>
          </el-popconfirm>
          <a
            v-if="isRowEditable && !$route.params.projectId && !isConfirmDelete"
            @click.prevent="deleteEntity"
          >
            Переместить в корзину
          </a>
        </li>
        <li>
          <a
            v-if="!isRowEditable"
            @click.prevent="restoreEntity(child.data.row, isMultipleSelected)"
            >Восстановить</a
          >
        </li>
      </template>
    </vue-context>
    <team-dialog
      v-if="dialogVisible"
      :id="dialogData"
      @close="dialogVisible = false"
      @submit="reloadData"
    ></team-dialog>
    <project-add-team-dialog
      v-if="dialogAddTeamVisible"
      @close="dialogAddTeamVisible = false"
      @submit="reloadData"
    />
    <project-edit-team-rights-dialog
      v-if="dialogEditTeamRightsVisible"
      :team="selectedRow"
      @close="dialogEditTeamRightsVisible = false"
    />
    <project-team-users-dialog
      v-if="dialogTeamUsersVisible"
      :team-id="selectedRow.id"
      @close="dialogTeamUsersVisible = false"
    />
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import { StateChanger } from 'vue-infinite-loading'

import teamsModule from '@/store/modules/teams.module'
import projectsModule from '@/store/modules/projects.module'
import TeamDialog from '@/components/Team/TeamDialog.vue'
import ProjectAddTeamDialog from '@/components/Project/ProjectAddTeamDialog.vue'
import ProjectEditTeamRightsDialog from '@/components/Project/ProjectEditTeamRightsDialog.vue'
import TableMixin from '@/mixins/table.mixin'
import Team from '@/types/team.type'
import ProjectTeamUsersDialog from '@/components/Project/ProjectTeamUsersDialog.vue'

@Component({
  components: {
    ProjectTeamUsersDialog,
    ProjectEditTeamRightsDialog,
    ProjectAddTeamDialog,
    TeamDialog,
  },
})
export default class TeamTable extends Mixins(TableMixin) {
  public data: Team[] = []
  private loading = false
  private dialogAddTeamVisible = false
  private dialogEditTeamRightsVisible = false
  private dialogTeamUsersVisible = false

  private async loadData($state: StateChanger): Promise<void> {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    let data: Team[] = []
    try {
      if (this.$route.params.projectId) data = await projectsModule.findTeams(this.query)
      else data = await teamsModule.findAll(this.query)
      if (this.query.pageNumber !== undefined) this.query.pageNumber++
      if (data.length) $state.loaded()
      else $state.complete()
    } catch (e) {
      this.$message.error('Не удаётся загрузить список команд')
      $state.error()
    }
    this.data = isFirstLoad ? data : this.data.concat(data)
    this.loading = false
  }

  public createEntity(): void {
    this.dialogData = undefined
    this.dialogVisible = true
  }

  public editEntity(entity: Team): void {
    this.dialogData = entity.id
    this.dialogVisible = true
  }

  private async deleteEntity(): Promise<void> {
    const entity = this.selectedRow as Team
    if (this.isMultipleSelected) await teamsModule.deleteMany(this.selectionIds as number[])
    else await teamsModule.deleteOne(entity.id as number)
    this.reloadData()
  }

  private async restoreEntity(entity: Team, multiple = false): Promise<void> {
    if (multiple) await teamsModule.restoreMany(this.selectionIds as number[])
    else await teamsModule.restoreOne(entity.id as number)
    this.reloadData()
  }

  public async onRowDoubleClick(row: Team): Promise<void> {
    if (!row.isRemoved && !this.$route.params.projectId) await this.$router.push(`/teams/${row.id}`)
    if (this.$route.params.projectId) this.dialogTeamUsersVisible = true
  }

  private async removeEntityFromProject(row: Team): Promise<void> {
    const projectId = parseInt(this.$route.params.projectId)
    const teamId = row.id || -1
    const teamIds = this.selectionIds as number[]
    if (this.isMultipleSelected) await projectsModule.removeTeams({ projectId, teamIds })
    else await projectsModule.removeTeam({ projectId, teamId })
    this.reloadData()
  }

  private addTeam(): void {
    this.dialogAddTeamVisible = true
  }

  private editProjectTeamRights(): void {
    this.dialogEditTeamRightsVisible = true
  }
}
</script>
