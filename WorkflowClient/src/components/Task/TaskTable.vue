<template lang="pug">
  div.table-container
    el-table(
      ref="table"
      height="100%"
      v-loading="loading"
      :data="tableData"
      :row-class-name="onSetIndex"
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
      el-table-column(prop="creationDate" label="Дата создания" width="180" :formatter="dateFormatter")
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
              a(@click.prevent="onItemStatusChange($event, child.data.row, 'New')") Новое
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

    task-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import tableMixin from '@/mixins/table.mixin';
import TaskDialog from '@/components/Task/TaskDialog';

export default {
  name: 'TaskTable',
  components: { TaskDialog },
  mixins: [tableMixin],
  data() {
    return {
      getters: {
        items: 'tasks/getTasks'
      },
      actions: {
        fetchItems: 'tasks/findAll',
        updateItem: 'tasks/updateOne',
        updateItems: 'tasks/updateMany',
        deleteItem: 'tasks/deleteOne',
        deleteItems: 'tasks/deleteMany',
        restoreItem: 'tasks/restoreOne',
        restoreItems: 'tasks/restoreMany',
      }
    };
  }
};
</script>
