<template lang="pug">
  div.list
    slot
</template>

<script>
export default {
  name: 'BaseList'
};
</script>

<style lang="scss" scoped>
.list {
  display: flex;
  position: relative;
  overflow: hidden;
  flex: 1;
  height: 100%;
  padding: 0 30px;
  .el-table {
    overflow: auto;
    position: unset !important;
  }
}
</style>
