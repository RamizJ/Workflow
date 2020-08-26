<template>
  <el-card class="checklist" shadow="never" :body-style="{ padding: '0px 10px' }">
    <el-input v-model="input" placeholder="Новый пункт" @keyup.enter.native="onAdd">
      <el-button slot="prefix" type="text" size="mini" @click="onAdd">
        <feather type="plus" size="18"></feather>
      </el-button>
    </el-input>
    <div
      class="checklist__item"
      v-for="(item, index) in currentChecklist"
      :key="index"
      :class="currentChecklist[index].completed ? 'completed' : 'new'"
    >
      <el-checkbox
        v-model="currentChecklist[index].completed"
        @change="onCheck(index)"
      ></el-checkbox>
      <el-input
        v-model="currentChecklist[index].title"
        @keyup.delete.native="onDelete(index)"
      ></el-input>
    </div>
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

import tasksModule from '@/store/modules/tasks.module'
import Task, { Status } from '@/types/task.type'
import moment from 'moment'

@Component
export default class Checklist extends Vue {
  @Prop()
  task!: Task

  // private loading = true;
  private input = ''
  private previousChecklist: Task[] = []
  private currentChecklist: Task[] = []

  // private get newItems(): Task[] {
  //   return this.currentChecklist.filter(item => !item.id);
  // }
  //
  // private get renamedItems(): Task[] {
  //   const changedItems = [];
  //   for (const previousItem of this.previousChecklist) {
  //     const changedItem = this.currentChecklist.find(
  //       currentItem =>
  //         currentItem.id &&
  //         currentItem.id === previousItem.id &&
  //         (currentItem.completed !== previousItem.completed ||
  //           currentItem.title !== previousItem.title)
  //     );
  //     if (changedItem) changedItems.push(changedItem);
  //   }
  //   return changedItems;
  // }
  //
  // private get removedItems(): Task[] {
  //   const removedItems: Task[] = [];
  //   for (const previousItem of this.previousChecklist) {
  //     const existingItem = this.currentChecklist.find(
  //       currentItem => currentItem.id === previousItem.id
  //     );
  //     if (!existingItem) removedItems.push(previousItem);
  //   }
  //   return removedItems;
  // }

  private async mounted() {
    if (!this.task.isChildsExist) return
    // this.loading = true;
    if (this.task.id && this.task.child) {
      // const child: Task[] = await tasksModule.findChild(this.task.id);
      const child: Task[] = this.task.child
      this.previousChecklist = Array.from(
        child.map(task => {
          task.completed = task.state === 'Succeed'
          return { ...task }
        })
      )
      this.currentChecklist = child.map(task => {
        task.completed = task.state === 'Succeed'
        return task
      })
    }
    // this.loading = false;
  }

  private async onAdd(): Promise<void> {
    if (!this.input) return
    const entity: Task = {
      title: this.input,
      projectId: this.task.projectId,
      creationDate: moment.utc(moment()).format(),
      state: Status.New,
      parentGoalId: this.task.id,
      completed: false
    }
    this.currentChecklist.unshift(entity)
    this.input = ''
    await tasksModule.createOne(entity)
    this.$emit('change', this.currentChecklist)
  }

  private async onCheck(index: number): Promise<void> {
    this.currentChecklist[index].state = this.currentChecklist[index].completed
      ? Status.Succeed
      : Status.New
    const entity = this.currentChecklist[index]
    await tasksModule.updateOne(entity)
    this.$emit('change', this.currentChecklist)
  }

  private async onDelete(index: number): Promise<void> {
    const entity = this.currentChecklist[index]
    if (entity.title === undefined) this.currentChecklist.splice(index, 1)
    if (entity && entity.title === '') {
      entity.title = undefined
      if (entity.id) await tasksModule.deleteOne(entity.id)
      this.$emit('change', this.currentChecklist)
    }
  }
}
</script>

<style lang="scss">
.checklist {
  &__item {
    display: flex;
    .el-input__inner {
      padding-left: 9px;
    }
    &.completed .el-input__inner {
      text-decoration: line-through;
    }
    &:last-child {
      margin-bottom: 2px;
    }
  }
  .el-input__inner,
  .el-input__inner:hover,
  .el-input__inner:focus {
    background-color: transparent;
  }
  .el-input__prefix {
    left: 0;
  }
  .el-input--prefix .el-input__inner {
    padding-left: 25px;
  }
  .el-input .el-button {
    height: 100%;
    padding: 0;
  }
  .el-checkbox {
    color: var(--text);
    margin-left: 2px;
    font-weight: 400;
    width: 14px;
  }
}
</style>
