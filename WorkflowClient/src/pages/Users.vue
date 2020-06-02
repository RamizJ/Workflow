<template lang="pug">
  div.container
    base-toolbar
      template(slot="title") Пользователи
      template(slot="subtitle")
        a(href="#" @click="dialogOpened = true") Создать
      template(slot="items")
        base-toolbar-item(title="Поиск")
          el-input(size="medium" placeholder="Искать..." v-model="query.search" @change="refresh")

    div.content
      el-table(
        :data="tableData"
        ref="table"
        height="auto"
        v-loading="loading"
        highlight-current-row
        stripe)
        el-table-column(type="selection" width="55")
        el-table-column(prop="name" label="Дата добавления")
        el-table-column(prop="description" label="Заголовок")
        el-table-column(prop="language" label="Статус")
        infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="400" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
          div(slot="no-more")
          div(slot="no-results")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseToolbar from '~/components/BaseToolbar';
import BaseToolbarItem from '~/components/BaseToolbarItem';

export default {
  name: 'Users',
  components: { BaseToolbar, BaseToolbarItem },
  data() {
    return {
      loading: false,
      tableData: [],
      query: {
        filter: '',
        pageNumber: 1,
        pageSize: 15
      },
      dialogOpened: false,
      selectedItemId: null,
      filters: {

      },
      value: '',
    };
  },
  computed: {
    ...mapGetters({ users: 'users' })
  },
  methods: {
    ...mapActions({ fetchUsers: 'fetchUsers'}),
    async refresh() {
      this.tableData = [];
      this.$refs.loader.stateChanger.reset();
    },
    async load($state) {
      const firstLoad = !this.tableData.length;
      if (firstLoad) this.loading = true;
      await this.fetch(this.query);
      if (this.users.length) $state.loaded(); else $state.complete();
      this.tableData = firstLoad ? this.users : this.tableData.concat(this.users);
      if (firstLoad) this.loading = false;
    },
    async fetch(params) {
      try {
        await this.fetchUsers(params);
        this.query.pageNumber++;
      } catch (e) {
        this.$message.error('Ошибка получения данных');
      }
    }
  }
};
</script>

<style scoped>

</style>