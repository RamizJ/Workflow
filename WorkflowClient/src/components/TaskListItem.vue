<template lang="pug">
  div.item
    div.item__header {{ item.title }}
</template>

<script>
export default {
  name: 'TaskListItem',
  props: ['item']
};
</script>

<style lang="scss" scoped>
.item {
  cursor: pointer;
  margin: 10px 0;
  padding: 12px 15px;
  border: 1px solid var(--card-border);
  background-color: var(--card-background);
  border-radius: 8px;
  &__header {
    font-size: 12.5px;
    line-height: normal;
  }
}
</style>
