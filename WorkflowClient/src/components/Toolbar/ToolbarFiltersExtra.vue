<template>
  <div class="toolbar-filters-extra">
    <el-button size="mini">
      <feather type="sliders" size="16"></feather>
    </el-button>
    <!--<el-popover
      popper-class="filters-panel"
      placement="bottom-start"
      transition="fade"
      trigger="click"
    >
      <el-button slot="reference" size="mini">
        <feather type="sliders" size="16"></feather>
      </el-button>
      <slot />
    </el-popover>-->
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class ToolbarFiltersExtra extends Vue {}
</script>

<style lang="scss" scoped>
.toolbar-filters-extra {
  float: right;
  margin-top: 26px;
  .el-button--mini {
    padding: 7.5px 8px;
    border-radius: 5px;
    &:hover:not(:focus) {
      background: var(--button-background);
      color: var(--text);
    }
  }
}
</style>

<style lang="scss">
.filters-panel {
  background-color: var(--dropdown-background);
  padding: 28px;
  //width: 650px;
  .label {
    color: var(--text);
    font-size: 14px;
    font-weight: 500;
    margin-bottom: 10px;
  }
  .el-range-input,
  .el-range-input:hover:not(:focus) {
    background-color: transparent;
  }
  .el-input__inner,
  .el-textarea__inner {
    background-color: var(--dropdown-item-hover-background);
    &:hover {
      background-color: var(--dropdown-item-hover-background);
    }
  }
  .el-input.is-active .el-input__inner,
  .el-select .el-input__inner:focus,
  .el-textarea__inner:focus,
  .el-input__inner:focus,
  .el-select .el-input.is-focus .el-input__inner {
    background-color: var(--dropdown-item-hover-background);
  }
  .el-row:not(:first-child) {
    margin-top: 22px;
  }
}
</style>

<style lang="scss">
.toolbar-filters-extra {
  .el-select .el-input__icon::before {
    content: '\e6e1' !important;
  }
  .el-select .is-focus .el-input__icon {
    transform: rotateZ(0) !important;
  }
}
</style>
