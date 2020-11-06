<template>
  <BasePage>
    <BasePageHeader>
      <GoalBreadcrumbs />
      <GoalToolbar
        slot="toolbar"
        @search="onSearch"
        @filters="onFiltersChange"
        @order="onOrderChange"
        @sort="onSortChange"
        @view="onViewChange"
      ></GoalToolbar>
    </BasePageHeader>
    <GoalTable v-if="view === 'list'" />
    <GoalBoard
      v-if="view === 'board'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></GoalBoard>
  </BasePage>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import BasePage from '@/core/components/base-page/base-page.vue'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import GoalToolbar from '@/modules/goals/components/goal-toolbar.vue'
import GoalBreadcrumbs from '@/modules/goals/components/goal-breadcrumbs.vue'
import GoalTable from '@/modules/goals/components/goal-table/goal-table.vue'
import GoalBoard from '@/modules/goals/components/goal-board.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    BasePageHeader,
    BasePage,
    GoalBreadcrumbs,
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
}
</script>
