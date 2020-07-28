<template lang="pug">
  div.project-tasks
    task-toolbar(
      @search="onSearch"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    task-table(
      v-if="view === 'list'"
      ref="items"
      :search="search"
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
  data() {
    return {
      sortFields: [
        { value: 'creationDate', label: 'По дате создания' },
        { value: 'title', label: 'По названию' },
        { value: 'state', label: 'По статусу' },
        { value: 'projectName', label: 'По проекту' },
        { value: 'priority', label: 'По приоритету' }
      ]
    };
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
