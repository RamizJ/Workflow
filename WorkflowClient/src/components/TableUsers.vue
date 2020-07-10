<template lang="pug">
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
      @sort-change="onSortChange"
      @filter-change="onFilterChange"
      highlight-current-row border)
      el-table-column(type="selection" width="38")
      el-table-column(prop="lastName" label="Фамилия" width="150" sortable="custom")
      el-table-column(prop="firstName" label="Имя" width="150" sortable="custom")
      el-table-column(prop="middleName" label="Отчество" width="150" sortable="custom")
      el-table-column(prop="userName" label="Логин" width="150" sortable="custom")
      el-table-column(prop="email" label="Почта" sortable="custom")
      el-table-column(prop="phone" label="Телефон" width="120" sortable="custom")
      el-table-column(prop="position" label="Должность" width="180" sortable="custom")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li(v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)") Изменить
        el-divider(v-if="isEditVisible")
        li(@click.prevent="onItemCreate") Новый пользователь
        el-divider
        li(
          v-if="isDeleteVisible"
          @click.prevent="onItemDelete($event, child.data.row)") Переместить в корзину
        li(
          v-if="isRestoreVisible"
          @click.prevent="onItemRestore($event, child.data.row)") Восстановить

    dialog-user(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import TableContent from '~/components/TableContent';
import DialogUser from '~/components/DialogUser';
import BaseSearch from '~/components/BaseSearch';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'TableUsers',
  props: {
    search: {
      type: String
    },
    status: {
      type: String
    }
  },
  components: {
    BaseSearch,
    TableContent,
    DialogUser
  },
  mixins: [tableMixin],
  computed: {
    ...mapGetters({ items: 'users/getUsers' })
  },
  mounted() {
    switch (this.status) {
      case 'All':
        this.isEditVisible = true;
        this.isDeleteVisible = true;
        break;
      case 'Deleted':
        this.isRestoreVisible = true;
        this.query.filterFields.push({
          fieldName: 'isRemoved',
          values: [true]
        });
        this.query.withRemoved = true;
        break;
      default:
        this.isEditVisible = true;
        this.isDeleteVisible = true;
        break;
    }
  },
  methods: {
    ...mapActions({
      fetchItems: 'users/fetchUsers',
      deleteItem: 'users/deleteUser',
      deleteItems: 'users/deleteUsers',
      restoreItem: 'users/restoreUser',
      restoreItems: 'users/restoreUsers'
    })
  }
};
</script>
