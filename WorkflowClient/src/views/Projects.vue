<template>
  <page>
    <base-header>
      <template slot="title">Проекты</template>
      <template slot="action">
        <el-button type="text" size="mini" @click="onCreate">Создать</el-button>
      </template>
    </base-header>
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
  </page>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import PageMixin from '@/mixins/page.mixin'
import Page from '@/components/Page.vue'
import BaseHeader from '@/components/BaseHeader.vue'
import ProjectToolbar from '@/components/Project/ProjectToolbar.vue'
import ProjectTable from '@/components/Project/ProjectTable.vue'
import { SortType } from '@/types/query.type'

@Component({
  components: {
    Page,
    BaseHeader,
    ProjectToolbar,
    ProjectTable
  }
})
export default class ProjectsPage extends mixins(PageMixin) {
  private created() {
    if (!this.$route.query.sort) this.onSortChange('creationDate')
    if (!this.$route.query.order) this.onOrderChange(SortType.Descending)
  }
}
</script>
