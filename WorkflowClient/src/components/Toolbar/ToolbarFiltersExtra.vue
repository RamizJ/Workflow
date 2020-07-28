<template lang="pug">
  div.toolbar-filters-extra
    el-popover(popper-class="filters-panel" placement="bottom-start" transition="fade" trigger="click")
      el-button(slot="reference" size="mini")
        feather(type="sliders" size="18")
      slot
</template>

<script>
export default {
  name: 'ToolbarFiltersExtra'
};
</script>

<style lang="scss" scoped>
.toolbar-filters-extra {
  float: right;
  margin-top: 26px;
  .el-button--mini {
    padding: 7.25px 8px;
    border-radius: 8px;
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
