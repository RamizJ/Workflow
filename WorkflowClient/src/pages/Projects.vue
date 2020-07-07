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
            el-popover(slot="suffix" placement="bottom" width="270" transition="fade" trigger="click")
              el-button(
                slot="reference"
                type="text"
                size="mini"
                :class="filtersActive ? 'active' : ''")
                feather(type="sliders" size="16")

              el-row(:gutter="10")
                el-col(:span="24")
                  el-select(
                    v-model="filterFields.owner.values[0]"
                    size="small"
                    placeholder="Руководитель"
                    :remote-method="searchUsers"
                    @focus="searchUsers('')"
                    @change="applyFilters"
                    filterable remote clearable default-first-option collapse-tags)
                    el-option(v-for="option in userList" :key="option.value" :value="option.value", :label="option.label")

      page-toolbar
        template(slot="actions")
          transition(name="fade")
            el-button(
              v-if="addButtonVisible"
              size="small"
              @click="dialogVisible = true; dialogData = null")
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
            el-button(
              v-if="editButtonVisible && !isMultipleSelected"
              size="small"
              @click="onItemEdit(null, selectedRow)")
              feather(type="edit-3" size="12")
              span Редактировать

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

    project-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

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
      sortFields: [
        { value: 'creationDate', label: 'По дате создания' },
        { value: 'name', label: 'По названию' },
        { value: 'ownerFio', label: 'По руководителю' }
      ],
      filterFields: {
        owner: { fieldName: 'ownerFio', values: [] }
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
      this.loader.stateChanger.reset();
      this.dialogVisible = false;
      this.editButtonVisible = false;
      this.completeButtonVisible = false;
      this.deleteButtonVisible = false;
      await this.fetchSidebarItems({ reload: true });
    },
    async applyFilters() {
      this.query.filterFields = [];
      if (this.filterFields.owner.values.length)
        this.query.filterFields.push({
          fieldName: this.filterFields.owner.fieldName,
          values: this.filterFields.owner.values
        });

      this.filtersActive = !!this.query.filterFields.filter(
        field => !!field.values[0]
      ).length;

      if (this.query.filterFields.length) await this.refresh();
    },
    onItemDoubleClick(row, column, event) {
      this.$router.push(`/projects/${row.id}`);
    }
  }
};
</script>
