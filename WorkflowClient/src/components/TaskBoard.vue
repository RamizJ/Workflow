<template lang="pug">
  div.board
    draggable.board__wrapper(v-model="columns" v-bind="getDragOptions")
      task-list(v-for="(column, index) in columns" :key="index" :state="column")
</template>

<script>
import Draggable from 'vuedraggable';
import TaskList from '@/components/TaskList';

export default {
  name: 'TaskBoard',
  components: {
    Draggable,
    TaskList
  },
  data() {
    return {
      columns: ['New', 'Perform', 'Testing', 'Succeed', 'Delay', 'Rejected']
    };
  },
  computed: {
    getDragOptions() {
      return {
        animation: '200',
        ghostClass: 'ghost',
        handle: '.list__header',
        disabled: false,
        group: 'kanban-board-lists'
      };
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
</style>
