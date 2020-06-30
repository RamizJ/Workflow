<template lang="pug">
  div.page-header
    div.page-header__title
      slot(name="title")
      div.action
        slot(name="action")
        i.el-icon-arrow-right
    div.page-header__subtitle
      slot(name="subtitle")

</template>

<script>
export default {
  name: 'PageHeader'
};
</script>

<style lang="scss" scoped>
.page-header {
  padding: 32px 35px 25px;
}
input {
  border: none;
  padding: 0;
  color: var(--text);
  background-color: var(--background);
  &.title {
    font-size: 28px;
    font-weight: 800;
  }
  &.subtitle {
    font-size: 14px;
    font-weight: 400;
  }
}
.page-header__title {
  cursor: default;
  display: flex;
  align-items: baseline;
  font-size: 28px;
  font-weight: 800;
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
.page-header__subtitle > * {
  margin-top: 25px;
  font-size: 14px;
}
</style>

<style lang="scss">
.page-header .actions {
  background-color: transparent;
  height: 21px;
  vertical-align: bottom;
  cursor: pointer;
  color: var(--text);
  opacity: 0.7;
  transition: opacity 0.25s;
  &:hover {
    opacity: 1;
  }
}
</style>
