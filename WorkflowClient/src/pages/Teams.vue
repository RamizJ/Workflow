<template lang="pug">
  page
    page-content
      page-header
        template(slot="title") Команды
        template(slot="search")
          el-input(
            v-model="query.filter"
            size="medium"
            placeholder="Поиск"
            @change="applyFilters")
            el-button(slot="prefix" type="text" size="mini")
              feather(type="search" size="16")
            el-button(slot="suffix" type="text" size="mini" :class="filtersVisible ? 'active' : ''" @click="filtersVisible = !filtersVisible")
              feather(type="sliders" size="16")

      page-toolbar
        template(v-if="filtersVisible" slot="filters")
          el-row
            el-select(v-model="filters.status.value" size="small" placeholder="Участник")
              el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
            el-select(v-model="filters.performer.value" size="small" placeholder="Проект")
              el-option(v-for="option in filters.performer.items" :key="option.value" :value="option.value", :label="option.label")

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
          el-table-column(prop="name" label="Команда")
          infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
            div(slot="no-more")
            div(slot="no-results")

        vue-context(ref="contextMenu")
          template(slot-scope="child")
            li Добавить участника
            li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
            li(v-if="!isMultipleSelected" @click.prevent="onItemDelete($event, child.data.row)") Удалить
            li(v-if="isMultipleSelected" @click.prevent="onItemMultipleDelete($event, child.data.row)") Удалить выделенное

    team-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import PageToolbar from '~/components/PageToolbar';
import PageContent from '~/components/PageContent';
import TableContent from '~/components/TableContent';
import TeamDialog from '~/components/TeamDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Teams',
  components: {
    Page,
    PageHeader,
    PageToolbar,
    PageContent,
    TableContent,
    TeamDialog
  },
  mixins: [tableMixin],
  data() {
    return {
      sort: {
        field: 'name',
        type: 'Ascending',
        fields: [{ value: 'name', label: 'По названию' }]
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
      },
      value: ''
    };
  },
  computed: {
    ...mapGetters({ items: 'teams/getTeams' })
  },
  methods: {
    ...mapActions({
      fetchItems: 'teams/fetchTeams',
      deleteItem: 'teams/deleteTeam',
      deleteItems: 'teams/deleteTeams'
    }),
    async applyFilters() {}
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
