<template>
  <div class="page">
    <div class="header">
      <div class="header__title">Задачи</div>
      <div class="header__action">
        <el-button type="text" size="mini" @click="onCreate">Создать</el-button>
      </div>
    </div>
    <task-toolbar
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange"
    ></task-toolbar>
    <task-table
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></task-table>
    <task-board
      v-if="view === 'board'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></task-board>
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import PageMixin from '@/mixins/page.mixin'
import TaskToolbar from '@/components/Task/TaskToolbar.vue'
import TaskTable from '@/components/Task/TaskTable.vue'
import TaskBoard from '@/components/Task/TaskBoard.vue'
import { View } from '@/types/view.type'
import { SortType } from '@/types/query.type'

@Component({
  components: {
    TaskToolbar,
    TaskTable,
    TaskBoard
  }
})
export default class Tasks extends mixins(PageMixin) {
  protected mounted() {
    if (!this.$route.query.sort) this.onSortChange('creationDate')
    if (!this.$route.query.order) this.onOrderChange(SortType.Descending)
  }

  private onCreate() {
    if (this.view === View.List) (this.$refs.items as TaskTable).createEntity()
    if (this.view === View.Board) (this.$refs.items as TaskBoard).createEntity()
  }
}
</script>
