<template lang="pug">
  header
    div.title
      slot(name="title")
      div.action
        slot(name="action")
        i.el-icon-arrow-right
</template>

<script>
export default {
  name: 'BaseHeader'
};
</script>

<style lang="scss" scoped>
header {
  padding: 32px 35px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  .title {
    cursor: default;
    display: flex;
    align-items: baseline;
    font-size: 28px;
    font-weight: 800;
    .action {
      margin-left: 12px;
      font-size: 13px;
      font-weight: 500;
      i {
        color: var(--link-hover);
        margin-left: -10px;
        font-size: 10px;
        transform: scale(0.1);
        transition: 0.3s;
        opacity: 0;
      }
      a:hover ~ i {
        margin-left: 0;
        transform: scale(1.3);
        opacity: 0.95;
      }
    }
  }
}
</style>
