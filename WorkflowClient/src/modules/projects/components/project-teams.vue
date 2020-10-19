<template>
  <div class="project-teams">
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
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import TeamToolbar from '@/modules/teams/components/team-toolbar.vue'
import TeamTable from '@/modules/teams/components/team-table.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    TeamTable,
    TeamToolbar,
  },
})
export default class ProjectTeams extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('name')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }
}
</script>

<style lang="scss" scoped>
.project-teams {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
