<template>
  <BasePage>
    <BasePageHeader>
      Команды
      <TeamToolbar
        slot="toolbar"
        @search="onSearch"
        @filters="onFiltersChange"
        @order="onOrderChange"
        @sort="onSortChange"
        @view="onViewChange"
      ></TeamToolbar>
    </BasePageHeader>
    <TeamTable />
  </BasePage>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import BasePage from '@/core/components/base-page/base-page.vue'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import TeamToolbar from '@/modules/teams/components/team-toolbar.vue'
import TeamTable from '@/modules/teams/components/team-table.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    BasePageHeader,
    BasePage,
    TeamToolbar,
    TeamTable,
  },
})
export default class TeamsPage extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('name')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }
}
</script>
