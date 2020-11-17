<template>
  <div class="project-tasks">
    <BasePageHeader size="small" height="45" :no-border="true">
      <GoalBreadcrumbs header-size="20" :root="{ path: $route.path, label: 'Задачи по проекту' }" />
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
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import GoalToolbar from '@/modules/goals/components/goal-toolbar.vue'
import GoalBoard from '@/modules/goals/components/goal-board.vue'
import GoalTable from '@/modules/goals/components/goal-table/goal-table.vue'
import GoalBreadcrumbs from '@/modules/goals/components/goal-breadcrumbs.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    BasePageHeader,
    GoalBreadcrumbs,
    GoalTable,
    GoalToolbar,
    GoalBoard,
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
