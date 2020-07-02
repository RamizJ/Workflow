<template lang="pug">
  div.page-toolbar
    div.page-toolbar__left
      div.page-toolbar__actions
        slot(name="actions")
      div.page-toolbar__filters
        slot(name="filters")
    div.page-toolbar__view
      slot(name="view")
</template>

<script>
export default {
  name: 'PageToolbar'
};
</script>

<style lang="scss">
.page-toolbar {
  padding: 0 0 10px;
  display: flex;
  justify-content: space-between;
}
.page-toolbar__left {
  width: 100%;
}
.page-toolbar__filters {
  width: 100%;
  .el-input {
    width: 170px;
    margin-right: 10px;
  }
  .el-row {
    &:first-child {
      margin-top: 20px;
    }
    margin-bottom: 10px;
  }
}
.page-toolbar__actions {
  width: 100%;
  .el-button {
    //color: var(--text-muted);
    padding: 8px 25px;
    //padding: 10px 35px;
    //margin-right: 10px;
  }
  .el-button--default {
    color: var(--text-placeholder);
    &:hover {
      color: white;
    }
  }
}
.page-toolbar__view {
  display: flex;
  justify-content: flex-end;
  align-items: flex-start;
  .el-select {
    width: 200px;
    .el-input__suffix {
      display: none;
    }
  }
  .el-input {
    i {
      color: var(--text-muted) !important;
    }
  }
  .el-input__inner {
    color: var(--text-muted) !important;
    font-weight: 500;
    background-color: transparent !important;
    &::placeholder {
      color: var(--text-muted);
    }
  }
  .el-button {
    color: var(--text-muted);
    &.active {
      color: var(--color-primary);
    }
  }
}
</style>
