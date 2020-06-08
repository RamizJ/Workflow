<template lang="pug">
  div.container
    base-toolbar
      template(slot="title") Проекты
      template(slot="subtitle")
        a(href="#" @click="dialogOpened = true") Создать
      template(slot="items")
        base-toolbar-item(title="Поиск")
          el-input(v-model="query.filter" size="small" placeholder="Искать..." @change="refresh")
        base-toolbar-item(title="Команда")
          el-select(v-model="filters.status.value" size="small" placeholder="Любая")
            el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item(title="Область")
          el-select(v-model="filters.status.value" size="small" placeholder="Любая")
            el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item(title="Руководитель")
          el-select(v-model="filters.performer.value" size="small" placeholder="Любой")
            el-option(v-for="option in filters.performer.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item(title="Тег")
          el-select(v-model="filters.tag.value" size="small" placeholder="Любой")
            el-option(v-for="option in filters.tag.items" :key="option.value" :value="option.value", :label="option.label")

    div.content
      el-table(
        :data="tableData"
        ref="table"
        height="auto"
        v-loading="loading"
        @row-contextmenu="onItemRightClick"
        @row-dblclick="onItemDoubleClick"
        highlight-current-row
        stripe)
        el-table-column(type="selection" width="55")
        el-table-column(prop="name" label="Дата добавления")
        el-table-column(prop="description" label="Заголовок")
        el-table-column(prop="language" label="Статус")
        infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="400" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
          div(slot="no-more")
          div(slot="no-results")

      vue-context(ref="contextMenu")
        template(slot-scope="child")
          li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
          li Добавить задачу
          li Удалить

    project-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseToolbar from "~/components/BaseToolbar";
import BaseToolbarItem from "~/components/BaseToolbarItem";
import ProjectDialog from '~/components/ProjectDialog';

export default {
  name: "Projects",
  components: { BaseToolbar, BaseToolbarItem, ProjectDialog },
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
    };
  },
  computed: {
    ...mapGetters({ projects: 'projects/getProjects' })
  },
  methods: {
    ...mapActions({ fetchProjects: 'projects/fetchProjects' }),
    async refresh() {
      this.tableData = [];
      this.$refs.loader.stateChanger.reset();
    },
    async load($state) {
      const firstLoad = !this.tableData.length;
      if (firstLoad) this.loading = true;
      await this.fetch(this.query);
      if (this.projects.length) $state.loaded(); else $state.complete();
      this.tableData = firstLoad ? this.getProjects : this.tableData.concat(this.getProjects);
      if (firstLoad) this.loading = false;
    },
    async fetch(params) {
      try {
        await this.fetchProjects(params);
        this.query.pageNumber++;
      } catch (e) {
        console.error(e);
        this.$message.error('Ошибка получения данных');
      }
    },
    onItemRightClick(row, column, event) {
      this.$refs.contextMenu.open(event, { row, column });
      event.preventDefault();
    },
    onItemDoubleClick(row, column, event) {
      this.onItemEdit(event, row);
    },
    onItemEdit(event, row) {
      this.selectedItemId = row.id;
      this.dialogOpened = true;
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
