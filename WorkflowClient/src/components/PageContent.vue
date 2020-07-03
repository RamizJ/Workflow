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
  padding: 0 30px;
  height: 100%;
  width: 100%;
  flex-direction: column;
}
</style>
