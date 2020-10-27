<template>
  <el-popover
    placement="top"
    trigger="click"
    transition="fade"
    :width="width || '225'"
    popper-class="popover"
    v-model="isPopoverVisible"
    v-click-outside="onClickOutside"
  >
    <div class="buttons">
      <slot></slot>
    </div>
    <slot name="reference" slot="reference"></slot>
  </el-popover>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class PopoverWithButtons extends Vue {
  @Prop() readonly width?: string
  private isPopoverVisible = false

  private onClickOutside(): void {
    this.isPopoverVisible = false
  }
}
</script>

<style lang="scss">
.popover {
  padding: 6px !important;
  .buttons {
    display: flex;
    flex-direction: column;
    .divider {
      margin: 4px;
      border-bottom: 1px solid var(--text);
      opacity: 0.08;
    }
  }
  .popper__arrow {
    display: none !important;
  }
  .el-button + .el-button {
    margin-left: 0;
  }
}
</style>
