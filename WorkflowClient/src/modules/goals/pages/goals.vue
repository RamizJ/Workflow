<template>
  <div class="page">
    <div class="header">
      <div class="header__title">
        <GoalBreadcrumbs />
      </div>
    </div>
    <task-toolbar
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange"
    ></task-toolbar>
    <GoalTableNew
      v-if="view === 'list'"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    />

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
import TaskTable from '@/modules/goals/components/goal-table/goal-table.vue'
import TaskBoard from '@/modules/goals/components/goal-board.vue'
import { View } from '@/core/types/view.type'
import { SortType } from '@/core/types/query.type'
import GoalTableNew from '@/modules/goals/components/goal-table/goal-table-new.vue'
import goalStore from '@/modules/goals/store/goals.store'
import breadcrumbStore from '@/modules/goals/store/breadcrumb.store'
import GoalBreadcrumbs from '@/modules/goals/components/goal-breadcrumbs.vue'

@Component({
  components: {
    GoalBreadcrumbs,
    GoalTableNew,
    TaskToolbar,
    TaskTable,
    TaskBoard,
  },
})
export default class Goals extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('creationDate')
    if (!this.$route.query.order) this.onOrderChange(SortType.Descending)
  }

  private get breadcrumbs(): { path: string; label: string }[] {
    return breadcrumbStore.breadcrumbs
  }

  private onCreate(): void {
    goalStore.openGoalWindow()
  }
}
</script>
