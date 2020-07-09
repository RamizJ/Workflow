<template lang="pug">
  div.page
    div.page-content
      slot
</template>

<script>
export default {
  name: 'Page'
};
</script>

<style lang="scss" scoped>
.page {
  width: 100%;
  display: flex;
  flex-direction: column;
}
.page-content {
  display: flex;
  position: relative;
  overflow: auto;
  padding: 0 30px;
  height: 100%;
  width: 100%;
  flex-direction: column;
}
</style>
