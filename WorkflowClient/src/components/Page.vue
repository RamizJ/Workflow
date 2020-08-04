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
  height: 100vh;
  display: flex;
  flex-direction: column;
}
.page-content {
  display: flex;
  position: relative;
  overflow: hidden;
  padding: 0 30px 20px;
  height: 100vh;
  width: 100%;
  flex-direction: column;
}
</style>
