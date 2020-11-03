<template>
  <el-popover
    trigger="click"
    :transition="transition || 'fade'"
    :width="width || undefined"
    :popper-class="'popover ' + popoverClass"
    v-model="isPopoverVisible"
    v-click-outside="onClickOutside"
  >
    <div v-if="title" class="title">
      {{ title }}
    </div>
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
  @Prop() readonly title?: string
  @Prop() readonly width?: string
  @Prop() readonly transition?: string
  @Prop() readonly popoverClass?: string
  private isPopoverVisible = false

  protected mounted(): void {
    this.listenButtonClicks()
  }

  private listenButtonClicks(): void {
    document.querySelectorAll('.popover .el-button')?.forEach((button) => {
      button.addEventListener('click', () => {
        this.isPopoverVisible = false
      })
    })
  }

  private onClickOutside(): void {
    this.isPopoverVisible = false
  }
}
</script>

<style lang="scss">
.popover {
  padding: 15px !important;
  .title {
    font-size: 18px;
    line-height: 20px;
    font-weight: 500;
    margin: 0 0 8px 8px;
  }
  .buttons {
    display: flex;
    //flex-direction: column;
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
