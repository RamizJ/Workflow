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
          el-button(size="mini" @click="dialogOpened = true; selectedItemId = null")
            feather(type="plus" size="16")
          el-button(size="mini")
            feather(type="edit-3" size="16")
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
            feather(type="list" size="20")

      table-content
        el-table(
          :data="tableData"
          ref="table"
          height="auto"
          v-loading="loading"
          @row-contextmenu="onItemRightClick"
          @row-dblclick="onItemDoubleClick"
          highlight-current-row)
          el-table-column(type="selection" width="55")
          el-table-column(prop="lastName" label="Фамилия")
          el-table-column(prop="firstName" label="Имя")
          el-table-column(prop="middleName" label="Отчество")
          el-table-column(prop="userName" label="Логин")
          el-table-column(prop="email" label="Почта")
          el-table-column(prop="phone" label="Телефон")
          infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="400" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
            div(slot="no-more")
            div(slot="no-results")

        vue-context(ref="contextMenu")
          template(slot-scope="child")
            li(@click.prevent="onItemAddToTeam($event, child.data.row)") Добавить в команду...
            li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
            li(v-if="!isMultipleSelected" @click.prevent="onItemDelete($event, child.data.row)") Удалить
            li(v-if="isMultipleSelected" @click.prevent="onItemMultipleDelete($event, child.data.row)") Удалить выделенное

    user-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false" @submit="refresh")

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
      query: {
        filter: '',
        pageNumber: 0,
        pageSize: 15
      },
      filtersVisible: false,
      filters: {
        sort: {
          value: null,
          items: [
            { value: 'lastName', label: 'Фамилия' },
            { value: 'firstName', label: 'Имя' },
            { value: 'middleName', label: 'Отчество' },
            { value: 'email', label: 'Почта' },
            { value: 'creationDate', label: 'Дата создания' }
          ]
        },
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
