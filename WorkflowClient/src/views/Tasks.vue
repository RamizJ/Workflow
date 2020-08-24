<template lang="pug">
  page
    base-header
      template(slot="title") Задачи
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    task-toolbar(
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    task-table(
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort")
    task-board(ref="items" v-if="view === 'board'")

</template>

<script>
import Page from '@/components/Page';
import BaseHeader from '@/components/BaseHeader';
import TaskToolbar from '@/components/Task/TaskToolbar';
import TaskTable from '@/components/Task/TaskTable';
import TaskBoard from '@/components/Task/TaskBoard';
import pageMixin from '@/mixins/page.mixin';

export default {
  components: {
    Page,
    BaseHeader,
    TaskToolbar,
    TaskTable,
    TaskBoard
  },
  mixins: [pageMixin],
  created() {
    if (!this.$route.query.sort) this.onSortChange('creationDate');
    if (!this.$route.query.order) this.onOrderChange('Descending');
  }
};
</script>
