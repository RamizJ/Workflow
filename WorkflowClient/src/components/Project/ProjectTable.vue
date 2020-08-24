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
      el-table-column(prop="name" label="Проект")
      el-table-column(prop="ownerFio" label="Руководитель" width="250")
      el-table-column(prop="creationDate" label="Дата создания" width="180" :formatter="formatDate")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="loadData" force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li
          a(v-if="isRowEditable" @click.prevent="onRowDoubleClick(child.data.row)") Открыть
        li
          a(v-if="isRowEditable" @click.prevent="editEntity($event, child.data.row)") Изменить
        el-divider(v-if="isRowEditable")
        li
          a(@click.prevent="createEntity") Новый проект
        el-divider
        li
          a(v-if="isRowEditable" @click.prevent="deleteEntity($event, child.data.row, isMultipleSelected)") Переместить в корзину
        li
          a(v-if="!isRowEditable" @click.prevent="restoreEntity($event, child.data.row, isMultipleSelected)") Восстановить

    project-dialog(v-if="modalVisible" :data="modalData" @close="modalVisible = false" @submit="reloadData")

</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'
import { StateChanger } from 'vue-infinite-loading'

import projectsModule from '@/store/modules/projects.module'
import TableMixin from '@/mixins/table.mixin'
import ProjectDialog from '@/components/Project/ProjectDialog.vue'
import Project from '@/types/project.type'

@Component({ components: { ProjectDialog } })
export default class ProjectTable extends mixins(TableMixin) {
  private loading = false

  private async loadData($state: StateChanger) {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    const data = await projectsModule.findAll(this.query)
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

  public editEntity(event: Event, entity: Project) {
    this.modalData = entity.id
    this.modalVisible = true
  }

  private async deleteEntity(event: Event, entity: Project, multiple = false) {
    if (multiple)
      await projectsModule.deleteMany(this.table.selection.map((item: Project) => item.id))
    else await projectsModule.deleteOne(entity.id as number)
    this.reloadData()
  }

  private async restoreEntity(event: Event, entity: Project, multiple = false) {
    if (multiple)
      await projectsModule.restoreMany(this.table.selection.map((item: Project) => item.id))
    else await projectsModule.restoreOne(entity.id as number)
    this.reloadData()
  }

  public async onRowDoubleClick(row: any) {
    if (!row.isRemoved) await this.$router.push(`/projects/${row.id}`)
  }
}
</script>
