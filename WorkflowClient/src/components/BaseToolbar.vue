<template lang="pug">
  div.toolbar
    div.toolbar__filters
      slot(name="filters")
</template>

<script>
export default {
  name: 'BaseToolbar'
};
</script>

<style lang="scss" scoped>
.toolbar {
  padding: 0 35px 25px;
}
.toolbar__filters {
  display: flex;
  //justify-content: space-between;
  width: fit-content;
  border-top: 1px solid var(--sidebar-background);
  border-bottom: 1px solid var(--sidebar-background);
}
</style>
