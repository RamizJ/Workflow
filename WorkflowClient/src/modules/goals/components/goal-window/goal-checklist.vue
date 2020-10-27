<template>
  <el-card class="checklist" shadow="never" :body-style="{ padding: '0px 10px' }">
    <div class="checklist__input">
      <el-button type="text" size="mini" @click="onAdd">
        <unicon name="plus" />
      </el-button>
      <el-input
        v-model="input"
        type="textarea"
        :rows="1"
        placeholder="Новый пункт"
        autosize
        @keydown.enter.exact.native.prevent
        @keyup.enter.exact.native.prevent="onAdd"
      />
    </div>
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
          type="textarea"
          :rows="1"
          autosize
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

import Goal, { Priority, Status } from '@/modules/goals/models/goal.type'
import moment from 'moment'

@Component
export default class Checklist extends Vue {
  @Prop() readonly goal!: Goal

  private input = ''
  private checklist: Goal[] = []

  protected mounted(): void {
    if (!this.goal.children) return
    this.checklist = this.goal.children.map((goal) => {
      goal.completed = goal.state === 'Succeed'
      return goal
    })
  }

  @Watch('goal', { deep: true })
  onItemsChange(goal: Goal): void {
    if (!goal.children) return
    this.checklist = goal.children.map((child) => {
      child.completed = child.state === 'Succeed'
      return child
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

  private onAdd(event?: KeyboardEvent): void {
    if (!this.input) return
    const entity: Goal = {
      title: this.input.trim(),
      projectId: this.goal.projectId,
      creationDate: moment.utc(moment()).format(),
      state: Status.New,
      priority: Priority.Normal,
      parentGoalId: this.goal.id,
      completed: false,
      metadataList: [],
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
  &__input {
    display: flex;
    align-items: center;
    .el-button svg {
      width: 18px;
    }
    .el-textarea__inner {
      padding: 4px 6px;
    }
  }

  .el-textarea__inner,
  .el-textarea__inner:hover,
  .el-textarea__inner:focus {
    background-color: transparent;
  }
  .el-input__prefix {
    left: 0;
  }
  .el-input--prefix .el-textarea__inner {
    //padding-left: 25px;
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
  .el-textarea__inner {
    padding: 7px 10px;
  }
  &.completed .el-textarea__inner {
    text-decoration: line-through;
  }
  &:last-child {
    margin-bottom: 2px;
  }
}
</style>
