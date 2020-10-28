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
  text-align: center;
  background-color: transparent;
  justify-content: center;
  span {
    display: flex;
    flex-direction: column;
    justify-content: center;
  }
  &:hover,
  &:focus {
    color: var(--text);
    svg {
      fill: var(--text);
    }
    background-color: var(--page-background);
    box-shadow: 0 2px 10px 0 rgba(0, 0, 0, 0.05);
  }
  .unicon {
    display: flex;
    justify-content: center;
  }
  svg {
    fill: var(--text);
    width: 28px;
    height: 28px;
    vertical-align: sub;
    margin-bottom: 10px;
  }
}
</style>
