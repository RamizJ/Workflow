<template lang="pug">
  page
    base-header
      template(slot="title") Задачи
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    task-toolbar(
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    task-table(
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort")
    task-board(ref="items" v-if="view === 'board'")

</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'
import { SortType } from '@/types/query.type'
import PageMixin from '@/mixins/page.mixin'
import Page from '@/components/Page.vue'
import BaseHeader from '@/components/BaseHeader.vue'
import TaskToolbar from '@/components/Task/TaskToolbar.vue'
import TaskTable from '@/components/Task/TaskTable.vue'
import TaskBoard from '@/components/Task/TaskBoard.vue'
import { View } from '@/types/view.type'

@Component({
  components: {
    Page,
    BaseHeader,
    TaskToolbar,
    TaskTable,
    TaskBoard
  }
})
export default class Tasks extends mixins(PageMixin) {
  private created() {
    if (!this.$route.query.sort) this.onSortChange('creationDate')
    if (!this.$route.query.order) this.onOrderChange(SortType.Descending)
  }

  private onCreate() {
    if (this.view === View.List) (this.$refs.items as TaskTable).createEntity()
    if (this.view === View.Board) (this.$refs.items as TaskBoard).createEntity()
  }
}
</script>
