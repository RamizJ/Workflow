<template lang="pug">
  page
    page-content
      page-header
        template(slot="title") Корзина
        template(slot="search")
          el-input(
            v-model="query.filter"
            size="medium"
            placeholder="Поиск"
            @change="refresh")
            el-button(slot="prefix" type="text" size="mini")
              feather(type="search" size="16")
            el-popover(slot="suffix" placement="bottom" width="335" transition="fade" trigger="click")
              el-button(
                slot="reference"
                type="text"
                size="mini"
                :class="filtersActive ? 'active' : ''")
                feather(type="sliders" size="16")

              el-row(:gutter="10" style="margin-bottom: 10px;")
                el-col(:span="12")
                  el-select(
                    v-model="filterFields.state.values"
                    size="small"
                    placeholder="Статус"
                    @change="applyFilters"
                    multiple collapse-tags)
                    el-option(v-for="option in statuses" :key="option.value" :value="option.value", :label="option.label")
                el-col(:span="12")
                  el-select(
                    v-model="filterFields.priority.values"
                    size="small"
                    placeholder="Приоритет"
                    @change="applyFilters"
                    multiple collapse-tags)
                    el-option(v-for="option in priorities" :key="option.value" :value="option.value", :label="option.label")
              el-row(:gutter="10")
                el-col(:span="12")
                  el-select(
                    v-model="filterFields.performer.values[0]"
                    size="small"
                    placeholder="Ответственный"
                    :remote-method="searchUsers"
                    @focus="searchUsers('')"
                    @change="applyFilters"
                    filterable remote clearable default-first-option collapse-tags)
                    el-option(v-for="option in userList" :key="option.value" :value="option.value", :label="option.label")
                el-col(:span="12")
                  el-select(
                    v-model="filterFields.project.values[0]"
                    size="small"
                    placeholder="Проект"
                    :remote-method="searchProjects"
                    @focus="searchProjects('')"
                    @change="applyFilters"
                    filterable remote clearable default-first-option collapse-tags)
                    el-option(v-for="option in projectList" :key="option.value" :value="option.value", :label="option.label")

      page-toolbar
        template(slot="view")
          el-button(type="text" size="mini" @click="switchOrder")
            feather(:type="query.sortFields[0].sortType === 'Ascending' ? 'align-left' : 'align-right'" size="20")
          el-select(
            v-model="query.sortFields[0].fieldName"
            size="medium"
            placeholder="По дате создания"
            @change="applySort"
            clearable)
            el-option(v-for="option in sortFields" :key="option.value" :value="option.value", :label="option.label")
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
          @select="onItemSelect"
          @row-click="onItemSingleClick"
          @row-contextmenu="onItemRightClick"
          @row-dblclick="onItemDoubleClick"
          highlight-current-row border)
          el-table-column(type="selection" width="38")
          el-table-column(prop="title" label="Задача" width="500")
          el-table-column(prop="projectName" label="Проект")
          el-table-column(prop="state" label="Статус" width="120" :formatter="stateFormatter")
          el-table-column(prop="priority" label="Приоритет" width="120" :formatter="priorityFormatter")
          el-table-column(prop="creationDate" label="Добавлено" width="170" :formatter="dateFormatter")
          infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
            div(slot="no-more")
            div(slot="no-results")

        vue-context(ref="contextMenu")
          template(slot-scope="child")
            li(v-if="!isMultipleSelected" @click.prevent="onItemComplete($event, child.data.row)") Восстановить
            li(v-if="isMultipleSelected" @click.prevent="onItemMultipleComplete($event, child.data.row)") Восстановить выделенное
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import PageContent from '~/components/PageContent';
import PageToolbar from '~/components/PageToolbar';
import TableContent from '~/components/TableContent';
import TaskDialog from '~/components/TaskDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Trash',
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
        pageSize: 20,
        filterFields: [{ fieldName: 'isRemoved', values: [true] }],
        withRemoved: true
      },
      sortFields: [
        { value: 'creationDate', label: 'По дате создания' },
        { value: 'title', label: 'По названию' },
        { value: 'state', label: 'По статусу' },
        { value: 'projectName', label: 'По проекту' },
        { value: 'priority', label: 'По приоритету' }
      ],
      filterFields: {
        state: { fieldName: 'state', values: [] },
        priority: { fieldName: 'priority', values: [] },
        performer: { fieldName: 'performerFio', values: [] },
        project: { fieldName: 'projectName', values: [] }
      }
    };
  },
  computed: {
    ...mapGetters({ items: 'tasks/getTasks' })
  },
  methods: {
    ...mapActions({
      fetchItems: 'tasks/fetchTasks',
      deleteItem: 'tasks/deleteTask',
      deleteItems: 'tasks/deleteTasks',
      completeItem: 'tasks/completeTask',
      completeItems: 'tasks/completeTasks'
    }),
    async applyFilters() {
      this.query.filterFields = [];
      if (this.filterFields.priority.values.length)
        this.query.filterFields.push({
          fieldName: this.filterFields.priority.fieldName,
          values: this.filterFields.priority.values
        });
      if (this.filterFields.performer.values.length)
        this.query.filterFields.push({
          fieldName: this.filterFields.performer.fieldName,
          values: this.filterFields.performer.values
        });
      if (this.filterFields.project.values.length)
        this.query.filterFields.push({
          fieldName: this.filterFields.project.fieldName,
          values: this.filterFields.project.values
        });

      this.filtersActive = !!this.query.filterFields.filter(
        field => !!field.values[0]
      ).length;

      this.query.filterFields.push({
        fieldName: 'state',
        values: ['Succeed', 'Rejected']
      });

      if (this.query.filterFields.length > 1) await this.refresh();
    }
  }
};
</script>

<style lang="scss" scoped></style>
