<template>
  <div class="page">
    <div class="header">
      <div class="header__left">Пользователи</div>
      <div class="header__right">
        <user-toolbar
          @search="onSearch"
          @filters="onFiltersChange"
          @order="onOrderChange"
          @sort="onSortChange"
          @view="onViewChange"
        ></user-toolbar>
      </div>
    </div>

    <UserTableNew />

    <!--    <user-table-->
    <!--      v-if="view === 'list'"-->
    <!--      ref="items"-->
    <!--      :search="search"-->
    <!--      :filters="filters"-->
    <!--      :order="order"-->
    <!--      :sort="sort"-->
    <!--    ></user-table>-->
  </div>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import PageMixin from '@/core/mixins/page.mixin'
import UserToolbar from '@/modules/users/components/user-toolbar.vue'
import UserTable from '@/modules/users/components/user-table.vue'
import { SortType } from '@/core/types/query.type'
import UserTableNew from '@/modules/users/components/user-table-new.vue'

@Component({
  components: {
    UserTableNew,
    UserToolbar,
    UserTable,
  },
})
export default class UsersPage extends Mixins(PageMixin) {
  protected mounted(): void {
    if (!this.$route.query.sort) this.onSortChange('lastName')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }

  private onCreate(): void {
    ;(this.$refs.items as UserTable).createEntity()
  }
}
</script>
