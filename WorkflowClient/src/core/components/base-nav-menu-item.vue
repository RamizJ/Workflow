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
  line-height: 30px;
  font-size: 14.5px;
  font-weight: 500;
  letter-spacing: 0.2px;
  padding: 0 16px 0 10px !important;
  margin: 4px 0;
  width: fit-content;
  border-radius: 20px;
  display: flex;
  align-items: center;
  z-index: 1;
  transition: 0.15s;
  span {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    word-break: break-all;
  }
  .unicon {
    fill: var(--sidebar-text);
    opacity: 0.85;
    transition: 0.15s;
  }
  &.is-active {
    //background-color: var(--color-primary);
    background-color: rgba(0, 0, 0, 0.07);
    color: var(--text);
    .unicon {
      fill: var(--text);
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
  width: 16px !important;
  margin-right: 8px;
  margin-left: 3px;
  margin-bottom: 2px;
  overflow: unset;
}
</style>
