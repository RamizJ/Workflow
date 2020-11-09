<template>
  <div class="page">
    <slot />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class BasePage extends Vue {}
</script>

<style lang="scss" scoped>
.page {
  display: flex;
  position: relative;
  overflow: hidden;
  padding: 0 30px 20px;
  width: calc(100% - var(--sidebar-width));
  border-left: var(--input-border);
  flex-direction: column;
  background: var(--main-background);
  transition: background 0.2s, border-color 0.2s;
}
</style>
