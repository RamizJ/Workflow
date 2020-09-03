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
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import PageMixin from '@/mixins/page.mixin'
import ProjectToolbar from '@/components/Project/ProjectToolbar.vue'
import ProjectTable from '@/components/Project/ProjectTable.vue'
import { SortType } from '@/types/query.type'

@Component({ components: { ProjectToolbar, ProjectTable } })
export default class TeamProjects extends mixins(PageMixin) {
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
