<template>
  <div class="page">
    <div class="header">
      <div class="header__left">Команды</div>
      <div class="header__left">
        <team-toolbar
          @search="onSearch"
          @filters="onFiltersChange"
          @order="onOrderChange"
          @sort="onSortChange"
          @view="onViewChange"
        ></team-toolbar>
      </div>
    </div>
    <TeamTableNew />
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import TeamToolbar from '@/modules/teams/components/team-toolbar.vue'
import TeamTable from '@/modules/teams/components/team-table.vue'
import { SortType } from '@/core/types/query.type'
import TeamTableNew from '@/modules/teams/components/team-table-new.vue'

@Component({
  components: {
    TeamTableNew,
    TeamToolbar,
    TeamTable,
  },
})
export default class TeamsPage extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('name')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }

  private onCreate(): void {
    ;(this.$refs.items as TeamTable).createEntity()
  }
}
</script>
