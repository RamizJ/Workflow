<template lang="pug">
  div.container
    base-header
      template(slot="title") Пользователи
      template(slot="action")
        a(href="#" @click="dialogOpened = true") Создать

    base-toolbar
      template(slot="filters")
        base-toolbar-item
          el-input(v-model="query.filter" size="small" placeholder="Поиск" @change="refresh")
        base-toolbar-item
          el-select(
            v-model="filters.position.value"
            size="small"
            placeholder="Должность"
            @change="applyFilters"
            multiple collapse-tags)
            el-option(v-for="option in filters.position.items" :key="option.value" :value="option.value", :label="option.label")

    div.content
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
          li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
          li Завершить
          li Удалить

    user-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseHeader from '~/components/BaseHeader';
import BaseToolbar from '~/components/BaseToolbar';
import BaseToolbarItem from '~/components/BaseToolbarItem';
import UserDialog from '~/components/UserDialog';

export default {
  name: 'Users',
  components: {
    BaseHeader,
    BaseToolbar,
    BaseToolbarItem,
    UserDialog
  },
  data() {
    return {
      loading: false,
      tableData: [],
      query: {
        filter: '',
        pageNumber: 0,
        pageSize: 15
      },
      dialogOpened: false,
      selectedItemId: null,
      filters: {
        position: {
          value: null,
          fieldName: 'position',
          items: []
        }
      },
      value: ''
    };
  },
  computed: {
    ...mapGetters({ users: 'users/getUsers' })
  },
  methods: {
    ...mapActions({ fetchUsers: 'users/fetchUsers' }),
    async applyFilters() {
      this.query.filterFields = [];
      if (this.filters.position.value)
        this.query.filterFields.push({
          fieldName: this.filters.position.fieldName,
          value: this.filters.position.value
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
      if (this.users.length) $state.loaded();
      else $state.complete();
      this.tableData = firstLoad
        ? this.users
        : this.tableData.concat(this.users);
      if (firstLoad) this.loading = false;
    },
    async fetch(params) {
      try {
        await this.fetchUsers(params);
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
