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
      @row-dblclick="onRowDoubleClick"
      @row-contextmenu="onRowRightClick"
      highlight-current-row="highlight-current-row"
      border="border"
    >
      <el-table-column type="selection" width="42"></el-table-column>
      <el-table-column prop="name" label="Проект"></el-table-column>
      <el-table-column prop="ownerFio" label="Руководитель" width="250"></el-table-column>
      <el-table-column
        prop="creationDate"
        label="Дата создания"
        width="180"
        :formatter="formatDate"
      ></el-table-column>
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
        <li><a v-if="isRowEditable" @click.prevent="editEntity(child.data.row)">Изменить</a></li>
        <el-divider v-if="isRowEditable && !$route.params.teamId"></el-divider>
        <li>
          <a v-if="isRowEditable && !$route.params.teamId" @click.prevent="createEntity"
            >Новый проект</a
          >
        </li>
        <el-divider v-if="isRowEditable && !$route.params.teamId"></el-divider>
        <li>
          <a
            v-if="isRowEditable && !$route.params.teamId"
            @click.prevent="deleteEntity(child.data.row, isMultipleSelected)"
            >Переместить в корзину</a
          >
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
    <project-dialog
      v-if="dialogVisible"
      :id="dialogData"
      @close="dialogVisible = false"
      @submit="reloadData"
    ></project-dialog>
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'
import { StateChanger } from 'vue-infinite-loading'

import projectsModule from '@/store/modules/projects.module'
import TableMixin from '@/mixins/table.mixin'
import ProjectDialog from '@/components/Project/ProjectDialog.vue'
import Project from '@/types/project.type'
import teamsModule from '@/store/modules/teams.module'

@Component({ components: { ProjectDialog } })
export default class ProjectTable extends mixins(TableMixin) {
  public data: Project[] = []
  private loading = false

  private async loadData($state: StateChanger): Promise<void> {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    let data: Project[]
    if (this.$route.params.teamId) data = await teamsModule.findProjects(this.query)
    else data = await projectsModule.findAll(this.query)
    if (this.query.pageNumber !== undefined) this.query.pageNumber++
    if (data.length) $state.loaded()
    else $state.complete()
    this.data = isFirstLoad ? data : this.data.concat(data)
    this.loading = false
  }

  public createEntity(): void {
    this.dialogData = undefined
    this.dialogVisible = true
  }

  public editEntity(entity: Project): void {
    this.dialogData = entity.id
    this.dialogVisible = true
  }

  private async deleteEntity(entity: Project, multiple = false): Promise<void> {
    if (multiple)
      await projectsModule.deleteMany(this.table.selection.map((item: Project) => item.id))
    else await projectsModule.deleteOne(entity.id as number)
    this.reloadData()
  }

  private async restoreEntity(entity: Project, multiple = false): Promise<void> {
    if (multiple)
      await projectsModule.restoreMany(this.table.selection.map((item: Project) => item.id))
    else await projectsModule.restoreOne(entity.id as number)
    this.reloadData()
  }

  public async onRowDoubleClick(row: Project): Promise<void> {
    if (!row.isRemoved) await this.$router.push(`/projects/${row.id}`)
  }
}
</script>
