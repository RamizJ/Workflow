<template lang="pug">
  div.board
    draggable.board__wrapper(v-model="lists" v-bind="listsDragOptions" @change="onListMove")
      div.list(v-for="(list, index) in lists" :key="index")
        div.list__header(v-if="list.name" :class="list.name.toLowerCase()") {{ list.label }}
        div.list__items
          draggable(v-model="lists[index].items" v-bind="itemsDragOptions" @change="onItemMove($event, list.name)")
            div.item(v-for="item in list.items" :key="item.id" :item="item" @contextmenu="onItemRightClick($event, item)")
              div.item__header {{ item.title }}
              div.item__footer
                div.item__performer {{ item.performerFio }}
                div.item__date {{ new Date(item.creationDate).toLocaleDateString() }}

    infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load")
      div(slot="no-more")
      div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li
          a(v-if="isEditVisible" @click.prevent="onItemEdit(child.data.item)") Изменить
        el-divider(v-if="isEditVisible")
        li
          a(@click.prevent="onItemCreate") Новая задача
        el-divider
        li.v-context__sub
          a(v-if="isStatusVisible") Изменить статус
          ul.v-context
            li
              a(@click.prevent="onItemStatusChange(child.data.item, 'New')") Новое
            li
              a(@click.prevent="onItemStatusChange(child.data.item, 'Succeed')") Выполнено
            li
              a(@click.prevent="onItemStatusChange(child.data.item, 'Delay')") Отложено
            li
              a(@click.prevent="onItemStatusChange(child.data.item, 'Rejected')") Отклонено
            el-divider
            li
              a(@click.prevent="onItemStatusChange(child.data.item, 'Perform')") Выполняется
            li
              a(@click.prevent="onItemStatusChange(child.data.item, 'Testing')") Проверяется
          li
            a(v-if="isDeleteVisible" @click.prevent="onItemDelete(child.data.item)") Переместить в корзину
          li
            a(v-if="isRestoreVisible" @click.prevent="onItemRestore(child.data.item)") Восстановить

    task-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import Draggable from 'vuedraggable';
import TaskDialog from '@/components/TaskDialog';
import boardMixin from '@/mixins/board.mixin';

export default {
  name: 'TaskBoard',
  components: {
    Draggable,
    TaskDialog
  },
  mixins: [boardMixin],
  data() {
    return {
      lists: [],
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
    listsDragOptions() {
      return {
        animation: '200',
        ghostClass: 'ghost',
        handle: '.list__header',
        disabled: false,
        group: 'lists'
      };
    },
    itemsDragOptions() {
      return {
        animation: '200',
        ghostClass: 'ghost',
        group: 'list-items',
        disabled: false
      };
    },
    boardLists() {
      if (localStorage.boardLists) return JSON.parse(localStorage.boardLists);
      else
        return [
          { label: 'Новое', name: 'New' },
          { label: 'Выполняется', name: 'Perform' },
          { label: 'Тестируется', name: 'Testing' },
          { label: 'Отложено', name: 'Delay' },
          { label: 'Выполнено', name: 'Succeed' },
          { label: 'Отклонено', name: 'Rejected' }
        ];
    }
  },
  methods: {
    async onListMove(event) {
      const newBoardLists = this.lists.map(list => {
        return {
          label: list.label,
          name: list.name
        };
      });
      localStorage.boardLists = JSON.stringify(newBoardLists);
    },
    async onItemMove(event, listName) {
      if (event.added)
        await this.onItemStatusChange(event.added.element, listName);
    },
    updateLists() {
      this.lists = [];
      this.boardLists.forEach(list => {
        this.lists.push({
          label: list.label,
          name: list.name,
          items: this.data.filter(item => item.state === list.name)
        });
      });
    }
  }
};
</script>

<style lang="scss" scoped>
.board {
  height: 100%;
  overflow-x: scroll;
  overflow-y: hidden;
  -webkit-overflow-scrolling: touch;
  &::-webkit-scrollbar {
    display: none;
  }
  &__wrapper {
    display: flex;
    flex-wrap: nowrap;
  }
}
.board::-webkit-scrollbar {
  display: none;
}
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
.item {
  cursor: pointer;
  margin: 10px 0;
  padding: 12px 15px;
  border: 1px solid var(--card-border);
  background-color: var(--card-background);
  border-radius: 8px;
  &__header {
    font-size: 12.5px;
    line-height: normal;
  }
  &__footer {
    margin-top: 8px;
    display: flex;
    justify-content: space-between;
  }
  &__performer {
    font-size: 11px;
    color: var(--text-placeholder);
  }
  &__date {
    font-size: 11px;
    color: var(--text-muted);
  }
}
</style>
