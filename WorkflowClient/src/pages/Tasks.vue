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
    this.onSortChange('creationDate');
    this.onOrderChange('Descending');
  }
};
</script>
