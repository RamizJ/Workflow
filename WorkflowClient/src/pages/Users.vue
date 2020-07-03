<template lang="pug">
  page
    page-content
      page-header
        template(slot="title") Пользователи
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
            el-select(
              v-model="filters.position.value"
              size="small"
              placeholder="Должность"
              @change="applyFilters"
              multiple collapse-tags)
              el-option(v-for="option in filters.position.items" :key="option.value" :value="option.value", :label="option.label")

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
          el-table-column(prop="lastName" label="Фамилия" width="150")
          el-table-column(prop="firstName" label="Имя" width="150")
          el-table-column(prop="middleName" label="Отчество" width="150")
          el-table-column(prop="userName" label="Логин" width="150")
          el-table-column(prop="email" label="Почта")
          el-table-column(prop="phone" label="Телефон" width="120")
          el-table-column(prop="position" label="Должность" width="180")
          infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="400" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
            div(slot="no-more")
            div(slot="no-results")

        vue-context(ref="contextMenu")
          template(slot-scope="child")
            li(@click.prevent="onItemAddToTeam($event, child.data.row)") Добавить в команду...
            li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
            li(v-if="!isMultipleSelected" @click.prevent="onItemDelete($event, child.data.row)") Удалить
            li(v-if="isMultipleSelected" @click.prevent="onItemMultipleDelete($event, child.data.row)") Удалить выделенное

    user-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import PageToolbar from '~/components/PageToolbar';
import PageContent from '~/components/PageContent';
import TableContent from '~/components/TableContent';
import UserDialog from '~/components/UserDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Users',
  components: {
    Page,
    PageHeader,
    PageToolbar,
    PageContent,
    TableContent,
    UserDialog
  },
  mixins: [tableMixin],
  data() {
    return {
      sort: {
        field: 'lastName',
        type: 'Ascending',
        fields: [
          { value: 'lastName', label: 'По фамилии' },
          { value: 'firstName', label: 'По имени' },
          { value: 'middleName', label: 'По отчеству' },
          { value: 'email', label: 'По почте' }
        ]
      },
      filtersVisible: false,
      filters: {
        position: {
          value: null,
          fieldName: 'position',
          items: []
        }
      }
    };
  },
  computed: {
    ...mapGetters({ items: 'users/getUsers' })
  },
  methods: {
    ...mapActions({
      fetchItems: 'users/fetchUsers',
      deleteItem: 'users/deleteUser',
      deleteItems: 'users/deleteUsers'
    }),
    async applyFilters() {
      this.query.filterFields = [];
      if (this.filters.position.value)
        this.query.filterFields.push({
          fieldName: this.filters.position.fieldName,
          value: this.filters.position.value
        });
      await this.refresh();
    },
    onItemAddToTeam(event, row) {}
  }
};
</script>
