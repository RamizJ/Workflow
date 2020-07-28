<template lang="pug">
  page
    base-header
      template(slot="title") Пользователи
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    user-toolbar(
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    user-table(
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort")

</template>

<script>
import Page from '@/components/Page';
import BaseHeader from '@/components/BaseHeader';
import UserToolbar from '@/components/Users/UserToolbar';
import UserTable from '@/components/Users/UserTable';
import pageMixin from '@/mixins/page.mixin';

export default {
  components: {
    Page,
    BaseHeader,
    UserToolbar,
    UserTable
  },
  mixins: [pageMixin],
  created() {
    this.onSortChange('lastName');
    this.onOrderChange('Ascending');
  }
};
</script>
