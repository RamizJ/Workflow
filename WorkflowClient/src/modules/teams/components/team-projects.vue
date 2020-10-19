<template>
  <div class="team-projects">
    <project-toolbar
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange"
    ></project-toolbar>
    <project-table
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></project-table>
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import ProjectToolbar from '@/modules/projects/components/project-toolbar.vue'
import ProjectTable from '@/modules/projects/components/project-table.vue'
import { SortType } from '@/core/types/query.type'

@Component({ components: { ProjectToolbar, ProjectTable } })
export default class TeamProjects extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('creationDate')
    if (!this.$route.query.order) this.onOrderChange(SortType.Descending)
  }
}
</script>

<style lang="scss" scoped>
.team-projects {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
