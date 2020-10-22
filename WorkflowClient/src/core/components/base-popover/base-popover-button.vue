<template>
  <el-button class="popover-button" @click="onClick">
    <unicon :name="icon" />
    <slot></slot>
  </el-button>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class PopoverButton extends Vue {
  @Prop() icon?: string

  private onClick(): void {
    this.$emit('click')
  }
}
</script>

<style lang="scss">
.popover-button.el-button {
  width: 100%;
  margin-left: 0;
  padding: 12px 16px;
  text-align: left;
  background-color: transparent;
  &:hover,
  &:focus {
    svg {
      fill: var(--button-hover-color);
    }
    background-color: var(--color-primary);
  }
  svg {
    fill: var(--text);
    width: 18px;
    height: 18px;
    margin-right: 12px;
    vertical-align: sub;
  }
}
</style>
