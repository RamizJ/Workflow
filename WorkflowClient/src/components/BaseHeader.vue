<template lang="pug">
  div.base-header
    div.base-header__title
      slot(name="title")
      div.action
        slot(name="action")
        i.el-icon-arrow-right
    div.base-header__subtitle
      slot(name="subtitle")

</template>

<script>
export default {
  name: 'BaseHeader'
};
</script>

<style lang="scss" scoped>
.base-header {
  padding: 32px 35px;
}
input {
  border: none;
  padding: 0;
  &.title {
    font-size: 28px;
    font-weight: 600;
  }
  &.subtitle {
    font-size: 14px;
    font-weight: 400;
  }
}
.base-header__title {
  cursor: default;
  display: flex;
  align-items: baseline;
  font-size: 26px;
  font-weight: 600;
  .action {
    margin-left: 12px;
    font-size: 13px;
    font-weight: 500;
    i.el-icon-arrow-right {
      color: var(--link-hover);
      margin-left: -10px;
      font-size: 10px;
      transform: scale(0.1);
      transition: 0.3s;
      opacity: 0;
    }
    a:hover ~ i.el-icon-arrow-right {
      margin-left: 0;
      transform: scale(1.3);
      opacity: 0.95;
    }
  }
}
.base-header__subtitle > * {
  margin-top: 25px;
  font-size: 14px;
}
</style>

<style lang="scss">
.base-header .el-button.actions {
  background-color: transparent;
  font-size: 24px;
  padding: 0 !important;
  color: gray;
  &:hover {
    color: gray;
    background-color: transparent;
  }
}
</style>
