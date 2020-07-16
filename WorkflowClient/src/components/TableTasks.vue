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
      el-table-column(prop="title" label="Задача" sortable="custom")
      el-table-column(v-if="!$route.params.projectId" prop="projectName" label="Проект" width="150" sortable="custom")
      el-table-column(prop="state" label="Статус" width="120" :formatter="stateFormatter" sortable="custom")
      el-table-column(prop="priority" label="Приоритет" width="120" :formatter="priorityFormatter" sortable="custom")
      el-table-column(prop="creationDate" label="Добавлено" width="170" :formatter="dateFormatter" sortable="custom")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li(v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)") Изменить
        el-divider(v-if="isEditVisible")
        li(@click.prevent="onItemCreate") Новая задача
        el-divider
        li.v-context__sub(v-if="isStatusVisible") Изменить статус
          ul.v-context
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Perform')") Выполняется
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Testing')") Проверяется
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Succeed')") Выполнено
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Delay')") Отложено
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Rejected')") Отклонено
        li(
          v-if="isDeleteVisible"
          @click.prevent="onItemDelete($event, child.data.row)") {{ isMultipleSelected ? 'Удалить выделенное' : 'Переместить в корзину' }}
        li(
          v-if="isRestoreVisible"
          @click.prevent="onItemRestore($event, child.data.row)") {{ isMultipleSelected ? 'Восстановить выделенное' : 'Восстановить' }}

    dialog-task(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import TableContent from '~/components/TableContent';
import DialogTask from '~/components/DialogTask';
import tableMixin from '~/mixins/table.mixin';
import BaseSearch from '~/components/BaseSearch';

export default {
  name: 'TableTasks',
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
    DialogTask
  },
  mixins: [tableMixin],
  computed: {
    ...mapGetters({ items: 'tasks/getTasks' })
  },
  mounted() {
    if (this.$route.params.projectId)
      this.query.projectId = this.$route.params.projectId;
    switch (this.status) {
      case 'All':
        this.isEditVisible = true;
        this.isStatusVisible = true;
        this.isDeleteVisible = true;
        this.isRestoreVisible = false;
        break;
      case 'Deleted':
        this.isEditVisible = false;
        this.isStatusVisible = false;
        this.isDeleteVisible = false;
        this.isRestoreVisible = true;
        this.query.filterFields.push({
          fieldName: 'isRemoved',
          values: [true]
        });
        this.query.withRemoved = true;
        break;
      default:
        this.isEditVisible = true;
        this.isStatusVisible = true;
        this.isDeleteVisible = true;
        this.isRestoreVisible = false;
        this.query.filterFields.push({
          fieldName: 'state',
          values: [this.status]
        });
        break;
    }
  },
  methods: {
    ...mapActions({
      fetchItems: 'tasks/fetchTasks',
      deleteItem: 'tasks/deleteTask',
      deleteItems: 'tasks/deleteTasks',
      restoreItem: 'tasks/restoreTask',
      restoreItems: 'tasks/restoreTasks',
      updateItem: 'tasks/updateTask',
      updateItems: 'tasks/updateTasks'
    })
  }
};
</script>
