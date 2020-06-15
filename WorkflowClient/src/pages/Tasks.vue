<template lang="pug">
  div.container
    base-header
      template(slot="title") Задачи
      template(slot="action")
        a(href="#" @click="dialogOpened = true") Создать

    base-toolbar
      template(slot="filters")
        base-toolbar-item
          el-input(
            v-model="query.filter"
            size="small"
            placeholder="Поиск"
            @change="applyFilters")
        base-toolbar-item
          el-select(
            v-model="filters.sort.value"
            size="small"
            placeholder="Сортировать"
            @change="applyFilters"
            clearable)
            el-option(v-for="option in filters.sort.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item
          el-select(
            v-model="filters.performer.value"
            size="small"
            placeholder="Ответственный"
            @change="applyFilters"
            multiple collapse-tags)
            el-option(v-for="option in filters.performer.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item
          el-select(
            v-model="filters.priority.value"
            size="small"
            placeholder="Приоритет"
            @change="applyFilters"
            multiple collapse-tags)
            el-option(v-for="option in filters.priority.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item
          el-select(
            v-model="filters.status.value"
            size="small"
            placeholder="Статус"
            @change="applyFilters"
            multiple collapse-tags)
            el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item
          el-select(
            v-model="filters.project.value"
            size="small"
            placeholder="Проект"
            @change="applyFilters"
            multiple collapse-tags)
            el-option(v-for="option in filters.project.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item
          el-select(
            v-model="filters.tags.value"
            size="small"
            placeholder="Метка"
            @change="applyFilters"
            multiple collapse-tags)
            el-option(v-for="option in filters.tags.items" :key="option.value" :value="option.value", :label="option.label")

    div.content
      el-table(
        :data="tableData"
        ref="table"
        height="100%"
        v-loading="loading"
        @row-contextmenu="onItemRightClick"
        @row-dblclick="onItemDoubleClick"
        highlight-current-row)
        el-table-column(type="selection" width="55")
        el-table-column(prop="description" label="Заголовок")
        el-table-column(prop="language" label="Статус")
        el-table-column(prop="language" label="Проект")
        el-table-column(prop="name" label="Добавлено")
        infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
          div(slot="no-more")
          div(slot="no-results")

      vue-context(ref="contextMenu")
        template(slot-scope="child")
          li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
          li Завершить
          li Удалить

    task-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseHeader from '~/components/BaseHeader';
import BaseToolbar from '~/components/BaseToolbar';
import BaseToolbarItem from '~/components/BaseToolbarItem';
import TaskDialog from '~/components/TaskDialog';

export default {
  components: {
    BaseHeader,
    BaseToolbar,
    BaseToolbarItem,
    TaskDialog
  },
  data() {
    return {
      loading: false,
      tableData: [],
      dialogOpened: false,
      selectedItemId: null,
      query: {
        filter: '',
        pageNumber: 1,
        pageSize: 30
      },
      filters: {
        sort: {
          value: null,
          items: [
            { value: 'title', label: 'Название' },
            { value: 'goalState', label: 'Статус' },
            { value: 'scopeId', label: 'Проект' },
            { value: 'creationDate', label: 'Дата создания' }
          ]
        },
        status: {
          fieldName: 'goalState',
          value: null,
          items: [
            { value: 0, label: 'Новое' },
            { value: 1, label: 'Завершённое' }
          ]
        },
        performer: {
          fieldName: 'performer',
          value: null,
          items: []
        },
        project: {
          fieldName: 'scopeId',
          value: null,
          items: []
        },
        priority: {
          fieldName: 'priority',
          value: null,
          items: [
            { value: 0, label: 'Низкий' },
            { value: 1, label: 'Средний' },
            { value: 2, label: 'Высокий' }
          ]
        },
        tags: {
          fieldName: 'tag',
          value: null,
          items: []
        }
      }
    };
  },
  computed: {
    ...mapGetters({ tasks: 'tasks/getTasks' })
  },
  methods: {
    ...mapActions({ fetchTasks: 'tasks/fetchTasks' }),
    async applyFilters() {
      this.query.filterFields = [];
      if (this.filters.status.value)
        this.query.filterFields.push({
          fieldName: this.filters.status.fieldName,
          value: this.filters.status.value
        });
      if (this.filters.performer.value)
        this.query.filterFields.push({
          fieldName: this.filters.performer.fieldName,
          value: this.filters.performer.value
        });
      if (this.filters.project.value)
        this.query.filterFields.push({
          fieldName: this.filters.project.fieldName,
          value: this.filters.project.value
        });
      if (this.filters.priority.value)
        this.query.filterFields.push({
          fieldName: this.filters.priority.fieldName,
          value: this.filters.priority.value
        });
      if (this.filters.tags.value?.length)
        this.query.filterFields.push({
          fieldName: this.filters.tags.fieldName,
          value: this.filters.tags.value
        });
      await this.refresh();
    },
    async refresh() {
      this.tableData = [];
      this.$refs.loader.stateChanger.reset();
    },
    async load($state) {
      const firstLoad = !this.tableData.length;
      if (firstLoad) this.loading = true;
      await this.fetch(this.query);
      if (this.tasks.length) $state.loaded();
      else $state.complete();
      this.tableData = firstLoad
        ? this.tasks
        : this.tableData.concat(this.tasks);
      if (firstLoad) this.loading = false;
    },
    async fetch(params) {
      try {
        await this.fetchTasks(params);
        this.query.pageNumber++;
      } catch (e) {
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
};
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
