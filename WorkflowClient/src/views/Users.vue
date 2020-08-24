<template>
  <page>
    <base-header>
      <template slot="title">Пользователи</template>
      <template slot="action">
        <el-button type="text" size="mini" @click="onCreate">Создать</el-button>
      </template>
    </base-header>
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
  </page>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import PageMixin from '@/mixins/page.mixin'
import Page from '@/components/Page.vue'
import BaseHeader from '@/components/BaseHeader.vue'
import UserToolbar from '@/components/User/UserToolbar.vue'
import UserTable from '@/components/User/UserTable.vue'
import { SortType } from '@/types/query.type'

@Component({
  components: {
    Page,
    BaseHeader,
    UserToolbar,
    UserTable
  }
})
export default class UsersPage extends mixins(PageMixin) {
  private created() {
    if (!this.$route.query.sort) this.onSortChange('lastName')
    if (!this.$route.query.order) this.onOrderChange(SortType.Ascending)
  }
}
</script>
