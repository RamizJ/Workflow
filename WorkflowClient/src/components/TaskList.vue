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
      el-table-column(prop="title" label="Задача")
      el-table-column(v-if="!$route.params.projectId" prop="projectName" label="Проект" width="150")
      el-table-column(prop="state" label="Статус" width="120" :formatter="stateFormatter")
      el-table-column(prop="priority" label="Приоритет" width="120" :formatter="priorityFormatter")
      el-table-column(prop="creationDate" label="Добавлено" width="180" :formatter="dateFormatter")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li
          a(v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)") Изменить
        el-divider(v-if="isEditVisible")
        li
          a(@click.prevent="onItemCreate") Новая задача
        el-divider
        li.v-context__sub
          a(v-if="isStatusVisible") Изменить статус
          ul.v-context
            li
              a(@click.prevent="onItemStatusChange($event, child.data.row, 'Succeed')") Выполнено
            li
              a(@click.prevent="onItemStatusChange($event, child.data.row, 'Delay')") Отложено
            li
              a(@click.prevent="onItemStatusChange($event, child.data.row, 'Rejected')") Отклонено
            el-divider
            li
              a(@click.prevent="onItemStatusChange($event, child.data.row, 'Perform')") Выполняется
            li
              a(@click.prevent="onItemStatusChange($event, child.data.row, 'Testing')") Проверяется
        li
          a(v-if="isDeleteVisible" @click.prevent="onItemDelete($event, child.data.row)") Переместить в корзину
        li
          a(v-if="isRestoreVisible" @click.prevent="onItemRestore($event, child.data.row)") {{ isMultipleSelected ? 'Восстановить выделенное' : 'Восстановить' }}



    //vue-context(ref="contextMenu")
      template(slot-scope="child")
        li(v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)") Изменить
        el-divider(v-if="isEditVisible")
        li(@click.prevent="onItemCreate") Новая задача
        el-divider
        li.v-context__sub(v-if="isStatusVisible") Изменить статус
          ul.v-context
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Succeed')") Выполнено
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Delay')") Отложено
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Rejected')") Отклонено
            el-divider
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Perform')") Выполняется
            li(@click.prevent="onItemStatusChange($event, child.data.row, 'Testing')") Проверяется
        li(
          v-if="isDeleteVisible"
          @click.prevent="onItemDelete($event, child.data.row)") Переместить в корзину
        li(
          v-if="isRestoreVisible"
          @click.prevent="onItemRestore($event, child.data.row)") {{ isMultipleSelected ? 'Восстановить выделенное' : 'Восстановить' }}

    task-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import listMixin from '~/mixins/list.mixin';
import TaskDialog from '~/components/TaskDialog';

export default {
  name: 'TaskList',
  components: { TaskDialog },
  mixins: [listMixin],
  data() {
    return {
      getters: {
        items: 'tasks/getTasks'
      },
      actions: {
        fetchItems: 'tasks/fetchTasks',
        deleteItem: 'tasks/deleteTask',
        deleteItems: 'tasks/deleteTasks',
        restoreItem: 'tasks/restoreTask',
        restoreItems: 'tasks/restoreTasks',
        updateItem: 'tasks/updateTask',
        updateItems: 'tasks/updateTasks'
      }
    };
  }
};
</script>
