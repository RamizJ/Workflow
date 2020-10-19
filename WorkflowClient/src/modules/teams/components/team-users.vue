<template>
  <div class="team-users">
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
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import UserToolbar from '@/modules/users/components/user-toolbar.vue'
import UserTable from '@/modules/users/components/user-table.vue'
import { SortType } from '@/core/types/query.type'

@Component({ components: { UserToolbar, UserTable } })
export default class TeamUsers extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('lastName')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }
}
</script>

<style lang="scss" scoped>
.team-users {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
