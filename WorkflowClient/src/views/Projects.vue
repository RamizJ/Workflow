<template>
  <div class="page">
    <div class="header">
      <div class="header__title">Проекты</div>
      <div class="header__action">
        <el-button type="text" size="mini" @click="onCreate">Создать</el-button>
      </div>
    </div>
    <project-toolbar
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange"
    ></project-toolbar>
    <project-table
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
import PageMixin from '@/mixins/page.mixin'
import ProjectToolbar from '@/components/Project/ProjectToolbar.vue'
import ProjectTable from '@/components/Project/ProjectTable.vue'
import { SortType } from '@/types/query.type'

@Component({
  components: {
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
