<template lang="pug">
  div.table-container
    el-table(
      ref="table"
      height="100%"
      v-loading="loading"
      :data="data"
      :row-class-name="setIndex"
      @select="onRowSelect"
      @row-click="onRowSingleClick"
      @row-dblclick="onRowDoubleClick"
      @row-contextmenu="onRowRightClick"
      highlight-current-row border)
      el-table-column(type="selection" width="38")
      el-table-column(prop="title" label="Задача")
      el-table-column(v-if="!$route.params.projectId" prop="projectName" label="Проект" width="150")
      el-table-column(prop="state" label="Статус" width="120" :formatter="formatStatus")
      el-table-column(prop="priority" label="Приоритет" width="120" :formatter="formatPriority")
      el-table-column(prop="creationDate" label="Дата создания" width="180" :formatter="formatDate")
      infinite-loading(
        slot="append"
        ref="loader"
        spinner="waveDots"
        @infinite="loadData"
        force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li
          a(v-if="isRowEditable" @click.prevent="editEntity($event, child.data.row)") Изменить
        el-divider(v-if="isRowEditable")
        li
          a(@click.prevent="createEntity") Новая задача
        el-divider
        li.v-context__sub
          a(v-if="isRowEditable") Изменить статус
          ul.v-context
            li
              a(@click.prevent="editEntityStatus($event, child.data.row, 'New')") Новое
            li
              a(@click.prevent="editEntityStatus($event, child.data.row, 'Succeed')") Выполнено
            li
              a(@click.prevent="editEntityStatus($event, child.data.row, 'Delay')") Отложено
            li
              a(@click.prevent="editEntityStatus($event, child.data.row, 'Rejected')") Отклонено
            el-divider
            li
              a(@click.prevent="editEntityStatus($event, child.data.row, 'Perform')") Выполняется
            li
              a(@click.prevent="editEntityStatus($event, child.data.row, 'Testing')") Проверяется
        li
          a(v-if="isRowEditable" @click.prevent="deleteEntity($event, child.data.row, isMultipleSelected)") Переместить в корзину
        li
          a(v-if="!isRowEditable" @click.prevent="restoreEntity($event, child.data.row, isMultipleSelected)") Восстановить

    task-dialog(v-if="modalVisible" :id="modalData" @close="modalVisible = false" @submit="reloadData")

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
  private loading = false

  private async loadData($state: StateChanger) {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    const data = await tasksModule.findAll(this.query)
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

  public editEntity(event: Event, entity: Task) {
    this.modalData = entity.id
    this.modalVisible = true
  }

  private async deleteEntity(event: Event, entity: Task, multiple = false) {
    if (multiple) await tasksModule.deleteMany(this.table.selection.map((item: Task) => item.id))
    else await tasksModule.deleteOne(entity.id as number)
    this.reloadData()
  }

  private async restoreEntity(event: Event, entity: Task, multiple = false) {
    if (multiple) await tasksModule.restoreMany(this.table.selection.map((item: Task) => item.id))
    else await tasksModule.restoreOne(entity.id as number)
    this.reloadData()
  }

  private async editEntityStatus(event: Event, entity: Task, status: string) {
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
    await this.reloadData()
  }
}
</script>
