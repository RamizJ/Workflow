<template>
  <div class="page">
    <div class="header">
      <div class="header__left">
        <GoalBreadcrumbs />
      </div>
      <div class="header__right">
        <GoalToolbar
          @search="onSearch"
          @filters="onFiltersChange"
          @order="onOrderChange"
          @sort="onSortChange"
          @view="onViewChange"
        ></GoalToolbar>
      </div>
    </div>

    <GoalTableNew
      v-if="view === 'list'"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    />

    <GoalBoard
      v-if="view === 'board'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></GoalBoard>
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import goalStore from '@/modules/goals/store/goals.store'
import breadcrumbStore from '@/modules/goals/store/breadcrumb.store'
import GoalToolbar from '@/modules/goals/components/goal-toolbar.vue'
import GoalTable from '@/modules/goals/components/goal-table/goal-table.vue'
import GoalBoard from '@/modules/goals/components/goal-board.vue'
import GoalTableNew from '@/modules/goals/components/goal-table/goal-table-new.vue'
import GoalBreadcrumbs from '@/modules/goals/components/goal-breadcrumbs.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    GoalBreadcrumbs,
    GoalTableNew,
    GoalToolbar,
    GoalTable,
    GoalBoard,
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
