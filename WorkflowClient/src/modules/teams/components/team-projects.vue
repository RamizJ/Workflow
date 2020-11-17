<template>
  <div class="team-projects">
    <BasePageHeader size="small" height="45" :no-border="true">
      <ProjectToolbar
        @search="onSearch"
        @filters="onFiltersChange"
        @order="onOrderChange"
        @sort="onSortChange"
        @view="onViewChange"
      ></ProjectToolbar>
    </BasePageHeader>
    <ProjectTable
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></ProjectTable>
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import ProjectToolbar from '@/modules/projects/components/project-toolbar.vue'
import ProjectTable from '@/modules/projects/components/project-table.vue'
import { SortType } from '@/core/types/query.type'

@Component({ components: { BasePageHeader, ProjectToolbar, ProjectTable } })
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
