<template lang="pug">
  div.table-content
    slot
</template>

<script>
export default {
  name: 'TableContent'
};
</script>

<style lang="scss" scoped>
.table-content {
  display: flex;
  position: relative;
  overflow: hidden;
  flex: 1;
  height: 100%;
  .el-table {
    overflow: auto;
    position: unset !important;
  }
}
</style>
