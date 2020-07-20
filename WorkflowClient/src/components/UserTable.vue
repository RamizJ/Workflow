<template lang="pug">
  div.table-container
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
      el-table-column(prop="lastName" label="Фамилия" width="150")
      el-table-column(prop="firstName" label="Имя" width="150")
      el-table-column(prop="middleName" label="Отчество" width="150")
      el-table-column(prop="userName" label="Логин" width="150")
      el-table-column(prop="email" label="Почта" sortable="custom")
      el-table-column(prop="phone" label="Телефон" width="120")
      el-table-column(prop="position" label="Должность" width="180")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li
          a(v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)") Изменить
        el-divider(v-if="isEditVisible")
        li
          a(@click.prevent="onItemCreate") Новый пользователь
        el-divider
        li
          a(v-if="isDeleteVisible" @click.prevent="onItemDelete($event, child.data.row)") Переместить в корзину
        li
          a(v-if="isRestoreVisible" @click.prevent="onItemRestore($event, child.data.row)") Восстановить

    user-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import listMixin from '~/mixins/list.mixin';
import UserDialog from '~/components/UserDialog';

export default {
  name: 'UserList',
  components: { UserDialog },
  mixins: [listMixin],
  data() {
    return {
      getters: {
        items: 'users/getUsers'
      },
      actions: {
        fetchItems: 'users/fetchUsers',
        deleteItem: 'users/deleteUser',
        deleteItems: 'users/deleteUsers',
        restoreItem: 'users/restoreUser',
        restoreItems: 'users/restoreUsers'
      }
    };
  }
};
</script>
