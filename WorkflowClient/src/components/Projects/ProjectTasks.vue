<template lang="pug">
  div.project-tasks
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
    task-board(v-if="view === 'board'")

</template>

<script>
import Page from '@/components/Page';
import BaseHeader from '@/components/BaseHeader';
import TaskToolbar from '@/components/Tasks/TaskToolbar';
import TaskTable from '@/components/Tasks/TaskTable';
import TaskBoard from '@/components/Tasks/TaskBoard';
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

<style lang="scss" scoped>
.project-tasks {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
