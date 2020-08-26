<template>
  <div class="project-teams">
    <team-toolbar
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange"
    ></team-toolbar>
    <team-list
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort"
    ></team-list>
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import PageMixin from '@/mixins/page.mixin'
import TeamToolbar from '@/components/Team/TeamToolbar.vue'
import TeamList from '@/components/Team/TeamTable.vue'
import { SortType } from '@/types/query.type'

@Component({
  components: {
    TeamList,
    TeamToolbar
  }
})
export default class ProjectTeams extends mixins(PageMixin) {
  private created() {
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
