<template lang="pug">
  div.toolbar-filters
    div.toolbar-filters__content
      slot
</template>

<script>
export default {
  name: 'ToolbarFilters'
};
</script>

<style lang="scss" scoped>
.toolbar-filters {
  justify-content: space-between;
  align-items: flex-end;
  display: inline-flex;
  &__content {
    display: grid;
    grid-gap: 24px;
    grid-template-columns: repeat(4, 200px);
  }
}
.label {
  color: var(--text);
  font-size: 14px;
  font-weight: 500;
  margin-bottom: 10px;
}
</style>

<style lang="scss">
.toolbar-filters {
  .el-select .el-input__icon::before {
    content: '\e6e1' !important;
  }
  .el-select .is-focus .el-input__icon {
    transform: rotateZ(0) !important;
  }
}
</style>
