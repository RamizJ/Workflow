<template>
  <div class="title-cell">
    <span class="cell-text">{{ row.title }}</span>
    <span class="cell-icon" v-if="row.description">
      <unicon name="file-alt" />
    </span>
    <span class="cell-icon" v-if="row.hasChildren">
      <unicon name="list-ul" />
    </span>
    <span class="cell-icon" v-if="row.isAttachmentsExist">
      <unicon name="paperclip" />
    </span>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import Task from '@/modules/goals/models/task.type'

@Component
export default class GoalTableNew extends Vue {
  @Prop() readonly row!: Task
}
</script>

<style scoped></style>
