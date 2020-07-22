<template lang="pug">
  page
    base-header
      template(slot="title") Задачи
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
      el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.value")
        base-toolbar(
          v-if="activeTab === tab.value"
          :sort-fields="sortFields"
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

</template>

<script>
import Page from '~/components/Page';
import BaseHeader from '~/components/BaseHeader';
import BaseToolbar from '~/components/BaseToolbar';
import TaskTable from '@/components/TaskTable';
import pageMixin from '~/mixins/page.mixin';

export default {
  components: {
    Page,
    BaseHeader,
    BaseToolbar,
    TaskTable
  },
  mixins: [pageMixin],
  data() {
    return {
      sortFields: [
        { value: 'title', label: 'По названию' },
        { value: 'projectName', label: 'По проекту' },
        { value: 'state', label: 'По статусу' },
        { value: 'priority', label: 'По приоритету' },
        { value: 'creationDate', label: 'По дате создания' }
      ]
    };
  }
};
</script>
