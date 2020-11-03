<template>
  <BasePage>
    <BasePageHeader>
      Проекты
      <ProjectToolbar
        slot="toolbar"
        @search="onSearch"
        @filters="onFiltersChange"
        @order="onOrderChange"
        @sort="onSortChange"
        @view="onViewChange"
      ></ProjectToolbar>
    </BasePageHeader>
    <ProjectTable />
  </BasePage>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import BasePage from '@/core/components/base-page.vue'
import BasePageHeader from '@/core/components/base-page-header.vue'
import ProjectToolbar from '@/modules/projects/components/project-toolbar.vue'
import ProjectTable from '@/modules/projects/components/project-table.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    BasePage,
    BasePageHeader,
    ProjectToolbar,
    ProjectTable,
  },
})
export default class ProjectsPage extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('creationDate')
    if (!this.$route.query.order) this.onOrderChange(SortType.Descending)
  }
}
</script>
