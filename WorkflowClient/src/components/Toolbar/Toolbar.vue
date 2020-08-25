<template>
  <div class="toolbar">
    <slot />
  </div>
</template>

<script>
export default {
  name: 'Toolbar'
}
</script>

<style lang="scss">
.toolbar {
  padding: 5px 0 10px;
}
</style>
