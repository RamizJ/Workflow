<template>
  <el-menu-item class="menu-item" :index="index" :disabled="disabled">
    <unicon v-if="icon" :name="icon" />
    <span>
      <slot></slot>
    </span>
  </el-menu-item>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class MenuItem extends Vue {
  @Prop() readonly index!: string
  @Prop() readonly icon?: string
  @Prop() readonly disabled?: boolean
}
</script>

<style lang="scss" scoped>
.el-menu-item {
  color: var(--sidebar-text);
  height: auto;
  line-height: 38px;
  font-size: 13.5px;
  font-weight: 500;
  letter-spacing: 0.2px;
  padding: 0 4px !important;
  margin: 4px 0;
  border-radius: 6px;
  display: flex;
  align-items: center;
  z-index: 1;
  transition: none;
  span {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    word-break: break-all;
  }
  .unicon {
    fill: var(--sidebar-text);
    opacity: 0.85;
  }
  &.is-active {
    background-color: var(--color-primary);
    color: white;
    .unicon {
      fill: white;
    }
  }
  &:hover:not(.is-active) {
    outline: none;
    background-color: var(--sidebar-item-hover-background);
  }
}
</style>

<style lang="scss">
.el-menu-item .unicon svg {
  width: 17.5px !important;
  margin-right: 10px;
  margin-left: 8px;
  margin-bottom: 2px;
  overflow: unset;
}
</style>
