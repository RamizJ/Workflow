<template lang="pug">
  page
    page-header
      template(slot="title") Пользователи
      template(slot="action")
        el-button(type="text" size="mini" @click="openDialog") Создать
      template(slot="search")
        base-search(:query.sync="searchQuery")

    el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
      el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.value")
        table-users(v-if="tab.value === activeTab" ref="users" :search="searchQuery" :status="tab.value")

</template>

<script>
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import TableUsers from '~/components/TableUsers';
import BaseSearch from '~/components/BaseSearch';

export default {
  components: {
    BaseSearch,
    Page,
    PageHeader,
    TableUsers
  },
  data() {
    return {
      searchQuery: '',
      activeTab: 'All',
      tabs: [
        { value: 'All', label: 'Активные' },
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
      this.$refs.users[0].dialogVisible = true;
    },
    onTabClick() {
      const query = { ...this.$route.query };
      query.status = this.activeTab;
      this.$router.push({ query });
    }
  }
};
</script>
