<template>
  <div class="cell title-cell">
    <span class="cell-icon left text" v-if="hasChildren">
      <unicon name="copy" />
    </span>
    <span class="cell-icon left text" v-else>
      <unicon name="file" />
    </span>
    <span class="cell-text">{{ row.title }}</span>
    <span class="cell-icon right" v-if="hasDescription">
      <unicon name="file-alt" />
    </span>
    <span class="cell-icon right" v-if="hasAttachments">
      <unicon name="paperclip" />
    </span>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import Goal from '@/modules/goals/models/goal.type'

@Component
export default class GoalTitleCell extends Vue {
  @Prop() readonly row!: Goal

  private get hasDescription(): boolean {
    return !!this.row.description
  }

  private get hasChildren(): boolean {
    return !!this.row.hasChildren
  }

  private get hasAttachments(): boolean {
    return !!this.row.isAttachmentsExist
  }
}
</script>

<style lang="scss" scoped>
.title-cell,
.cell {
  padding-left: 0 !important;
}
</style>
