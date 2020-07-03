<template lang="pug">
  page
    page-content
      page-header
        template(slot="title") Проекты
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
            el-select(v-model="filters.status.value" size="small" placeholder="Команда")
              el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
            el-select(v-model="filters.status.value" size="small" placeholder="Область")
              el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
            el-select(v-model="filters.performer.value" size="small" placeholder="Руководитель")
              el-option(v-for="option in filters.performer.items" :key="option.value" :value="option.value", :label="option.label")
            el-select(v-model="filters.tag.value" size="small" placeholder="Тег")
              el-option(v-for="option in filters.tag.items" :key="option.value" :value="option.value", :label="option.label")

        template(slot="actions")
          transition(name="fade")
            el-button(
              v-if="addButtonVisible"
              size="small"
              @click="dialogOpened = true; selectedItemId = null")
              feather(type="plus" size="12")
              span Создать
          transition(name="fade")
            el-button(
              v-if="deleteButtonVisible"
              size="small"
              @click="isMultipleSelected ? onItemMultipleDelete(null, selectedRow) : onItemDelete(null, selectedRow)")
              feather(type="trash" size="12")
              span {{ isMultipleSelected ? 'Удалить выделенное' : 'Удалить' }}
          transition(name="fade")
            el-button(v-if="editButtonVisible && !isMultipleSelected" size="small")
              feather(type="edit-3" size="12")
              span Редактировать

        template(slot="view")
          el-button(type="text" size="mini" @click="switchSortType")
            feather(:type="sort.type === 'Ascending' ? 'align-left' : 'align-right'" size="20")
          el-select(
            v-model="sort.field"
            size="medium"
            placeholder="По дате создания"
            @change="applySort"
            clearable)
            el-option(v-for="option in sort.fields" :key="option.value" :value="option.value", :label="option.label")
          el-button(type="text" size="mini")
            feather(type="grid" size="20")
          el-button.active(type="text" size="mini")
            feather(type="list" size="20")

      table-content
        el-table(
          :data="tableData"
          ref="table"
          height="auto"
          v-loading="loading"
          @select="onItemSelect"
          @row-click="onItemSingleClick"
          @row-contextmenu="onItemRightClick"
          @row-dblclick="onItemDoubleClick"
          highlight-current-row border)
          el-table-column(type="selection" width="38")
          el-table-column(prop="name" label="Проект")
          el-table-column(prop="ownerFio" label="Руководитель" width="250")
          el-table-column(prop="creationDate" label="Добавлено" width="170" :formatter="dateFormatter")
          infinite-loading(slot="append" ref="loader" spinner="waveDots" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
            div(slot="no-more")
            div(slot="no-results")

        vue-context(ref="contextMenu")
          template(slot-scope="child")
            //li Добавить задачу
            li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
            li(v-if="!isMultipleSelected" @click.prevent="onItemDelete($event, child.data.row)") Удалить
            li(v-if="isMultipleSelected" @click.prevent="onItemMultipleDelete($event, child.data.row)") Удалить выделенное

    project-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import PageToolbar from '~/components/PageToolbar';
import PageContent from '~/components/PageContent';
import TableContent from '~/components/TableContent';
import ProjectDialog from '~/components/ProjectDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Projects',
  components: {
    Page,
    PageHeader,
    PageToolbar,
    PageContent,
    TableContent,
    ProjectDialog
  },
  mixins: [tableMixin],
  data() {
    return {
      sort: {
        field: 'creationDate',
        type: 'Descending',
        fields: [
          { value: 'creationDate', label: 'По дате создания' },
          { value: 'name', label: 'По названию' },
          { value: 'ownerFio', label: 'По руководителю' },
          { value: 'teamName', label: 'По команде' },
          { value: 'groupName', label: 'По области' }
        ]
      },
      filtersVisible: false,
      filters: {
        status: {
          value: null,
          field: 'goalState',
          items: [
            { value: 0, label: 'Новое' },
            { value: 1, label: 'Завершённое' }
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
            { value: 2, label: 'Высокий' }
          ]
        },
        tag: {
          value: null,
          field: 'tag',
          items: []
        }
      }
    };
  },
  computed: {
    ...mapGetters({ items: 'projects/getProjects' })
  },
  methods: {
    ...mapActions({
      fetchItems: 'projects/fetchProjects',
      fetchSidebarItems: 'projects/fetchSidebarProjects',
      deleteItem: 'projects/deleteProject',
      deleteItems: 'projects/deleteProjects'
    }),
    async refresh() {
      this.tableData = [];
      this.query.pageNumber = 0;
      this.$refs.loader.stateChanger.reset();
      await this.fetchSidebarItems({ reload: true });
      this.dialogOpened = false;
    },
    async applyFilters() {},
    onItemDoubleClick(row, column, event) {
      console.log(row);
      this.$router.push(`/projects/${row.id}`);
      // this.onItemEdit(event, row);
    }
  }
};
</script>
