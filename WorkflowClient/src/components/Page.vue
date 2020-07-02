<template lang="pug">
  div.page
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
</style>
