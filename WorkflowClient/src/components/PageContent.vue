<template lang="pug">
  div.page-content
    slot
</template>

<script>
export default {
  name: 'PageContent'
};
</script>

<style lang="scss" scoped>
.page-content {
  display: flex;
  position: relative;
  overflow: hidden;
  flex: 1;
  height: 100%;
  padding: 0 35px;
}
</style>
