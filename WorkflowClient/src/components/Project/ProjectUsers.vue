<template>
  <div class="project-users">
    <user-toolbar
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange"
    ></user-toolbar>
    <user-table
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></user-table>
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import PageMixin from '@/mixins/page.mixin'
import UserToolbar from '@/components/User/UserToolbar.vue'
import UserTable from '@/components/User/UserTable.vue'
import { SortType } from '@/types/query.type'

@Component({
  components: {
    UserToolbar,
    UserTable,
  },
})
export default class ProjectUsers extends mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('name')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }
}
</script>

<style lang="scss" scoped>
.project-users {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
