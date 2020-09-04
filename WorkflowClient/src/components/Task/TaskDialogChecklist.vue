<template>
  <el-card class="checklist" shadow="never" :body-style="{ padding: '0px 10px' }">
    <el-input v-model="input" placeholder="Новый пункт" @keyup.enter.native="onAdd">
      <el-button slot="prefix" type="text" size="mini" @click="onAdd">
        <feather type="plus" size="18"></feather>
      </el-button>
    </el-input>
    <div
      class="item"
      v-for="(item, index) in checklist"
      :key="index"
      :class="checklist[index].completed ? 'completed' : 'new'"
    >
      <div class="item__wrapper" v-if="!item.isRemoved">
        <el-checkbox v-model="checklist[index].completed" @change="onCheck(index)"></el-checkbox>
        <el-input
          v-model="checklist[index].title"
          @keyup.delete.native="onDelete(index)"
          @change="onChange"
          @blur="onBlur(index)"
        ></el-input>
      </div>
    </div>
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'

import Task, { Status } from '@/types/task.type'
import moment from 'moment'

@Component
export default class Checklist extends Vue {
  @Prop() readonly task!: Task

  private input = ''
  private checklist: Task[] = []

  protected mounted(): void {
    if (!this.task.childTasks) return
    this.checklist = this.task.childTasks.map((task) => {
      task.completed = task.state === 'Succeed'
      return task
    })
  }

  @Watch('task', { deep: true })
  onItemsChange(task: Task): void {
    if (!task.childTasks) return
    this.checklist = task.childTasks.map((task) => {
      task.completed = task.state === 'Succeed'
      return task
    })
  }

  private onChange(): void {
    this.$emit('change', this.checklist)
  }

  private onBlur(index: number): void {
    const entity = this.checklist[index]
    if (!entity.title) entity.isRemoved = true
    this.$forceUpdate()
  }

  private onAdd(): void {
    if (!this.input) return
    const entity: Task = {
      title: this.input,
      projectId: this.task.projectId,
      creationDate: moment.utc(moment()).format(),
      state: Status.New,
      parentGoalId: this.task.id,
      completed: false,
    }
    this.checklist.unshift(entity)
    this.input = ''
    this.$emit('change', this.checklist)
  }

  private onCheck(index: number): void {
    this.checklist[index].state = this.checklist[index].completed ? Status.Succeed : Status.New
    this.$emit('change', this.checklist)
  }

  private onDelete(index: number): void {
    const entity = this.checklist[index]
    if (entity.title === undefined) {
      this.checklist[index].isRemoved = true
      this.$forceUpdate()
      this.$emit('change', this.checklist)
    }
    if (entity && entity.title === '') entity.title = undefined
  }
}
</script>

<style lang="scss">
.checklist {
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

.item {
  .item__wrapper {
    display: flex;
  }
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
</style>
