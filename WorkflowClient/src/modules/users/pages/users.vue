<template>
  <BasePage>
    <BasePageHeader>
      Пользователи
      <UserToolbar
        slot="toolbar"
        @search="onSearch"
        @filters="onFiltersChange"
        @order="onOrderChange"
        @sort="onSortChange"
        @view="onViewChange"
      ></UserToolbar>
    </BasePageHeader>
    <UserTable />
  </BasePage>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import BasePage from '@/core/components/base-page.vue'
import BasePageHeader from '@/core/components/base-page-header.vue'
import UserToolbar from '@/modules/users/components/user-toolbar.vue'
import UserTable from '@/modules/users/components/user-table.vue'
import { SortType } from '@/core/types/query.type'

@Component({
  components: {
    BasePageHeader,
    BasePage,
    UserToolbar,
    UserTable,
  },
})
export default class UsersPage extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('lastName')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }
}
</script>
