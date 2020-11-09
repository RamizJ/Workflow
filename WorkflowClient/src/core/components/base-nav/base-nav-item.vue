<template>
  <el-menu-item @click="onClick" class="menu-item" :index="index" :disabled="disabled">
    <unicon v-if="icon" :name="icon" />
    <span>
      <slot></slot>
    </span>
  </el-menu-item>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class BaseNavItem extends Vue {
  @Prop() readonly index!: string
  @Prop() readonly icon?: string
  @Prop() readonly disabled?: boolean

  private onClick(): void {
    this.$emit('click')
  }
}
</script>

<style lang="scss" scoped>
.el-menu-item {
  color: var(--text);
  height: auto;
  line-height: 38px;
  font-size: 14px;
  font-weight: 500;
  padding: 0 16px 0 10px !important;
  margin: 1px 0;
  border-radius: 5px;
  display: flex;
  align-items: center;
  z-index: 1;
  transition: 0.1s;
  span {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    word-break: break-all;
  }
  .unicon {
    fill: var(--color-primary);
    transition: 0.15s;
  }
  &.is-active {
    background-color: var(--color-primary);
    color: var(--nav-item-selected);
    .unicon {
      fill: var(--nav-item-selected);
    }
  }
  &:hover:not(.is-active) {
    outline: none;
    background-color: transparent;
  }
}
</style>

<style lang="scss">
.el-menu-item .unicon svg {
  width: 17px !important;
  margin-right: 10px;
  margin-left: 2px;
  margin-bottom: 2px;
  overflow: unset;
}
</style>
