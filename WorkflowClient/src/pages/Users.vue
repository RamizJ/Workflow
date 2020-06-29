<template lang="pug">
  div.container
    base-header
      template(slot="title") Пользователи
      template(slot="action")
        a(href="#" @click="dialogOpened = true; selectedItemId = null") Создать

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

    base-list
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
import BaseHeader from '~/components/BaseHeader';
import BaseToolbar from '~/components/BaseToolbar';
import BaseToolbarItem from '~/components/BaseToolbarItem';
import BaseList from '~/components/BaseList';
import UserDialog from '~/components/UserDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Users',
  components: {
    BaseHeader,
    BaseToolbar,
    BaseToolbarItem,
    BaseList,
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
