<template>
  <div class="page">
    <div class="header">
      <div class="header__left">Проекты</div>
      <div class="header__right">
        <project-toolbar
          @search="onSearch"
          @filters="onFiltersChange"
          @order="onOrderChange"
          @sort="onSortChange"
          @view="onViewChange"
        ></project-toolbar>
      </div>
    </div>

    <ProjectTableNew />

    <!--    <project-table-->
    <!--      ref="items"-->
    <!--      :search="search"-->
    <!--      :filters="filters"-->
    <!--      :order="order"-->
    <!--      :sort="sort"-->
    <!--    ></project-table>-->
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import ProjectToolbar from '@/modules/projects/components/project-toolbar.vue'
import ProjectTable from '@/modules/projects/components/project-table.vue'
import { SortType } from '@/core/types/query.type'
import ProjectTableNew from '@/modules/projects/components/project-table-new.vue'

@Component({
  components: {
    ProjectTableNew,
    ProjectToolbar,
    ProjectTable,
  },
})
export default class ProjectsPage extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('creationDate')
    if (!this.$route.query.order) this.onOrderChange(SortType.Descending)
  }

  private onCreate() {
    ;(this.$refs.items as ProjectTable).createEntity()
  }
}
</script>
