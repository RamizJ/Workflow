<template lang="pug">
  div.toolbar-item
    slot
</template>

<script>
export default {
  name: 'BaseToolbarItem'
};
</script>

<style lang="scss">
.toolbar-item {
  margin: 6px 0;
  &:not(:last-child) {
    border-right: 1px solid var(--sidebar-background);
  }
  .el-select__caret {
    font-size: 13px !important;
    margin-top: 0.5px;
  }
  .el-input__inner {
    color: var(--link-hover);
    font-size: 13px;
    font-weight: 500;
    background-color: transparent !important;
  }
}
</style>
