<template lang="pug">
  page
    page-header
      template(slot="title") Задачи
      template(slot="action")
        el-button(type="text" size="mini" @click="openDialog") Создать
      template(slot="search")
        base-search(:query.sync="searchQuery")

    el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
      el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.value")
        table-tasks(v-if="tab.value === activeTab" ref="list" :search="searchQuery" :status="tab.value")

</template>

<script>
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import TableTasks from '~/components/TableTasks';
import BaseSearch from '~/components/BaseSearch';

export default {
  components: {
    BaseSearch,
    Page,
    PageHeader,
    TableTasks
  },
  data() {
    return {
      searchQuery: '',
      activeTab: 'All',
      tabs: [
        { value: 'All', label: 'Все' },
        { value: 'New', label: 'Новые' },
        { value: 'Perform', label: 'Выполняются' },
        { value: 'Testing', label: 'Проверяются' },
        { value: 'Delay', label: 'Отложенные' },
        { value: 'Succeed', label: 'Выполненные' },
        { value: 'Rejected', label: 'Отклоненные' },
        { value: 'Deleted', label: 'Удаленные' }
      ]
    };
  },
  mounted() {
    const query = { ...this.$route.query };
    query.status = query.status || this.activeTab;
    this.activeTab = query.status;
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query))
      this.$router.push({ query });
  },
  methods: {
    openDialog() {
      this.$refs.list[0].onItemCreate();
    },
    onTabClick() {
      const query = { ...this.$route.query };
      query.status = this.activeTab;
      this.$router.push({ query });
    }
  }
};
</script>
