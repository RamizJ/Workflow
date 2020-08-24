<template>
  <page>
    <base-header>
      <template slot="title">Команды</template>
      <template slot="action">
        <el-button type="text" size="mini" @click="onCreate">Создать</el-button>
      </template>
    </base-header>
    <team-toolbar
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange"
    ></team-toolbar>
    <team-table
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></team-table>
  </page>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import PageMixin from '@/mixins/page.mixin'
import Page from '@/components/Page.vue'
import BaseHeader from '@/components/BaseHeader.vue'
import TeamToolbar from '@/components/Team/TeamToolbar.vue'
import TeamTable from '@/components/Team/TeamTable.vue'
import { SortType } from '@/types/query.type'

@Component({
  components: {
    Page,
    BaseHeader,
    TeamToolbar,
    TeamTable
  }
})
export default class TeamsPage extends mixins(PageMixin) {
  private created() {
    if (!this.$route.query.sort) this.onSortChange('name')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }
}
</script>
