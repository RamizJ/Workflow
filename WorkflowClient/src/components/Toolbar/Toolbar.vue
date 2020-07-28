<template lang="pug">
  div.toolbar
    slot
</template>

<script>
export default {
  name: 'Toolbar'
};
</script>

<style lang="scss">
.toolbar {
  padding: 5px 0 10px;
}
</style>
