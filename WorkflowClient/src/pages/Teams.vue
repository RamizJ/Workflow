<template lang="pug">
  page
    base-header
      template(slot="title") Команды
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    team-toolbar(
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    team-table(
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
import TeamToolbar from '@/components/Teams/TeamToolbar';
import TeamTable from '@/components/Teams/TeamTable';
import pageMixin from '@/mixins/page.mixin';

export default {
  components: {
    Page,
    BaseHeader,
    TeamToolbar,
    TeamTable
  },
  mixins: [pageMixin],
  created() {
    this.onSortChange('name');
    this.onOrderChange('Ascending');
  }
};
</script>
