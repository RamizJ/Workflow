<template lang="pug">
  div.item
    div.item__name {{ this.title }}
    div.item__content
      slot
</template>

<script>
export default {
  props: {
    title: String
  },
  name: "BaseToolbarItem"
}
</script>

<style lang="scss" scoped>
.item {
  .item__name {
    font-size: 14px;
    font-weight: 600;
    margin-bottom: 12px;
    text-transform: capitalize;
  }
}
</style>
