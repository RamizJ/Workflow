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
import TeamToolbar from '@/components/Team/TeamToolbar';
import TeamTable from '@/components/Team/TeamTable';
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
    if (!this.$route.query.sort) this.onSortChange('name');
    if (!this.$route.query.order) this.onOrderChange('Ascending');
  }
};
</script>
