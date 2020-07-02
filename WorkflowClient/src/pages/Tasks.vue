<template lang="pug">
  page
    page-content
      page-header
        template(slot="title") Задачи
        template(slot="search")
          el-input(
            v-model="query.filter"
            size="medium"
            placeholder="Поиск"
            @change="refresh")
            el-button(slot="prefix" type="text" size="mini")
              feather(type="search" size="16")
            el-button(slot="suffix" type="text" size="mini" :class="filtersVisible ? 'active' : ''" @click="filtersVisible = !filtersVisible")
              feather(type="sliders" size="16")

      page-toolbar
        template(v-if="filtersVisible" slot="filters")
          el-row
            el-select(
              v-model="filters.performer.value"
              size="small"
              placeholder="Ответственный"
              @change="applyFilters"
              multiple collapse-tags)
              el-option(v-for="option in filters.performer.items" :key="option.value" :value="option.value", :label="option.label")
            el-select(
              v-model="filters.priority.value"
              size="small"
              placeholder="Приоритет"
              @change="applyFilters"
              multiple collapse-tags)
              el-option(v-for="option in filters.priority.items" :key="option.value" :value="option.value", :label="option.label")
            el-select(
              v-model="filters.status.value"
              size="small"
              placeholder="Статус"
              @change="applyFilters"
              multiple collapse-tags)
              el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
          el-row
            el-select(
              v-model="filters.project.value"
              size="small"
              placeholder="Проект"
              @change="applyFilters"
              multiple collapse-tags)
              el-option(v-for="option in filters.project.items" :key="option.value" :value="option.value", :label="option.label")
            el-select(
              v-model="filters.tags.value"
              size="small"
              placeholder="Тег"
              @change="applyFilters"
              multiple collapse-tags)
              el-option(v-for="option in filters.tags.items" :key="option.value" :value="option.value", :label="option.label")

        template(slot="actions")
          el-button(size="mini" @click="dialogOpened = true; selectedItemId = null")
            feather(type="plus" size="16")
          el-button(size="mini")
            feather(type="edit-3" size="16")
          el-button(size="mini")
            feather(type="check" size="16")
          el-button(size="mini")
            feather(type="trash" size="16")

        template(slot="view")
          el-select(
            v-model="filters.sort.value"
            size="medium"
            placeholder="По дате создания"
            @change="applyFilters"
            clearable)
            el-button(slot="prefix" type="text" size="mini")
              feather(type="align-right" size="18")
            el-option(v-for="option in filters.sort.items" :key="option.value" :value="option.value", :label="option.label")
            el-divider
            el-option(value="acs" label="Возрастанию")
            el-option(value="desc" label="Убыванию")
          el-button(type="text" size="mini")
            feather(type="grid" size="20")
          el-button.active(type="text" size="mini")
            feather(type="menu" size="20")

      table-content
        el-table(
          :data="tableData"
          ref="table"
          height="100%"
          v-loading="loading"
          @row-contextmenu="onItemRightClick"
          @row-dblclick="onItemDoubleClick"
          highlight-current-row)
          el-table-column(type="selection" width="55")
          el-table-column(prop="title" label="Название")
          el-table-column(prop="description" label="Заметки")
          el-table-column(prop="projectName" label="Проект")
          el-table-column(prop="creationDate" label="Добавлено" :formatter="dateFormatter")
          infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
            div(slot="no-more")
            div(slot="no-results")

        vue-context(ref="contextMenu")
          template(slot-scope="child")
            li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
            li(v-if="!isMultipleSelected" @click.prevent="onItemComplete($event, child.data.row)") Завершить
            li(v-if="!isMultipleSelected" @click.prevent="onItemDelete($event, child.data.row)") Удалить
            li(v-if="isMultipleSelected" @click.prevent="onItemMultipleComplete($event, child.data.row)") Завершить выделенное
            li(v-if="isMultipleSelected" @click.prevent="onItemMultipleDelete($event, child.data.row)") Удалить выделенное

    task-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import PageToolbar from '~/components/PageToolbar';
import PageContent from '~/components/PageContent';
import TableContent from '~/components/TableContent';
import TaskDialog from '~/components/TaskDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  components: {
    Page,
    PageHeader,
    PageToolbar,
    PageContent,
    TableContent,
    TaskDialog
  },
  mixins: [tableMixin],
  data() {
    return {
      query: {
        filter: '',
        pageNumber: 0,
        pageSize: 30
      },
      filtersVisible: false,
      filters: {
        sort: {
          value: null,
          items: [
            { value: 'title', label: 'Названию' },
            { value: 'goalState', label: 'Статусу' },
            { value: 'scopeId', label: 'Проекту' },
            { value: 'creationDate', label: 'Дате создания' }
          ]
        },
        status: {
          fieldName: 'goalState',
          value: null,
          items: [
            { value: 'New', label: 'Новое' },
            { value: 'Completed', label: 'Завершённое' }
          ]
        },
        performer: {
          fieldName: 'performer',
          value: null,
          items: []
        },
        project: {
          fieldName: 'projectId',
          value: null,
          items: []
        },
        priority: {
          fieldName: 'priority',
          value: null,
          items: [
            { value: 'Low', label: 'Низкий' },
            { value: 'Normal', label: 'Обычный' },
            { value: 'High', label: 'Высокий' }
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
    ...mapGetters({ items: 'tasks/getTasks' })
  },
  methods: {
    ...mapActions({
      fetchItems: 'tasks/fetchTasks',
      deleteItem: 'tasks/deleteTask'
    }),
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
    }
  }
};
</script>
