<template>
  <div class="board">
    <draggable
      class="board__wrapper"
      v-model="lists"
      v-bind="listsDragOptions"
      @change="onListMove"
    >
      <div class="list" v-for="(list, index) in lists" :key="index">
        <div class="list__header" v-if="list.name" :class="list.name.toLowerCase()">
          {{ list.label }}
        </div>
        <div class="list__items">
          <draggable
            v-model="lists[index].items"
            v-bind="itemsDragOptions"
            @change="onEntityMove($event, list.name)"
          >
            <div
              class="item"
              v-for="item in list.items"
              :key="item.id"
              @contextmenu="onRowRightClick(item, null, $event)"
            >
              <a class="item__header" @click="onRowDoubleClick(item)">{{ item.title }}</a>
              <div class="item__footer">
                <div class="item__performer">{{ shortenFullName(item.performerFio) }}</div>
                <div class="item__date">{{ formatDate(null, null, item.creationDate) }}</div>
              </div>
            </div>
          </draggable>
        </div>
      </div>
    </draggable>
    <infinite-loading
      slot="append"
      ref="loader"
      spinner="waveDots"
      :distance="300"
      @infinite="loadData"
    >
      <div slot="no-more"></div>
      <div slot="no-results"></div>
    </infinite-loading>
    <vue-context ref="contextMenu">
      <template slot-scope="child">
        <li>
          <a @click.prevent="editEntity(child.data.row)">
            {{ isRowEditable ? 'Изменить' : 'Информация' }}
          </a>
        </li>
        <el-divider v-if="isRowEditable"></el-divider>
        <li><a @click.prevent="createEntity">Новая задача</a></li>
        <el-divider></el-divider>
        <li class="v-context__sub">
          <a v-if="isRowEditable">Изменить статус</a>
          <ul class="v-context">
            <li><a @click.prevent="editEntityStatus(child.data.row, 'New')">Новое</a></li>
            <li>
              <a @click.prevent="editEntityStatus(child.data.row, 'Succeed')">Выполнено</a>
            </li>
            <li><a @click.prevent="editEntityStatus(child.data.row, 'Delay')">Отложено</a></li>
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
          <el-popconfirm
            v-if="isRowEditable && isConfirmDelete"
            :title="
              isMultipleSelected ? 'Удалить выбранные элементы?' : 'Удалить выбранный элемент?'
            "
            @onConfirm="deleteEntity"
          >
            <a slot="reference">Переместить в корзину</a>
          </el-popconfirm>
          <a v-if="isRowEditable && !isConfirmDelete" @click.prevent="deleteEntity">
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
    <task-dialog
      v-if="dialogVisible"
      :id="dialogData"
      @close="dialogVisible = false"
      @submit="reloadData"
    ></task-dialog>
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import Draggable from 'vuedraggable'
import TableMixin from '@/core/mixins/table.mixin'
import TaskDialog from '@/modules/goals/components/goal-window/goal-window.vue'
import { StateChanger } from 'vue-infinite-loading'
import tasksModule from '@/modules/goals/store/goals.store'
import Goal, { Status } from '@/modules/goals/models/goal.type'
import Entity from '@/core/types/entity.type'

@Component({ components: { Draggable, TaskDialog } })
export default class TaskBoard extends Mixins(TableMixin) {
  public data: Goal[] = []
  public lists: { label: string; name: string; items: Goal[] }[] = []
  private loading = false

  private get listsDragOptions() {
    return {
      animation: '200',
      ghostClass: 'ghost',
      handle: '.list__header',
      disabled: false,
      group: 'lists',
    }
  }
  private get itemsDragOptions() {
    return {
      animation: '200',
      ghostClass: 'ghost',
      group: 'list-items',
      disabled: false,
    }
  }
  private get boardLists(): { label: string; name: string; items?: Goal[] }[] {
    if (localStorage.boardLists) return JSON.parse(localStorage.boardLists)
    else
      return [
        { label: 'Новое', name: 'New' },
        { label: 'Выполняется', name: 'Perform' },
        { label: 'Проверяется', name: 'Testing' },
        { label: 'Отложено', name: 'Delay' },
        { label: 'Выполнено', name: 'Succeed' },
        { label: 'Отклонено', name: 'Rejected' },
      ]
  }

  private async onListMove(): Promise<void> {
    const newBoardLists = this.lists.map((list) => {
      return {
        label: list.label,
        name: list.name,
      }
    })
    localStorage.boardLists = JSON.stringify(newBoardLists)
  }

  private async onEntityMove(
    event: { added?: { element: Goal } },
    listName: string
  ): Promise<void> {
    if (event.added) await this.editEntityStatus(event.added.element, listName, false)
  }

  private updateLists(): void {
    this.lists = []
    this.boardLists.forEach((list: { label: string; name: string; items?: Goal[] }) => {
      this.lists.push({
        label: list.label,
        name: list.name,
        items: this.data.filter((item) => item.state === list.name),
      })
    })
  }

  private async loadData($state: StateChanger): Promise<void> {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    const data = await tasksModule.findAll(this.query)
    if (this.query.pageNumber !== undefined) this.query.pageNumber++
    if (data.length) $state.loaded()
    else $state.complete()
    this.data = isFirstLoad ? data : this.data.concat(data)
    this.loading = false
    this.updateLists()
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
    const entity = this.selectedRow as Goal
    if (this.isMultipleSelected) await tasksModule.deleteMany(this.selectionIds as number[])
    else await tasksModule.deleteOne(entity.id as number)
    this.reloadData()
  }

  private async restoreEntity(entity: Goal, multiple = false): Promise<void> {
    if (multiple) await tasksModule.restoreMany(this.selectionIds as number[])
    else await tasksModule.restoreOne(entity.id as number)
    this.reloadData()
  }

  private async editEntityStatus(entity: Goal, status: string, reload = true): Promise<void> {
    if (this.isMultipleSelected) {
      const items = this.table.selection.map((entity: Entity) => {
        const modifiedEntity = entity as Goal
        modifiedEntity.state = status as Status
        return modifiedEntity
      })
      await tasksModule.updateMany(items)
    } else {
      const item = entity
      item.state = status as Status
      await tasksModule.updateOne(item)
    }
    if (reload) this.reloadData()
  }
}
</script>

<style lang="scss" scoped>
.board {
  height: 100%;
  overflow: auto;
  &__wrapper {
    display: flex;
    flex-wrap: nowrap;
  }
}
.list {
  min-width: 260px;
  max-width: 260px;
  margin-right: 20px;
  position: relative;
  width: 100%;
  flex: 0 0 25%;
  &__header {
    cursor: grab;
    border-bottom: 2px solid transparent;
    font-size: 14px;
    font-weight: 500;
    padding: 10px 0;
    border-top-left-radius: 10px;
    border-top-right-radius: 10px;
    &.new {
      border-bottom-color: rgba(33, 150, 243, 0.5);
    }
    &.perform {
      border-bottom-color: rgb(241, 222, 51);
    }
    &.delay {
      border-bottom-color: lightgrey;
    }
    &.testing {
      border-bottom-color: #ff6d37;
    }
    &.succeed {
      border-bottom-color: #00cf3a;
    }
    &.rejected {
      border-bottom-color: #ca0000;
    }
  }
  &__items {
    min-height: 300px;
    overflow: hidden;
    border-bottom-left-radius: 10px;
    border-bottom-right-radius: 10px;
  }
}
.item {
  cursor: grab;
  margin: 10px 0;
  padding: 12px 15px;
  border: 1px solid var(--card-border);
  background-color: var(--card-background);
  border-radius: 8px;
  &__header {
    cursor: pointer;
    color: var(--text);
    font-size: 12.5px;
    line-height: normal;
    &:hover {
      text-decoration: underline;
    }
  }
  &__footer {
    margin-top: 8px;
    display: flex;
    justify-content: space-between;
  }
  &__performer {
    font-size: 11px;
    color: var(--text-placeholder);
  }
  &__date {
    font-size: 11px;
    color: var(--text-muted);
  }
}
</style>
