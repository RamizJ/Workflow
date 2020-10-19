<template>
  <div class="project-tasks">
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
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import TaskToolbar from '@/modules/goals/components/goal-toolbar.vue'
import TaskTable from '@/modules/goals/components/goal-table.vue'
import TaskBoard from '@/modules/goals/components/goal-board.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    TaskToolbar,
    TaskTable,
    TaskBoard,
  },
})
export default class ProjectTasks extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('creationDate')
    if (!this.$route.query.order) this.onOrderChange(SortType.Descending)
  }
}
</script>

<style lang="scss" scoped>
.project-tasks {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
