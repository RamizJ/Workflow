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
          <a v-if="isRowEditable" @click.prevent="onRowDoubleClick(child.data.row)">Открыть</a>
        </li>
        <li>
          <a v-if="isRowEditable" @click.prevent="editEntity(child.data.row)">Изменить</a>
        </li>
        <el-divider v-if="isRowEditable"></el-divider>
        <li><a v-if="isRowEditable" @click.prevent="createEntity">Новая команда</a></li>
        <el-divider v-if="isRowEditable"></el-divider>
        <li>
          <a v-if="$route.params.projectId" @click.prevent="removeEntityFromProject(child.data.row)"
            >Убрать из проекта</a
          >
        </li>
        <li>
          <a v-if="isRowEditable" @click.prevent="deleteEntity(child.data.row)"
            >Переместить в корзину</a
          >
        </li>
        <li>
          <a v-if="!isRowEditable" @click.prevent="restoreEntity(child.data.row)">Восстановить</a>
        </li>
      </template>
    </vue-context>
    <team-dialog
      v-if="modalVisible"
      :id="modalData"
      @close="modalVisible = false"
      @submit="reloadData"
    ></team-dialog>
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'
import { StateChanger } from 'vue-infinite-loading'

import teamsModule from '@/store/modules/teams.module'
import projectsModule from '@/store/modules/projects.module'
import TeamDialog from '@/components/Team/TeamDialog.vue'
import TableMixin from '@/mixins/table.mixin'
import Team from '@/types/team.type'
import usersModule from '@/store/modules/users.module'

@Component({ components: { TeamDialog } })
export default class TeamTable extends mixins(TableMixin) {
  private loading = false

  private async loadData($state: StateChanger) {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    let data: Team[]
    if (this.$route.params.projectId) data = await projectsModule.findTeams(this.query)
    else data = await teamsModule.findAll(this.query)
    if (this.query.pageNumber !== undefined) this.query.pageNumber++
    if (data.length) $state.loaded()
    else $state.complete()
    this.data = isFirstLoad ? data : this.data.concat(data)
    this.loading = false
  }

  public createEntity() {
    this.modalData = undefined
    this.modalVisible = true
  }

  public editEntity(entity: Team) {
    this.modalData = entity.id
    this.modalVisible = true
  }

  private async deleteEntity(entity: Team, multiple = false) {
    if (multiple) await teamsModule.deleteMany(this.table.selection.map((item: Team) => item.id))
    else await teamsModule.deleteOne(entity.id as number)
    this.reloadData()
  }

  private async restoreEntity(entity: Team, multiple = false) {
    if (multiple) await teamsModule.restoreMany(this.table.selection.map((item: Team) => item.id))
    else await teamsModule.restoreOne(entity.id as number)
    this.reloadData()
  }

  public async onRowDoubleClick(row: Team) {
    if (!row.isRemoved) await this.$router.push(`/teams/${row.id}`)
  }

  private async removeEntityFromProject(row: Team) {
    const projectId = parseInt(this.$route.params.projectId)
    const teamId = row.id || -1
    const teamIds = this.table.selection.map((item: Team) => item.id)
    if (this.isMultipleSelected) await projectsModule.removeTeams({ projectId, teamIds })
    else await projectsModule.removeTeam({ projectId, teamId })
    this.reloadData()
  }
}
</script>
