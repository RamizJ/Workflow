<template>
  <div class="page">
    <div class="header">
      <div class="header__title">Команды</div>
      <div class="header__action">
        <el-button type="text" size="mini" @click="onCreate">Создать</el-button>
      </div>
    </div>
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
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import PageMixin from '@/mixins/page.mixin'
import TeamToolbar from '@/components/Team/TeamToolbar.vue'
import TeamTable from '@/components/Team/TeamTable.vue'
import { SortType } from '@/types/query.type'

@Component({
  components: {
    TeamToolbar,
    TeamTable
  }
})
export default class TeamsPage extends mixins(PageMixin) {
  private created() {
    if (!this.$route.query.sort) this.onSortChange('name')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }

  private onCreate() {
    ;(this.$refs.items as TeamTable).createEntity()
  }
}
</script>
