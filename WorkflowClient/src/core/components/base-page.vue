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
  margin: 24px 24px 24px 0;
  width: calc(100% - var(--sidebar-width));
  border-radius: 3px;
  box-shadow: 0 1px 5px rgba(0, 0, 0, 0.07);
  flex-direction: column;
  background: var(--page-background);
}
</style>
