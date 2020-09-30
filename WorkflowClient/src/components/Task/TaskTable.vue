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
      border
      show-overflow-tooltip
    >
      <el-table-column type="selection" width="42"></el-table-column>
      <el-table-column prop="title" label="Задача"></el-table-column>
      <el-table-column
        prop="performerFio"
        label="Ответственный"
        width="150"
        :formatter="formatFio"
      ></el-table-column>
      <el-table-column
        v-if="!$route.params.projectId"
        prop="projectName"
        label="Проект"
        width="150"
      ></el-table-column>
      <el-table-column
        prop="state"
        label="Статус"
        width="120"
        :formatter="formatStatus"
      ></el-table-column>
      <el-table-column
        prop="priority"
        label="Приоритет"
        width="120"
        :formatter="formatPriority"
      ></el-table-column>
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
          <a v-if="isRowEditable" @click.prevent="editEntity(child.data.row)">Изменить</a>
        </li>
        <el-divider v-if="isRowEditable"></el-divider>
        <li><a v-if="isRowEditable" @click.prevent="createEntity">Новая задача</a></li>
        <el-divider v-if="isRowEditable"></el-divider>
        <li class="v-context__sub">
          <a v-if="isRowEditable">Изменить статус</a>
          <ul class="v-context">
            <li><a @click.prevent="editEntityStatus(child.data.row, 'New')">Новое</a></li>
            <li>
              <a @click.prevent="editEntityStatus(child.data.row, 'Succeed')">Выполнено</a>
            </li>
            <li>
              <a @click.prevent="editEntityStatus(child.data.row, 'Delay')">Отложено</a>
            </li>
            <li>
              <a @click.prevent="editEntityStatus(child.data.row, 'Rejected')">Отклонено</a>
            </li>
            <el-divider></el-divider>
            <li>
              <a @click.prevent="editEntityStatus(child.data.row, 'Perform')">Выполняется</a>
            </li>
            <li>
              <a @click.prevent="editEntityStatus(child.data.row, 'Testing')">Проверяется</a>
            </li>
          </ul>
        </li>
        <li>
          <a v-if="isRowEditable" @click.prevent="deleteEntity(child.data.row, isMultipleSelected)"
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
    <task-dialog
      v-if="dialogVisible"
      :id="dialogData"
      @close="dialogVisible = false"
      @submit="reloadData"
    ></task-dialog>
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'
import { StateChanger } from 'vue-infinite-loading'

import tasksModule from '@/store/modules/tasks.module'
import TableMixin from '@/mixins/table.mixin.ts'
import TaskDialog from '@/components/Task/TaskDialog.vue'
import Task, { Status } from '@/types/task.type'

@Component({ components: { TaskDialog } })
export default class TaskTable extends mixins(TableMixin) {
  public data: Task[] = []
  private loading = false

  private async loadData($state: StateChanger): Promise<void> {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    const data = await tasksModule.findAll(this.query)
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

  public editEntity(entity: Task): void {
    this.dialogData = entity.id
    this.dialogVisible = true
  }

  private async deleteEntity(entity: Task, multiple = false): Promise<void> {
    if (multiple) await tasksModule.deleteMany(this.table.selection.map((item: Task) => item.id))
    else await tasksModule.deleteOne(entity.id as number)
    this.reloadData()
  }

  private async restoreEntity(entity: Task, multiple = false): Promise<void> {
    if (multiple) await tasksModule.restoreMany(this.table.selection.map((item: Task) => item.id))
    else await tasksModule.restoreOne(entity.id as number)
    this.reloadData()
  }

  private async editEntityStatus(entity: Task, status: string): Promise<void> {
    if (this.isMultipleSelected) {
      const items = this.table.selection.map((item: Task) => {
        item.state = status as Status
        return item
      })
      await tasksModule.updateMany(items)
    } else {
      const item = entity
      item.state = status as Status
      await tasksModule.updateOne(item)
    }
    this.reloadData()
  }
}
</script>
