<template lang="pug">
  page
    base-header
      template(slot="title") Задачи
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
      el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.value")
        task-toolbar(
          v-if="activeTab === tab.value"
          @search="onSearch"
          @order="onOrderChange"
          @sort="onSortChange"
          @view="onViewChange")
        task-table(
          v-if="activeTab === tab.value && view === 'list'"
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
  mixins: [pageMixin]
};
</script>
