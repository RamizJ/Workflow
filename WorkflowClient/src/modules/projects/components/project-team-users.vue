<template>
  <div class="project-users">
    <!--    <user-toolbar-->
    <!--      @search="onSearch"-->
    <!--      @filters="onFiltersChange"-->
    <!--      @order="onOrderChange"-->
    <!--      @sort="onSortChange"-->
    <!--      @view="onViewChange"-->
    <!--    ></user-toolbar>-->
    <user-table ref="items" :team-id="teamId"></user-table>
  </div>
</template>

<script lang="ts">
import { Component, Mixins, Prop } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import UserToolbar from '@/modules/users/components/user-toolbar.vue'
import UserTable from '@/modules/users/components/user-table-old.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    UserToolbar,
    UserTable,
  },
})
export default class ProjectTeamUsers extends Mixins(PageMixin) {
  @Prop() readonly teamId!: number

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
