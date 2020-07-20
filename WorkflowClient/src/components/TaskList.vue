<template lang="pug">
  div.list
    div.list__header(:class="state.toLowerCase()") {{ title }}
    div.list__items
      draggable(v-model="tableData" v-bind="dragOptions")
        task-list-item(v-for="item in tableData" :key="item.id" :item="item" @contextmenu.native="onItemRightClick(item, null, $event)")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load")
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

</template>

<script>
import Draggable from 'vuedraggable';
import TaskListItem from '@/components/TaskListItem';
import listMixin from '@/mixins/list.mixin';

export default {
  name: 'TaskList',
  props: ['state'],
  components: {
    Draggable,
    TaskListItem
  },
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
  },
  computed: {
    dragOptions() {
      return {
        animation: '200',
        ghostClass: 'ghost',
        group: 'kanban-board-list-items',
        disabled: false
      };
    },
    title() {
      switch (this.state) {
        case 'New':
          return 'Новое';
        case 'Perform':
          return 'Выполняется';
        case 'Testing':
          return 'Тестируется';
        case 'Succeed':
          return 'Выполнено';
        case 'Delay':
          return 'Отложено';
        case 'Rejected':
          return 'Отклонено';
        default:
          return 'Все';
      }
    }
  },
  mounted() {
    if (this.state) {
      this.query.filterFields[0] = {
        fieldName: 'state',
        values: [this.state]
      };
    }
  },
  methods: {}
};
</script>

<style lang="scss" scoped>
.list {
  min-width: 260px;
  max-width: 260px;
  margin-right: 20px;
  position: relative;
  width: 100%;
  flex: 0 0 25%;
  &__header {
    border-bottom: 2px solid transparent;
    font-size: 14px;
    font-weight: 500;
    padding: 10px 0;
    border-top-left-radius: 10px;
    border-top-right-radius: 10px;
    &.new {
      border-bottom-color: rgba(33, 150, 243, 0.5);
    }
    &.perform {
      border-bottom-color: rgb(241, 222, 51);
    }
    &.delay {
      border-bottom-color: lightgrey;
    }
    &.testing {
      border-bottom-color: #ff6d37;
    }
    &.succeed {
      border-bottom-color: #00cf3a;
    }
    &.rejected {
      border-bottom-color: #ca0000;
    }
  }
  &__items {
    min-height: 300px;
    overflow: scroll;
    border-bottom-left-radius: 10px;
    border-bottom-right-radius: 10px;
  }
}
</style>
