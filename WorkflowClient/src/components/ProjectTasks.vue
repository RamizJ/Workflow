<template lang="pug">
  div.project-tasks
    base-toolbar(
      :sort-fields="sortFields"
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
import Page from '~/components/Page';
import BaseHeader from '~/components/BaseHeader';
import BaseToolbar from '~/components/BaseToolbar';
import TaskTable from '@/components/TaskTable';
import TaskBoard from '@/components/TaskBoard';
import pageMixin from '~/mixins/page.mixin';

export default {
  components: {
    Page,
    BaseHeader,
    BaseToolbar,
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
