<template lang="pug">
  div.toolbar
    div.title
      h1
        slot(name="title")
      div.subtitle
        slot(name="subtitle")
        i.el-icon-arrow-right
    div.search
      slot(name="search")
    div.items
      slot(name="items")
</template>

<script>
import BaseToolbarItem from "~/components/BaseToolbarItem";

export default {
  name: 'BaseToolbar',
  components: { BaseToolbarItem }
};
</script>

<style lang="scss" scoped>
.toolbar {
  padding: 32px 35px;
  .search {
    margin-top: 10px;
    width: 20%;
  }
  .title {
    cursor: default;
    display: flex;
    align-items: baseline;
    font-size: 28px;
    font-weight: 800;
    .subtitle {
      margin-left: 12px;
      font-size: 13px;
      font-weight: 500;
      i {
        color: var(--link-hover);
        margin-left: -10px;
        font-size: 10px;
        transform: scale(0.1);
        transition: .3s;
        opacity: 0;
      }
      a:hover ~ i {
        margin-left: 0;
        transform: scale(1.3);
        opacity: 0.95;
      }
    }
  }
  .items {
    display: grid;
    grid-gap: 24px;
    grid-template-columns: repeat(6,160px);
    & > * {
      margin-top: 25px;
    }
  }
}
</style>
