<template>
  <div class="toolbar">
    <slot />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class Toolbar extends Vue {}
</script>

<style lang="scss">
.toolbar {
  padding: 5px 0 10px;
}
</style>
