<template>
  <div class="page">
    <div class="header">
      <div class="header__title">
        Пользователи
        <div class="header__action">
          <el-button type="text" size="mini" @click="onCreate">Создать</el-button>
        </div>
      </div>
    </div>
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
