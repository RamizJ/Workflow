<template>
  <div class="table-container">
    <el-table
      ref="table"
      height="100%"
      v-loading="loading"
      row-key="id"
      :data="data"
      :row-class-name="setIndex"
      :expand-row-keys="expandRowKeys"
      tabindex="0"
      @keydown.space.native.prevent="onSpaceClick"
      @select="onRowSelect"
      @row-click="onRowSingleClick"
      @row-dblclick="onRowDoubleClick"
      @row-contextmenu="onRowRightClick"
      highlight-current-row="highlight-current-row"
      show-overflow-tooltip
      border
      :tree-props="{ children: 'children', hasChildren: 'hasChildren-hide' }"
      :load="loadChildren"
      lazy
    >
      <el-table-column type="selection" width="42"></el-table-column>
      <el-table-column prop="title" label="Задача">
        <template slot-scope="scope">
          <span class="cell-text">{{ scope.row.title }}</span>
          <span class="cell-icon" v-if="scope.row.description">
            <unicon name="file-alt" />
          </span>
          <span class="cell-icon" v-if="scope.row.hasChildren">
            <unicon name="list-ul" />
          </span>
          <span class="cell-icon" v-if="scope.row.isAttachmentsExist">
            <unicon name="paperclip" />
          </span>
        </template>
      </el-table-column>
      <el-table-column prop="performerFio" label="Исполнитель" width="150" :formatter="formatFio">
      </el-table-column>
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
          <a @click.prevent="editEntity(child.data.row)">
            {{ isRowEditable ? 'Изменить' : 'Информация' }}
          </a>
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
          <a v-if="isRowEditable" @click.prevent="deleteEntity"> Переместить в корзину </a>
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
    <GoalWindow
      v-if="dialogVisible"
      :id="dialogData"
      @close="dialogVisible = false"
      @submit="reloadData"
    ></GoalWindow>
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import { StateChanger } from 'vue-infinite-loading'

import goalsStore from '@/modules/goals/store/goals.store'
import TableMixin from '@/core/mixins/table.mixin.ts'
import GoalWindow from '@/modules/goals/components/goal-window/goal-window.vue'
import Goal, { Status } from '@/modules/goals/models/goal.type'
import Entity from '@/core/types/entity.type'
import { FilterField } from '@/core/types/query.type'

@Component({ components: { GoalWindow } })
export default class GoalTableOld extends Mixins(TableMixin) {
  public data: Goal[] = []
  private loading = false
  private sections: Goal[] = []
  private currentSection: Goal | undefined
  private expandRowKeys: number[] = []
  private tasksLoaded = false

  private async loadData($state: StateChanger): Promise<void> {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad

    const data = await goalsStore.findAll(this.query)
    if (this.query.pageNumber !== undefined) this.query.pageNumber++
    if (data.length) $state.loaded()
    else $state.complete()
    this.data = isFirstLoad ? data : this.data.concat(data)

    /*
    if (!this.tasksLoaded) {
      console.log(`Loading tasks... Page: ${this.query.pageNumber}`)
      const filterNoChildren: FilterField = { fieldName: 'hasChildren', values: [false] }
      if (this.query.filterFields?.length) this.query.filterFields.push(filterNoChildren)
      else this.query.filterFields = [filterNoChildren]
      let data = await goalsStore.findAll(this.query)

      // Temp fix due to a broken filter
      data = data.filter((item) => !item.hasChildren)

      this.data = isFirstLoad ? data : this.data.concat(data)
      if (this.query.pageNumber !== undefined) this.query.pageNumber++
      if (data.length) {
        console.log(`Page loaded. Continue...`)
        $state.loaded()
      } else {
        console.log(`Tasks loaded. Start loading sections...`)
        this.tasksLoaded = true
        this.query.pageNumber = 0
        this.loading = false
        $state.loaded()
      }
    } else {
      console.log(`Loading sections... Page: ${this.query.pageNumber}`)

      if (this.currentSection?.id) {
        const sectionId = this.currentSection.id
        const children = await goalsStore.findChild({ id: sectionId, query: this.queryChildren })
        const section = this.data.find((item) => item.id === sectionId)
        if (!section) return
        section.children = section.children?.length ? section.children.concat(children) : children
        section.children.forEach((child) => this.expandRowKeys.push(child.id!))
        this.table.toggleRowExpansion(this.currentSection, true)

        if (children?.length) {
          console.log(`Children received, page ${this.queryChildren.pageNumber} loaded`)
          if (this.queryChildren.pageNumber !== undefined) this.queryChildren.pageNumber++
          $state.loaded()
        } else {
          console.log(`For section ${sectionId} all tasks loaded. Set another section...`)
          this.deleteSection(this.currentSection)
          this.queryChildren.pageNumber = 0
          console.log(`Set current section to ${this.currentSection.id}`)
          if (this.sections.length) {
            $state.loaded()
          } else {
            $state.loaded()
            console.log(this.sections)
            console.log(`Loading completed`)
          }
        }
      } else {
        const filterWithChildren: FilterField = { fieldName: 'hasChildren', values: [true] }
        if (this.query.filterFields?.length) this.query.filterFields.push(filterWithChildren)
        else this.query.filterFields = [filterWithChildren]
        let data = await goalsStore.findAll(this.query)

        // Temp fix due to a broken filter
        data = data.filter((item) => item.hasChildren)

        this.sections = [...data]

        this.currentSection = this.sections[0]
        this.data = isFirstLoad ? data : this.data.concat(data)
        if (this.query.pageNumber !== undefined) this.query.pageNumber++
        if (data.length) {
          console.log(`Page loaded. Continue...`)
          $state.loaded()
        } else {
          console.log(`Sections loaded. All loaded`)
          $state.complete()
          this.loading = false
        }
      }
    }
    */

    this.loading = false
  }

  private async findSections(): Promise<Goal[]> {
    const filterWithChildren: FilterField = { fieldName: 'hasChildren', values: [true] }
    if (this.query.filterFields?.length) this.query.filterFields.push(filterWithChildren)
    else this.query.filterFields = [filterWithChildren]
    // const sections =
    //   this.sections && this.sections.length
    //     ? this.sections
    //     : data.filter((entity) => entity.hasChildren)
    const sections = await goalsStore.findAll(this.query)
    this.currentSection = { ...sections[0] }
    return [...sections]
  }

  private deleteSection(sectionToDelete: Goal): Goal[] {
    if (!(this.sections && this.sections.length)) return []
    const sections = this.sections?.filter((section) => section.id !== sectionToDelete.id)
    this.currentSection = { ...sections[0] }
    this.sections = [...sections]
    return [...sections]
  }

  private async loadChildren(parent: Goal, treeNode, resolve: CallableFunction) {
    console.log('loadChildren')
    if (!parent.id) return
    const children = await goalsStore.findChild({ id: parent.id })
    resolve(children)
  }

  public createEntity(): void {
    this.dialogData = undefined
    this.dialogVisible = true
  }

  public editEntity(entity: Goal): void {
    this.dialogData = entity.id
    this.dialogVisible = true
  }

  private async deleteEntity(): Promise<void> {
    const allowDelete = await this.confirmDelete()
    if (!allowDelete) return
    const entity = this.selectedRow as Goal
    if (this.isMultipleSelected) await goalsStore.deleteMany(this.selectionIds as number[])
    else await goalsStore.deleteOne(entity.id as number)
    this.reloadData()
  }

  private async restoreEntity(entity: Goal, multiple = false): Promise<void> {
    if (multiple) await goalsStore.restoreMany(this.selectionIds as number[])
    else await goalsStore.restoreOne(entity.id as number)
    this.reloadData()
  }

  private async editEntityStatus(entity: Goal, status: string): Promise<void> {
    if (this.isMultipleSelected) {
      const items = this.table.selection.map((entity: Entity) => {
        const modifiedEntity = entity as Goal
        modifiedEntity.state = status as Status
        return modifiedEntity
      })
      await goalsStore.updateMany(items)
    } else {
      const item = entity
      item.state = status as Status
      await goalsStore.updateOne(item)
    }
    this.reloadData()
  }

  private onSpaceClick(): void {
    if (
      !this.dialogVisible &&
      (this.$route.path === '/tasks' || this.$route.query.tab === 'tasks')
    ) {
      this.dialogData = undefined
      this.dialogVisible = true
    }
  }
}
</script>
