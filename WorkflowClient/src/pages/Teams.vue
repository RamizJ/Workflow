<template lang="pug">
  div.container
    base-toolbar
      template(slot="title") Команды
      template(slot="subtitle")
        a(href="#" @click="dialogOpened = true") Создать
      template(slot="items")
        base-toolbar-item(title="Поиск")
          el-input(v-model="query.filter" size="small" placeholder="Искать..." @change="refresh")
        base-toolbar-item(title="Участник")
          el-select(v-model="filters.status.value" size="small" placeholder="Любой")
            el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item(title="Проект")
          el-select(v-model="filters.performer.value" size="small" placeholder="Любой")
            el-option(v-for="option in filters.performer.items" :key="option.value" :value="option.value", :label="option.label")

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

    team-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseToolbar from '~/components/BaseToolbar';
import BaseToolbarItem from '~/components/BaseToolbarItem';
import TeamDialog from '~/components/TeamDialog';

export default {
  name: "Teams",
  components: {
    BaseToolbar,
    BaseToolbarItem,
    TeamDialog
  },
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
        status: {
          value: null,
          field: 'goalState',
          items: [
            { value: 0, label: 'Новое' },
            { value: 1, label: 'Завершённое' },
          ]
        },
        performer: {
          value: null,
          field: 'performer',
          items: []
        },
        project: {
          value: null,
          field: 'project',
          items: []
        },
        priority: {
          value: null,
          field: 'priority',
          items: [
            { value: 0, label: 'Низкий' },
            { value: 1, label: 'Средний' },
            { value: 2, label: 'Высокий' },
          ]
        },
        tag: {
          value: null,
          field: 'tag',
          items: []
        },
      },
      value: '',
    };
  },
  computed: {
    ...mapGetters({ teams: 'teams/getTeams' })
  },
  methods: {
    ...mapActions({ fetchTeams: 'teams/fetchTeams' }),
    async refresh() {
      this.tableData = [];
      this.$refs.loader.stateChanger.reset();
    },
    async load($state) {
      const firstLoad = !this.tableData.length;
      if (firstLoad) this.loading = true;
      await this.fetch(this.query);
      if (this.teams.length) $state.loaded(); else $state.complete();
      this.tableData = firstLoad ? this.teams : this.tableData.concat(this.teams);
      if (firstLoad) this.loading = false;
    },
    async fetch(params) {
      try {
        await this.fetchTeams(params);
        this.query.pageNumber++;
      } catch (e) {
        this.$message.error('Ошибка получения данных');
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.content {
  display: flex;
  position: relative;
  overflow: hidden;
  flex: 1;
  height: 100%;
  padding: 0 30px;
  .el-table {
    overflow: auto;
    position: unset !important;
  }
}
</style>
